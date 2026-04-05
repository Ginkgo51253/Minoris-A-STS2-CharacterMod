
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD050_BANDAGE_HEAL
中文名称: 包扎愈合
英文名称: Bandage Heal
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 回复 {Heal:diff()} 点生命。获得 {Block:diff()} 点格挡。
卡牌描述(ZHS): 回复 {Heal:diff()} 点生命。获得 {Block:diff()} 点格挡。
卡牌描述(ENG): Heal {Heal:diff()} HP. Gain {Block:diff()} Block.
升级效果: Heal+2；格挡+2
*/
public class Card050_BandageHeal() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(3m),
        new BlockVar(3m, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}









