using System;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace LethalRogueLike.src.Hooks;

public static class TerminalHooks
{
    public static void Initialize()
    {
        Plugin.Logger.LogInfo("[TerminalHooks] Initialized.");
    }

    [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
    public class Terminal_ParsePlayerSentence_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Terminal __instance, ref TerminalNode __result)
        {
            try
            {
                string screenText = __instance.screenText.text;
                int textAdded = __instance.textAdded;
                if (textAdded <= 0 || textAdded > screenText.Length) return;

                string input = screenText.Substring(screenText.Length - textAdded).Trim().ToLower();
                if (input != "modifiers") return;

                Plugin.Logger.LogInfo("[TerminalHooks] 'modifiers' command received.");
                __result = BuildModifiersNode();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[TerminalHooks] Error in ParsePlayerSentence patch: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }

    private static TerminalNode BuildModifiersNode()
    {
        var node = ScriptableObject.CreateInstance<TerminalNode>();
        node.clearPreviousText = true;
        node.displayText = BuildModifiersText();
        node.terminalEvent = "";
        return node;
    }

    private static string BuildModifiersText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("ACTIVE MODIFIERS");
        sb.AppendLine("----------------");

        var active = RunState.Instance?.ActiveModifiers;
        if (active == null || active.Count == 0)
        {
            sb.AppendLine("No active modifiers.");
        }
        else
        {
            foreach (var am in active)
            {
                var modifier = Modifiers.ModifierRegistry.Instance.GetModifier(am.ModifierId);
                if (modifier == null) continue;

                string tag = modifier.IsDebuff ? "[DEBUFF]" : "[BUFF]  ";
                sb.AppendLine($"{tag} {modifier.Name}");
                sb.AppendLine($"         {modifier.Description}");
            }
        }

        sb.AppendLine();
        string runId = RunState.Instance?.RunId ?? "none";
        int landings = RunState.Instance?.LandingCount ?? 0;
        sb.AppendLine($"Run: {runId}  |  Landings: {landings}");
        sb.AppendLine();

        return sb.ToString();
    }
}
