namespace Minoris.MinorisCode.Relics;

public class GildedSphinx : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (Owner?.Creature == null) return;
        if (card.Owner != Owner) return;
        if (Owner.Creature.IsDead) return;
        CombatState? combat = Owner.Creature.CombatState;
        if (combat == null) return;

        var candidates = Owner.Character.CardPool
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .Where(c => !c.EnergyCost.CostsX)
            .Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
            .ToList();
        if (candidates.Count == 0) return;

        CardModel? pickedModel = Owner.RunState.Rng.CombatCardGeneration.NextItem(candidates);
        if (pickedModel == null) return;

        Flash();
        CardModel generated = combat.CreateCard(pickedModel, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(generated, PileType.Hand, addedByPlayer: true, CardPilePosition.Top);
    }
}
