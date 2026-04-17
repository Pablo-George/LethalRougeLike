using System;

namespace LethalRogueLike.src;

public static class ModifierEvents
{
    public static event Action<int>? OnLanding;
    public static event Action<Modifiers.Modifier[]>? OnChoicesOffered;
    public static event Action<Modifiers.Modifier>? OnModifierApplied;
    public static event Action? OnRunEnd;
    public static event Action? OnRunStart;

    public static void InvokeOnLanding(int count) => OnLanding?.Invoke(count);
    public static void InvokeOnChoicesOffered(Modifiers.Modifier[] choices) => OnChoicesOffered?.Invoke(choices);
    public static void InvokeOnModifierApplied(Modifiers.Modifier modifier) => OnModifierApplied?.Invoke(modifier);
    public static void InvokeOnRunEnd() => OnRunEnd?.Invoke();
    public static void InvokeOnRunStart() => OnRunStart?.Invoke();
}
