using System;
using HarmonyLib;
using UnityEngine;

namespace LethalRogueLike.src.Hooks;

public static class ScrapHooks
{
    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[ScrapHooks] Initialized.");
    }

    [HarmonyPatch(typeof(GrabbableObject), "Start")]
    public class GrabbableObject_Start_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(GrabbableObject __instance)
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value) return;
                if (!RunState.Instance.HasActiveModifier(Modifiers.ModifierEffectType.ScrapValueModifier)) return;

                if (__instance == null)
                {
                    Plugin.Logger.LogWarning("[ScrapHooks] GrabbableObject instance is null in Start patch!");
                    return;
                }

                var scrapValueField = AccessTools.Field(typeof(GrabbableObject), "scrapValue");
                if (scrapValueField == null)
                {
                    Plugin.Logger.LogWarning("[ScrapHooks] Could not find scrapValue field!");
                    return;
                }

                var originalValue = (int)scrapValueField.GetValue(__instance)!;
                var multiplier = RunState.Instance.GetCombinedSeverity(Modifiers.ModifierEffectType.ScrapValueModifier);
                var newValue = Mathf.RoundToInt(originalValue * multiplier);
                scrapValueField.SetValue(__instance, newValue);
                Plugin.Logger.LogDebug($"[ScrapHooks] Modified scrap value: {originalValue} -> {newValue} (multiplier: {multiplier})");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[ScrapHooks] Error in GrabbableObject Start patch: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(GrabbableObject), "SetScrapValue")]
    public class GrabbableObject_SetScrapValue_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(GrabbableObject __instance, ref int setValueTo)
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value) return;
                if (!RunState.Instance.HasActiveModifier(Modifiers.ModifierEffectType.ScrapValueModifier)) return;

                var multiplier = RunState.Instance.GetCombinedSeverity(Modifiers.ModifierEffectType.ScrapValueModifier);
                var originalValue = setValueTo;
                setValueTo = Mathf.RoundToInt(setValueTo * multiplier);
                Plugin.Logger.LogDebug($"[ScrapHooks] SetScrapValue: {originalValue} -> {setValueTo} (multiplier: {multiplier})");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[ScrapHooks] Error in SetScrapValue patch: {ex.Message}");
            }
        }
    }
}
