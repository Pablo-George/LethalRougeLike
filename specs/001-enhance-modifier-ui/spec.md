# Feature Specification: In-Game Active Modifiers Overlay

**Feature Branch**: `001-enhance-modifier-ui`  
**Created**: 2026-04-16  
**Status**: Draft  
**Input**: User description: "Add an in-game overlay accessible via F2 during gameplay to view active modifiers (read-only)."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Active Modifiers During Gameplay (Priority: P1)

A player can view which modifiers are currently affecting their active game run by pressing F2 during gameplay. A small overlay appears showing a read-only list of all active modifiers for the current session. This allows players to quickly check what challenges or changes are in effect without leaving the game or pausing.

**Why this priority**: Players may forget which modifiers they enabled before starting a run, especially when returning to the game after a break. This feature provides quick, in-context information about the current game state without disrupting gameplay flow.

**Independent Test**: Can be tested by starting a game with known modifiers, pressing F2 during gameplay, and verifying the overlay displays the correct active modifiers.

**Acceptance Scenarios**:

1. **Given** the player is in an active game run, **When** they press F2, **Then** an overlay appears displaying all currently active modifiers for that run.
2. **Given** the active modifiers overlay is displayed, **When** the player views it, **Then** they see a list of only the modifiers that are currently affecting gameplay (not all available modifiers).
3. **Given** the active modifiers overlay is displayed, **When** the player views it, **Then** the display is clearly read-only with no option to toggle or modify modifiers.
4. **Given** the active modifiers overlay is displayed, **When** the player presses F2 again or presses Escape, **Then** the overlay dismisses and gameplay continues.
5. **Given** the player has no modifiers active, **When** they press F2, **Then** the overlay displays a message indicating no modifiers are active.
6. **Given** the active modifiers overlay is displayed, **When** the player presses any other game control, **Then** the overlay remains visible (game may or may not be paused depending on implementation).

---

### User Story 2 - In-Game Overlay Visual Design (Priority: P1)

The in-game active modifiers overlay has a compact, non-intrusive design that displays information clearly without obstructing gameplay. The overlay uses consistent styling with the Lethal Company aesthetic while remaining readable during action.

**Why this priority**: The overlay exists within the game environment where players are actively engaged. Poor design could obstruct critical gameplay elements or be difficult to read during intense moments.

**Independent Test**: Can be tested by displaying the overlay during various game scenarios and verifying readability and minimal obstruction.

**Acceptance Scenarios**:

1. **Given** the active modifiers overlay is displayed, **When** the player views it, **Then** the overlay uses a semi-transparent background that does not fully obscure the game view.
2. **Given** the active modifiers overlay is displayed, **When** the player views it, **Then** modifier names are clearly visible and legible even during gameplay.
3. **Given** the active modifiers overlay is displayed, **When** there are many active modifiers, **Then** the overlay has a scrollable list or compact layout that fits within the screen.
4. **Given** the active modifiers overlay is displayed, **When** the player is in a dark area of the game, **Then** the overlay remains readable.

---

## Edge Cases

- What happens if the player presses F2 while in a menu or loading screen - should the overlay be suppressed?
- What happens if the player opens the overlay while near the edge of the screen - does the overlay reposition to stay visible?
- How does the overlay handle cases where modifier names are very long?
- What happens if the player rapidly opens and closes the overlay - does it remain responsive?
- How does the overlay handle different screen resolutions and aspect ratios?
- What happens if another mod or the game itself uses the F2 hotkey - is there conflict resolution?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide an in-game overlay accessible via F2 that displays all currently active modifiers for the running game session.
- **FR-002**: System MUST display only the modifiers that are currently active in the in-game overlay (not all available modifiers).
- **FR-003**: System MUST make the in-game overlay read-only - no toggle controls or modification options.
- **FR-004**: System MUST allow the player to dismiss the in-game overlay by pressing F2 again or pressing Escape.
- **FR-005**: System MUST NOT block or pause gameplay when the in-game overlay is displayed.
- **FR-006**: System MUST display a clear "No active modifiers" message when the player has no modifiers enabled and opens the in-game overlay.
- **FR-007**: System MUST use a semi-transparent background that does not fully obscure the game view.
- **FR-008**: System MUST ensure modifier names are clearly visible and legible during gameplay.
- **FR-009**: System MUST provide a scrollable list or compact layout when there are many active modifiers.
- **FR-010**: System MUST suppress the overlay when the player is in menus or loading screens.
- **FR-011**: System MUST remain responsive (overlay opens within 500ms of pressing F2).

### Key Entities *(include if feature involves data)*

- **Modifier**: Represents a game modification option. Key attributes: identifier (unique name), display name, description text, enabled state (boolean).
- **ActiveModifierOverlay**: Represents the in-game overlay UI. Key attributes: visibility state (boolean), position on screen, active modifiers list (derived from current game state), and dismiss behavior.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: The in-game active modifiers overlay opens within 500ms of pressing F2.
- **SC-002**: The in-game overlay correctly displays 100% of active modifiers for the current game session.
- **SC-003**: The in-game overlay does not obstruct more than 20% of the screen area.
- **SC-004**: The overlay remains readable in both bright and dark game lighting conditions.

## Assumptions

- Players have a minimum screen resolution of 1280x720 to properly view the overlay.
- The existing modifier data structure (names, descriptions, enabled state) is accessible during gameplay.
- The F2 hotkey does not conflict with critical game controls.
- The in-game overlay should not pause the game to maintain immersion.
- The in-game overlay is only available during active gameplay, not in menus or loading screens.
- The number of active modifiers is expected to be manageable (1-20 based on typical usage).

(End of file - total 124 lines)
