# Data Model: Hunger System

## Entities

### PlayerHunger

Represents a player's current hunger state.

| Field | Type | Description |
|-------|------|-------------|
| CurrentHunger | float | Current hunger value (0.0 - 100.0) |
| MaxHunger | float | Maximum hunger (always 100.0) |
| HungerDepletionRate | float | Hunger lost per second |
| StarvationDamageRate | float | Health lost per second when starving |
| IsStarving | bool | True when hunger is at 0 |

### FoodItem

Represents a food item that can restore hunger.

| Field | Type | Description |
|-------|------|-------------|
| ItemID | string | Unique identifier |
| DisplayName | string | Player-facing name |
| HungerRestore | float | Amount of hunger restored (e.g., 25.0) |
| Price | int | Purchase price in credits |
| Description | string | Flavor text |

### HungerBarUI

UI element displaying current hunger.

| Field | Type | Description |
|-------|------|-------------|
| Position | Vector2 | Screen position (top-right) |
| Width | float | Bar width in pixels |
| Height | float | Bar height in pixels |
| BackgroundColor | Color | Empty bar color |
| FillColor | Color | Filled bar color |
| ShowPercentage | bool | Display numeric percentage |

## State Transitions

### Hunger State Machine

```
[Full (100%)] ----decrease----> [Normal (1-99%)]
[Normal (1-99%)] ----decrease----> [Critical (0%)]
[Critical (0%)] ----starvation----> [Starving (taking damage)]
[Starving] ---- eat food ----> [Normal (1-99%)]
[Any state] ---- eat food ----> [Increased but ≤100%]
```

## Relationships

- **Player** → has one → **PlayerHunger**
- **Player** → has many → **FoodItem** (via inventory)
- **UI** → displays → **HungerBarUI** (reads PlayerHunger)
- **Store** → sells → **FoodItem**
- **PlayerHunger** → triggers → **StarvationDamage** (when at 0)

## Validation Rules

- Hunger cannot exceed 100.0
- Hunger cannot go below 0.0
- Food items must have positive HungerRestore value
- Food items must have non-negative Price
- StarvationDamageRate should be balanced to allow survival with food