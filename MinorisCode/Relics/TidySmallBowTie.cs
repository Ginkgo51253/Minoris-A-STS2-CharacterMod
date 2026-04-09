namespace Minoris.MinorisCode.Relics;

[Pool(typeof(DeprecatedRelicPool))]
public class TidySmallBowTie : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner?.Creature == null) return;
        if (side != Owner.Creature.Side) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList();
        if (hand.Count == 0) return;
        var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(hand);
        var deck = PileType.Deck.GetPile(Owner);
        var deckCard = deck?.Cards.FirstOrDefault(c => c.Id == pick.Id);
        if (deckCard != null && !ReferenceEquals(deckCard, pick) && deckCard.IsUpgradable)
        {
            CardCmd.Upgrade(deckCard, CardPreviewStyle.None);
        }
        CardCmd.Upgrade(pick, CardPreviewStyle.None);
        Flash();
        await Task.CompletedTask;
    }
}
