﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD022_JUSTICE
中文名称: 正义
英文名称: Justice
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 3
卡牌效果: 造成 {Damage:diff()} 点伤害。获得 {Block:diff()} 点格挡。消耗你抽牌堆、手牌、弃牌堆中所有 0 费牌。每消耗一张，本牌额外造成 {BonusPerCard:diff()} 点伤害并额外获得 {BonusPerCard:diff()} 点格挡。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。获得 {Block:diff()} 点格挡。消耗你抽牌堆、手牌、弃牌堆中所有 0 费牌。每消耗一张，本牌额外造成 {BonusPerCard:diff()} 点伤害并额外获得 {BonusPerCard:diff()} 点格挡。
卡牌描述(ENG): Deal {Damage:diff()} damage. Gain {Block:diff()} Block. Exhaust ALL 0-cost cards in your draw pile, hand, and discard pile. For each exhausted card, gain +{BonusPerCard:diff()} damage and +{BonusPerCard:diff()} Block.
升级效果: 伤害+2；格挡+2；每消耗一张，本牌额外造成伤害并额外获得格挡+2
*/
public class Card022_Justice() : MinorisCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string BonusPerCardKey = "BonusPerCard";
    private int _combatBonus;
    private const string CalcDamageKey = "CalculatedDamage";
    private const string CalcBlockKey = "CalculatedBlock";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new BlockVar(3m, ValueProp.Move),
        new IntVar(BonusPerCardKey, 3),
        new ExtraDamageVar(1m),
        new CalculationBaseVar(3m),
        new CalculationExtraVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CalcFinalBonusMultiplier),
        new CalculatedBlockVar(ValueProp.Move).WithMultiplier(CalcFinalBonusMultiplier)
    ];

    private static decimal CalcFinalBonusMultiplier(CardModel card, Creature? target)
    {
        if (card is Card022_Justice c) return c._combatBonus;
        return 0m;
    }

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
                if (cost == 0 && !c.EnergyCost.CostsX) zeroCostCards.Add(c);
            }
        }

        var bonusPerCard = DynamicVars[BonusPerCardKey].IntValue;
        var addThisPlay = bonusPerCard * zeroCostCards.Count;
        _combatBonus += addThisPlay;

        foreach (var c in zeroCostCards)
            await CardCmd.Exhaust(choiceContext, c);

        // Step 2: Deal damage once with total amount (base + accumulated bonus)
        var totalDamage = DynamicVars.Damage.IntValue + _combatBonus;
        await DamageCmd.Attack(totalDamage).FromCard(this).Targeting(cardPlay.Target)
            //.WithHitFx("vfx/vfx_attack_slash")//似乎是特效相关
            .Execute(choiceContext);
        // Step 3: Gain block once with total amount (base + accumulated bonus)
        var totalBlock = DynamicVars.Block.IntValue + _combatBonus;
        await CreatureCmd.GainBlock(Owner.Creature, totalBlock, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars[BonusPerCardKey].UpgradeValueBy(2m);
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
    }
}




