using System;

namespace LethalRogueLike.src.Modifiers.Effects;

public class EnemySpawnEffect : IModifierEffect
{
    public ModifierEffectType EffectType => ModifierEffectType.EnemySpawnModifier;

    public void Apply()
    {
        Plugin.Logger.LogDebug("EnemySpawnEffect applied");
    }

    public void Remove()
    {
        Plugin.Logger.LogDebug("EnemySpawnEffect removed");
    }
}
