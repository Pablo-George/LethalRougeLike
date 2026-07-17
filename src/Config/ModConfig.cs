using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;

namespace LethalRogueLike.src.Config;

public static class ModConfig
{
    public static ConfigEntry<bool> EnableMod { get; private set; } = null!;
    public static ConfigEntry<SelectionMethodType> SelectionMethod { get; private set; } = null!;
    public static ConfigEntry<int> SelectionTimeout { get; private set; } = null!;
    public static ConfigEntry<bool> DefaultOnTimeout { get; private set; } = null!;

    public static Dictionary<string, ConfigEntry<bool>> ModifierEnabledStates { get; private set; } = new();

    public enum SelectionMethodType
    {
        HostOnly,
        RandomAuto
    }

    public static void Load()
    {
        var section = "General";

        EnableMod = Plugin.Instance.Config.Bind(section, "EnableMod", true,
            "Enable or disable the modifier system");

        SelectionMethod = Plugin.Instance.Config.Bind(section, "SelectionMethod", SelectionMethodType.HostOnly,
            "How modifiers are selected: HostOnly (host picks) or RandomAuto (random selection)");

        SelectionTimeout = Plugin.Instance.Config.Bind(section, "SelectionTimeout", 30,
            "Seconds to wait before auto-selecting a modifier (0 = disabled)");

        DefaultOnTimeout = Plugin.Instance.Config.Bind(section, "DefaultOnTimeout", true,
            "Automatically select first option on timeout");

        Plugin.Logger.LogInfo("Configuration loaded");
    }

    public static void InitializeModifierSettings(IEnumerable<string> modifierIds)
    {
        var section = "Modifiers";

        foreach (var modId in modifierIds)
        {
            if (ModifierEnabledStates.ContainsKey(modId))
                continue;

            var entry = Plugin.Instance.Config.Bind(section, modId, false,
                $"Enable or disable the '{modId}' modifier");

            ModifierEnabledStates[modId] = entry;
            Plugin.Logger.LogDebug($"[ModConfig] Registered setting for modifier: {modId}");
        }

        Plugin.Logger.LogInfo($"[ModConfig] Initialized {ModifierEnabledStates.Count} modifier settings");
    }

    public static bool IsModifierEnabled(string modifierId)
    {
        if (ModifierEnabledStates.TryGetValue(modifierId, out var entry))
        {
            return entry.Value;
        }
        return false;
    }

    public static void SetModifierEnabled(string modifierId, bool enabled)
    {
        if (ModifierEnabledStates.TryGetValue(modifierId, out var entry))
        {
            entry.Value = enabled;
            Plugin.Logger.LogInfo($"[ModConfig] Set modifier '{modifierId}' enabled: {enabled}");
        }
        else
        {
            Plugin.Logger.LogWarning($"[ModConfig] Cannot set modifier '{modifierId}' - not registered");
        }
    }
}
