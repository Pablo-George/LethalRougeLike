# Lethal Company Modding Learnings & Patterns

**Created**: 2026-04-17  
**Purpose**: Document key learnings, patterns, and gotchas from building this Lethal Company mod.

---

## 1. Plugin Architecture

### Core Setup
- Use `BaseUnityPlugin` as base class with `[BepInPlugin(...)]` attribute
- Create static `Instance` and `Logger` for global access
- Initialize `Harmony` with unique GUID (your plugin's ID)

```csharp
[BepInPlugin("com.pablogeorge.lethalroguelike", "LethalRogueLike", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; private set; } = null!;
    public static new ManualLogSource Logger { get; private set; } = null!;
    public static Harmony Harmony { get; private set; } = null!;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);  // Keep plugin object alive
        Logger = base.Logger;
        Harmony = new Harmony(PLUGIN_GUID);
        // Initialize hooks, UI, etc.
    }
}
```

---

## 2. Scene Transitions & Object Lifetime

### Critical Gotcha
The plugin GameObject gets **destroyed** during the boot → main-menu scene transition, but:
- **Harmony patches persist** for the entire game session
- **Static singletons persist** (RunState, ModifierRegistry, etc.)
- **UI components with DontDestroyOnLoad persist**

### Pattern
```csharp
// In Awake of MonoBehaviours that need to persist
private void Awake()
{
    if (Instance != null)
    {
        Destroy(gameObject);  // Prevent duplicates
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);  // Survives scene transitions
}
```

### OnDestroy
```csharp
private void OnDestroy()
{
    if (ReferenceEquals(Instance, this))
    {
        Instance = null!;  // Only clear if we're the instance
    }
}
```

---

## 3. Harmony Patching

### Target Game Methods
```csharp
[HarmonyPatch(typeof(StartOfRound), "OnShipLandedMiscEvents")]
public class StartOfRound_OnShipLanded_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        // Your code here
    }
}
```

### Key Points
- Use `Prefix` for before game code runs, `Postfix` for after
- Wrap all patch code in try-catch (game can crash otherwise)
- Check config flags early to allow disabling mod
- Log extensively for debugging

---

## 4. Finding Game Objects & Players

### Finding Scene Objects
```csharp
var roundManager = UnityEngine.Object.FindObjectOfType<StartOfRound>();
if (roundManager == null) return;
```

### Finding Local Player
```csharp
var players = UnityEngine.Object.FindObjectsOfType<PlayerControllerB>();
foreach (var player in players)
{
    if (player.IsOwner) return player;
}
return players[0];  // Fallback
```

---

## 5. UI Creation (Programmatic)

### Create Canvas with Overlay
```csharp
var canvasObj = new GameObject("MyCanvas");
var canvas = canvasObj.AddComponent<Canvas>();
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
canvas.sortingOrder = 1000;  // High to appear on top
var scaler = canvasObj.AddComponent<CanvasScaler>();
scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
scaler.referenceResolution = new Vector2(1920, 1080);
```

### Use CanvasGroup for Fading
```csharp
var canvasGroup = panelObj.AddComponent<CanvasGroup>();
canvasGroup.alpha = 0f;
canvasGroup.blocksRaycasts = false;  // Let clicks pass through

// In Update for fade
_canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _targetAlpha, elapsed);
```

---

## 6. Networking (Netcode for GameObjects)

### NetworkBehaviour Pattern
```csharp
public class NetworkHandler : NetworkBehaviour
{
    public static NetworkHandler? Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        Instance = this;
        if (NetworkManager.Singleton?.IsServer == true)
        {
            // Register message handlers
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                "MyMessageType", HandleMyMessage);
        }
    }
}
```

### Sending Messages
```csharp
using var writer = new FastBufferWriter(256, Allocator.Temp);
writer.WriteValueSafe(myMessage);
NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
    "MyMessageType", targetClientId, writer);
```

### Important Checks
```csharp
// Only run on server/host
if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer) return;

// Only run on clients
if (NetworkManager.Singleton.IsServer) return;
```

---

## 7. Configuration

### Using BepInEx Configuration
```csharp
public static ConfigEntry<bool>? EnableMod { get; private set; }

// In Load():
EnableMod = Config.Bind("General", "EnableMod", true, "Enable the mod");
```

### Safe Access with Defaults
```csharp
if (!Config.ModConfig.EnableMod?.Value ?? true)  // Default to true if null
{
    // mod enabled
}
```

---

## 8. Debug Logging Patterns

### Log Levels
```csharp
Logger.LogMessage(...)  // Startup/important events
Logger.LogInfo(...)     // Normal operation
Logger.LogDebug(...)   // Verbose debugging
Logger.LogWarning(...) // Recoverable issues
Logger.LogError(...)   // Errors (always include ex.Message)
```

### Debug Output in Patches
```csharp
System.Console.WriteLine("DEBUG: Variable=" + value);  // Shows in game console
```

---

## 9. Run State Management

### Static Singleton Pattern
```csharp
public class RunState
{
    public static RunState Instance { get; private set; } = new RunState();
    // Properties: RunId, LandingCount, ActiveModifiers, etc.
}
```

### Reset on New Run
```csharp
public void StartNewRun()
{
    Reset();
    RunId = Guid.NewGuid().ToString("N").Substring(0, 8);
}
```

---

## 10. Common Hook Points

| Game Event | Target Method | Patch Type |
|------------|--------------|------------|
| Game Start | `StartOfRound.StartGame` | Prefix |
| Ship Landed | `StartOfRound.OnShipLandedMiscEvents` | Postfix |
| Ship Leaves | `StartOfRound.ShipLeave` | Prefix |
| Player Death | `PlayerControllerB.KillPlayer` | Postfix |

---

## 11. Key Classes & Namespaces

- `StartOfRound` - Main game manager (scene management, players, etc.)
- `PlayerControllerB` - Player controls, health, inventory
- `GrabbableObject` - Scrap items, held objects
- `EnemyAI` - All enemy types
- `HUDManager` - UI elements like Scrap total
- `Terminal` - In-game terminal

---

## 12. Important Gotchas

1. **Always null-check** - Unity objects can be null in different scenes
2. **Wrap Update() logic** - Game can change scenes mid-frame
3. **Use try-catch in Update** - Prevents mod from breaking entire game
4. **Don't destroy on load** - UI needs to persist across scene transitions
5. **Check config first** - Allow mod to be disabled at runtime
6. **Static is forever** - Use for cross-scene state, but be careful with initialization

---

## 13. Testing Tips

- Use `System.Console.WriteLine` for real-time debugging in game
- Check BepInEx log file for errors
- Test in both solo and multiplayer
- Verify behavior across scene transitions (boot → menu → game → results → menu)