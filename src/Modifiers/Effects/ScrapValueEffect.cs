using System;

namespace LethalRogueLike.src.Modifiers.Effects;

public class ScrapValueEffect : IModifierEffect
{
    public ModifierEffectType EffectType => ModifierEffectType.ScrapValueModifier;

    public void Apply()
    {
        Plugin.Logger.LogDebug("ScrapValueEffect applied");
    }

    public void Remove()
    {
        Plugin.Logger.LogDebug("ScrapValueEffect removed");
    }
}
