using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models.Enchantments;
namespace Minoris.MinorisCode.Cards;


/*
本地键: 
中文名称: 
英文名称: 
卡牌类型: CardType.Skill
卡牌稀有度: rarity
tag标签: 
关键字: CardKeyword.Exhaust, CardKeyword.Ethereal
费用: 3
卡牌描述(ZHS): 
卡牌描述(ENG): 
升级效果(代码): EnergyCost.UpgradeBy(-1);
*/
public abstract class TothSlateBase<TNext>(CardRarity rarity, TargetType targetType) :
    MinorisCard(3, CardType.Skill, rarity, targetType)
    where TNext : CardModel
{
    private const string BlockKey = "Block";
    private const string DamageKey = "Damage";
    private const string EnergyKey = "Energy";
    private const string HealKey = "Heal";
    private const string StrengthKey = "Strength";
    private const string DrawKey = "Draw";
    private const string DexterityKey = "Dexterity";
    private const string VigorKey = "Vigor";
    private const string ArtifactKey = "Artifact";

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new IntVar(BlockKey, 5),
        new IntVar(DamageKey, 5),
        new EnergyVar(EnergyKey, 1),
        new IntVar(HealKey, 2),
        new IntVar(StrengthKey, 1),
        new IntVar(DrawKey, 1),
        new IntVar(DexterityKey, 1),
        new IntVar(VigorKey, 4),
        new IntVar(ArtifactKey, 1)
    ];

    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    public override bool ShouldReceiveCombatHooks => true;
    private bool _wasPlayedThisCombat;
    private int _upgradeLevelWhenPlayed;

    protected abstract int Stage { get; }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip>();
            if (Stage >= 2) tips.Add(HoverTipFactory.Static(StaticHoverTip.Block));
            if (Stage >= 4) tips.Add(HoverTipFactory.ForEnergy(this));
            if (Stage >= 7) tips.Add(HoverTipFactory.FromPower<StrengthPower>());
            if (Stage >= 8) tips.Add(HoverTipFactory.FromPower<DexterityPower>());
            if (Stage >= 9) tips.Add(HoverTipFactory.FromPower<VigorPower>());
            if (Stage >= 10) tips.Add(HoverTipFactory.FromPower<ArtifactPower>());
            tips.Add(HoverTipFactory.FromKeyword(CardKeyword.Exhaust));
            return tips;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _wasPlayedThisCombat = true;
        _upgradeLevelWhenPlayed = CurrentUpgradeLevel;
        await ApplyStageEffects(choiceContext, cardPlay);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (!_wasPlayedThisCombat) return;
        if (DeckVersion == null) return;
        var result = await CardCmd.TransformTo<TNext>(DeckVersion);
        if (result == null) return;
        var added = result.Value.cardAdded;

        while (_upgradeLevelWhenPlayed > 0 && added.IsUpgradable)
        {
            CardCmd.Upgrade(added);
            _upgradeLevelWhenPlayed--;
        }

        var fromEnchant = DeckVersion.Enchantment;
        if (fromEnchant != null && typeof(TNext) != typeof(Card056_12_TothSlateXiii))
        {
            var cloned = (EnchantmentModel)fromEnchant.ClonePreservingMutability();
            CardCmd.Enchant(cloned, added, fromEnchant.Amount);
        }
    }

    protected virtual async Task ApplyStageEffects(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Stage >= 2)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars[BlockKey].IntValue, ValueProp.Move, cardPlay);
        }

        if (Stage >= 3)
        {
            if (cardPlay.Target != null)
            {
                await DamageCmd.Attack(DynamicVars[DamageKey].IntValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
            }
        }

        if (Stage >= 4)
        {
            await PlayerCmd.GainEnergy(DynamicVars[EnergyKey].IntValue, Owner);
        }

        if (Stage >= 5)
        {
            await CreatureCmd.Heal(Owner.Creature, DynamicVars[HealKey].IntValue);
        }

        if (Stage >= 6)
        {
            var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
            if (hand.Count > 0)
            {
                CardSelectorPrefs prefs = new(new LocString("cards", "SELECT_TO_EXHAUST"), 1);
                var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, prefs);
                var chosen = selected.FirstOrDefault();
                if (chosen != null)
                {
                    await CardCmd.Exhaust(choiceContext, chosen);
                }
            }
        }

        if (Stage >= 7)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars[StrengthKey].IntValue, Owner.Creature, this);
        }

        if (Stage >= 8)
        {
            var drawCount = DynamicVars[DrawKey].IntValue;
            for (var d = 0; d < drawCount; d++)
            {
                var drawn = await CardPileCmd.Draw(choiceContext, Owner);
                if (drawn != null) CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);
            }
        }

        if (Stage >= 9)
        {
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, DynamicVars[DexterityKey].IntValue, Owner.Creature, this);
        }

        if (Stage >= 10)
        {
            await PowerCmd.Apply<VigorPower>(Owner.Creature, DynamicVars[VigorKey].IntValue, Owner.Creature, this);
        }

        if (Stage >= 11)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars[ArtifactKey].IntValue, Owner.Creature, this);
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
