namespace Minoris.MinorisCode.Potions;

public class BottledSandstorm : MinorisPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        var tgt = target ?? Owner.Creature;
        await PowerCmd.Apply<Minoris.MinorisCode.Powers.SandstormPower>(tgt, 1m, Owner.Creature, null);
    }
}

