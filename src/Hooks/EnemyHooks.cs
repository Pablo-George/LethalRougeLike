using System;
using HarmonyLib;

namespace LethalRogueLike.src.Hooks;

public static class EnemyHooks
{
    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[EnemyHooks] Initialized.");
    }

    [HarmonyPatch(typeof(RoundManager), "RefreshEnemiesList")]
    public class RoundManager_RefreshEnemiesList_Patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value) return;
                if (!RunState.Instance.HasActiveModifier(Modifiers.ModifierEffectType.EnemySpawnModifier)) return;

                var multiplier = RunState.Instance.GetCombinedSeverity(Modifiers.ModifierEffectType.EnemySpawnModifier);
                Plugin.Logger.LogInfo($"[EnemyHooks] Enemy spawn modifier ACTIVE. Spawn rate multiplier: {multiplier}x");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[EnemyHooks] Error in RefreshEnemiesList patch: {ex.Message}");
            }
        }
    }

}
