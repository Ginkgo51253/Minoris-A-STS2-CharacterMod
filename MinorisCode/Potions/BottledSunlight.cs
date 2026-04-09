namespace Minoris.MinorisCode.Potions;

public class BottledSunlight : MinorisPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var affected = target?.IsPlayer == true ? target.Player : Owner;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var drawCards = PileType.Draw.GetPile(affected).Cards.OrderBy(c => c.Rarity).ThenBy(c => c.Id.Entry).ToList();
        var chosen = (await CardSelectCmd.FromSimpleGrid(choiceContext, drawCards, affected, prefs)).FirstOrDefault();
        if (chosen == null) return;

        await CardPileCmd.Add(chosen, PileType.Hand);
        chosen.EnergyCost.SetThisTurnOrUntilPlayed(0);
    }
}
