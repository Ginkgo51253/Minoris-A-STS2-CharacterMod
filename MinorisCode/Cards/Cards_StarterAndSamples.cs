using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Models.CardPools;
using Minoris.MinorisCode.Character;
using System;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.HoverTips;
using Godot;

namespace Minoris.MinorisCode.Cards;

public class Card001_Strike() : MinorisCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
public class Card002_Defend() : MinorisCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
public class Card003_InstinctScratch() : MinorisCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar(HitsKey, 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(1m);
    }
}
public class Card004_IntuitiveDodge() : MinorisCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}

public class Card005_Whip() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}
public class Card006_Chase() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack;
    private bool WasLastCardPlayedAttack
    {
        get
        {
            var last = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card != this);
            if (last == null) return false;
            return last.CardPlay.Card.Type == CardType.Attack;
        }
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (WasLastCardPlayedAttack) Owner.PlayerCombatState!.GainEnergy(1);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
public class Card007_SpinStrike() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<Powers.NextSkillCostReducePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
public class Card008_ImprovisedStrike() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        bool IsStrikeName(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return false;
            return title.Contains("打击") || title.Contains("Strike", StringComparison.OrdinalIgnoreCase);
        }

        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.GetType() != typeof(Card008_ImprovisedStrike))
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .Where(c => c.Tags.Contains(CardTag.Strike) || IsStrikeName(c.Title))
            .ToList();
        if (candidates.Count > 0)
        {
            var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
            var card = CombatState.CreateCard(pick, Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, false, CardPilePosition.Top));
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card009_TestingBlow() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new PowerVar<VigorPower>(2)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<VigorPower>(Owner.Creature, DynamicVars["VigorPower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["VigorPower"].UpgradeValueBy(1m);
    }
}
public class Card010_Quadracut() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
public class Card011_PreciseScratch() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new IntVar(HitsKey, 2)
    ];

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(1m);
    }
}
public class Card012_Slam() : MinorisCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card013_Ambush() : MinorisCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        var hand = PileType.Hand.GetPile(Owner);
        var scratchInHand = hand.Cards.FirstOrDefault(c => c.Type == CardType.Attack && c.GetType().Name.Contains("Scratch"));
        if (scratchInHand != null)
        {
            await CardCmd.AutoPlay(choiceContext, scratchInHand, cardPlay.Target ?? Owner.Creature);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
public class Card014_FullScratch() : MinorisCard(3, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new IntVar(HitsKey, 4)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}

public class Card015_Boomerang() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
public class Card016_Supersonic() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
public class Card017_ElegantScratch() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string ScratchAmpKey = "ScratchAmp";

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amp = DynamicVars[ScratchAmpKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(2m).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(2).Execute(choiceContext);
            await PowerCmd.Apply<Powers.ScratchAmpPower>(Owner.Creature, amp, Owner.Creature, this);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(2m).FromCard(this).Targeting(cardPlay.Target).WithHitCount(2).Execute(choiceContext);
        await PowerCmd.Apply<Powers.ScratchAmpPower>(Owner.Creature, amp, Owner.Creature, this);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(ScratchAmpKey, 1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars[ScratchAmpKey].UpgradeValueBy(1m);
    }
}
public class Card018_TipOver() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2m, ValueProp.Move),
        new CardsVar(2)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        var hand = PileType.Hand.GetPile(Owner);
        var before = hand.Cards.ToHashSet();
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        var after = hand.Cards.ToHashSet();
        var newly = after.Where(c => !before.Contains(c)).ToList();
        foreach (var c in newly.Where(c => c.Type == CardType.Attack && c != this))
        {
            if (CombatState == null) break;
            if (CombatManager.Instance.IsEnding) break;
            var enemies = CombatState.HittableEnemies.Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) break;
            var idx = (int)(GD.Randi() % (uint)enemies.Count);
            await CardCmd.AutoPlay(choiceContext, c, enemies[idx]);
            if (CombatManager.Instance.IsEnding) break;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
public class Card019_SobekStrike() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<Powers.GripPower>(1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GripPower"].UpgradeValueBy(1m);
    }
}
public class Card020_CallOfJackal() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
    public override bool ShouldReceiveCombatHooks => true;
    private int _extraHits;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var hits = 1 + _extraHits;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this) _extraHits++;
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
public class Card021_MasterCraft() : MinorisCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => int.MaxValue;
    private const string HitsKey = "Hits";
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new IntVar(HitsKey, 2)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitCount(DynamicVars[HitsKey].IntValue)
            .Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(1m);
    }
}
public class Card022_Justice() : MinorisCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string BonusPerCardKey = "BonusPerCard";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new BlockVar(3m, ValueProp.Move),
        new IntVar(BonusPerCardKey, 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        // Step 1: Exhaust all 0-cost cards from draw, hand, discard
        var zeroCostCards = new List<CardModel>();
        var piles = new[] { PileType.Draw.GetPile(Owner), PileType.Hand.GetPile(Owner), PileType.Discard.GetPile(Owner) };
        foreach (var pile in piles)
        {
            foreach (var c in pile.Cards)
            {
                var cost = c.EnergyCost.GetWithModifiers(CostModifiers.Local);
                if (cost == 0) zeroCostCards.Add(c);
            }
        }

        var bonusPerCard = DynamicVars[BonusPerCardKey].IntValue;
        var totalBonus = bonusPerCard * zeroCostCards.Count;

        foreach (var c in zeroCostCards)
            await CardPileCmd.Add(c, PileType.Exhaust, CardPilePosition.Top);

        // Step 2: Deal damage once with total amount (base + bonus)
        var totalDamage = DynamicVars.Damage.IntValue + totalBonus;
        await DamageCmd.Attack(totalDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // Step 3: Gain block once with total amount (base + bonus)
        var totalBlock = DynamicVars.Block.IntValue + totalBonus;
        await CreatureCmd.GainBlock(Owner.Creature, totalBlock, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars[BonusPerCardKey].UpgradeValueBy(2m);
    }
}
public class Card023_SolarFlare() : MinorisCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (hand.Count > 0)
        {
            var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1))).FirstOrDefault();
            if (pick != null)
            {
                var cost = pick.EnergyCost.GetWithModifiers(CostModifiers.Local);
                await CardCmd.Exhaust(choiceContext, pick);
                var current = EnergyCost.GetWithModifiers(CostModifiers.Local);
                var next = Math.Max(0, current - cost);
                EnergyCost.SetThisCombat(next);
            }
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
public class Card024_HurricaneStrike() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private const string HitsPerXKey = "HitsPerX";

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar(HitsPerXKey, 2)
    ];

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var x = ResolveEnergyXValue();
        var hits = DynamicVars[HitsPerXKey].IntValue * x;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(hits)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsPerXKey].UpgradeValueBy(1m);
    }
}

public class Card025_SunLens() : MinorisCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
        var before = enemies.ToDictionary(e => e, e => e.CurrentHp);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        var afterKills = enemies.Any(e => !e.IsAlive || e.CurrentHp <= 0);
        if (afterKills) Owner.PlayerCombatState!.GainEnergy(1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
public class Card026_DivineLight() : MinorisCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var history = CombatManager.Instance.History.CardPlaysStarted;
        var scratchPlays = history.Count(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.GetType().Name.Contains("Scratch"));
        var hits = 1 + scratchPlays;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
public class Card027_OfferingOfSobek() : MinorisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const string BonusKey = "SobekBonus";
    private int _permanentBonus;

    [SavedProperty]
    public int PermanentBonus
    {
        get => _permanentBonus;
        set
        {
            AssertMutable();
            _permanentBonus = value;
            DynamicVars[BonusKey].BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new IntVar(BonusKey, 0)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (hand.Count > 0)
        {
            var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1))).FirstOrDefault();
            if (pick != null)
            {
                var cost = pick.EnergyCost.GetWithModifiers(CostModifiers.Local);
                await CardCmd.Exhaust(choiceContext, pick);
                if (cost > 0)
                {
                    PermanentBonus += cost;
                    if (DeckVersion is Card027_OfferingOfSobek deckCard)
                    {
                        deckCard.PermanentBonus += cost;
                    }
                }
            }
        }
        var total = DynamicVars.Damage.IntValue + DynamicVars[BonusKey].IntValue;
        await DamageCmd.Attack(total).FromCard(this).TargetingAllOpponents(CombatState).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card028_HammerOfGods() : MinorisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override int MaxUpgradeLevel => int.MaxValue;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
        if (CombatState != null && DeckVersion != null && DeckVersion.IsUpgradable)
        {
            var runEntry = Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId);
            runEntry?.UpgradedCards.Add(DeckVersion.Id);
            DeckVersion.UpgradeInternal();
            DeckVersion.FinalizeUpgradeInternal();
        }
    }
}
public class Card029_WrathOfVengeance() : MinorisCard(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];
    private int _totalTriggersThisCombat;

    private int GetDamageTakenThisCombat()
    {
        if (Owner?.Creature == null) return 0;
        return CombatManager.Instance.History.Entries
            .OfType<MegaCrit.Sts2.Core.Combat.History.Entries.DamageReceivedEntry>()
            .Count(e => e.Receiver == Owner.Creature && e.Result.UnblockedDamage > 0);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var damageTakenThisCombat = GetDamageTakenThisCombat();
            var description = $"总触发次数：{_totalTriggersThisCombat}\n本场战斗受伤次数：{damageTakenThisCombat}";
            return [new HoverTip(TitleLocString, description)];
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        async Task HitOnce()
        {
            var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var idx = (int)(GD.Randi() % (uint)enemies.Count);
            var target = enemies[idx];
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            _totalTriggersThisCombat++;
        }
        await HitOnce();
        if (CombatManager.Instance.IsEnding) return;
        var repeats = Math.Max(0, GetDamageTakenThisCombat());
        for (var i = 0; i < repeats; i++)
        {
            if (CombatManager.Instance.IsEnding) return;
            await HitOnce();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
    }
}

public class Card030_PanickedDodge() : MinorisCard(0, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var candidates = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (candidates.Count == 0) return;
        var idx = (int)(GD.Randi() % (uint)candidates.Count);
        var options = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token);
        await CardCmd.Transform(new CardTransformation(candidates[idx], options).Yield(), Owner.RunState.Rng.CombatCardSelection);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
public class Card031_Misdirect() : MinorisCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("LoseStrength", 6)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var lose = DynamicVars["LoseStrength"].IntValue;
        await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -lose, Owner.Creature, this);
        await PowerCmd.Apply<Powers.RevertStrengthAtTurnEndPower>(cardPlay.Target, lose, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
public class Card032_Training() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    private const string UpgradeCountKey = "UpgradeCount";
    private static bool ShouldSelectLocalCard(Player player) => LocalContext.IsMe(player) && RunManager.Instance.NetService.Type != NetGameType.Replay;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new IntVar(UpgradeCountKey, 1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var count = DynamicVars[UpgradeCountKey].IntValue;
        if (count <= 0) return;

        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this && c.IsUpgradable).ToList();
        if (hand.Count == 0) return;

        var prefs = new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_UPGRADE_HEADER"), count);
        if (!prefs.RequireManualConfirmation && hand.Count <= prefs.MinSelect)
        {
            foreach (var c in hand) CardCmd.Upgrade(c);
            return;
        }

        IEnumerable<CardModel> picks;
        if (CardSelectCmd.Selector != null)
        {
            picks = await CardSelectCmd.Selector.GetSelectedCards(hand, prefs.MinSelect, prefs.MaxSelect);
        }
        else
        {
            uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(Owner);
            await choiceContext.SignalPlayerChoiceBegun(PlayerChoiceOptions.CancelPlayCardActions);
            if (ShouldSelectLocalCard(Owner))
            {
                NPlayerHand.Instance?.CancelAllCardPlay();
                picks = await NCombatRoom.Instance!.Ui.Hand.SelectCards(
                    prefs,
                    c => c != this && c.IsUpgradable,
                    this,
                    NPlayerHand.Mode.UpgradeSelect
                );
                RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(Owner, choiceId, PlayerChoiceResult.FromMutableCombatCards(picks));
            }
            else
            {
                picks = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(Owner, choiceId)).AsCombatCards();
            }
            await choiceContext.SignalPlayerChoiceEnded();
        }

        foreach (var c in picks) CardCmd.Upgrade(c);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[UpgradeCountKey].UpgradeValueBy(1m);
    }
}
public class Card033_Construct() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    public override int MaxUpgradeLevel => int.MaxValue;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<Powers.ConstructCounterPower>(Owner.Creature, 0, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
public class Card034_Roll() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (CombatState == null) return;
        var d1 = CombatState.CreateCard<Dazed>(Owner);
        var d2 = CombatState.CreateCard<Dazed>(Owner);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(d1, PileType.Draw, true, CardPilePosition.Random));
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(d2, PileType.Draw, true, CardPilePosition.Random));
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}
public class Card035_Bind() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.GripPower>(1)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GripPower"].UpgradeValueBy(1m);
    }
}
public class Card036_CatStep() : MinorisCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SlipperyPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card037_Evade() : MinorisCard(3, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(24m, ValueProp.Move)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, new[] { Owner.Creature }, 3m, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}

public class Card038_PaperPlane() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    public override bool ShouldReceiveCombatHooks => true;
    private int _attacksThisTurn;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Creature.Side == side) _attacksThisTurn = 0;
        await Task.CompletedTask;
    }
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner) return;
        if (playedCard.Type == CardType.Attack)
        {
            _attacksThisTurn++;
            if (_attacksThisTurn >= 3)
            {
                var inHand = PileType.Hand.GetPile(Owner).Cards.Contains(this);
                if (!inHand)
                {
                    if (PileType.Draw.GetPile(Owner).Cards.Contains(this)
                        || PileType.Discard.GetPile(Owner).Cards.Contains(this)
                        || PileType.Exhaust.GetPile(Owner).Cards.Contains(this))
                    {
                        await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Top);
                    }
                }
                _attacksThisTurn = 0;
            }
        }
    }
}
public class Card039_CatTalisman() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 1m);
        Owner.PlayerCombatState!.GainEnergy(1);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card040_Halftime() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string EnergyGainKey = "EnergyGain";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(EnergyGainKey, 1)];

    protected override bool ShouldGlowGoldInternal => WasLastCardPlayedAttack;
    private bool WasLastCardPlayedAttack
    {
        get
        {
            var last = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card != this);
            if (last == null) return false;
            return last.CardPlay.Card.Type == CardType.Attack;
        }
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (WasLastCardPlayedAttack)
        {
            Owner.PlayerCombatState!.GainEnergy(DynamicVars[EnergyGainKey].IntValue);
        }
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars[EnergyGainKey].UpgradeValueBy(1m);
    }
}
public class Card041_DeadlyCoffee() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string EnergyGainKey = "EnergyGain";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(EnergyGainKey, 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner.PlayerCombatState!.GainEnergy(DynamicVars[EnergyGainKey].IntValue);
        await PowerCmd.Apply<Powers.DeadlyCoffeePower>(Owner.Creature, 6, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars[EnergyGainKey].UpgradeValueBy(1m);
    }
}
public class Card042_Allergen() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.AllergenPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
public class Card043_CityOfPillars() : MinorisCard(0, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        var options = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token);
        foreach (var c in hand)
        {
            await CardCmd.Transform(new CardTransformation(c, options).Yield(), Owner.RunState.Rng.CombatCardSelection);
        }
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
public class Card044_MintCandy() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<Powers.NextAttackCostReducePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
public class Card045_Commanding() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawnCards = (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner)).ToList();
        foreach (var c in drawnCards) CardCmd.Upgrade(c);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}
public class Card046_CatapultStart() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override int MaxUpgradeLevel => int.MaxValue;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
public class Card047_SmallFreezeDried() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card048_CardboardHouse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner?.Creature?.CombatState;
        var attacksPlayedThisTurn = combatState == null
            ? 0
            : CombatManager.Instance.History.CardPlaysStarted.Count(e =>
                e.HappenedThisTurn(combatState)
                && e.CardPlay.Card.Owner == Owner
                && e.CardPlay.Card.Type == CardType.Attack);

        var blockToGain = attacksPlayedThisTurn * DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, blockToGain, ValueProp.Move, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}
public class Card049_StressResponse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private const string AmountKey = "Amount";

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(AmountKey, 8)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.StressResponsePower>(Owner.Creature, DynamicVars[AmountKey].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[AmountKey].UpgradeValueBy(4m);
    }
}
public class Card050_BandageHeal() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3m),
        new BlockVar(3m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
public class Card051_ButterTrap() : MinorisCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<Powers.ButterTrapRetaliationPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}

public class Card052_MoonStep() : MinorisCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    public override bool ShouldReceiveCombatHooks => true;
    private int _playsThisTurn;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Owner.PlayerCombatState!.GainEnergy(2);
        await CardPileCmd.Draw(choiceContext, 2, Owner);
        _playsThisTurn++;
        EnergyCost.SetThisCombat(_playsThisTurn);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        _playsThisTurn = 0;
        EnergyCost.SetThisCombat(0);
        await Task.CompletedTask;
    }
}
public class Card053_AlchemicalCrucible() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (hand.Count > 0)
        {
            var toExhaust = hand.Count >= 2
                ? await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 2))
                : hand;
            foreach (var c in toExhaust) await CardCmd.Exhaust(choiceContext, c);
        }
        if (CombatState == null) return;
        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .ToList();
        if (candidates.Count > 0)
        {
            var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
            var generated = CombatState.CreateCard(pick, Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, false, CardPilePosition.Top));
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card054_DeathRoll() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.GripPower>(6)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        if (IsUpgraded)
        {
            await PowerCmd.Apply<Powers.GripPower>(CombatState.HittableEnemies, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
        }
        else
        {
            if (cardPlay.Target == null) return;
            await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
        }
    }
}
public class Card055_LightAsFeather() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await PowerCmd.Apply<SlowPower>(cardPlay.Target, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class Card057_EyeCharm() : MinorisCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(20m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<Powers.BlockPreservePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
    }
}
public class Card058_GatherStrength() : MinorisCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(16m, ValueProp.Move),
        new PowerVar<StrengthPower>(2),
        new PowerVar<Powers.DrawExtraNextTurnPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<Powers.DrawExtraNextTurnPower>(Owner.Creature, DynamicVars["DrawExtraNextTurnPower"].IntValue, Owner.Creature, this);
        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        foreach (var c in hand) c.GiveSingleTurnRetain();
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(8m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
        DynamicVars["DrawExtraNextTurnPower"].UpgradeValueBy(1m);
    }
}
public class Card059_Fortune() : MinorisCard(4, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity != CardRarity.Token && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event)
            .ToList();
        if (candidates.Count == 0) return;

        var options = new List<CardModel>();
        foreach (var m in candidates)
        {
            var created = CombatState.CreateCard(m, Owner);
            if (IsUpgraded) CardCmd.Upgrade(created);
            options.Add(created);
        }
        if (CombatManager.Instance.IsEnding) return;
        var pick = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            options,
            Owner,
            new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_HEADER"), 1)
        )).FirstOrDefault();

        foreach (var card in options)
        {
            if (card != pick)
            {
                CombatState.RemoveCard(card);
            }
        }

        if (pick != null) await CardCmd.AutoPlay(choiceContext, pick, null);
    }
}

public class Card060_KittenKnife() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.KittenKnifePower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.KittenKnifePower>(Owner.Creature, DynamicVars["KittenKnifePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["KittenKnifePower"].UpgradeValueBy(1m);
    }
}
public class Card061_BronzeGauntlet() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BronzeGauntletPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card062_ToyClaw() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.ToyClawPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ToyClawPower>(Owner.Creature, DynamicVars["ToyClawPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ToyClawPower"].UpgradeValueBy(1m);
    }
}
public class Card062_1_Scratch() : MinorisCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const string HitsKey = "Hits";

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1m, ValueProp.Move),
        new IntVar(HitsKey, 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}
public class Card063_CatScratchBoard() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.CatScratchBoardPower>(Owner.Creature, 2, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card064_LittleBell() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.LittleBellPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card065_Generator() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.GeneratorPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
public class Card066_DimensionalScratch() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.DimensionalScratchPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card067_Metronome() : MinorisCard(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.MetronomePower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class Card068_SelfTaught() : MinorisCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<Powers.SelfTaughtDrawPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card069_SetsChoice() : MinorisCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var options = new List<CardModel>
        {
            CombatState.CreateCard<Card069_1_Sandstorm>(Owner),
            CombatState.CreateCard<Card069_2_ThunderFlash>(Owner),
            CombatState.CreateCard<Card069_3_Betrayal>(Owner)
        };
        if (CombatManager.Instance.IsEnding) return;
        var pick = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner);
        if (pick != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(pick, PileType.Hand, true, CardPilePosition.Top));
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Innate);
    }
}
public class Card069_1_Sandstorm() : MinorisCard(1, CardType.Skill, CardRarity.Token, TargetType.None)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.SandstormPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card069_2_ThunderFlash() : MinorisCard(3, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new IntVar(HitsKey, 8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        var dmg = DynamicVars.Damage.BaseValue;
        for (var i = 0; i < hits; i++)
        {
            if (CombatState == null) return;
            var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
            if (enemies.Count == 0) return;
            var idx = (int)(GD.Randi() % (uint)enemies.Count);
            var target = enemies[idx];
            await DamageCmd.Attack(dmg).FromCard(this).Targeting(target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}
public class Card069_3_Betrayal() : MinorisCard(2, CardType.Power, CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.BetrayalPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BetrayalPower>(Owner.Creature, DynamicVars["BetrayalPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BetrayalPower"].UpgradeValueBy(1m);
    }
}
public class Card070_IsisRebirth() : MinorisCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(5)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RegenPower>(Owner.Creature, DynamicVars["RegenPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["RegenPower"].UpgradeValueBy(2m);
    }
}
public class Card071_SolarForm() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.SolarFormPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.SolarFormPower>(Owner.Creature, DynamicVars["SolarFormPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
public class Card072_BastBlessing() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BastBlessingPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
public class Card073_ApophisArrives() : MinorisCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.ApophisArrivesPower>(20)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ApophisArrivesPower>(Owner.Creature, DynamicVars["ApophisArrivesPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ApophisArrivesPower"].UpgradeValueBy(10m);
    }
}

public abstract class TothSlateBase<TNext>(CardRarity rarity, TargetType targetType) :
    MinorisCard(3, CardType.Skill, rarity, targetType)
    where TNext : CardModel
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    public override bool ShouldReceiveCombatHooks => true;
    private bool _wasPlayedThisCombat;

    protected abstract int Stage { get; }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _wasPlayedThisCombat = true;
        await ApplyStageEffects(choiceContext, cardPlay);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (!_wasPlayedThisCombat) return;
        if (DeckVersion == null) return;
        await CardCmd.TransformTo<TNext>(DeckVersion);
    }

    protected virtual async Task ApplyStageEffects(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Stage >= 2)
        {
            await CreatureCmd.GainBlock(Owner.Creature, 5m, ValueProp.Move, cardPlay);
        }

        if (Stage >= 3)
        {
            if (cardPlay.Target != null)
            {
                await DamageCmd.Attack(5m).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
            }
        }

        if (Stage >= 4)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }

        if (Stage >= 5)
        {
            await CreatureCmd.Heal(Owner.Creature, 2m);
        }

        if (Stage >= 6)
        {
            var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
            if (hand.Count > 0)
            {
                var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
                var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, prefs)).FirstOrDefault();
                if (pick != null) await CardCmd.Exhaust(choiceContext, pick);
            }
        }

        if (Stage >= 7)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        if (Stage >= 8)
        {
            var drawn = await CardPileCmd.Draw(choiceContext, Owner);
            if (drawn != null)
            {
                CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);
            }
        }

        if (Stage >= 9)
        {
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        if (Stage >= 10)
        {
            await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, this);
        }

        if (Stage >= 11)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

public class Card056_TothSlateI() : TothSlateBase<Card056_1_TothSlateIi>(CardRarity.Rare, TargetType.None)
{
    protected override int Stage => 1;
}

[Pool(typeof(TokenCardPool))]
public class Card056_1_TothSlateIi() : TothSlateBase<Card056_2_TothSlateIii>(CardRarity.Token, TargetType.Self)
{
    protected override int Stage => 2;
}

[Pool(typeof(TokenCardPool))]
public class Card056_2_TothSlateIii() : TothSlateBase<Card056_3_TothSlateIv>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 3;
}

[Pool(typeof(TokenCardPool))]
public class Card056_3_TothSlateIv() : TothSlateBase<Card056_4_TothSlateV>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 4;
}

[Pool(typeof(TokenCardPool))]
public class Card056_4_TothSlateV() : TothSlateBase<Card056_5_TothSlateVi>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 5;
}

[Pool(typeof(TokenCardPool))]
public class Card056_5_TothSlateVi() : TothSlateBase<Card056_6_TothSlateVii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 6;
}

[Pool(typeof(TokenCardPool))]
public class Card056_6_TothSlateVii() : TothSlateBase<Card056_7_TothSlateViii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 7;
}

[Pool(typeof(TokenCardPool))]
public class Card056_7_TothSlateViii() : TothSlateBase<Card056_8_TothSlateIx>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 8;
}

[Pool(typeof(TokenCardPool))]
public class Card056_8_TothSlateIx() : TothSlateBase<Card056_9_TothSlateX>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 9;
}

[Pool(typeof(TokenCardPool))]
public class Card056_9_TothSlateX() : TothSlateBase<Card056_10_TothSlateXi>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 10;
}

[Pool(typeof(TokenCardPool))]
public class Card056_10_TothSlateXi() : TothSlateBase<Card056_11_TothSlateXii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 11;
}

[Pool(typeof(TokenCardPool))]
public class Card056_11_TothSlateXii() : TothSlateBase<Card056_12_TothSlateXiii>(CardRarity.Token, TargetType.AnyEnemy)
{
    protected override int Stage => 12;

    public override void AfterCreated()
    {
        base.AfterCreated();
        BaseReplayCount = 2;
    }

    protected override void AfterDeserialized()
    {
        base.AfterDeserialized();
        BaseReplayCount = 2;
    }

    public override void AfterTransformedTo()
    {
        base.AfterTransformedTo();
        BaseReplayCount = 2;
    }
}

[Pool(typeof(TokenCardPool))]
public class Card056_12_TothSlateXiii() : MinorisCard(3, CardType.Power, CardRarity.Token, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.TothSlateXIIIPower>(Owner.Creature, 3, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
