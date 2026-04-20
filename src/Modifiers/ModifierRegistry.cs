using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalRogueLike.src.Modifiers;

public class ModifierRegistry
{
    public static ModifierRegistry Instance { get; private set; } = new ModifierRegistry();

    private readonly Dictionary<string, Modifier> _modifiers = new();
    private readonly Random _random = new();

    public void RegisterModifier(Modifier modifier)
    {
        try
        {
            if (string.IsNullOrEmpty(modifier?.Id))
            {
                Plugin.Logger.LogWarning("[ModifierRegistry] Cannot register modifier: Id is null or empty!");
                return;
            }
            if (_modifiers.ContainsKey(modifier.Id))
            {
                Plugin.Logger.LogWarning($"[ModifierRegistry] Modifier already registered: {modifier.Id}");
                return;
            }
            _modifiers[modifier.Id] = modifier;
            Plugin.Logger.LogDebug($"[ModifierRegistry] Registered: {modifier.Id} ('{modifier.Name}')");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[ModifierRegistry] Failed to register modifier: {ex.Message}");
        }
    }

    public void RegisterBuiltInModifiers()
    {
        Plugin.Logger.LogInfo("[ModifierRegistry] Registering built-in modifiers...");

        try
        {
            RegisterModifier(new Modifier
            {
                Id = "reduced_scrap_value",
                Name = "Less Loot",
                Description = "Scrap is worth 30% less credits",
                IsDebuff = true,
                EffectType = ModifierEffectType.ScrapValueModifier,
                Severity = 0.7f,
                Effect = new Effects.ScrapValueEffect()
            });

            RegisterModifier(new Modifier
            {
                Id = "reduced_stamina",
                Name = "Tiring Work",
                Description = "25% less stamina available",
                IsDebuff = true,
                EffectType = ModifierEffectType.StaminaModifier,
                Severity = 0.75f,
                Effect = new Effects.StaminaEffect()
            });

            RegisterModifier(new Modifier
            {
                Id = "increased_enemy_spawn",
                Name = "Hostile Planet",
                Description = "30% more enemies spawn",
                IsDebuff = true,
                EffectType = ModifierEffectType.EnemySpawnModifier,
                Severity = 1.3f,
                Effect = new Effects.EnemySpawnEffect()
            });

RegisterModifier(new Modifier
            {
                Id = "increased_scrap_qty",
                Name = "Scrap Rush",
                Description = "50% more scrap items spawn",
                IsDebuff = false,
                EffectType = ModifierEffectType.ScrapQuantityModifier,
                Severity = 1.5f,
                Effect = new Effects.ScrapQuantityEffect()
            });

            RegisterModifier(new Modifier
            {
                Id = "hunger",
                Name = "Hunger",
                Description = "You must eat food to survive. Buy food at the terminal.",
                IsDebuff = true,
                EffectType = ModifierEffectType.HungerModifier,
                Severity = 1f,
                Effect = new Effects.HungerEffect()
            });

        Plugin.Logger.LogInfo($"[ModifierRegistry] Successfully registered {_modifiers.Count} modifiers.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[ModifierRegistry] Error registering built-in modifiers: {ex.Message}");
        }
    }

    public Modifier GetModifier(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                Plugin.Logger.LogWarning("[ModifierRegistry] GetModifier called with null/empty id!");
                return null!;
            }

            if (_modifiers.TryGetValue(id, out var modifier))
            {
                return modifier;
            }

            Plugin.Logger.LogWarning($"[ModifierRegistry] Modifier not found: {id}");
            return null!;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[ModifierRegistry] Error getting modifier: {ex.Message}");
            return null!;
        }
    }

    public List<Modifier> GetAllModifiers()
    {
        try
        {
            return _modifiers.Values.ToList();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[ModifierRegistry] Error getting all modifiers: {ex.Message}");
            return new List<Modifier>();
        }
    }

    public List<Modifier> GetRandomChoices(int count)
    {
        try
        {
            var all = _modifiers.Values.ToList();

            if (all.Count == 0)
            {
                Plugin.Logger.LogError("[ModifierRegistry] No modifiers registered! Cannot get choices.");
                return new List<Modifier>();
            }

            if (all.Count <= count)
            {
                Plugin.Logger.LogDebug($"[ModifierRegistry] Returning all {all.Count} modifiers (less than requested {count})");
                return all;
            }

            var hungerMod = _modifiers.TryGetValue("hunger", out var hm) ? hm : null;
            var forceHunger = Config.ModConfig.ForceHungerModifier?.Value ?? false;

            List<Modifier> shuffled;
            if (forceHunger && hungerMod != null && count > 0)
            {
                var others = all.Where(m => m.Id != "hunger").OrderBy(_ => _random.Next()).Take(count - 1).ToList();
                others.Insert(0, hungerMod);
                shuffled = others;
                Plugin.Logger.LogDebug($"[ModifierRegistry] Testing mode: Forced hunger modifier into first choice.");
            }
            else
            {
                shuffled = all.OrderBy(_ => _random.Next()).Take(count).ToList();
            }

            Plugin.Logger.LogDebug($"[ModifierRegistry] Selected {shuffled.Count} random modifiers from {all.Count} total.");
            return shuffled;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[ModifierRegistry] Error getting random choices: {ex.Message}");
            return new List<Modifier>();
        }
    }
}
