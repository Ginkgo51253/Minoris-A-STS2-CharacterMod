namespace Minoris.MinorisCode.Potions;

public class BottledSalt : MinorisPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var affected = target?.IsPlayer == true ? target.Player : Owner;
        var hand = PileType.Hand.GetPile(affected).Cards;
        var maxSelect = hand.Count;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, maxSelect);
        var selected = (await CardSelectCmd.FromHand(choiceContext, affected, prefs, c => c.IsTransformable, this)).ToList();
        if (selected.Count == 0) return;

        var options = affected.Character.CardPool
            .GetUnlockedCards(affected.UnlockState, affected.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token);

        foreach (var c in selected)
        {
            await CardCmd.Transform(new CardTransformation(c, options).Yield(), affected.RunState.Rng.CombatCardSelection);
        }
    }
}
