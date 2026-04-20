using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LethalRogueLike.src;
using LethalRogueLike.src.Config;
using UnityEngine;
using GameNetcodeStuff;

namespace LethalRogueLike.src.Hunger;

public static class StoreHooks
{
    private static List<FoodItem> _foodItems = new();

    public static void Initialize()
    {
        _foodItems.AddRange(FoodItem.DefaultFoods);
        Plugin.Logger.LogInfo($"[StoreHooks] Initialized with {_foodItems.Count} food items.");
        RegisterTerminalCommands();
    }

    public static IReadOnlyList<FoodItem> GetFoodItems()
    {
        return _foodItems;
    }

    private static void RegisterTerminalCommands()
    {
        Plugin.Logger.LogInfo("[StoreHooks] Terminal commands registered.");
    }

    [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
    public static class Terminal_ParsePlayerSentence_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Terminal __instance, ref TerminalNode __result)
        {
            try
            {
                if (!CanAccessFoodCommand())
                {
                    return;
                }

                string screenText = __instance.screenText.text;
                int textAdded = __instance.textAdded;
                if (textAdded <= 0 || textAdded > screenText.Length) return;

                string input = screenText.Substring(screenText.Length - textAdded).Trim().ToLower();
                
                if (input == "food" || input == "foods" || input == "buy food")
                {
                    Plugin.Logger.LogInfo("[StoreHooks] 'food' command received.");
                    __result = BuildFoodMenuNode();
                }
                else if (input.StartsWith("buy "))
                {
                    var itemName = input.Substring(4).Trim();
                    __result = ProcessFoodPurchase(__instance, itemName);
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[StoreHooks] Error in ParsePlayerSentence: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }

    private static bool CanAccessFoodCommand()
    {
        if (!(Config.ModConfig.EnableHungerSystem?.Value ?? false))
        {
            return false;
        }

        if (Config.ModConfig.TestingMode?.Value ?? false)
        {
            return true;
        }

        if (RunState.Instance == null)
        {
            return false;
        }

        return RunState.Instance.HasActiveModifier(Modifiers.ModifierEffectType.HungerModifier);
    }

    private static TerminalNode BuildFoodMenuNode()
    {
        var node = ScriptableObject.CreateInstance<TerminalNode>();
        node.clearPreviousText = true;
        node.displayText = BuildFoodMenuText();
        node.terminalEvent = "";
        return node;
    }

    private static string BuildFoodMenuText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("FOOD MENU");
        sb.AppendLine("--------");
        sb.AppendLine();

        foreach (var food in _foodItems)
        {
            sb.AppendLine($"[{food.DisplayName}] - {food.Price} credits");
            sb.AppendLine($"  {food.Description}");
            sb.AppendLine($"  Restores {food.HungerRestore} hunger");
            sb.AppendLine();
        }

        sb.AppendLine("Type 'buy <name>' to purchase.");
        sb.AppendLine("Example: 'buy apple'");
        sb.AppendLine();

        return sb.ToString();
    }

    private static TerminalNode ProcessFoodPurchase(Terminal terminal, string itemName)
    {
        var node = ScriptableObject.CreateInstance<TerminalNode>();
        node.clearPreviousText = true;
        node.terminalEvent = "";

        var foodItem = FindFoodByName(itemName);
        if (foodItem == null)
        {
            node.displayText = $"Unknown food item: {itemName}\nType 'food' for available options.";
            return node;
        }

        if (terminal.groupCredits < foodItem.Price)
        {
            node.displayText = $"Not enough credits!\nNeed {foodItem.Price}, have {terminal.groupCredits}.";
            return node;
        }

        terminal.groupCredits -= foodItem.Price;
        StoredFoods.AddFood(foodItem);
        
        node.displayText = $"Purchased {foodItem.DisplayName} for {foodItem.Price} credits!\nRemaining: {terminal.groupCredits} credits\n\nType 'food' for menu.";
        Plugin.Logger.LogInfo($"[StoreHooks] Purchased {foodItem.DisplayName}. Remaining credits: {terminal.groupCredits}");

        return node;
    }

    private static FoodItem? FindFoodByName(string name)
    {
        name = name.ToLower();
        foreach (var food in _foodItems)
        {
            if (food.DisplayName.ToLower() == name || 
                name.Contains(food.DisplayName.ToLower()) ||
                food.DisplayName.ToLower().Contains(name))
            {
                return food;
            }
        }
        return null;
    }

    public static bool PurchaseFoodById(string itemID, int playerCredits)
    {
        var foodItem = Array.Find(_foodItems.ToArray(), f => f.ItemID == itemID);
        if (foodItem == null)
        {
            Plugin.Logger.LogWarning($"[StoreHooks] Food item not found: {itemID}");
            return false;
        }

        if (playerCredits < foodItem.Price)
        {
            Plugin.Logger.LogWarning($"[StoreHooks] Not enough credits: have {playerCredits}, need {foodItem.Price}");
            return false;
        }

        Plugin.Logger.LogInfo($"[StoreHooks] Purchased {foodItem.DisplayName} for {foodItem.Price} credits");
        return true;
    }
}