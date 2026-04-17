using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace LethalRogueLike.src.Config;

public static class ModConfig
{
    public static ConfigEntry<bool> EnableMod { get; private set; } = null!;
    public static ConfigEntry<SelectionMethodType> SelectionMethod { get; private set; } = null!;
    public static ConfigEntry<int> SelectionTimeout { get; private set; } = null!;
    public static ConfigEntry<bool> DefaultOnTimeout { get; private set; } = null!;
    public static ConfigEntry<KeyCode> OverlayHotkey { get; private set; } = null!;

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

        var gameplaySection = "Gameplay";
        OverlayHotkey = Plugin.Instance.Config.Bind(gameplaySection, "OverlayHotkey", KeyCode.F2,
            "Hotkey to view active modifiers during gameplay");

        Plugin.Logger.LogInfo("Configuration loaded");
    }
}
