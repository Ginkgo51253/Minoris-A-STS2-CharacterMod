
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD045_COMMANDING
中文名称: 居高临下
英文名称: Commanding
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 抽 {Cards:diff()} 张牌，然后升级它们。
卡牌描述(ZHS): 抽 {Cards:diff()} 张牌，然后升级它们。
卡牌描述(ENG): Draw {Cards:diff()} cards, then upgrade them.
升级效果: Cards+2
*/
public class Card045_Commanding() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawnCards = (await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner)).ToList();
        foreach (var c in drawnCards) CardCmd.Upgrade(c);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2m);
    }
}









