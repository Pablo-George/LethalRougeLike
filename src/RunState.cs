using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalRogueLike.src;

public class RunState
{
    public static RunState Instance { get; private set; } = new RunState();

    public string RunId { get; private set; }
    public int LandingCount { get; private set; }
    public List<Modifiers.AppliedModifier> ActiveModifiers { get; } = new();
    public List<Modifiers.Modifier> PendingChoices { get; } = new();
    public bool IsSelectionPending { get; private set; }

    private Dictionary<string, string> _votes = new();
    public int ConnectedClientsCount { get; private set; }

    private RunState()
    {
        RunId = string.Empty;
        LandingCount = 0;
        IsSelectionPending = false;
    }

    public void Initialize()
    {
        Plugin.Logger.LogInfo("[RunState] Initialized successfully.");
    }

    public void Shutdown()
    {
        try
        {
            Reset();
            Plugin.Logger.LogInfo("[RunState] Shutdown complete.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Error during shutdown: {ex.Message}");
        }
    }

    public void StartNewRun()
    {
        try
        {
            Reset();
            RunId = Guid.NewGuid().ToString("N").Substring(0, 8);
            Plugin.Logger.LogInfo($"[RunState] New run started. RunId: {RunId}");
            ModifierEvents.InvokeOnRunStart();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Failed to start new run: {ex.Message}");
        }
    }

    public void OnShipLanded()
    {
        try
        {
            LandingCount++;
            Plugin.Logger.LogInfo($"[RunState] Ship landed. Landing count: {LandingCount}");

            Plugin.Logger.LogDebug($"[RunState] Landing {LandingCount}. Triggering modifier selection.");
            OfferModifierChoices();

            ModifierEvents.InvokeOnLanding(LandingCount);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Error in OnShipLanded: {ex.Message}");
        }
    }

    public void TriggerModifierSelection()
    {
        try
        {
            Plugin.Logger.LogInfo("[RunState] Triggering modifier selection at game start.");
            OfferModifierChoices();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Failed to trigger modifier selection: {ex.Message}");
        }
    }

    private void OfferModifierChoices()
    {
        try
        {
            PendingChoices.Clear();
            var choices = Modifiers.ModifierRegistry.Instance.GetRandomChoices(2);

            if (choices == null || choices.Count < 2)
            {
                Plugin.Logger.LogWarning($"[RunState] Failed to get 2 random modifiers. Got: {choices?.Count ?? 0}");
                return;
            }

            foreach (var modifier in choices)
            {
                PendingChoices.Add(modifier);
            }

            IsSelectionPending = true;
            Plugin.Logger.LogInfo($"[RunState] Offering {choices.Count} modifier choices: {string.Join(", ", choices.Select(m => m.Name))}");

            ModifierEvents.InvokeOnChoicesOffered(PendingChoices.ToArray());
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Failed to offer modifier choices: {ex.Message}");
            IsSelectionPending = false;
        }
    }

    public void SelectModifier(string modifierId)
    {
        try
        {
            if (string.IsNullOrEmpty(modifierId))
            {
                Plugin.Logger.LogWarning("[RunState] SelectModifier called with null or empty modifierId!");
                return;
            }

            var modifier = PendingChoices.FirstOrDefault(m => m.Id == modifierId);
            if (modifier == null)
            {
                Plugin.Logger.LogWarning($"[RunState] Modifier not found: '{modifierId}'. Available choices: {string.Join(", ", PendingChoices.Select(m => m.Id))}");
                return;
            }

            if (!IsSelectionPending)
            {
                Plugin.Logger.LogWarning($"[RunState] SelectModifier called but IsSelectionPending is false. Ignoring.");
                return;
            }

            var applied = new Modifiers.AppliedModifier
            {
                ModifierId = modifier.Id,
                AppliedAtLanding = LandingCount,
                Severity = modifier.Severity
            };

            ActiveModifiers.Add(applied);
            IsSelectionPending = false;
            PendingChoices.Clear();

            try
            {
                modifier.Effect?.Apply();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[RunState] Modifier effect failed to apply: {ex.Message}");
            }

            Plugin.Logger.LogInfo($"[RunState] Modifier APPLIED: '{modifier.Name}' (Id: {modifier.Id}, Severity: {modifier.Severity}). Active modifiers: {ActiveModifiers.Count}");
            ModifierEvents.InvokeOnModifierApplied(modifier);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Failed to select modifier: {ex.Message}");
        }
    }

    public void OnRunEnd()
    {
        try
        {
            Plugin.Logger.LogInfo($"[RunState] Run ended. Clearing {ActiveModifiers.Count} active modifiers.");
            ModifierEvents.InvokeOnRunEnd();
            Reset();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Error in OnRunEnd: {ex.Message}");
        }
    }

    private void Reset()
    {
        RunId = string.Empty;
        LandingCount = 0;
        ActiveModifiers.Clear();
        PendingChoices.Clear();
        IsSelectionPending = false;
        _votes.Clear();
    }

    public void SetConnectedClientsCount(int count)
    {
        ConnectedClientsCount = count;
    }

    public void RecordVote(string clientId, string modifierId)
    {
        _votes[clientId] = modifierId;
    }

    public bool AllVotesReceived()
    {
        return _votes.Count >= ConnectedClientsCount;
    }

    public string GetVoteWinner()
    {
        if (_votes.Count == 0)
        {
            return string.Empty;
        }

        var counts = _votes.GroupBy(v => v.Value).ToDictionary(g => g.Key, g => g.Count());
        var maxVotes = counts.Values.Max();

        var winners = counts.Where(kv => kv.Value == maxVotes).Select(kv => kv.Key).ToList();

        if (winners.Count == 1)
        {
            return winners[0];
        }

        return winners[UnityEngine.Random.Range(0, winners.Count)];
    }

    public void ClearVotes()
    {
        _votes.Clear();
    }

    public bool HasActiveModifier(Modifiers.ModifierEffectType effectType)
    {
        try
        {
            return ActiveModifiers.Any(am =>
            {
                var modifier = Modifiers.ModifierRegistry.Instance.GetModifier(am.ModifierId);
                return modifier?.EffectType == effectType;
            });
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Error checking HasActiveModifier: {ex.Message}");
            return false;
        }
    }

    public float GetCombinedSeverity(Modifiers.ModifierEffectType effectType)
    {
        try
        {
            float combined = 1.0f;
            foreach (var am in ActiveModifiers)
            {
                var modifier = Modifiers.ModifierRegistry.Instance.GetModifier(am.ModifierId);
                if (modifier?.EffectType == effectType)
                {
                    combined *= am.Severity;
                }
            }
            return combined;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[RunState] Error getting combined severity: {ex.Message}");
            return 1.0f;
        }
    }
}
