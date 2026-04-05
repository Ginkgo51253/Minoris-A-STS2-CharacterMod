
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD035_BIND
中文名称: 束缚
英文名称: Bind
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 给予一名敌人 {GripPower:diff()} 层牵制。
卡牌描述(ZHS): 给予一名敌人 {GripPower:diff()} 层牵制。
卡牌描述(ENG): Apply {GripPower:diff()} Grip.
升级效果: 给予一名敌人层牵制+1
*/
public class Card035_Bind() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<Powers.GripPower>(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<Powers.GripPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await PowerCmd.Apply<Powers.GripPower>(cardPlay.Target, DynamicVars["GripPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GripPower"].UpgradeValueBy(1m);
    }
}









