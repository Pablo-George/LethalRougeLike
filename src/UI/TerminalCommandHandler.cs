using System;
using System.Linq;
using LethalRogueLike.src.Modifiers;

namespace LethalRogueLike.src.UI;

public static class TerminalCommandHandler
{
    private static bool _initialized = false;
    private static bool _terminalExtenderAvailable = false;

    public static void Initialize()
    {
        if (_initialized)
        {
            Plugin.Logger.LogWarning("[Terminal] Initialize called multiple times!");
            return;
        }

        _initialized = true;

        try
        {
            RegisterTerminalCommand();
            _terminalExtenderAvailable = true;
            Plugin.Logger.LogInfo("[Terminal] Successfully registered terminal commands");
        }
        catch (Exception ex)
        {
            _terminalExtenderAvailable = false;
            Plugin.Logger.LogWarning($"[Terminal] LethalTerminalExtender not found - terminal commands disabled: {ex.Message}");
        }
    }

    private static void RegisterTerminalCommand()
    {
        try
        {
            var terminalExtenderType = Type.GetType("LethalAPI.TerminalExtender.Utils.TerminalExtenderUtils, LethalTerminalExtender");
            if (terminalExtenderType == null)
            {
                throw new Exception("LethalTerminalExtender assembly not loaded");
            }

            var addQuickCommand = terminalExtenderType.GetMethod("AddQuickCommand", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (addQuickCommand == null)
            {
                throw new Exception("AddQuickCommand method not found");
            }

            var terminalType = Type.GetType("Terminal, Assembly-CSharp");
            var nodeType = Type.GetType("TerminalNode, Assembly-CSharp");

            if (terminalType == null || nodeType == null)
            {
                throw new Exception("Terminal or TerminalNode type not found in game assemblies");
            }

            var keyword = "mods";
            var displayText = "Browse and enable/disable run modifiers";
            var showInDefault = true;

            var actionType = typeof(Action<,>).MakeGenericType(terminalType, nodeType);
            var methodInfo = typeof(TerminalCommandHandler).GetMethod(nameof(OnModsCommandInternal), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!;
            var callbackDelegate = System.Delegate.CreateDelegate(actionType, methodInfo);

            addQuickCommand.Invoke(null, new object[] { keyword, displayText, showInDefault, callbackDelegate });

            Plugin.Logger.LogInfo($"[Terminal] Registered command: {keyword}");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Terminal] Failed to register terminal command: {ex.Message}");
            throw;
        }
    }

    public static void OnModsCommandInternal(Terminal terminal, TerminalNode node)
    {
        try
        {
            Plugin.Logger.LogInfo("[Terminal] 'mods' command invoked");

            var modifiers = ModifierRegistry.Instance.GetAllModifiers();

            if (modifiers == null || modifiers.Count == 0)
            {
                Plugin.Logger.LogWarning("[Terminal] No modifiers available to display");
                return;
            }

            if (ModifierSelectionUI.Instance == null)
            {
                Plugin.Logger.LogError("[Terminal] ModifierSelectionUI.Instance is null!");
                return;
            }

            var modifiersArray = modifiers.ToArray();
            ModifierSelectionUI.Instance.ShowSelection(modifiersArray);

            Plugin.Logger.LogInfo($"[Terminal] Opened modifier selection with {modifiersArray.Length} modifiers");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Terminal] Error in OnModsCommandInternal: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static bool IsTerminalExtenderAvailable()
    {
        return _terminalExtenderAvailable;
    }
}
