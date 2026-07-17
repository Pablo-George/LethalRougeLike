using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LethalRogueLike.src.Hooks;

public static class LandingHooks
{
    public static void Initialize()
    {
        var methods = typeof(StartOfRound)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .Distinct()
            .OrderBy(n => n);
        Plugin.Logger.LogInfo("[LandingHooks] StartOfRound methods: " + string.Join(", ", methods));
    }

    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    public class StartOfRound_StartGame_Patch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value)
                {
                    return;
                }

                RunState.Instance.StartNewRun();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[LandingHooks] Error in StartGame: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
    public class StartOfRound_OnShipLanded_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value)
                {
                    return;
                }

                Plugin.Logger.LogDebug("[LandingHooks] OnShipLanded triggered.");
                RunState.Instance.OnShipLanded();

                if (RunState.Instance.IsSelectionPending)
                {
                    UpdateClientCount();
                    Plugin.Logger.LogInfo("[LandingHooks] Modifier selection pending - use 'mods' command in terminal to select");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[LandingHooks] Error in OnShipLanded patch: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
    public class StartOfRound_EndOfRound_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value)
                {
                    return true;
                }

                if (RunState.Instance.IsSelectionPending)
                {
                    Plugin.Logger.LogWarning("[LandingHooks] Cannot leave - modifier selection pending!");
                    return false;
                }

                Plugin.Logger.LogDebug("[LandingHooks] EndOfRound triggered.");
                RunState.Instance.OnRunEnd();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[LandingHooks] Error in EndOfRound patch: {ex.Message}");
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "Update")]
    public class StartOfRound_Update_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(StartOfRound __instance)
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value)
                {
                    return;
                }

                if (RunState.Instance.IsSelectionPending)
                {
                    var shipLeverField = typeof(StartOfRound).GetField("lever", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (shipLeverField != null)
                    {
                        var lever = shipLeverField.GetValue(__instance);
                        if (lever != null)
                        {
                            var leverType = lever.GetType();
                            var interactTriggerField = leverType.GetField("hoveringOverInfo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (interactTriggerField != null)
                            {
                                var interactTrigger = interactTriggerField.GetValue(lever);
                                if (interactTrigger != null)
                                {
                                    var descField = interactTrigger.GetType().GetField("descriptionText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                    if (descField != null)
                                    {
                                        var currentDesc = descField.GetValue(interactTrigger) as string;
                                        if (!string.IsNullOrEmpty(currentDesc) && !currentDesc.Contains("Select a modifier"))
                                        {
                                            descField.SetValue(interactTrigger, currentDesc + " - SELECT MODIFIER FIRST!");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently ignore - hover text update
            }
        }
    }

    private static void UpdateClientCount()
    {
        try
        {
            var handler = Networking.NetworkHandler.Instance;
            if (handler != null && handler.IsHost)
            {
                var clientCount = Unity.Netcode.NetworkManager.Singleton?.ConnectedClientsList.Count ?? 1;
                RunState.Instance.SetConnectedClientsCount(clientCount);
                Plugin.Logger.LogDebug($"[LandingHooks] Connected clients: {clientCount}");
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[LandingHooks] Error updating client count: {ex.Message}");
        }
    }
}