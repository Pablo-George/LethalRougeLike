using System;
using HarmonyLib;
using LethalRogueLike.src.Config;
using UnityEngine;
using UnityEngine.InputSystem;
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
                var isTestingMode = Config.ModConfig.TestingMode?.Value ?? false;
                var isHungerEnabled = Config.ModConfig.EnableHungerSystem?.Value ?? false;

                if (!isHungerEnabled && !isTestingMode)
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

                var kb = Keyboard.current;
                if (kb == null) return;

                if (kb[Key.F3].wasPressedThisFrame)
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

                if (kb[Key.F4].wasPressedThisFrame)
                {
                    var mgr = HungerManager.Instance;
                    var ui = HungerUI.Instance;
                    var isEnabled = Config.ModConfig.EnableHungerSystem?.Value;
                    var showBar = Config.ModConfig.ShowHungerBar?.Value;
                    var testing = Config.ModConfig.TestingMode?.Value;
                    Plugin.Logger.LogInfo("===== [HungerDebug] F4 dump =====");
                    Plugin.Logger.LogInfo($"  EnableHungerSystem = {isEnabled}");
                    Plugin.Logger.LogInfo($"  ShowHungerBar      = {showBar}");
                    Plugin.Logger.LogInfo($"  TestingMode        = {testing}");
                    Plugin.Logger.LogInfo($"  HungerManager      = {(mgr != null ? "exists" : "NULL")}");
                    if (mgr != null)
                    {
                        var data = mgr.GetHungerData();
                        Plugin.Logger.LogInfo($"  HungerData         = {(data != null ? $"{data.CurrentHunger:F1}/{data.MaxHunger} ({data.GetHungerPercentage()*100:F0}%)" : "NULL")}");
                        Plugin.Logger.LogInfo($"  IsStarving         = {mgr.IsStarving()}");
                    }
                    Plugin.Logger.LogInfo($"  HungerUI           = {(ui != null ? "exists" : "NULL")}");
                    var canvasObj = UnityEngine.GameObject.Find("HungerBarCanvas");
                    Plugin.Logger.LogInfo($"  HungerBarCanvas    = {(canvasObj != null ? $"found, active={canvasObj.activeSelf}" : "NOT FOUND in scene")}");
                    Plugin.Logger.LogInfo($"  Foods in inventory = {StoredFoods.GetFoods().Count}");
                    Plugin.Logger.LogInfo("=================================");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[HungerHooks] Error in Update: {ex.Message}");
            }
        }
    }
}