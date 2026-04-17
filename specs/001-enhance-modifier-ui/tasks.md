---

description: "Task list for In-Game Active Modifiers Overlay feature implementation"
---

# Tasks: In-Game Active Modifiers Overlay

**Input**: Design documents from `/specs/001-enhance-modifier-ui/`
**Prerequisites**: plan.md, spec.md, data-model.md, research.md, contracts/ui-contracts.md, quickstart.md
**Tests**: Manual testing only (no automated test framework available per plan.md)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

---

## Phase 1: Setup

**Purpose**: Verify project structure and dependencies are ready for overlay development

**Note**: Project structure already exists from previous feature work. Skip if ModifierSelectionUI.cs compiles successfully.

- [X] T001 Verify LethalCompanyMod project structure in LethalCompanyMod/src/
- [X] T002 Confirm UnityEngine.UI and Unity.TextMeshPro dependencies in LethalCompanyMod.csproj
- [X] T003 [P] Review existing ModifierSelectionUI.cs for UI patterns to follow in src/UI/ModifierSelectionUI.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 Add OverlayHotkey config entry to ModConfig.cs with default KeyCode.F2
- [X] T005 Create UIConfiguration static class in src/UI/UIConfiguration.cs with overlay styling constants
- [X] T006 [P] Create OverlayPosition enum in src/UI/OverlayPosition.cs
- [X] T007 [P] Create ActiveModifierInfo class in src/UI/ActiveModifierInfo.cs
- [X] T008 Review RunState.ActiveModifiers to confirm data source accessibility

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - View Active Modifiers During Gameplay (Priority: P1) - MVP

**Goal**: Player can press F2 during gameplay to view a read-only overlay of active modifiers

**Independent Test**: Start game with known modifiers, press F2, verify overlay displays correct modifiers

### Implementation for User Story 1

- [X] T009 [P] [US1] Create ActiveModifiersOverlay MonoBehaviour class in src/UI/ActiveModifiersOverlay.cs
- [X] T010 [US1] Implement singleton pattern (Instance property, Awake, OnDestroy) in src/UI/ActiveModifiersOverlay.cs
- [X] T011 [US1] Implement IsGameActive() method to check StartOfRound.Instance state in src/UI/ActiveModifiersOverlay.cs
- [X] T012 [US1] Implement Update() method with F2 hotkey detection and Escape dismiss in src/UI/ActiveModifiersOverlay.cs
- [X] T013 [US1] Implement Show() method to display overlay with active modifiers in src/UI/ActiveModifiersOverlay.cs
- [X] T014 [US1] Implement Hide() method to dismiss overlay in src/UI/ActiveModifiersOverlay.cs
- [X] T015 [US1] Implement Toggle() method combining Show/Hide logic in src/UI/ActiveModifiersOverlay.cs
- [X] T016 [US1] Add "No active modifiers" empty state message handling in src/UI/ActiveModifiersOverlay.cs
- [X] T017 [US1] Add logging for overlay toggle events in src/UI/ActiveModifiersOverlay.cs

**Checkpoint**: At this point, User Story 1 should be fully functional - F2 shows/hides overlay with active modifiers

---

## Phase 4: User Story 2 - In-Game Overlay Visual Design (Priority: P1)

**Goal**: Overlay has compact, semi-transparent design that remains readable during gameplay

**Independent Test**: Display overlay during various game scenarios, verify readability and minimal obstruction

### Implementation for User Story 2

- [X] T018 [P] [US2] Create Overlay Canvas with ScreenSpaceOverlay render mode and sortingOrder=1000 in src/UI/ActiveModifiersOverlay.cs
- [X] T019 [P] [US2] Implement semi-transparent background panel with alpha ~70% in src/UI/ActiveModifiersOverlay.cs
- [X] T020 [US2] Create ScrollRect container for modifier list when >8 items in src/UI/ActiveModifiersOverlay.cs
- [X] T021 [US2] Add TextMeshProUGUI for modifier names with white text color in src/UI/ActiveModifiersOverlay.cs
- [X] T022 [US2] Add colored indicator (red=buff, green=debuff) next to each modifier in src/UI/ActiveModifiersOverlay.cs
- [X] T023 [US2] Position overlay in top-right corner with 10px margin using UIConfiguration values in src/UI/ActiveModifiersOverlay.cs
- [X] T024 [US2] Implement fade animation (0.2s fade-in, 0.15s fade-out) using canvas group alpha in src/UI/ActiveModifiersOverlay.cs
- [X] T025 [US2] Add game menu detection to auto-hide overlay when menu opens in src/UI/ActiveModifiersOverlay.cs

**Checkpoint**: At this point, User Stories 1 AND 2 should both work - overlay is visually polished and readable

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Final verification and documentation updates

- [X] T026 [P] Update quickstart.md testing checklist for in-game overlay in specs/001-enhance-modifier-ui/quickstart.md
- [X] T027 Add debug logging for F2 press, overlay state, and modifier count
- [X] T028 Verify overlay does not pause gameplay (non-blocking canvas)
- [X] T029 Test overlay responsiveness (<500ms open time per SC-001)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-4)**: All depend on Foundational phase completion
  - US1 and US2 can proceed in parallel after foundational complete
- **Polish (Phase 5)**: Depends on both user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - Can run parallel with US1

### Within Each User Story

- Core classes before UI implementation
- Basic functionality before polish
- Story complete before moving to polish phase

---

## Parallel Execution Opportunities

### Parallel: Phase 2 Foundational Tasks

```text
Task: "Create OverlayPosition enum in src/UI/OverlayPosition.cs"
Task: "Create ActiveModifierInfo class in src/UI/ActiveModifierInfo.cs"
```

### Parallel: Phase 3 US1 Core Structure

```text
Task: "Create ActiveModifiersOverlay MonoBehaviour class in src/UI/ActiveModifiersOverlay.cs"
Task: "Review RunState.ActiveModifiers for data source"
```

### Parallel: Phase 4 US2 Visual Elements

```text
Task: "Create Overlay Canvas with ScreenSpaceOverlay render mode"
Task: "Implement semi-transparent background panel"
```

### Parallel: User Stories After Foundational

```text
Developer A: User Story 1 - Core overlay functionality
Developer B: User Story 2 - Visual design
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test F2 overlay with active modifiers
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational - Foundation ready
2. Add User Story 1 - Test independently - Deploy/Demo (MVP!)
3. Add User Story 2 - Test independently - Deploy/Demo
4. Polish phase - Final verification

### Recommended Implementation Order

1. T004 (Config) -> T005, T006, T007 (Foundational) -> (blocker removed)
2. T009-T017 (US1 core) -> MVP ready
3. T018-T025 (US2 visual) -> Full feature complete
4. T026-T029 (Polish)

---

## File Locations

### New Files to Create

```
LethalCompanyMod/LethalCompanyMod/src/UI/
├── ActiveModifiersOverlay.cs   # Main MonoBehaviour (T009)
├── UIConfiguration.cs           # Styling constants (T005)
├── OverlayPosition.cs           # Position enum (T006)
└── ActiveModifierInfo.cs        # Data class (T007)
```

### Files to Modify

```
LethalCompanyMod/LethalCompanyMod/src/Config/
└── ModConfig.cs                 # Add OverlayHotkey (T004)
```

### Reference Files

```
LethalCompanyMod/LethalCompanyMod/src/UI/
└── ModifierSelectionUI.cs       # UI patterns reference (T003)

LethalCompanyMod/LethalCompanyMod/src/
└── RunState.cs                  # ActiveModifiers data source (T008)
```

---

## Verification Checklist

- [ ] FR-001: F2 opens overlay with active modifiers
- [ ] FR-002: Only active modifiers shown (not all available)
- [ ] FR-003: Overlay is read-only (no toggle controls)
- [ ] FR-004: F2 or Escape dismisses overlay
- [ ] FR-005: Game does not pause when overlay shown
- [ ] FR-006: "No active modifiers" message when empty
- [ ] FR-007: Semi-transparent background (alpha ~70%)
- [ ] FR-008: Modifier names clearly visible
- [ ] FR-009: Scrollable list for >8 modifiers
- [ ] FR-010: Overlay suppressed in menus
- [ ] FR-011: Overlay opens within 500ms
- [ ] SC-001: <500ms response time
- [ ] SC-002: 100% of active modifiers displayed
- [ ] SC-003: <20% screen obstruction
- [ ] SC-004: Readable in bright and dark lighting

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at Phase 3 checkpoint to validate US1 MVP independently
- Manual testing required - no automated test framework available
