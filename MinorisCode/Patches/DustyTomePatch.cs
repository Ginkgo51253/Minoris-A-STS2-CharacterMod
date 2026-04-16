namespace Minoris.MinorisCode.Patches;

[HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
public static class DustyTomePatch
{
    private static bool Prefix(DustyTome __instance, Player player)
    {
        if (player.Character is Minoris.MinorisCode.Character.Minoris)
        {
            __instance.AncientCard = ModelDb.Card<Card075_WildForm>().Id;
            return false;
        }

        return true;
    }
}