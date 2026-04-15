
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD053_ALCHEMICAL_CRUCIBLE
中文名称: 炼金坩埚
英文名称: Alchemical Crucible
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 选择两张手牌消耗，然后随机获得一张牌
卡牌描述(ZHS): 选择两张手牌消耗，然后随机获得一张牌
卡牌描述(ENG): Exhaust 2 cards in your hand, then add a random card to your hand
升级效果: 费用-1
*/
public class Card053_AlchemicalCrucible() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private const string ExhaustCountKey = "ExhaustCount";
    private const string GainCountKey = "GainCount";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar(ExhaustCountKey, 2),
        new IntVar(GainCountKey, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
        if (hand.Count > 0)
        {
            var exhaustCount = DynamicVars[ExhaustCountKey].IntValue;
            var toExhaust = hand.Count >= exhaustCount
                ? await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, exhaustCount))
                : hand;
            foreach (var c in toExhaust) await CardCmd.Exhaust(choiceContext, c);
        }
        if (CombatState == null) return;
        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .ToList();
        if (candidates.Count > 0)
        {
            var gainCount = DynamicVars[GainCountKey].IntValue;
            for (var i = 0; i < gainCount; i++)
            {
                var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
                var generated = CombatState.CreateCard(pick, Owner);
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, false, CardPilePosition.Top));
            }
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}








