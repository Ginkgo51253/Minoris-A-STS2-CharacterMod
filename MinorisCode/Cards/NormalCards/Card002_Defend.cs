
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD002_DEFEND
中文名称: 防御
英文名称: Defend
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Basic
tag标签: CardTag.Defend
费用: 1
卡牌效果: 获得 {Block:diff()} 点格挡。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。
卡牌描述(ENG): Gain {Block:diff()} Block.
升级效果: 格挡+3
*/
public class Card002_Defend() : MinorisCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}









