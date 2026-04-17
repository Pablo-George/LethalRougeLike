using System;
using System.Collections.Generic;
using HarmonyLib;
using LethalRogueLike.src.Config;

namespace LethalRogueLike.src.Hunger;

public static class StoreHooks
{
    private static List<FoodItem> _foodItems = new();

    public static void Initialize()
    {
        _foodItems.AddRange(FoodItem.DefaultFoods);
        Plugin.Logger.LogInfo($"[StoreHooks] Initialized with {_foodItems.Count} food items.");
    }

    public static IReadOnlyList<FoodItem> GetFoodItems()
    {
        return _foodItems;
    }

    public static bool PurchaseFood(string itemID, int playerCredits)
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