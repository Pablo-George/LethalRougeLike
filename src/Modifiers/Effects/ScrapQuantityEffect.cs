using System;

namespace LethalRogueLike.src.Modifiers.Effects;

public class ScrapQuantityEffect : IModifierEffect
{
    public ModifierEffectType EffectType => ModifierEffectType.ScrapQuantityModifier;

    public void Apply()
    {
        Plugin.Logger.LogDebug("ScrapQuantityEffect applied");
    }

    public void Remove()
    {
        Plugin.Logger.LogDebug("ScrapQuantityEffect removed");
    }
}
