
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD057_EYE_CHARM
中文名称: 眼之护符
英文名称: Eye Charm
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 获得 {Block:diff()} 点格挡。在格挡被击破前，你的格挡不会自动消失。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。在格挡被击破前，你的格挡不会自动消失。
卡牌描述(ENG): Gain {Block:diff()} Block. Until your Block is broken, your Block does not disappear.
升级效果: 格挡+10
*/
public class Card057_EyeCharm() : MinorisCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(20m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<Powers.BlockPreservePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(10m);
    }
}









