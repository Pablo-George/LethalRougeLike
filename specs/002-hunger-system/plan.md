# Implementation Plan: Hunger System

**Branch**: `002-hunger-system` | **Date**: 2026-04-17 | **Spec**: [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add a hunger survival system to Lethal Company where players must manage their hunger level by purchasing and consuming food from the store. Hunger depletes over time, and reaching 0% causes the player to take starvation damage. This creates an additional survival challenge that adds depth to the gameplay loop.

## Technical Context

**Language/Version**: C# (.NET Standard 2.1)  
**Primary Dependencies**: BepInEx 5.x, Lib.Harmony 2.4.2, Unity Engine (via game DLLs)  
**Storage**: Game state persisted by existing save system (no additional storage)  
**Testing**: In-game testing only (no unit tests for Harmony mods)  
**Target Platform**: Windows PC (Lethal Company game)  
**Project Type**: BepInEx mod (gameplay modification)  
**Performance Goals**: Minimal impact - hunger tick every 1-2 seconds, UI updates at frame rate  
**Constraints**: Must sync across multiplayer, must not break existing game mechanics  
**Scale/Scope**: New stat system, UI element, store integration, ~500 LOC estimated

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

No constitution file defined - skipping gates.

## Project Structure

### Documentation (this feature)

```text
specs/002-hunger-system/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (if needed)
└── tasks.md             # Phase 2 output
```

### Source Code (repository root)

```text
LethalCompanyMod/
├── src/
│   ├── Hunger/           # New: Hunger system
│   │   ├── HungerManager.cs       # Core hunger logic
│   │   ├── HungerData.cs        # Hunger data component
│   │   ├── FoodItem.cs          # Food item definition
│   │   ├── HungerUI.cs         # Hunger bar display
│   │   └── HungerHooks.cs       # Harmony patches
│   ├── Config/          # Existing: Mod configuration
│   ├── Hooks/           # Existing: Game hooks
│   ├── Modifiers/       # Existing: Modifier system
│   ├── UI/              # Existing: UI components
│   ├── Networking/      # Existing: Sync
│   └── RunState.cs       # Existing: Game state
├── specs/               # Design docs
└── LethalCompanyMod.csproj
```

**Structure Decision**: Hunger system follows existing mod structure patterns in `src/Hunger/` directory with separate files for core logic, data, UI, and game hooks.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| None | N/A | N/A |

## Research Phase

### Technology Choices

1. **UI Display**: Use existing Unity Canvas API with game-style progress bar
2. **Storage**: Use game's existing PlayerData component or create new SyncObject for multiplayer
3. **Food Items**: Extend existing item system or create new food item definitions
4. **Damage**: Use game's existing damage system via Harmony patch on health change

### Integration Points

- Store: Patch to add food items to shop
- Player: Add hunger component to player object
- UI: Add hunger bar to HUD canvas
- Damage: Patch health modification to apply starvation damage

---

## Phase 1: Design

### Entities

- **PlayerHunger**: Component storing current hunger (float 0-100)
- **FoodItem**: Item type with hunger restore value
- **HungerBar**: UI element showing hunger level

### Data Model

See [data-model.md](data-model.md)

### Quickstart

See [quickstart.md](quickstart.md)