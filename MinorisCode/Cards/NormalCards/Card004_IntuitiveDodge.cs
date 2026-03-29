
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD004_INTUITIVE_DODGE
中文名称: 直觉闪避
英文名称: Intuitive Dodge
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Basic
tag标签: 
费用: 0
卡牌效果: 获得 {Block:diff()} 点格挡。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。
卡牌描述(ENG): Gain {Block:diff()} Block.
升级效果: 格挡+4
*/
public class Card004_IntuitiveDodge() : MinorisCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}









