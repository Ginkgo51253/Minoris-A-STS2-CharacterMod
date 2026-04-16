namespace Minoris.MinorisCode.Patches;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<SmallBowTie>().Id) __result = ModelDb.Relic<TidySmallBowTie>().ToMutable();
    }
}
