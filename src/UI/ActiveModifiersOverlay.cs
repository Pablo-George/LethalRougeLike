using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

namespace LethalRogueLike.src.UI;

public class ActiveModifiersOverlay : MonoBehaviour
{
    public static ActiveModifiersOverlay Instance { get; private set; } = null!;
    public static bool NeedsRecreation { get; set; } = false;

    private bool _isVisible = false;
    private bool _isGameActive = false;
    private bool _isFading = false;
    private int _updateFrameCount = 0;

    private GameObject _canvasObj = null!;
    private GameObject _panelObj = null!;
    private GameObject _viewportObj = null!;
    private GameObject _contentObj = null!;
    private TextMeshProUGUI _titleText = null!;
    private TextMeshProUGUI _listText = null!;
    private CanvasGroup _canvasGroup = null!;
    private ScrollRect _scrollRect = null!;

    private float _fadeInDuration = 0.2f;
    private float _fadeOutDuration = 0.15f;
    private float _fadeTimer = 0f;
    private float _targetAlpha = 0f;

    private void Awake()
    {
        Plugin.Logger.LogInfo("[Overlay] Awake: starting.");
        try
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            Plugin.Logger.LogInfo($"[Overlay] Awake: scene='{activeScene.name}', buildIndex={activeScene.buildIndex}, gameObject.name={gameObject.name}, transform.parent={(transform.parent != null ? transform.parent.name : "none")}");

            if (Instance != null)
            {
                Plugin.Logger.LogWarning("[Overlay] Awake: instance already exists! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Plugin.Logger.LogInfo("[Overlay] Awake: calling CreateOverlayElements...");
            CreateOverlayElements();
            Plugin.Logger.LogInfo($"[Overlay] Awake: CreateOverlayElements done. _canvasGroup={((_canvasGroup != null) ? "OK" : "NULL")}, _titleText={((_titleText != null) ? "OK" : "NULL")}, _listText={((_listText != null) ? "OK" : "NULL")}");

            gameObject.SetActive(false);
            Plugin.Logger.LogInfo("[Overlay] ActiveModifiersOverlay initialized successfully.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Overlay] Error in Awake: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void OnDestroy()
    {
        Plugin.Logger.LogInfo("[Overlay] OnDestroy: clearing Instance.");
        if (ReferenceEquals(Instance, this))
        {
            Instance = null!;
            NeedsRecreation = true;
        }
    }

    private void Update()
    {
        try
        {
            _updateFrameCount++;
            bool logThisFrame = (_updateFrameCount % 300 == 1);

            if (logThisFrame)
                Plugin.Logger.LogInfo($"[Overlay] Update heartbeat #{_updateFrameCount}: Instance={(Instance != null ? "OK" : "NULL")}, _isVisible={_isVisible}, _isGameActive={_isGameActive}, scene='{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}'");

            if (Instance == null)
            {
                if (logThisFrame) Plugin.Logger.LogWarning("[Overlay] Update: Instance is null — this component is orphaned.");
                return;
            }

            bool prevGameActive = _isGameActive;
            _isGameActive = IsGameActive();
            if (logThisFrame || _isGameActive != prevGameActive)
                Plugin.Logger.LogInfo($"[Overlay] IsGameActive={_isGameActive} (changed={_isGameActive != prevGameActive})");

            if (!_isGameActive && _isVisible)
            {
                Plugin.Logger.LogInfo("[Overlay] Game no longer active, hiding overlay.");
                Hide();
                return;
            }

            if (!_isGameActive) return;

            HandleFade();

            var kb = Keyboard.current;
            if (kb == null) return;

            var hotkeyCode = Config.ModConfig.OverlayHotkey?.Value ?? KeyCode.O;
            if (logThisFrame)
                Plugin.Logger.LogInfo($"[Overlay] Polling hotkey={hotkeyCode}");

            if (System.Enum.TryParse<Key>(hotkeyCode.ToString(), out var hotkeyKey) && hotkeyKey != Key.None)
            {
                if (kb[hotkeyKey].wasPressedThisFrame)
                {
                    Plugin.Logger.LogInfo($"[Overlay] Hotkey {hotkeyCode} pressed — toggling overlay.");
                    Toggle();
                }
            }

            if (_isVisible && kb[Key.Escape].wasPressedThisFrame)
            {
                Plugin.Logger.LogInfo("[Overlay] Escape pressed — hiding overlay.");
                Hide();
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Overlay] Error in Update (frame {_updateFrameCount}): {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void HandleFade()
    {
        if (_isFading)
        {
            _fadeTimer += Time.deltaTime;
            var duration = _targetAlpha > 0 ? _fadeInDuration : _fadeOutDuration;
            var elapsed = Mathf.Min(_fadeTimer / duration, 1f);
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _targetAlpha, elapsed);

            if (elapsed >= 1f)
            {
                _isFading = false;
                _canvasGroup.alpha = _targetAlpha;
                if (_targetAlpha <= 0)
                {
                    _isVisible = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private bool IsGameActive()
    {
        try
        {
            var roundManager = UnityEngine.Object.FindObjectOfType<StartOfRound>();
            if (roundManager == null)
            {
                Plugin.Logger.LogDebug("[Overlay] IsGameActive: StartOfRound not found — game not active.");
                return false;
            }

            bool result = !roundManager.allPlayersDead;
            Plugin.Logger.LogDebug($"[Overlay] IsGameActive: StartOfRound found, allPlayersDead={roundManager.allPlayersDead}, returning {result}.");
            return result;
        }
        catch (System.Exception ex)
        {
            Plugin.Logger.LogWarning($"[Overlay] IsGameActive check threw {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    public void Show()
    {
        try
        {
            if (Instance == null) return;

            if (!_isGameActive)
            {
                _isGameActive = IsGameActive();
                if (!_isGameActive)
                {
                    return;
                }
            }

            UpdateDisplay();
            _isVisible = true;
            gameObject.SetActive(true);
            _targetAlpha = 1f;
            _fadeTimer = 0f;
            _isFading = true;
            _canvasGroup.alpha = 0f;
            Plugin.Logger.LogInfo("[Overlay] Show: overlay visible.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Overlay] Error in Show: {ex.GetType().Name}: {ex.Message}");
        }
    }

    public void Hide()
    {
        try
        {
            _targetAlpha = 0f;
            _fadeTimer = 0f;
            _isFading = true;
            Plugin.Logger.LogDebug("[Overlay] Hide: fading out overlay.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Overlay] Error in Hide: {ex.GetType().Name}: {ex.Message}");
            _isVisible = false;
            gameObject.SetActive(false);
        }
    }

    public void Toggle()
    {
        if (_isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void UpdateDisplay()
    {
        try
        {
            if (RunState.Instance == null || RunState.Instance.ActiveModifiers == null)
            {
                _titleText.text = "Active Modifiers";
                _listText.text = "No active modifiers";
                return;
            }

            var active = RunState.Instance.ActiveModifiers;
            if (active.Count == 0)
            {
                _titleText.text = "Active Modifiers";
                _listText.text = "No active modifiers";
                return;
            }

            _titleText.text = $"Active Modifiers ({active.Count})";

            var lines = new List<string>();
            foreach (var am in active)
            {
                var modifier = Modifiers.ModifierRegistry.Instance.GetModifier(am.ModifierId);
                var isDebuff = modifier?.IsDebuff ?? false;
                var colorHex = isDebuff ? "<color=#ff6666>[D]</color> " : "<color=#66ff66>[B]</color> ";
                var name = modifier?.Name ?? am.ModifierId;
                lines.Add($"{colorHex}{name}");
            }

            _listText.text = string.Join("\n", lines);

            if (_scrollRect != null)
            {
                _scrollRect.verticalNormalizedPosition = 1f;
            }

            Plugin.Logger.LogDebug($"[Overlay] Displaying {active.Count} active modifiers.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[Overlay] Error in UpdateDisplay: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void CreateOverlayElements()
    {
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 1 — creating canvas.");
        _canvasObj = new GameObject("ActiveModifiersCanvas");
        var canvas = _canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        var scaler = _canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        _canvasObj.AddComponent<GraphicRaycaster>();
        _canvasObj.transform.SetParent(gameObject.transform, false);
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 1 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 2 — creating panel.");
        _panelObj = new GameObject("OverlayPanel");
        _panelObj.transform.SetParent(_canvasObj.transform, false);
        var panelImage = _panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.75f);
        var panelRect = _panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-10f, -10f);
        panelRect.sizeDelta = new Vector2(280f, 220f);
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 2 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 3 — adding CanvasGroup.");
        _canvasGroup = _panelObj.AddComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 3 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 4 — creating viewport + ScrollRect.");
        _viewportObj = new GameObject("Viewport");
        _viewportObj.transform.SetParent(_panelObj.transform, false);
        var viewportRect = _viewportObj.AddComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0f, 0f);
        viewportRect.anchorMax = new Vector2(1f, 1f);
        viewportRect.offsetMin = new Vector2(8f, 8f);
        viewportRect.offsetMax = new Vector2(-8f, -28f);
        var viewportImage = _viewportObj.AddComponent<Image>();
        viewportImage.color = Color.clear;
        _scrollRect = _viewportObj.AddComponent<ScrollRect>();
        _scrollRect.scrollSensitivity = 20f;
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 4 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 5 — creating content.");
        _contentObj = new GameObject("Content");
        _contentObj.transform.SetParent(_viewportObj.transform, false);
        var contentRect = _contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 0f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        _scrollRect.content = contentRect;
        _scrollRect.viewport = viewportRect;
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 5 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 6 — creating list text.");
        _listText = CreateTextObj("ListText", _contentObj.transform, "", 16);
        _listText.alignment = TextAlignmentOptions.TopLeft;
        var listRect = _listText.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 0f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.offsetMin = Vector2.zero;
        listRect.offsetMax = Vector2.zero;
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 6 done.");

        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 7 — creating title text.");
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(_panelObj.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -26f);
        titleRect.sizeDelta = new Vector2(0f, 24f);
        _titleText = titleObj.AddComponent<TextMeshProUGUI>();
        _titleText.text = "Active Modifiers";
        _titleText.fontSize = 18;
        _titleText.color = Color.white;
        _titleText.alignment = TextAlignmentOptions.Center;
        Plugin.Logger.LogInfo("[Overlay] CreateOverlayElements: step 7 done — all elements created.");
    }

    private TextMeshProUGUI CreateTextObj(string name, Transform parent, string text, int fontSize = 18)
    {
        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = Color.white;
        var rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return textMesh;
    }
}