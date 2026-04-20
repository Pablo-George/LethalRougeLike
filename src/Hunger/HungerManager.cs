using System;
using HarmonyLib;
using LethalRogueLike.src.Config;
using UnityEngine;
using GameNetcodeStuff;

namespace LethalRogueLike.src.Hunger;

public class HungerManager : MonoBehaviour
{
    public static HungerManager Instance { get; private set; } = null!;

    private HungerData _hungerData = new();
    private bool _isInitialized = false;

    private float _lastTickTime = 0f;
    private float _tickInterval = 1f;

    private void Awake()
    {
        try
        {
            if (Instance != null)
            {
                Plugin.Logger.LogWarning("[HungerManager] Awake: instance already exists! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Plugin.Logger.LogInfo("[HungerManager] Initialized.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[HungerManager] Error in Awake: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(Instance, this))
        {
            Instance = null!;
        }
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        _hungerData = new HungerData();
        _hungerData.HungerDepletionRate = ModConfig.HungerDepletionRate?.Value ?? 2f;
        _hungerData.StarvationDamageRate = ModConfig.StarvationDamageRate?.Value ?? 5f;
        _isInitialized = true;
        Plugin.Logger.LogInfo("[HungerManager] Initialize: hunger reset to 100%.");
    }

    public void Reset()
    {
        _hungerData = new HungerData();
        _isInitialized = false;
    }

    private void Update()
    {
        var isTestingMode = Config.ModConfig.TestingMode?.Value ?? false;
        var isHungerEnabled = Config.ModConfig.EnableHungerSystem?.Value ?? false;

        if (!isHungerEnabled && !isTestingMode)
        {
            return;
        }

        if (!_isInitialized)
        {
            return;
        }

        var currentTime = Time.time;
        if (currentTime - _lastTickTime >= _tickInterval)
        {
            _lastTickTime = currentTime;
            ProcessHungerTick();
        }
    }

    private void ProcessHungerTick()
    {
        var depletionRate = ModConfig.HungerDepletionRate?.Value ?? 2f;
        _hungerData.HungerDepletionRate = depletionRate;
        var hungerLost = depletionRate / 60f;
        _hungerData.SetHunger(_hungerData.CurrentHunger - hungerLost);

        if (_hungerData.IsStarving && !_hungerData.DamagePending)
        {
            _hungerData.DamagePending = true;
            ApplyStarvationDamage();
        }
    }

    private void ApplyStarvationDamage()
    {
        try
        {
            var player = GetLocalPlayer();
            if (player == null)
            {
                return;
            }

            var damageRate = ModConfig.StarvationDamageRate?.Value ?? 5f;
            var damage = damageRate * _tickInterval;

            var healthField = AccessTools.Field(typeof(PlayerControllerB), "health");
            if (healthField != null)
            {
                var currentHealth = (float)healthField.GetValue(player);
                var newHealth = Mathf.Max(0, currentHealth - damage);
                healthField.SetValue(player, newHealth);
                Plugin.Logger.LogDebug($"[HungerManager] Starvation damage: -{damage:F1}, health: {newHealth}");
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[HungerManager] Error applying starvation damage: {ex.Message}");
        }
    }

    public bool ConsumeFood(float hungerRestore)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        _hungerData.DamagePending = false;

        var newHunger = _hungerData.CurrentHunger + hungerRestore;
        _hungerData.SetHunger(newHunger);

        Plugin.Logger.LogInfo($"[HungerManager] Consumed food: +{hungerRestore:F1} hunger, now: {_hungerData.CurrentHunger:F1}%");
        return true;
    }

    public HungerData GetHungerData()
    {
        return _hungerData;
    }

    public float GetHungerPercentage()
    {
        return _hungerData.GetHungerPercentage();
    }

    public bool IsStarving()
    {
        return _hungerData.IsStarving;
    }

    public void ConsumeNextFood()
    {
        var foods = StoredFoods.GetFoods();
        if (foods.Count == 0)
        {
            Plugin.Logger.LogInfo("[HungerManager] No food in inventory");
            return;
        }

        var food = foods[0];
        StoredFoods.RemoveFood(food.ItemID);
        ConsumeFood(food.HungerRestore);
    }

    private PlayerControllerB? GetLocalPlayer()
    {
        try
        {
            var players = UnityEngine.Object.FindObjectsOfType<PlayerControllerB>();
            if (players == null || players.Length == 0)
            {
                Plugin.Logger.LogDebug("[HungerManager] No players found");
                return null;
            }

            foreach (var player in players)
            {
                if (player.IsOwner)
                {
                    return player;
                }
            }

            return players[0];
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[HungerManager] Error finding local player: {ex.Message}");
            return null;
        }
    }
}