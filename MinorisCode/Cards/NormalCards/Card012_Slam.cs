
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD012_SLAM
中文名称: 猛击
英文名称: Slam
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 2
卡牌效果: 造成20点伤害
卡牌描述(ZHS): 造成20点伤害
卡牌描述(ENG): Deal 20 damage
升级效果: 费用-1
*/
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









