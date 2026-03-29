
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD021_MASTER_CRAFT
中文名称: 高超武艺
英文名称: Master Craft
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 2
卡牌效果: 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。这张牌可以无限次升级。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。这张牌可以无限次升级。
卡牌描述(ENG): Deal {Damage:diff()} damage {Hits:diff()} times. Can be upgraded infinitely.
升级效果: 造成{Damage:diff()}点伤害次+1
*/
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









