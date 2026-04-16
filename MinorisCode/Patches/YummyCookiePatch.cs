namespace Minoris.MinorisCode.Patches;

[HarmonyPatch]
public static class YummyCookiePatch
{
    private const string CookiePath = "MINORIS-YUMMY_COOKIE";
    private const string MissingRelicIconPath = "res://images/powers/missing_power.png";

    [HarmonyPatch(typeof(YummyCookie), "IconBaseName", MethodType.Getter)]
    [HarmonyPrefix]
    private static bool IconBaseNamePrefix(YummyCookie __instance, ref string __result)
    {
        if (__instance.IsCanonical || __instance.Owner == null)
        {
            return true;
        }

        if (__instance.Owner.Character is Minoris.MinorisCode.Character.Minoris)
        {
            __result = CookiePath;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "PackedIconPath", MethodType.Getter)]
    [HarmonyPrefix]
    private static bool PackedIconPathPrefix(RelicModel __instance, ref string __result)
    {
        if (__instance is not YummyCookie cookie)
        {
            return true;
        }

        if (cookie.IsCanonical || cookie.Owner == null)
        {
            return true;
        }

        if (cookie.Owner.Character is Minoris.MinorisCode.Character.Minoris)
        {
            var idEntry = CookiePath;
            var atlas = $"res://atlases/relic_atlas.sprites/{idEntry.RemovePrefix().ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlas))
            {
                __result = atlas;
            }
            else
            {
                var explicitPath = $"{idEntry}_S.png".RelicImagePath();
                if (ResourceLoader.Exists(explicitPath))
                {
                    __result = explicitPath;
                }
                else
                {
                    var legacyPath = $"{idEntry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
                    __result = ResourceLoader.Exists(legacyPath) ? legacyPath : MissingRelicIconPath;
                }
            }

            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "PackedIconOutlinePath", MethodType.Getter)]
    [HarmonyPrefix]
    private static bool PackedIconOutlinePathPrefix(RelicModel __instance, ref string __result)
    {
        if (__instance is not YummyCookie cookie)
        {
            return true;
        }

        if (cookie.IsCanonical || cookie.Owner == null)
        {
            return true;
        }

        if (cookie.Owner.Character is Minoris.MinorisCode.Character.Minoris)
        {
            var idEntry = CookiePath;
            var atlas = $"res://atlases/relic_outline_atlas.sprites/{idEntry.RemovePrefix().ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlas))
            {
                __result = atlas;
            }
            else
            {
                var explicitPath = $"{idEntry}_S_outline.png".RelicImagePath();
                if (ResourceLoader.Exists(explicitPath))
                {
                    __result = explicitPath;
                }
                else
                {
                    var legacyPath = $"{idEntry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
                    __result = ResourceLoader.Exists(legacyPath) ? legacyPath : MissingRelicIconPath;
                }
            }

            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "BigIconPath", MethodType.Getter)]
    [HarmonyPrefix]
    private static bool BigIconPathPrefix(RelicModel __instance, ref string __result)
    {
        if (__instance is not YummyCookie cookie)
        {
            return true;
        }

        if (cookie.IsCanonical || cookie.Owner == null)
        {
            return true;
        }

        if (cookie.Owner.Character is Minoris.MinorisCode.Character.Minoris)
        {
            var idEntry = CookiePath;
            var explicitPath = $"{idEntry}_B.png".BigRelicImagePath();
            if (ResourceLoader.Exists(explicitPath))
            {
                __result = explicitPath;
            }
            else
            {
                var legacyPath = $"{idEntry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
                if (ResourceLoader.Exists(legacyPath))
                {
                    __result = legacyPath;
                }
                else
                {
                    var smallExplicitPath = $"{idEntry}_S.png".RelicImagePath();
                    if (ResourceLoader.Exists(smallExplicitPath))
                    {
                        __result = smallExplicitPath;
                    }
                    else
                    {
                        var smallLegacyPath = $"{idEntry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
                        __result = ResourceLoader.Exists(smallLegacyPath) ? smallLegacyPath : MissingRelicIconPath;
                    }
                }
            }

            return false;
        }

        return true;
    }
}
