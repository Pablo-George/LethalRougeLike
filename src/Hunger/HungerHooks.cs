using System;
using HarmonyLib;
using LethalRogueLike.src.Config;
using UnityEngine;
using GameNetcodeStuff;

namespace LethalRogueLike.src.Hunger;

public static class HungerHooks
{
    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[HungerHooks] Initializing hunger system hooks.");
    }

    [HarmonyPatch(typeof(StartOfRound), "StartGame")]
    public class StartOfRound_StartGame_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                if (!Config.ModConfig.EnableHungerSystem?.Value ?? false)
                {
                    return;
                }

                var manager = HungerManager.Instance;
                if (manager != null)
                {
                    manager.Initialize();
                    Plugin.Logger.LogInfo("[HungerHooks] HungerManager initialized on game start.");
                }

                var ui = HungerUI.Instance;
                if (ui == null)
                {
                    var uiGo = new UnityEngine.GameObject("HungerUI");
                    uiGo.AddComponent<HungerUI>();
                    Plugin.Logger.LogInfo("[HungerHooks] Created HungerUI.");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[HungerHooks] Error in StartGame: {ex.Message}");
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
                var manager = HungerManager.Instance;
                if (manager != null)
                {
                    manager.Reset();
                    Plugin.Logger.LogInfo("[HungerHooks] HungerManager reset on round end.");
                }

                StoredFoods.Clear();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[HungerHooks] Error in ShipLeave: {ex.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public class PlayerControllerB_Update_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(PlayerControllerB __instance)
        {
            try
            {
                if (!Config.ModConfig.EnableHungerSystem?.Value ?? false)
                {
                    return;
                }

                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F3))
                {
                    var manager = HungerManager.Instance;
                    if (manager != null)
                    {
                        var foods = StoredFoods.GetFoods();
                        if (foods.Count > 0)
                        {
                            manager.ConsumeNextFood();
                            Plugin.Logger.LogDebug("[HungerHooks] Consumed food via keybind");
                        }
                        else
                        {
                            Plugin.Logger.LogInfo("[HungerHooks] No food to consume");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[HungerHooks] Error in Update: {ex.Message}");
            }
        }
    }
}