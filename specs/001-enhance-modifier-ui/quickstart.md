# Quickstart: In-Game Active Modifiers Overlay

**Feature**: 001-enhance-modifier-ui | **Date**: 2026-04-17

## Prerequisites

- Unity 2021.4+ (Lethal Company version)
- Visual Studio or VS Code with C# extension
- BepInEx 5.x installed in Lethal Company

## Build & Deploy

### Build the Project

```bash
# From LethalCompanyMod/LethalCompanyMod directory
dotnet build
```

The project includes a custom target that copies the built DLL to your BepInEx plugins directory:

```xml
<Target Name="DeployToGame" AfterTargets="Build">
  <Copy SourceFiles="$(OutputPath)$(AssemblyName).dll" 
       DestinationFolder="$(GamePath)/BepInEx/plugins/$(AssemblyName)"
       OverwriteReadOnlyFiles="true" />
</Target>
```

**GamePath**: Configure in `LethalCompanyMod.csproj` (default: `/mnt/c/Program Files (x86)/Steam/steamapps/common/Lethal Company`)

### Manual Deploy (for development)

Copy `bin/Debug/netstandard2.1/LethalRogueLike.dll` to:
`{LethalCompany}/BepInEx/plugins/LethalRogueLike/LethalRogueLike.dll`

---

## Key Files

### Source Code Location

```
LethalCompanyMod/LethalCompanyMod/
├── src/
│   ├── UI/
│   │   ├── ModifierSelectionUI.cs    # Existing - pre-game selection
│   │   └── ActiveModifiersOverlay.cs  # NEW: In-game overlay (F2)
│   └── Config/
│       └── ModConfig.cs               # Existing - add OverlayHotkey config
```

### Key Implementation Points

**To change overlay hotkey**:
Edit `ModConfig.OverlayHotkey` (default: F2)

**To change overlay position**:
Edit `UIConfiguration` class constants

**To change styling**:
Edit `UIConfiguration` color values

---

## Testing

### In-Game Overlay Testing

- [ ] Start a game with known modifiers selected
- [ ] Press F2 during gameplay - verify overlay appears within 500ms
- [ ] Verify overlay shows only ACTIVE modifiers (not all available)
- [ ] Verify modifier names are readable
- [ ] Verify overlay is semi-transparent (game visible behind)
- [ ] Verify overlay does not obstruct >20% of screen
- [ ] Press F2 again - verify overlay dismisses
- [ ] Press Escape with overlay visible - verify dismisses
- [ ] Press F2 with no modifiers active - verify "No active modifiers" message
- [ ] Enter a building/moon - verify overlay still works
- [ ] Open game menu (ESC during gameplay) - verify overlay hides
- [ ] Check that overlay does NOT pause the game
- [ ] Test with 10+ modifiers - verify scroll works

### Debug Output

The mod logs to `BepInEx/LogOutput.log`:

```csharp
Plugin.Logger.LogInfo("[Overlay] F2 pressed - toggling overlay");
// Check log for: [Overlay] F2 pressed - toggling overlay

Plugin.Logger.LogInfo("[Overlay] Showing overlay with {count} active modifiers");
// Check log for: [Overlay] Showing overlay with X active modifiers

Plugin.Logger.LogInfo("[Overlay] Game not active - overlay suppressed");
// Check log when pressing F2 in menu

Plugin.Logger.LogInfo("[Overlay] No active modifiers - showing empty message");
// Check log when no modifiers active
```

---

## Common Issues

### In-game overlay not appearing

- Verify `ActiveModifiersOverlay.Instance` is not null
- Check that you're in an active game (not menu/title)
- Check for `[Overlay]` logs
- Verify hotkey is not bound to another mod
- Try pressing F2 again (toggle behavior)

### Overlay showing wrong modifiers

- Verify `RunState.ActiveModifiers` contains expected modifiers
- Check that overlay reads from correct data source
- Verify no stale data in ActiveModifiers list

### Overlay obstructs too much screen

- Check `UIConfiguration.OverlayMaxWidth` (should be 300px)
- Verify scroll is enabled for >8 modifiers

---

## Next Steps

1. Run `/speckit.tasks` to generate task list
2. Implement `ActiveModifiersOverlay.cs` component
3. Add `OverlayHotkey` to `ModConfig.cs`
4. Test in-game
5. Verify against acceptance criteria in spec.md

---

**Status**: COMPLETE | **Ready for**: /speckit.tasks
