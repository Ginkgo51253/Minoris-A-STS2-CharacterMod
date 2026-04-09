namespace Minoris.MinorisCode.Powers;

public class ApophisArrivesSelfDamagePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsInstanced => true;
    private int _turnsUntilSelfDamage = 3;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("Turns", 3)];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        _turnsUntilSelfDamage = System.Math.Max(0, _turnsUntilSelfDamage - 1);
        if (DynamicVars.TryGetValue("Turns", out var v))
        {
            v.BaseValue = _turnsUntilSelfDamage;
        }
        if (_turnsUntilSelfDamage == 0)
        {
            await CreatureCmd.Damage(choiceContext, new[] { Owner }, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
            Flash();
            _turnsUntilSelfDamage = 3;
            if (DynamicVars.TryGetValue("Turns", out var v2))
            {
                v2.BaseValue = _turnsUntilSelfDamage;
            }
        }
    }
}
