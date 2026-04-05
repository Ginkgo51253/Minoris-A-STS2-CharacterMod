
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD075_WILD_FORM
中文名称: 狂野形态
英文名称: Wild Form
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Ancient
tag标签: 
费用: 3
卡牌效果: 使攻击牌的耗能降低为 0。你的攻击牌具有虚无。
卡牌描述(ZHS): 使攻击牌的耗能降低为 0。你的攻击牌具有虚无。
卡牌描述(ENG): Set the cost of your Attacks to 0. Your Attacks are Ethereal.
升级效果: 移除虚无
*/
public class Card075_WildForm() : MinorisCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Minoris.MinorisCode.Powers.WildFormPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}









