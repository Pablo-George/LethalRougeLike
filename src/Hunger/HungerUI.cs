using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LethalRogueLike.src.Config;

namespace LethalRogueLike.src.Hunger;

public class HungerUI : MonoBehaviour
{
    public static HungerUI Instance { get; private set; } = null!;

    private GameObject _canvasObj = null!;
    private GameObject _barContainer = null!;
    private GameObject _backgroundBar = null!;
    private GameObject _fillBar = null!;
    private TextMeshProUGUI _percentageText = null!;

    private RectTransform _fillBarRect = null!;
    private Image _fillImage = null!;

    private float _barWidth = 150f;
    private float _barHeight = 20f;
    private float _padding = 10f;

    private void Awake()
    {
        try
        {
            if (Instance != null)
            {
                Plugin.Logger.LogWarning("[HungerUI] Awake: instance already exists! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CreateHungerBar();
            DontDestroyOnLoad(gameObject);
            Plugin.Logger.LogInfo("[HungerUI] Initialized.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[HungerUI] Error in Awake: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(Instance, this))
        {
            Instance = null!;
        }
    }

    private void Update()
    {
        if (!Config.ModConfig.EnableHungerSystem?.Value ?? false)
        {
            Hide();
            return;
        }

        if (!Config.ModConfig.ShowHungerBar?.Value ?? true)
        {
            Hide();
            return;
        }

        var hungerManager = HungerManager.Instance;
        if (hungerManager == null)
        {
            Hide();
            return;
        }

        var hungerData = hungerManager.GetHungerData();
        if (hungerData == null)
        {
            Hide();
            return;
        }

        Show();
        UpdateBar(hungerData.GetHungerPercentage());
    }

    private void UpdateBar(float percentage)
    {
        try
        {
            percentage = Mathf.Clamp01(percentage);
            if (_fillBarRect != null)
            {
                _fillBarRect.sizeDelta = new Vector2(_barWidth * percentage, _barHeight);
            }

            if (_fillImage != null)
            {
                var hungerPercent = percentage * 100f;
                if (hungerPercent <= 25f)
                {
                    _fillImage.color = new Color(0.8f, 0.2f, 0.2f);
                }
                else if (hungerPercent <= 50f)
                {
                    _fillImage.color = new Color(0.9f, 0.6f, 0.2f);
                }
                else
                {
                    _fillImage.color = new Color(0.2f, 0.8f, 0.2f);
                }
            }

            if (_percentageText != null)
            {
                _percentageText.text = $"{percentage * 100f:F0}%";
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[HungerUI] Error updating bar: {ex.Message}");
        }
    }

    private void Show()
    {
        if (_canvasObj != null && !_canvasObj.activeSelf)
        {
            _canvasObj.SetActive(true);
        }
    }

    private void Hide()
    {
        if (_canvasObj != null && _canvasObj.activeSelf)
        {
            _canvasObj.SetActive(false);
        }
    }

    private void CreateHungerBar()
    {
        _canvasObj = new GameObject("HungerBarCanvas");
        var canvas = _canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var scaler = _canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        _canvasObj.AddComponent<GraphicRaycaster>();
        _canvasObj.transform.SetParent(gameObject.transform, false);
        _canvasObj.SetActive(false);

        _barContainer = new GameObject("HungerBarContainer");
        _barContainer.transform.SetParent(_canvasObj.transform, false);
        var containerRect = _barContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(1f, 1f);
        containerRect.anchorMax = new Vector2(1f, 1f);
        containerRect.pivot = new Vector2(1f, 1f);
        containerRect.anchoredPosition = new Vector2(-_padding, -_padding);
        containerRect.sizeDelta = Vector2.zero;

        _backgroundBar = new GameObject("BackgroundBar");
        _backgroundBar.transform.SetParent(_barContainer.transform, false);
        var bgRect = _backgroundBar.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(_barWidth, _barHeight);
        var bgImage = _backgroundBar.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        _fillBar = new GameObject("FillBar");
        _fillBar.transform.SetParent(_backgroundBar.transform, false);
        _fillBarRect = _fillBar.AddComponent<RectTransform>();
        _fillBarRect.anchorMin = new Vector2(0f, 0f);
        _fillBarRect.anchorMax = new Vector2(0f, 1f);
        _fillBarRect.pivot = new Vector2(0f, 0.5f);
        _fillBarRect.anchoredPosition = Vector2.zero;
        _fillBarRect.sizeDelta = new Vector2(_barWidth, _barHeight);
        _fillImage = _fillBar.AddComponent<Image>();
        _fillImage.color = new Color(0.2f, 0.8f, 0.2f);

        var textObj = new GameObject("PercentageText");
        textObj.transform.SetParent(_barContainer.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(-_barWidth / 2f, -_barHeight - 5f);
        textRect.sizeDelta = new Vector2(_barWidth, 20f);
        _percentageText = textObj.AddComponent<TextMeshProUGUI>();
        _percentageText.text = "100%";
        _percentageText.fontSize = 14;
        _percentageText.color = Color.white;
        _percentageText.alignment = TextAlignmentOptions.Center;
    }
}