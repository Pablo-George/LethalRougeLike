using System;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalRogueLike;

[BepInPlugin("com.pablogeorge.lethalroguelike", "LethalRogueLike", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "com.pablogeorge.lethalroguelike";
    public const string PLUGIN_NAME = "LethalRogueLike";
    public const string PLUGIN_VERSION = "1.0.0";

    public static Plugin Instance { get; private set; } = null!;
    public static new ManualLogSource Logger { get; private set; } = null!;
    public static Harmony Harmony { get; private set; } = null!;

    private void Awake()
    {
        try
        {
            if (Instance != null)
            {
                base.Logger.LogError("FATAL: Plugin instance already exists! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Logger = base.Logger;
            Harmony = new Harmony(PLUGIN_GUID);

            Logger.LogMessage($"[{PLUGIN_NAME}] Loading v{PLUGIN_VERSION}...");

            Logger.LogInfo("[Init] Loading configuration...");
            src.Config.ModConfig.Load();

            Logger.LogInfo("[Init] Initializing RunState...");
            src.RunState.Instance.Initialize();

            Logger.LogInfo("[Init] Registering built-in modifiers...");
            src.Modifiers.ModifierRegistry.Instance.RegisterBuiltInModifiers();

            Logger.LogInfo("[Init] Initializing LandingHooks...");
            src.Hooks.LandingHooks.Initialize();

            Logger.LogInfo("[Init] Initializing ScrapHooks...");
            src.Hooks.ScrapHooks.Initialize();

            Logger.LogInfo("[Init] Initializing StaminaHooks...");
            src.Hooks.StaminaHooks.Initialize();

            Logger.LogInfo("[Init] Initializing EnemyHooks...");
            src.Hooks.EnemyHooks.Initialize();

            Logger.LogInfo("[Init] Initializing NetworkHandler...");
            src.Networking.NetworkHandler.Initialize();

            Logger.LogInfo("[Init] Creating ModifierSelectionUI...");
            var uiGo = new UnityEngine.GameObject("ModifierSelectionUI");
            uiGo.AddComponent<src.UI.ModifierSelectionUI>();

            Logger.LogInfo("[Init] Applying Harmony patches...");
            foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                try { Harmony.CreateClassProcessor(type).Patch(); }
                catch (Exception ex) { Logger.LogWarning($"[Harmony] Skipped patch {type.Name}: {ex.Message}"); }
            }
            var patched = string.Join(", ", Harmony.GetAllPatchedMethods().Select(m => m.DeclaringType?.Name + "." + m.Name));
            Logger.LogInfo($"[Harmony] Patched methods: {(string.IsNullOrEmpty(patched) ? "NONE" : patched)}");

            Logger.LogInfo($"[{PLUGIN_NAME}] Successfully loaded! Modifiers will trigger at game start and every 3 landings.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"[FATAL] Failed to initialize {PLUGIN_NAME}: {ex.GetType().Name}: {ex.Message}");
            Logger.LogError($"Stack trace: {ex.StackTrace}");
        }
    }

    private void OnDestroy()
    {
        // Do NOT unregister Harmony patches here. The plugin GameObject is destroyed
        // during the boot->main-menu scene transition, but patches must remain active
        // for the entire game session. All patch methods and singletons (RunState,
        // ModifierSelectionUI) are static and survive independently.
        Logger.LogDebug("[Shutdown] Plugin object destroyed (expected during scene transition - patches remain active).");
    }
}
