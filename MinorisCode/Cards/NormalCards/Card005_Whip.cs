
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD005_WHIP
中文名称: 鞭打
英文名称: Whip
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 造成6点伤害。给予1层易伤
卡牌描述(ZHS): 造成6点伤害。给予1层易伤
卡牌描述(ENG): Deal 6 damage. Apply 1 Vulnerable
升级效果: VulnerablePower+1
*/
public class Card005_Whip() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["VulnerablePower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}









