
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD008_IMPROVISED_STRIKE
中文名称: 即兴打击
英文名称: Improvised Strike
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: CardTag.Strike
费用: 1
卡牌效果: 造成12点伤害。将另一张卡名带有打击的牌置入你的手牌
卡牌描述(ZHS): 造成12点伤害。将另一张卡名带有打击的牌置入你的手牌
卡牌描述(ENG): Deal 12 damage. Add another card with "Strike" in its name to your hand
升级效果: 费用-1
*/
public class Card008_ImprovisedStrike() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        bool IsStrikeName(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return false;
            return title.Contains("打击") || title.Contains("Strike", StringComparison.OrdinalIgnoreCase);
        }

        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.GetType() != typeof(Card008_ImprovisedStrike))
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .Where(c => c.Tags.Contains(CardTag.Strike) || IsStrikeName(c.Title))
            .ToList();
        if (candidates.Count > 0)
        {
            var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
            var card = CombatState.CreateCard(pick, Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, false, CardPilePosition.Top));
        }
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









