
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD009_TESTING_BLOW
中文名称: 试探一击
英文名称: Testing Blow
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 造成4点伤害。获得2点活力
卡牌描述(ZHS): 造成4点伤害。获得2点活力
卡牌描述(ENG): Deal 4 damage. Gain 2 Vigor
升级效果: 伤害+1；VigorPower+1
*/
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









