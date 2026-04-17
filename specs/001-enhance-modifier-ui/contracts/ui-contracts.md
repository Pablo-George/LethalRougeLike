# UI Contracts: In-Game Active Modifiers Overlay

**Feature**: 001-enhance-modifier-ui | **Date**: 2026-04-17

## Overview

This document defines the interface contracts for the in-game active modifiers overlay. Since this is a game mod with no external public API, these contracts are for internal code organization and extensibility.

---

## Component Contracts

### IActiveModifierOverlay

Interface for the in-game active modifiers overlay component.

```csharp
public interface IActiveModifierOverlay
{
    bool IsVisible { get; }
    
    void Show();
    void Hide();
    void Toggle();
}
```

**Lifecycle**:
1. Component attached to GameObject, initialized in Awake
2. Show → Display overlay with current active modifiers
3. Hide → Animate out and deactivate
4. Toggle → Switch between show/hide states

---

### IOverlayEvents

Events emitted by ActiveModifiersOverlay.

```csharp
public interface IOverlayEvents
{
    // Fired when overlay is shown
    event Action OnOverlayShown;
    
    // Fired when overlay is hidden
    event Action OnOverlayHidden;
}
```

---

## Unity Integration Contracts

### ActiveModifiersOverlay MonoBehaviour

```csharp
public class ActiveModifiersOverlay : MonoBehaviour, IActiveModifierOverlay
{
    public static ActiveModifiersOverlay Instance { get; private set; }
    
    // Unity lifecycle
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        // Check for hotkey (only when game is active)
        if (Input.GetKeyDown(UIConfiguration.OverlayHotkey))
        {
            Toggle();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsVisible)
        {
            Hide();
        }
    }
    
    // IActiveModifierOverlay implementation
    public bool IsVisible => gameObject.activeSelf;
    
    public void Show() { /* ... */ }
    public void Hide() { /* ... */ }
    public void Toggle() { /* ... */ }
}
```

**Game State Check**:
```csharp
private bool IsGameActive()
{
    return StartOfRound.Instance != null && 
           !StartOfRound.Instance.isPlayerDead &&
           Time.time > 1f;  // Allow time for initialization
}
```

---

### Canvas Integration

```csharp
// Canvas setup for In-game Overlay
Canvas (OverlayPanel):
  - renderMode: RenderMode.ScreenSpaceOverlay
  - sortingOrder: 1000 (above pre-game panel if visible)
  - pixelPerfect: false
  // Note: No CanvasScaler needed for fixed-size overlay
```

---

## Hotkey Configuration Contract

```csharp
// In ModConfig.cs
public static ConfigEntry<KeyCode> OverlayHotkey { get; }
    = config.Bind("Gameplay", "OverlayHotkey", KeyCode.F2,
        "Hotkey to view active modifiers during gameplay");

// Usage in ActiveModifiersOverlay
private void Update()
{
    if (Input.GetKeyDown(UIConfiguration.OverlayHotkey))
    {
        Toggle();
    }
}
```

---

## Event Dispatch Pattern

### Using C# Events

```csharp
public class ActiveModifiersOverlay : MonoBehaviour
{
    public static ActiveModifiersOverlay Instance { get; private set; }
    
    public event Action OnOverlayShown;
    public event Action OnOverlayHidden;
    
    internal void FireOverlayShown() => OnOverlayShown?.Invoke();
    internal void FireOverlayHidden() => OnOverlayHidden?.Invoke();
}
```

---

## Summary

| Contract | Purpose | Public |
|---------|---------|--------|
| IActiveModifierOverlay | In-game overlay lifecycle | No |
| IOverlayEvents | Overlay visibility events | Yes |
| Canvas config | Unity rendering setup | Reference |
| Hotkey config | Input binding | Reference |

---

**Status**: COMPLETE | **Ready for**: /speckit.tasks
