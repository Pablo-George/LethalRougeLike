# Data Model: In-Game Active Modifiers Overlay

**Feature**: 001-enhance-modifier-ui | **Date**: 2026-04-17

## Entity Definitions

### 1. ActiveModifierOverlay

Represents the in-game overlay UI for viewing active modifiers during gameplay.

```csharp
public class ActiveModifierOverlay : MonoBehaviour
{
    public static ActiveModifierOverlay Instance { get; private set; }
    
    public bool IsVisible => gameObject.activeSelf;
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Toggle();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsVisible)
        {
            Hide();
        }
    }
    
    public void Show() { /* Display overlay with active modifiers */ }
    public void Hide() { /* Hide overlay */ }
    public void Toggle() { /* Switch visibility */ }
}
```

**Validation Rules**:
- IsVisible: Toggle visibility state
- Position: Screen corner anchor
- AnchorOffset: Margin from screen edge (10px recommended)
- MaxDisplayCount: Before vertical scroll, prevents >20% screen obstruction

**Constraints**:
- Read-only display (no toggle controls)
- Semi-transparent background (alpha ~70%)
- Does not pause game
- Only available during active gameplay

---

### 2. ActiveModifierInfo

Represents a single active modifier as displayed in the overlay.

```csharp
public class ActiveModifierInfo
{
    public string ModifierId { get; set; }
    public string DisplayName { get; set; }
    public bool IsDebuff { get; set; }
    public int AppliedAtLanding { get; set; }
}
```

**Data Source**: Derived from `RunState.ActiveModifiers` at overlay open time.

---

### 3. UIConfiguration

Centralized styling and behavior configuration for the overlay.

```csharp
public static class UIConfiguration
{
    // Colors - In-game Overlay
    public static Color OverlayBackground { get; set; } = new Color(0, 0, 0, 0.70f);
    public static Color OverlayDebuffIndicator { get; set; } = new Color(0.906f, 0.298f, 0.235f, 1f);  // #E74C3C
    public static Color OverlayBuffIndicator { get; set; } = new Color(0.180f, 0.800f, 0.443f, 1f);    // #2ECC71
    
    // Layout
    public static float OverlayMaxWidth { get; set; } = 300f;
    public static float OverlayMargin { get; set; } = 10f;
    public static int MaxDisplayCount { get; set; } = 8;  // Before scroll
    
    // Text
    public static float OverlayFontSize { get; set; } = 16f;
    public static float OverlayTitleFontSize { get; set; } = 18f;
    
    // Animation
    public static float FadeInDuration { get; set; } = 0.2f;
    public static float FadeOutDuration { get; set; } = 0.15f;
}
```

---

### 4. OverlayPosition (enum)

Screen corner anchor options.

```csharp
public enum OverlayPosition
{
    TopLeft,
    TopRight,     // DEFAULT
    BottomLeft,
    BottomRight
}
```

---

## State Transitions

### In-game Overlay State

```
[Hidden] → (F2 pressed + game active) → [Visible]
[Visible] → (F2 pressed) → [Hidden]
[Visible] → (Escape pressed) → [Hidden]
[Visible] → (game menu opens) → [Hidden]
[Visible] → (game ends) → [Hidden]
```

**Important**: Overlay should NOT show during menus, loading screens, or title screen. Check `StartOfRound.Instance != null && !isPlayerDead` before showing.

---

## Data Relationships

```
ActiveModifiersOverlay (MonoBehaviour component)
    ├── IsVisible (visibility state)
    ├── Position (screen corner: TopRight default)
    └── List<ActiveModifierInfo>
        └── Derived from RunState.ActiveModifiers
            └── AppliedModifier (ModifierId, AppliedAtLanding, Severity)
                └── Modifier (Name, IsDebuff via Registry)
```

---

## Requirements Mapping

| Requirement | Entity | Field |
|-------------|--------|-------|
| FR-001: F2 overlay hotkey | ActiveModifierOverlay | Update() key check |
| FR-002: Show active only | ActiveModifierInfo | Derived from RunState |
| FR-003: Read-only | ActiveModifierOverlay | No toggle controls |
| FR-004: Dismiss with F2/Escape | ActiveModifierOverlay | Toggle/Hide methods |
| FR-005: No game pause | ActiveModifierOverlay | Non-blocking canvas |
| FR-006: "No modifiers" message | ActiveModifierOverlay | Empty state handling |
| FR-007: Semi-transparent | UIConfiguration | OverlayBackground alpha |
| FR-008: Readable names | ActiveModifierInfo | DisplayName |
| FR-009: Scrollable list | ActiveModifierOverlay | ScrollRect for >8 items |
| FR-010: Suppress in menus | ActiveModifierOverlay | IsGameActive() check |
| FR-011: <500ms response | Performance | Simple UI, no heavy ops |

| Success Criteria | Implementation |
|-----------------|----------------|
| SC-001: 500ms open | Lightweight canvas, simple layout |
| SC-002: 100% active shown | Source from RunState.ActiveModifiers |
| SC-003: <20% screen | MaxWidth 300px, scrollable |
| SC-004: Readable in light/dark | Semi-transparent + high contrast text |

---

**Status**: COMPLETE | **Ready for**: /speckit.tasks
