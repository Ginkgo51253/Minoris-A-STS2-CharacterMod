
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD020_CALL_OF_JACKAL
中文名称: 胡狼的召唤
英文名称: Call of the Jackal
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。这张牌每打出一次，本场战斗中其造成伤害的次数+1。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。这张牌每打出一次，本场战斗中其造成伤害的次数+1。
卡牌描述(ENG): Deal {Damage:diff()} damage {Hits:diff()} times. Each time you play this, it hits 1 additional time this combat.
升级效果: 伤害+4
*/
public class Card020_CallOfJackal() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string HitsKey = "Hits";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DamageVar(7m, ValueProp.Move),
         new IntVar(HitsKey, 1)];
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









