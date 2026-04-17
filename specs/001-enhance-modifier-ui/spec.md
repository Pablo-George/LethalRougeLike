# Feature Specification: In-Game Active Modifiers View

**Feature Branch**: `001-enhance-modifier-ui`  
**Created**: 2026-04-16  
**Updated**: 2026-04-17  
**Status**: Complete  
**Input**: User description: "Add an in-game view accessible via Terminal to view active modifiers (read-only)."

## Change Log

- 2026-04-17: Changed from F2 overlay to Terminal-based display. Press "view modifiers" in terminal to see active modifiers.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - View Active Modifiers During Gameplay (Priority: P1)

A player can view which modifiers are currently affecting their active game run by typing "view modifiers" in the in-game Terminal. The terminal displays a read-only list of all active modifiers for the current session. This allows players to quickly check what challenges or changes are in effect without leaving the game or pausing.

**Why this priority**: Players may forget which modifiers they enabled before starting a run, especially when returning to the game after a break. This feature provides quick, in-context information about the current game state without disrupting gameplay flow.

**Independent Test**: Can be tested by starting a game with known modifiers, opening the terminal, typing "view modifiers", and verifying the terminal displays the correct active modifiers.

**Acceptance Scenarios**:

1. **Given** the player is in an active game run, **When** they open the terminal and type "view modifiers", **Then** the terminal displays all currently active modifiers for that run.
2. **Given** the active modifiers are displayed in the terminal, **When** the player views them, **Then** they see a list of only the modifiers that are currently affecting gameplay (not all available modifiers).
3. **Given** the active modifiers are displayed in the terminal, **When** the player views them, **Then** the display is clearly read-only with no option to toggle or modify modifiers.
4. **Given** the player has no modifiers active, **When** they type "view modifiers" in the terminal, **Then** the terminal displays a message indicating no modifiers are active.

---

### User Story 2 - Terminal Visual Design (Priority: P1)

The in-game terminal display for active modifiers has a clear design that displays information in line with Lethal Company's terminal aesthetic. The display uses the existing terminal UI with consistent styling.

**Why this priority**: The terminal is an existing game UI that players are familiar with. Using it ensures consistent experience and leverages existing UI infrastructure.

**Independent Test**: Can be tested by displaying modifiers in terminal during various game scenarios and verifying readability and functionality.

**Acceptance Scenarios**:

1. **Given** the active modifiers are displayed in the terminal, **When** the player views them, **Then** the modifier names use terminal-compatible formatting with [D] for debuffs and [B] for buffs.
2. **Given** the active modifiers are displayed in the terminal, **When** the player views them, **Then** modifier names are clearly visible and legible.
3. **Given** the active modifiers are displayed in the terminal, **When** there are many active modifiers, **Then** the terminal scrolls or displays all items.
4. **Given** the player has no active modifiers, **When** they type "view modifiers", **Then** the terminal displays "No active modifiers".

---

## Edge Cases

- What happens if the player types "view modifiers" while in a menu or loading screen - should it be suppressed?
- What happens if the player rapidly opens/closes the terminal - does it remain responsive?
- How does the terminal handle different screen resolutions and aspect ratios?
- Does the "view modifiers" command conflict with existing terminal commands?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a terminal command "view modifiers" that displays all currently active modifiers for the running game session.
- **FR-002**: System MUST display only the modifiers that are currently active in the terminal (not all available modifiers).
- **FR-003**: System MUST make the terminal display read-only - no toggle controls or modification options.
- **FR-004**: System MUST use the existing Terminal UI infrastructure (no custom overlay).
- **FR-005**: System MUST NOT block or pause gameplay when viewing modifiers in terminal.
- **FR-006**: System MUST display a clear "No active modifiers" message when the player has no modifiers enabled and types "view modifiers".
- **FR-007**: System MUST use [D] prefix for debuffs and [B] prefix for buffs in the modifier list.
- **FR-008**: System MUST ensure modifier names are clearly visible and legible.
- **FR-009**: System MUST handle display of multiple modifiers without truncation.
- **FR-010**: System MUST suppress the modifier display when the player is in loading screens or not in a game session.

### Key Entities *(include if feature involves data)*

- **Modifier**: Represents a game modification option. Key attributes: identifier (unique name), display name, description text, isDebuff flag (boolean).
- **TerminalModifierDisplay**: Represents the terminal output for modifiers. Key attributes: visibility state (boolean), active modifiers list (derived from current game state).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: The "view modifiers" command executes within 500ms of pressing enter.
- **SC-002**: The terminal correctly displays 100% of active modifiers for the current game session.
- **SC-003**: The modifier list does not interfere with existing terminal functionality.
- **SC-004**: The modifier list remains readable in both bright and dark game lighting conditions (terminal always lit).

## Assumptions

- Players have access to the in-game terminal (default in ship).
- The existing modifier data structure (names, descriptions, isDebuff state) is accessible during gameplay.
- The "view modifiers" command does not conflict with critical terminal commands.
- The terminal display should not pause the game to maintain immersion.
- The "view modifiers" command is only available during active gameplay, not in menus or loading screens.
- The number of active modifiers is expected to be manageable (1-20 based on typical usage).

## Implementation Notes

- The original F2 overlay approach was replaced with terminal-based display for better integration with game UI.
- The overlay code remains in the codebase but is not actively used.
- Future enhancement: Could add "Modifiers" as a dedicated terminal page.

(End of file - total 124 lines)
