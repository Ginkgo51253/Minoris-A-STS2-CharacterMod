using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Minoris.MinorisCode.Cards;
using Minoris.MinorisCode.Character;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Relics;

[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    private static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic.Id == ModelDb.Relic<SmallBowTie>().Id) __result = ModelDb.Relic<TidySmallBowTie>().ToMutable();
    }
}

[HarmonyPatch(typeof(CharacterModel), "IconOutlineTexturePath", MethodType.Getter)]
public static class MinorisIconOutlineTexturePathPatch
{
    private static bool Prefix(CharacterModel __instance, ref string? __result)
    {
        if (__instance is not Minoris.MinorisCode.Character.Minoris)
            return true;

        var outlinePath = "character_icon_minoris_outline.png".CharacterUiPath();
        __result = ResourceLoader.Exists(outlinePath) ? outlinePath : "character_icon_minoris.png".CharacterUiPath();
        return false;
    }
}
