
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD037_EVADE
中文名称: 回避
英文名称: Evade
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 3
卡牌效果: 失去 3 点生命。获得 {Block:diff()} 点格挡。
卡牌描述(ZHS): 失去 3 点生命。获得 {Block:diff()} 点格挡。
卡牌描述(ENG): Lose 3 HP. Gain {Block:diff()} Block.
升级效果: 格挡+6
*/
public class Card037_Evade() : MinorisCard(3, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(24m, ValueProp.Move)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    private int _LoseHp = 3;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Retain),
         HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, new[] { Owner.Creature }, _LoseHp, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
    }
}








