using System;

namespace LethalRogueLike.src.Hunger;

[Serializable]
public class FoodItem
{
    public string ItemID { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public float HungerRestore { get; set; } = 25f;
    public int Price { get; set; } = 10;
    public string Description { get; set; } = string.Empty;

    public static FoodItem[] DefaultFoods => new FoodItem[]
    {
        new FoodItem
        {
            ItemID = "food_apple",
            DisplayName = "Apple",
            HungerRestore = 15f,
            Price = 5,
            Description = "A fresh red apple. Restores 15 hunger."
        },
        new FoodItem
        {
            ItemID = "food_sandwich",
            DisplayName = "Sandwich",
            HungerRestore = 30f,
            Price = 15,
            Description = "A hearty sandwich. Restores 30 hunger."
        },
        new FoodItem
        {
            ItemID = "food_meat",
            DisplayName = "Premium Steak",
            HungerRestore = 50f,
            Price = 30,
            Description = "A premium steak. Restores 50 hunger."
        }
    };
}