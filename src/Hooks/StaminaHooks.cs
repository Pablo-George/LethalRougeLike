using System;
using HarmonyLib;
using GameNetcodeStuff;

namespace LethalRogueLike.src.Hooks;

public static class StaminaHooks
{
    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[StaminaHooks] Initialized.");
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class PlayerControllerB_Update_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(PlayerControllerB __instance)
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value) return;
                if (!RunState.Instance.HasActiveModifier(Modifiers.ModifierEffectType.StaminaModifier)) return;

                if (__instance == null)
                {
                    Plugin.Logger.LogWarning("[StaminaHooks] PlayerControllerB instance is null!");
                    return;
                }

                var staminaValueField = AccessTools.Field(typeof(PlayerControllerB), "stamina");
                if (staminaValueField == null)
                {
                    Plugin.Logger.LogWarning("[StaminaHooks] Could not find stamina field!");
                    return;
                }

                var currentStamina = (float)staminaValueField.GetValue(__instance)!;
                var multiplier = RunState.Instance.GetCombinedSeverity(Modifiers.ModifierEffectType.StaminaModifier);
                var maxStamina = currentStamina / multiplier;
                staminaValueField.SetValue(__instance, UnityEngine.Mathf.Min(currentStamina, maxStamina));
                Plugin.Logger.LogDebug($"[StaminaHooks] Stamina modified: current={currentStamina}, max={maxStamina} (multiplier: {multiplier})");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[StaminaHooks] Error in PlayerControllerB Update patch: {ex.Message}");
            }
        }
    }
}
