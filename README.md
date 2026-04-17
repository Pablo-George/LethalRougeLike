# LethalCompanyMod

A BepInEx mod for Lethal Company that adds a roguelike-style run modifier system.

## Features

- **Run Modifier System**: After every 3 landings, players choose from 2 random modifiers
- **4 Initial Modifiers**:
  - Less Loot (30% reduced scrap value)
  - Tiring Work (25% reduced stamina)
  - Hostile Planet (30% increased enemy spawns)
  - Scrap Rush (50% increased scrap quantity)
- **Multiplayer Synchronized**: All players see the same choices and effects
- **Modular Design**: Easy to add new modifiers

## Requirements

- BepInEx 5.4.21+ for Lethal Company
- Lethal Company (Steam)

## Installation

1. Install [BepInEx](https://github.com/BepInEx/BepInEx/releases) for Lethal Company
2. Copy `LethalCompanyMod.dll` to `BepInEx/plugins/`
3. Launch the game

## Configuration

Config file: `BepInEx/config/com.author.lethalcompanymod.cfg`

| Setting | Default | Description |
|---------|---------|-------------|
| `SelectionMethod` | `HostOnly` | How modifiers are selected |
| `SelectionTimeout` | 30 | Seconds before auto-selection |
| `EnableMod` | `true` | Master toggle |

## Building from Source

```bash
dotnet restore
dotnet build --configuration Release
```

Output: `bin/Release/net7.0/LethalCompanyMod.dll`

## Testing

1. Start a new run
2. Complete 3 landings
3. You should see a modifier selection prompt
4. Select a modifier and verify the effect

## License

MIT
