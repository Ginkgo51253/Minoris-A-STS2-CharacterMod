namespace Minoris.MinorisCode.Patches;

[HarmonyPatch]
public static class CommonPatch
{
	[HarmonyPatch(typeof(ProgressSaveManager))]
    [HarmonyPatch("ObtainCharUnlockEpoch")]
    [HarmonyPatch([typeof(Player), typeof(int)])]
    public static class ProgressSaveManager_ObtainCharUnlockEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return localPlayer.Character is not Minoris.MinorisCode.Character.Minoris;
        }
    }

	[HarmonyPatch(typeof(ProgressSaveManager))]
    [HarmonyPatch("CheckFifteenElitesDefeatedEpoch")]
    [HarmonyPatch([typeof(Player)])]
    public static class ProgressSaveManager_CheckFifteenElitesDefeatedEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return localPlayer.Character is not Minoris.MinorisCode.Character.Minoris;
        }
    }

	[HarmonyPatch(typeof(ProgressSaveManager))]
    [HarmonyPatch("CheckFifteenBossesDefeatedEpoch")]
    [HarmonyPatch([typeof(Player)])]
    public static class ProgressSaveManager_CheckFifteenBossesDefeatedEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return !(localPlayer.Character.Id.ToString().Contains("Minoris", StringComparison.OrdinalIgnoreCase));
        }
    }
}
