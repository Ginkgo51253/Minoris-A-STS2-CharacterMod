
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD043_CITY_OF_PILLARS
中文名称: 千柱之城
英文名称: City of Pillars
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 0
卡牌效果: 变化你的所有手牌
卡牌描述(ZHS): 变化你的所有手牌
卡牌描述(ENG): Transform ALL cards in your hand
升级效果: 移除消耗
*/
public class Card043_CityOfPillars() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Transform)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        var options = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token);
        foreach (var c in hand)
        {
            await CardCmd.Transform(new CardTransformation(c, options).Yield(), Owner.RunState.Rng.CombatCardSelection);
        }
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}









