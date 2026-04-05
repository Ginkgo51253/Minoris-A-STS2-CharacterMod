
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD046_CATAPULT_START
中文名称: 弹射起步
英文名称: Catapult Start
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 抽 2 张牌。这张牌可以无限次升级。
卡牌描述(ZHS): 抽 2 张牌。这张牌可以无限次升级。
卡牌描述(ENG): Draw 2 cards. Can be upgraded infinitely.
升级效果: Cards+1
*/
public class Card046_CatapultStart() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override int MaxUpgradeLevel => int.MaxValue;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}









