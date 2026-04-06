﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD027_OFFERING_OF_SOBEK
中文名称: 索贝克的供奉
英文名称: Offering of Sobek
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 2
卡牌效果: 对所有敌人造成12点伤害。选择一张手牌消耗。本卡的数值永久提升等同于消耗卡牌的费用的数值
卡牌描述(ZHS): 对所有敌人造成12点伤害。选择一张手牌消耗。本卡的数值永久提升等同于消耗卡牌的费用的数值
卡牌描述(ENG): Deal 12 damage to ALL enemies. Exhaust 1 card from your hand. Permanently increase this card's value by that card's cost
升级效果: 费用-1
*/
public class Card027_OfferingOfSobek() : MinorisCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private int _permanentBonus;

    [SavedProperty]
    public int PermanentBonus
    {
        get => _permanentBonus;
        set
        {
            AssertMutable();
            _permanentBonus = value;
            DynamicVars.ExtraDamage.BaseValue = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(0m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(CalcDamageMultiplier)
    ];
    
    private static decimal CalcDamageMultiplier(CardModel card, Creature? target) => 1m;
    
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
        var total = (int)DynamicVars.CalculationBase.BaseValue + (int)DynamicVars.ExtraDamage.BaseValue;
        await DamageCmd.Attack(total).FromCard(this).TargetingAllOpponents(CombatState).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









