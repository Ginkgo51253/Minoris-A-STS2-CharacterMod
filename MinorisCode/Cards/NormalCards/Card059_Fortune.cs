
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD059_FORTUNE
中文名称: 强运
英文名称: Fortune
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 4
卡牌效果: 从你的职业卡中选择一张{IfUpgraded:show:[gold]升级过[/gold]的|}牌并打出。
卡牌描述(ZHS): 从你的职业卡中选择一张{IfUpgraded:show:[gold]升级过[/gold]的|}牌并打出。
卡牌描述(ENG): Choose a{IfUpgraded:show: [gold]Upgraded[/gold]|} class card and play it.
升级效果: 选出的牌会以升级状态被打出
*/
public class Card059_Fortune() : MinorisCard(4, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.Rarity != CardRarity.Token && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event)
            .ToList();
        if (candidates.Count == 0) return;

        var options = new List<CardModel>();
        foreach (var m in candidates)
        {
            var created = CombatState.CreateCard(m, Owner);
            if (IsUpgraded) CardCmd.Upgrade(created);
            options.Add(created);
        }
        if (CombatManager.Instance.IsEnding) return;
        var pick = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            options,
            Owner,
            new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_HEADER"), 1)
        )).FirstOrDefault();

        foreach (var card in options)
        {
            if (card != pick)
            {
                CombatState.RemoveCard(card);
            }
        }

        if (pick != null) await CardCmd.AutoPlay(choiceContext, pick, null);
    }
}









