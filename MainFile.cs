using Godot;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot.Collections;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Array = Godot.Collections.Array;

namespace Minoris;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Minoris";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, LogType.Generic);

    [CustomEnum("GRIP")]
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Grip;

    public static void Initialize()
    {
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Logger.Error($"Unobserved task exception: {e.Exception}");
            e.SetObserved();
        };
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        PatchAllProgressSaveManagerMethods(harmony);
    }

    private static void PatchAllProgressSaveManagerMethods(Harmony harmony)
    {
        var prefix = new HarmonyMethod(typeof(MainFile), nameof(SkipForCustomCharacter));
        foreach (var method in AccessTools.GetDeclaredMethods(typeof(ProgressSaveManager)))
        {
            var ps = method.GetParameters();
            if (ps.Length > 0 && ps[0].ParameterType == typeof(Player))
            {
                try
                {
                    harmony.Patch(method, prefix: prefix);
                    Logger.Info($"Patched ProgressSaveManager.{method.Name}");
                }
                catch (Exception e)
                {
                    Logger.Warn($"Failed to patch ProgressSaveManager.{method.Name}: {e.Message}");
                }
            }
        }
    }

    private static bool SkipForCustomCharacter(Player localPlayer)
    {
        return localPlayer.Character is not ICustomModel;
    }
}

[HarmonyPatch(typeof(CreatureCmd))]
public static class CreatureCmdPatches
{
    [HarmonyPatch(nameof(CreatureCmd.Kill), new[] { typeof(IReadOnlyCollection<Creature>), typeof(bool) })]
    [HarmonyPostfix]
    public static void Kill_Postfix(ref Task __result)
    {
        __result = Wrap(__result);
    }

    private static async Task Wrap(Task original)
    {
        try
        {
            await original;
            if (!CombatManager.Instance.IsInProgress) return;
            await CombatManager.Instance.CheckWinCondition();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Error in Kill wrap: {e}");
        }
    }

    [HarmonyPatch("KillWithoutCheckingWinCondition")]
    [HarmonyPrefix]
    private static bool KillWithoutCheckingWinCondition_Prefix(Creature creature, bool force, int recursion, ref Task __result)
    {
        if (creature == null)
        {
            __result = Task.CompletedTask;
            return false;
        }
        if (creature.CombatState != null)
        {
            return true;
        }
        if (!creature.IsPlayer)
        {
            __result = Task.CompletedTask;
            return false;
        }
        __result = KillPlayerWithoutCombatState(creature, force, recursion);
        return false;
    }

    private static async Task KillPlayerWithoutCombatState(Creature creature, bool force, int recursion)
    {
        if (recursion >= 10)
        {
            throw new InvalidOperationException("Combat is ending, but something is continually preventing the last creature from being killed!");
        }
        IRunState runState = creature.Player?.RunState ?? IRunState.GetFrom(new[] { creature });
        int currentHp = creature.CurrentHp;
        if (currentHp > 0)
        {
            creature.LoseHpInternal(currentHp, ValueProp.Unblockable | ValueProp.Unpowered);
            await Hook.AfterCurrentHpChanged(runState, null, creature, -currentHp);
        }
        await Hook.BeforeDeath(runState, null, creature);
        AbstractModel? preventer = null;
        if (force || creature.MaxHp <= 0 || Hook.ShouldDie(runState, null, creature, out preventer))
        {
            creature.InvokeDiedEvent();
            await Hook.AfterDeath(runState, null, creature, wasRemovalPrevented: false, 0f);
            foreach (PowerModel item in creature.RemoveAllPowersAfterDeath() ?? Enumerable.Empty<PowerModel>())
            {
                await item.AfterRemoved(creature);
            }
            creature.Player?.DeactivateHooks();
        }
        else
        {
            await Hook.AfterDeath(runState, null, creature, wasRemovalPrevented: true, 0f);
            if (preventer != null)
            {
                await Hook.AfterPreventingDeath(runState, null, preventer, creature);
            }
            if (creature.IsDead)
            {
                await KillPlayerWithoutCombatState(creature, force, recursion + 1);
            }
        }
    }
}

[HarmonyPatch(typeof(NRelic), nameof(NRelic.Model), MethodType.Getter)]
public static class NRelicModelGetterPatch
{
    private static readonly FieldInfo RelicModel = AccessTools.Field(typeof(NRelic), "_model");

    [HarmonyPrefix]
    private static bool Prefix(NRelic __instance, ref RelicModel __result)
    {
        if (RelicModel.GetValue(__instance) is RelicModel model) return true;

        __result = ModelDb.Relic<Akabeko>();
        return false;
    }
}

public partial class NTestParticlesContainer : NParticlesContainer
{
    public override void _Ready()
    {
        base._Ready();
        Traverse.Create(this).Field("_particles").SetValue(new Array<GpuParticles2D>());
    }
}