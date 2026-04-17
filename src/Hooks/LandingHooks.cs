using System;
using System.Linq;
using HarmonyLib;

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
                    System.Console.WriteLine("DEBUG: IsSelectionPending is TRUE, about to show UI");
                    UpdateClientCount();
                    Plugin.Logger.LogDebug("[LandingHooks] Modifier selection pending after landing, showing UI...");
                    
                    var ui = UI.ModifierSelectionUI.Instance;
                    System.Console.WriteLine("DEBUG: ModifierSelectionUI.Instance=" + (ui != null ? "OK" : "NULL"));
                    
                    if (ui == null)
                    {
                        System.Console.WriteLine("DEBUG: Creating new UI on demand...");
                        Plugin.Logger.LogInfo("[LandingHooks] Creating new ModifierSelectionUI on demand...");
                        var newGo = new UnityEngine.GameObject("ModifierSelectionUI");
                        newGo.AddComponent<UI.ModifierSelectionUI>();
                        ui = UI.ModifierSelectionUI.Instance;
                        System.Console.WriteLine("DEBUG: After creation, Instance=" + (ui != null ? "OK" : "NULL"));
                    }
                    
                    if (ui != null)
                    {
                        ui.ShowSelection(RunState.Instance.PendingChoices.ToArray());
                    }
                    else
                    {
                        Plugin.Logger.LogError("[LandingHooks] Failed to create Instance!");
                    }
                }
                else
                {
                    System.Console.WriteLine("DEBUG: IsSelectionPending is FALSE");
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
        public static void Prefix()
        {
            try
            {
                if (!Config.ModConfig.EnableMod.Value)
                {
                    return;
                }

                Plugin.Logger.LogDebug("[LandingHooks] EndOfRound triggered.");
                RunState.Instance.OnRunEnd();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[LandingHooks] Error in EndOfRound patch: {ex.Message}");
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