# Research: Hunger System

## Technical Research

### Technology Choice: UI Display

**Decision**: Use Unity Canvas API with Image component for progress bar

**Rationale**: Game uses Unity UI system. Existing mod uses OverlayPosition and UIConfiguration. Can follow similar patterns.

**Alternatives considered**:
- TextMeshPro for text display (not needed - bar is visual)
- Custom shader for animated bar (overkill for v1)

---

### Technology Choice: Multiplayer Sync

**Decision**: Use game's existing network sync or SyncManager pattern

**Rationale**: Existing NetworkHandler.cs uses custom sync. Hunger must sync hunger value across clients.

**Alternatives considered**:
- No sync (single-player only) - rejected, breaks multiplayer
- Full custom sync - overkill, reuse existing patterns

---

### Technology Choice: Food Items

**Decision**: Create food item definitions as ScriptableObjects or configuration-based

**Rationale**: Need way to define different food with different restore values and prices.

**Alternatives considered**:
- Hard-coded foods - rejected, no flexibility
- Patch store generation - acceptable for v1

---

### Technology Choice: Starvation Damage

**Decision**: Patch into existing damage/health system

**Rationale**: Existing game has health system. Use Harmony to apply damage when hunger at 0.

**Alternatives considered**:
- Custom damage system - adds complexity
- Direct health modification - requires patch

---

## Summary

All technical decisions resolved - no NEEDS CLARIFICATION markers needed.
- UI: Unity UI Image component
- Storage: Add component to player
- Food: Configuration-based definitions
- Damage: Harmony patch on health system
- Sync: Reuse existing NetworkHandler patterns