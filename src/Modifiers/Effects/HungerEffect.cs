using System;

namespace LethalRogueLike.src.Modifiers.Effects;

public class HungerEffect : IModifierEffect
{
    public ModifierEffectType EffectType => ModifierEffectType.HungerModifier;

    public void Apply()
    {
        Plugin.Logger.LogDebug("HungerEffect applied");
    }

    public void Remove()
    {
        Plugin.Logger.LogDebug("HungerEffect removed");
    }
}