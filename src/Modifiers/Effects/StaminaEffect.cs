using System;

namespace LethalRogueLike.src.Modifiers.Effects;

public class StaminaEffect : IModifierEffect
{
    public ModifierEffectType EffectType => ModifierEffectType.StaminaModifier;

    public void Apply()
    {
        Plugin.Logger.LogDebug("StaminaEffect applied");
    }

    public void Remove()
    {
        Plugin.Logger.LogDebug("StaminaEffect removed");
    }
}
