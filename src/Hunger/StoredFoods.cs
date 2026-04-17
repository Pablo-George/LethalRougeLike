using System;
using System.Collections.Generic;

namespace LethalRogueLike.src.Hunger;

public static class StoredFoods
{
    private static List<FoodItem> _foods = new();

    public static void AddFood(FoodItem food)
    {
        _foods.Add(food);
        Plugin.Logger.LogInfo($"[StoredFoods] Added {food.DisplayName}. Total: {_foods.Count}");
    }

    public static void RemoveFood(string itemID)
    {
        var food = _foods.Find(f => f.ItemID == itemID);
        if (food != null)
        {
            _foods.Remove(food);
            Plugin.Logger.LogInfo($"[StoredFoods] Removed {food.DisplayName}. Total: {_foods.Count}");
        }
    }

    public static IReadOnlyList<FoodItem> GetFoods()
    {
        return _foods;
    }

    public static void Clear()
    {
        _foods.Clear();
    }

    public static void AddDefaultFoods()
    {
        foreach (var food in FoodItem.DefaultFoods)
        {
            _foods.Add(food);
        }
    }
}