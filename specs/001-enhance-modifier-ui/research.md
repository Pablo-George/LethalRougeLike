# Research: In-Game Active Modifiers Overlay

**Feature**: 001-enhance-modifier-ui | **Date**: 2026-04-17

## Research Tasks Completed

### Task 1: Technical Stack Analysis

**Question**: What Unity UI technologies are available for this mod?

**Findings**:
- Target: Unity 2021+ (Lethal Company uses Unity 2021.4.x based on assembly info)
- Available packages in project:
  - UnityEngine.UI (Canvas, Image, Button, ScrollRect)
  - Unity.TextMeshPro (TextMeshProUGUI for text)
  - UnityEngine.UIModule (additional UI utilities)

**Decision**: Use Unity's built-in UI components + TextMeshPro
**Rationale**: Already in use by existing code, no additional dependencies needed
**Alternatives considered**: DOTween (adds dependency, rejected), UI Toolkit (different paradigm, rejected)

---

### Task 2: Visual Design Research

**Question**: What aesthetic should the overlay match?

**Findings**:
- Lethal Company visual style: Dark industrial, horror-comedy tone
- Color palette: Dark grays, muted earthy tones, red accents for danger
- Typography: Clean, industrial (matches TextMeshPro default)
- UI feel: Functional, slightly gritty, unintentional-but-charming

**Decision**: Semi-transparent overlay with:
- Background: Semi-transparent black (alpha ~70%)
- Text: White/default TMP white
- Debuff indicator: Red (#E74C3C)
- Buff indicator: Green (#2ECC71)

**Rationale**: Matches existing code style (`new Color(0, 0, 0, 0.85f)` panel, etc.), semi-transparency required per spec (FR-007)
**Alternatives considered**: Fully opaque overlay (rejected - obstructs gameplay)

---

### Task 3: Overlay Position & Layout

**Question**: Where should the overlay be positioned?

**Findings**:
- Must not obstruct >20% of screen (SC-003)
- Must be readable in bright and dark game lighting (SC-004)
- 1-20 active modifiers expected (assumption 8)

**Decision**: Corner-anchored overlay:
- Position: Top-right corner by default, 10px margin
- Size: Max 300px wide, auto-height (scrollable if >8 modifiers)
- Content: Modifier names with colored indicator

**Rationale**: Minimal screen impact, quick reference during gameplay
**Alternatives considered**: 
- Center screen (rejected - obstructs gameplay)
- Full-width bottom bar (rejected - loses too much vertical space)
- Top-left corner (rejected - F2 is right-hand key, mirroring looks odd)

---

### Task 4: Hotkey Implementation

**Question**: How to implement the F2 hotkey?

**Findings**:
- Unity's Input.GetKeyDown can detect F2
- F2 is not used by Lethal Company's default controls
- Need to handle key conflict detection with other mods
- Should be configurable in case of conflicts

**Decision**: Primary implementation:
```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.F2))
    {
        ToggleOverlay();
    }
}
```
- Configurable via ModConfig (default: F2)
- Check for conflicts on startup, warn if detected

**Rationale**: Simple, no external dependencies, matches game mod patterns
**Alternatives considered**: 
- BepInEx.InputInterceptor (adds complexity, overkill for single key)
- Configurable key (good practice, implemented)

---

### Task 5: Game State Detection

**Question**: When should the overlay be available?

**Findings**:
- Spec says: overlay only during active gameplay, not menus/loading
- Unity game state can be detected via StartOfRound.Instance
- Need to handle: title screen, menus, loading, active gameplay

**Decision**: Check game state before showing overlay:
```csharp
private bool IsGameActive()
{
    return StartOfRound.Instance != null && 
           !StartOfRound.Instance.isPlayerDead &&
           Time.time > 1f;  // Allow time for initialization
}
```

**Rationale**: Standard pattern for Lethal Company mods
**Alternatives considered**: 
- Menu open detection via HUD (fragile)
- Check for specific scene (fragile with mods changing scenes)

---

### Task 6: Dismiss Behavior

**Question**: How should the overlay be dismissed?

**Findings**:
- Spec says: F2 again or Escape key
- ESC is already used by game for menu
- F2 toggle is intuitive for overlay

**Decision**: 
- F2: Toggle overlay visibility
- Escape: Hide overlay (if visible)
- Game menu opens: Auto-hide overlay

**Rationale**: Matches standard overlay patterns, ESC during gameplay opens menu anyway
**Alternatives considered**: 
- Click outside to dismiss (rejected - spec says F2/Escape only)
- Separate close button (rejected - spec says F2/Escape only)

---

## Consolidated Research Decisions

| Decision Area | Choice | Rationale |
|--------------|--------|----------|
| UI Framework | UnityEngine.UI + TextMeshPro | Already in use, no dependencies |
| Visual Style | Dark semi-transparent (game matching) | Consistent with Lethal Company |
| Position | Top-right corner, 10px margin | Minimal gameplay obstruction |
| Size | Max 300px wide, scrollable | Meets 20% screen constraint |
| Hotkey | F2 (configurable) | Not used by game, accessible |
| Dismiss | F2 or Escape | Standard dismiss pattern |
| Game State | Check StartOfRound.Instance | Standard mod pattern |

---

**Status**: COMPLETE | **Ready for**: /speckit.tasks
