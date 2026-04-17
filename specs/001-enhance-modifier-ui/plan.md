# Implementation Plan: In-Game Active Modifiers Overlay

**Branch**: `001-enhance-modifier-ui` | **Date**: 2026-04-17 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-enhance-modifier-ui/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Add an in-game overlay accessible via F2 during gameplay to view active modifiers (read-only). The overlay appears in the top-right corner of the screen, showing only currently active modifiers for the running game session. Players can dismiss it by pressing F2 again or Escape. This is purely informational - no modifier toggling or modification is possible.

## Technical Context

**Language/Version**: C# 12 (targeting .NET Standard 2.1)  
**Primary Dependencies**: Lib.Harmony 2.4.2, UnityEngine.UI, Unity TextMeshPro  
**Storage**: In-memory (no persistence required per spec assumption)  
**Testing**: Manual testing required (no automated test framework present)  
**Target Platform**: Windows (Lethal Company game via BepInEx)  
**Project Type**: Game mod / Unity UI application  
**Performance Goals**: Overlay opens in <500ms, renders at 60fps  
**Constraints**: Must work within Unity's UI system, F2 hotkey must not conflict with game controls, overlay must not pause gameplay  
**Scale/Scope**: Single overlay component, 1-20 active modifiers displayed

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Constitution Status**: The project's constitution file (`.specify/memory/constitution.md`) is a template and has not been filled in. No gating checks can be enforced.

**NOTE**: Project lacks defined constitutional principles. If a constitution existed, gates would include:
- Test-First development requirement
- Library vs application structure requirements
- Code quality standards

## Project Structure

### Documentation (this feature)

```text
specs/001-enhance-modifier-ui/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (UI contracts)
│   └── ui-contracts.md
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (LethalCompanyMod/)

```text
LethalCompanyMod/
├── src/
│   ├── UI/
│   │   ├── ModifierSelectionUI.cs    # Existing - pre-game selection
│   │   └── ActiveModifiersOverlay.cs  # NEW: In-game overlay (F2)
│   ├── Config/
│   │   └── ModConfig.cs             # Existing - add OverlayHotkey config
│   └── Modifiers/
│       ├── Modifier.cs               # Existing
│       ├── AppliedModifier.cs        # Existing
│       └── ModifierRegistry.cs       # Existing
```

**Structure Decision**: Add new `ActiveModifiersOverlay.cs` MonoBehaviour component. Integrate hotkey configuration into existing `ModConfig.cs`. Reuse existing `RunState.ActiveModifiers` as the data source. No changes to pre-game selection UI.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| N/A - No constitution violations | N/A | N/A |

---

**Generated**: 2026-04-17 | **Status**: Phase 1 Complete | **Ready for**: /speckit.tasks
