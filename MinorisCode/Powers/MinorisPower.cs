using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Minoris.MinorisCode.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using Minoris.MinorisCode.Character;
using MegaCrit.Sts2.Core.Runs;

namespace Minoris.MinorisCode.Powers;

public abstract class MinorisPower : CustomPowerModel
{
    private const string MissingPowerIconPath = "res://images/powers/missing_power.png";

    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : MissingPowerIconPath;
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : MissingPowerIconPath;
        }
    }

    protected static async Task CheckWinConditionIfCombatEnding()
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (!CombatManager.Instance.IsEnding) return;
        if (RunManager.Instance?.ActionExecutor?.CurrentlyRunningAction != null) return;
        await CombatManager.Instance.CheckWinCondition();
    }
}

public class TothSlateXIIIPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        for (var i = 0; i < Amount; i++)
        {
            await CreatureCmd.GainBlock(Owner, 5m, ValueProp.Unpowered, null);

            var enemies = CombatState.GetOpponentsOf(Owner).Where(c => c.IsAlive).ToList();
            if (enemies.Count > 0)
            {
                var idx = (int)(GD.Randi() % (uint)enemies.Count);
                await CreatureCmd.Damage(choiceContext, enemies[idx], 5m, ValueProp.Unpowered, Owner);
                await CheckWinConditionIfCombatEnding();
                if (!CombatManager.Instance.IsInProgress) return;
            }

            if (Owner.Player != null) await PlayerCmd.GainEnergy(1, Owner.Player);
            await CreatureCmd.Heal(Owner, 2m);

            if (Owner.Player != null)
            {
                var hand = PileType.Hand.GetPile(Owner.Player).Cards.ToList();
                if (hand.Count > 0)
                {
                    var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
                    var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner.Player, prefs)).FirstOrDefault();
                    if (pick != null) await CardCmd.Exhaust(choiceContext, pick);
                }

                await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, null);

                var drawn = await CardPileCmd.Draw(choiceContext, Owner.Player);
                if (drawn != null) CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);

                await PowerCmd.Apply<DexterityPower>(Owner, 1, Owner, null);
                await PowerCmd.Apply<VigorPower>(Owner, 4, Owner, null);
                await PowerCmd.Apply<ArtifactPower>(Owner, 1, Owner, null);
            }
        }

        Flash();
    }
}

public class ScratchAmpPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return 0m;
        if (cardSource == null) return 0m;
        if (!cardSource.GetType().Name.Contains("Scratch")) return 0m;
        return Amount;
    }
}

public class DeadlyCoffeePower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        if (Amount > 0)
        {
            await CreatureCmd.Damage(choiceContext, new[] { Owner }, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
        RemoveInternal();
    }
}

public class AllergenPower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        if (Amount > 0)
        {
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
            await CreatureCmd.Damage(choiceContext, Owner, 2m * Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class NextAttackEnergyRefundPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        Owner.Player?.PlayerCombatState?.GainEnergy(1);
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class NextSkillEnergyRefundPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Skill) return;
        Owner.Player?.PlayerCombatState?.GainEnergy(1);
        RemoveInternal();
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class NextSkillCostReducePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Skill) return false;
        if (originalCost <= 0m) return false;
        modifiedCost = originalCost - Amount;
        if (modifiedCost < 0m) modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Skill) return;
        RemoveInternal();
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class NextAttackCostReducePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Attack) return false;
        if (originalCost <= 0m) return false;
        modifiedCost = originalCost - Amount;
        if (modifiedCost < 0m) modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        RemoveInternal();
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class DrawExtraNextTurnPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        if (Amount > 0 && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player, fromHandDraw: true);
        }
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class RevertStrengthAtTurnEndPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        if (Amount > 0)
        {
            await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
        }
        RemoveInternal();
    }
}

public class GripPower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Type != CardType.Attack) return;
        var dealer = playedCard.Owner.Creature;
        if (dealer.Side != CombatSide.Player) return;
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, dealer);
        await CheckWinConditionIfCombatEnding();
        if (!CombatManager.Instance.IsInProgress) return;
        Flash();
    }
}

public class CardboardBlockOnAttackPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class StressResponsePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, $"失去生命：1\n获得格挡：{Amount}", isSmart: false)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        await CreatureCmd.Damage(choiceContext, Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class BlockPreservePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected override bool IsVisibleInternal => true;

    public override bool ShouldClearBlock(Creature creature)
    {
        if (Owner != creature) return true;
        return false;
    }

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (preventer != this) return;
        if (creature != Owner) return;
        Flash();
        await Task.CompletedTask;
    }

    public override async Task AfterBlockBroken(Creature creature)
    {
        if (creature != Owner) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class KittenKnifePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return;
        if (cardSource == null) return;
        if (cardSource.Type != CardType.Attack) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.Damage(choiceContext, new[] { target }, Amount, ValueProp.Unpowered, Owner);
        await CheckWinConditionIfCombatEnding();
    }
}

public class BronzeGauntletPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner) return;
        if (cardSource == null) return;
        if (cardSource.Type != CardType.Attack) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public class ToyClawPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var scratch = CombatState.CreateCard<Minoris.MinorisCode.Cards.Card062_1_Scratch>(Owner.Player);
        if (Amount >= 2) CardCmd.Upgrade(scratch);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(scratch, PileType.Hand, false, CardPilePosition.Top));
    }
}

public class CatScratchBoardPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Skill) return;
        await PowerCmd.Apply<CatScratchBoardTemporaryDexterityPower>(Owner, Amount, Owner, null);
    }
}

public class CatScratchBoardTemporaryDexterityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Minoris.MinorisCode.Cards.Card063_CatScratchBoard>();
}

public class LittleBellPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (result.UnblockedDamage <= 0) return;
        if (CombatState.CurrentSide != Owner.Side) return;
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
    }
}

public class GeneratorPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    private int _currentTurnEnergyToAdd;
    private int _pendingNextTurnEnergy;

    public override int DisplayAmount => _pendingNextTurnEnergy + _currentTurnEnergyToAdd;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (CombatState.CurrentSide != Owner.Side) return;
        if (playedCard.Type == CardType.Attack)
        {
            var energyPerAttack = (int)Amount;
            if (energyPerAttack > 0)
            {
                _currentTurnEnergyToAdd += energyPerAttack;
                InvokeDisplayAmountChanged();
            }
        }
        await Task.CompletedTask;
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        _pendingNextTurnEnergy += _currentTurnEnergyToAdd;
        _currentTurnEnergyToAdd = 0;
        InvokeDisplayAmountChanged();
        await Task.CompletedTask;
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player.PlayerCombatState == null) return;
        if (_pendingNextTurnEnergy > 0)
        {
            Owner.Player.PlayerCombatState.GainEnergy(_pendingNextTurnEnergy);
            _pendingNextTurnEnergy = 0;
            Flash();
            InvokeDisplayAmountChanged();
        }
        await Task.CompletedTask;
    }
}

public class DimensionalScratchPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}

public class MetronomePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (!playedCard.GetType().Name.Contains("Strike")) return;
        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}

public class SelfTaughtDrawPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}

public class ApophisArrivesPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool IsInstanced => true;
    private int _turnsUntilSelfDamage = 3;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, $"每回合：对所有敌人造成 {Amount} 伤害\n倒计时：{_turnsUntilSelfDamage}\n倒计时归零：自身失去 {Amount} 生命", isSmart: false)
    ];

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side || CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner).Where(e => e.IsAlive);
        await CreatureCmd.Damage(choiceContext, enemies, Amount, ValueProp.Unpowered, Owner);
        await CheckWinConditionIfCombatEnding();
        if (!CombatManager.Instance.IsInProgress) return;
        Flash();
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        _turnsUntilSelfDamage = System.Math.Max(0, _turnsUntilSelfDamage - 1);
        if (_turnsUntilSelfDamage == 0)
        {
            await CreatureCmd.Damage(choiceContext, new[] { Owner }, Amount, ValueProp.Unpowered, Owner);
            Flash();
            _turnsUntilSelfDamage = 3;
        }
    }
}

public class DamageTakenCounterPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => false;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (result.UnblockedDamage <= 0) return;
        Amount++;
        await Task.CompletedTask;
    }
}

public class SandstormPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return target == Owner ? 0.5m : 1m;
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await PowerCmd.TickDownDuration(this);
        await Task.CompletedTask;
    }
}

public class SolarFormPower : MinorisPower
{
    private class Data
    {
        public bool upgradeGeneratedCards;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool UpgradeGeneratedCards => GetInternalData<Data>().upgradeGeneratedCards;

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradeGeneratedCards = true;
        }
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradeGeneratedCards = true;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var all = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .ToList();
        var pickA = all.Where(c => c.Type == CardType.Attack).ToList();
        var pickS = all.Where(c => c.Type == CardType.Skill).ToList();
        var pickP = all.Where(c => c.Type == CardType.Power).ToList();
        if (pickA.Count == 0 || pickS.Count == 0 || pickP.Count == 0) return;
        var rng = player.RunState.Rng.CombatCardGeneration;
        var countPerType = System.Math.Max(1, (int)Amount);

        async Task CreateAndAdd(IReadOnlyList<CardModel> pool)
        {
            var canonical = rng.NextItem(pool);
            if (canonical == null) return;
            var card = CombatState.CreateCard(canonical, Owner.Player);
            if (UpgradeGeneratedCards) CardCmd.Upgrade(card);
            card.EnergyCost.SetThisCombat(0);
            card.AddKeyword(CardKeyword.Ethereal);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, false, CardPilePosition.Top));
        }

        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickA);
        }
        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickS);
        }
        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickP);
        }
        Flash();
    }
}

public class BastBlessingPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner != Owner.Player) return playCount;
        if (card.Type != CardType.Attack) return playCount;
        return playCount + Amount;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (card.Owner == Owner.Player && card.Type == CardType.Attack) Flash();
        return Task.CompletedTask;
    }
}

public class BetrayalPower : MinorisPower
{
    private class Data
    {
        public int upgradedAmount;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int UpgradedAmount => GetInternalData<Data>().upgradedAmount;
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var normalTimes = System.Math.Max(0, Amount - UpgradedAmount);
            var upgradedTimes = System.Math.Max(0, UpgradedAmount);
            return
            [
                new HoverTip(this, $"普通次数：{normalTimes}\n升级次数：{upgradedTimes}", isSmart: false)
            ];
        }
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradedAmount = Amount;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount != 0m && cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradedAmount = System.Math.Max(0, GetInternalData<Data>().upgradedAmount + (int)amount);
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var myPoolTitle = player.Character.CardPool.Title;
        var otherPools = player.UnlockState.CharacterCardPools
            .Where(p => p.Title != myPoolTitle)
            .ToList();
        var candidates = otherPools
            .SelectMany(p => p.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(c => c.Rarity != CardRarity.Token && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event)
            .ToList();
        if (candidates.Count == 0) return;
        var rng = player.RunState.Rng.CombatCardGeneration;

        async Task OfferOnce(bool upgradeOfferedCards)
        {
            if (CombatManager.Instance.IsEnding) return;
            if (candidates.Count == 0) return;
            var options = new List<CardModel>();
            var optionCount = System.Math.Min(3, candidates.Count);
            for (var i = 0; i < optionCount; i++)
            {
                var pick = rng.NextItem(candidates);
                if (pick == null) break;
                candidates.Remove(pick);
                var card = CombatState.CreateCard(pick, Owner.Player);
                if (upgradeOfferedCards) CardCmd.Upgrade(card);
                card.EnergyCost.SetUntilPlayed(0);
                options.Add(card);
            }
            if (options.Count == 0) return;
            if (CombatManager.Instance.IsEnding) return;
            var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner.Player);
            if (chosen == null) return;
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, false, CardPilePosition.Top));
            Flash();
        }

        var normalTimes = System.Math.Max(0, Amount - UpgradedAmount);
        var upgradedTimes = System.Math.Max(0, UpgradedAmount);

        for (var i = 0; i < normalTimes; i++)
        {
            await OfferOnce(false);
        }

        for (var i = 0; i < upgradedTimes; i++)
        {
            await OfferOnce(true);
        }
    }
}

public class ButterTrapRetaliationPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner) return;
        if (dealer == null) return;
        if (CombatState != null && CombatState.CurrentSide != Owner.Side)
        {
            await PowerCmd.Apply<WeakPower>(dealer, Amount, Owner, null);
            await PowerCmd.Apply<VulnerablePower>(dealer, Amount, Owner, null);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}

public class WildFormPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && Amount != 1)
        {
            Amount = 1;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        var hand = PileType.Hand.GetPile(player).Cards.ToList();
        foreach (var c in hand)
        {
            if (c.Type != CardType.Attack) continue;
            c.EnergyCost.SetThisCombat(0);
            c.AddKeyword(CardKeyword.Ethereal);
        }
        await Task.CompletedTask;
    }
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard.Owner != Owner.Player) return;
        if (drawnCard.Type != CardType.Attack) return;
        drawnCard.EnergyCost.SetThisCombat(0);
        drawnCard.AddKeyword(CardKeyword.Ethereal);
        await Task.CompletedTask;
    }
}

public class CommandTheHuntPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    private bool _echoing;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (CombatState == null) return;
        if (playedCard.Owner == Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        if (_echoing) return;
        _echoing = true;
        var copy = CombatState.CreateCard(playedCard.CanonicalInstance, playedCard.Owner);
        copy.EnergyCost.SetThisCombat(0);
        copy.AddKeyword(CardKeyword.Exhaust);
        await CardCmd.AutoPlay(choiceContext, copy, cardPlay.Target);
        _echoing = false;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await PowerCmd.TickDownDuration(this);
        await Task.CompletedTask;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return Task.CompletedTask;
    }
}
