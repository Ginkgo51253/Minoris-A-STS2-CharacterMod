
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD023_SOLAR_FLARE
中文名称: 太阳耀斑
英文名称: Solar Flare
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 3
卡牌效果: 造成 {Damage:diff()} 点伤害。选择一张手牌消耗，本牌费用在本场战斗中减少该牌费用的数值。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。选择一张手牌消耗，本牌费用在本场战斗中减少该牌费用的数值。
卡牌描述(ENG): Deal {Damage:diff()} damage. Exhaust 1 card from your hand. Reduce this card's cost this combat by that card's cost.
升级效果: 伤害+10
*/
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
                var cost = Math.Max(0, pick.EnergyCost.GetWithModifiers(CostModifiers.Local));
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









