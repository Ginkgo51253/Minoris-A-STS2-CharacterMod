
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD026_DIVINE_LIGHT
中文名称: 神之光
英文名称: Divine Light
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。本场战斗中你每打出过一张“抓挠”攻击牌，本牌额外造成 1 次伤害。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。本场战斗中你每打出过一张“抓挠”攻击牌，本牌额外造成 1 次伤害。
卡牌描述(ENG): Deal {Damage:diff()} damage. For each "Scratch" Attack you played this combat, deal 1 additional hit.
升级效果: 伤害+3
*/
public class Card026_DivineLight() : MinorisCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DamageVar(6m, ValueProp.Move),
         new CalculationBaseVar(1m),
         new CalculationExtraVar(1m),
         new CalculatedVar(HitsKey).WithMultiplier(CalcHitsMultiplier)];
    

    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var history = CombatManager.Instance.History.CardPlaysStarted;
        var scratchPlays = history.Count(e => e.CardPlay.Card.Owner == Owner && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.GetType().Name.Contains("Scratch"));
        var hits = 1 + scratchPlays;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    private static decimal CalcHitsMultiplier(CardModel card, Creature? target)
    {
        if (!CombatManager.Instance.IsInProgress) return 0m;
        var history = CombatManager.Instance.History.CardPlaysStarted;
        return history.Count(e => e.CardPlay.Card.Owner == card.Owner && e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Card.GetType().Name.Contains("Scratch"));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}








