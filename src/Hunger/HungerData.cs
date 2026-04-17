using System;

namespace LethalRogueLike.src.Hunger;

public class HungerData
{
    public float CurrentHunger { get; set; } = 100f;
    public float MaxHunger { get; } = 100f;
    public float HungerDepletionRate { get; set; } = 1f;
    public float StarvationDamageRate { get; set; } = 5f;
    public bool IsStarving => CurrentHunger <= 0f;
    public bool DamagePending { get; set; }

    public HungerData()
    {
        CurrentHunger = MaxHunger;
    }

    public void SetHunger(float value)
    {
        CurrentHunger = Math.Clamp(value, 0f, MaxHunger);
    }

    public float GetHungerPercentage()
    {
        return CurrentHunger / MaxHunger;
    }
}