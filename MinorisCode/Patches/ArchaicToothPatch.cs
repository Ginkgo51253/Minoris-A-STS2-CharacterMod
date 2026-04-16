namespace Minoris.MinorisCode.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddMinorisTranscendence(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<Card003_InstinctScratch>().Id] = ModelDb.Card<Card074_WildScratch>();
    }
}