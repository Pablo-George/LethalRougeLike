using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalRogueLike.src.Modifiers;

public class Modifier
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDebuff { get; set; }
    public ModifierEffectType EffectType { get; set; }
    public float Severity { get; set; }
    public IModifierEffect Effect { get; set; } = null!;
}

public enum ModifierEffectType
{
    ScrapValueModifier,
    StaminaModifier,
    EnemySpawnModifier,
    ScrapQuantityModifier
}

public interface IModifierEffect
{
    ModifierEffectType EffectType { get; }
    void Apply();
    void Remove();
}
