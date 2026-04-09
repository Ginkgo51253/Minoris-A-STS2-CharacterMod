namespace Minoris.MinorisCode.Relics;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<SmallBowTie>().Id) __result = ModelDb.Relic<TidySmallBowTie>().ToMutable();
    }
}

//[HarmonyPatch(typeof(CharacterModel), "IconOutlineTexturePath", MethodType.Getter)]
//public static class MinorisIconOutlineTexturePathPatch
//{
//    private static bool Prefix(CharacterModel __instance, ref string? __result)
//    {
//        if (__instance is not Minoris.MinorisCode.Character.Minoris)
//            return true;
//
//        var outlinePath = "character_icon_minoris_outline.png".CharacterUiPath();
//        __result = ResourceLoader.Exists(outlinePath) ? outlinePath : "character_icon_minoris.png".CharacterUiPath();
//        return false;
//    }
//}
