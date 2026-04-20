using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LethalRogueLike.src.UI;

public class ModifierSelectionUI : MonoBehaviour
{
    public static ModifierSelectionUI Instance { get; private set; } = null!;

    private bool _isVisible = false;
    private List<Modifiers.Modifier> _currentChoices = new();

    private GameObject _canvasObj = null!;
    private GameObject _panelObj = null!;
    private TextMeshProUGUI _titleText = null!;
    private Transform _buttonsContainer = null!;
    private TextMeshProUGUI _waitingText = null!;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
        CreateUIElements();
        Plugin.Logger.LogInfo("[UI] ModifierSelectionUI initialized.");
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(Instance, this))
            Instance = null!;
    }

    public void ShowSelection(Modifiers.Modifier[] choices)
    {
        try
        {
            if (choices == null || choices.Length < 2)
            {
                Plugin.Logger.LogWarning($"[UI] ShowSelection called with insufficient choices: {choices?.Length ?? 0}");
                return;
            }

            if (Instance == null)
            {
                Plugin.Logger.LogError("[UI] ShowSelection: Instance is null!");
                return;
            }

            _currentChoices.Clear();
            _currentChoices.AddRange(choices);
            _isVisible = true;

            UpdateButtonDisplay();
            gameObject.SetActive(true);

            Plugin.Logger.LogInfo($"[UI] Showing selection with {choices.Length} choices: {string.Join(", ", choices.Select(c => $"{c.Name} ({c.Id})"))}");

            if (Config.ModConfig.SelectionMethod.Value == Config.ModConfig.SelectionMethodType.RandomAuto)
            {
                var randomChoice = choices[UnityEngine.Random.Range(0, choices.Length)];
                Plugin.Logger.LogInfo($"[UI] Auto-selected: {randomChoice.Name}");
                SelectModifier(randomChoice.Id);
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] EXCEPTION in ShowSelection: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
            HideSelection();
        }
    }

    public void SelectModifier(string modifierId)
    {
        try
        {
            if (string.IsNullOrEmpty(modifierId) || !_isVisible) return;

            if (Networking.NetworkHandler.Instance != null && !Networking.NetworkHandler.Instance.IsHost)
            {
                Plugin.Logger.LogWarning("[UI] Non-host attempted to select modifier! Ignored.");
                return;
            }

            Plugin.Logger.LogInfo($"[UI] Selecting modifier: {modifierId}");
            RunState.Instance.SelectModifier(modifierId);
            HideSelection();
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] Error selecting modifier: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void VoteForModifier(string modifierId)
    {
        try
        {
            if (string.IsNullOrEmpty(modifierId) || !_isVisible) return;

            if (Networking.NetworkHandler.Instance != null)
            {
                if (Networking.NetworkHandler.Instance.IsHost)
                {
                    RunState.Instance.RecordVote("host", modifierId);
                    Plugin.Logger.LogInfo($"[UI] Host voted: {modifierId}");
                }
                else
                {
                    Networking.NetworkHandler.Instance.SendVote(modifierId);
                    Plugin.Logger.LogInfo($"[UI] Sent vote: {modifierId}");
                }
            }
            else
            {
                RunState.Instance.SelectModifier(modifierId);
                HideSelection();
                return;
            }

            _waitingText.text = "Vote cast! Waiting for others...";
            _waitingText.gameObject.SetActive(true);
            foreach (Transform child in _buttonsContainer)
                child.gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] Error voting: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void HideSelection()
    {
        try
        {
            _isVisible = false;
            _currentChoices.Clear();
            if (gameObject != null)
                gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] EXCEPTION in HideSelection: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void CreateUIElements()
    {
        _canvasObj = new GameObject("ModifierSelectionCanvas");
        var canvas = _canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var scaler = _canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        _canvasObj.AddComponent<GraphicRaycaster>();
        _canvasObj.transform.SetParent(gameObject.transform, false);

        // Main panel
        _panelObj = new GameObject("SelectionPanel");
        _panelObj.transform.SetParent(_canvasObj.transform, false);
        var panelImage = _panelObj.AddComponent<Image>();
        panelImage.color = UIConfiguration.SelectionBackground;
        var panelRect = _panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(840, 500);

        // Header area (top 18%)
        var headerObj = new GameObject("Header");
        headerObj.transform.SetParent(_panelObj.transform, false);
        var headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.82f);
        headerRect.anchorMax = new Vector2(1, 1f);
        headerRect.offsetMin = new Vector2(30, 0);
        headerRect.offsetMax = new Vector2(-30, 0);

        _titleText = CreateText("Title", headerObj.transform, "MODIFIER SELECTION", 30f, UIConfiguration.SelectionTitleColor, TextAlignmentOptions.Center);
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.characterSpacing = 4f;
        var titleRect = _titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.5f);
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        var subtitleText = CreateText("Subtitle", headerObj.transform, "Choose wisely — this modifier affects the entire crew", 15f, UIConfiguration.SelectionSubtitleColor, TextAlignmentOptions.Center);
        subtitleText.fontStyle = FontStyles.Italic;
        var subtitleRect = subtitleText.GetComponent<RectTransform>();
        subtitleRect.anchorMin = Vector2.zero;
        subtitleRect.anchorMax = new Vector2(1, 0.5f);
        subtitleRect.offsetMin = Vector2.zero;
        subtitleRect.offsetMax = Vector2.zero;

        // Cards container (middle 72%)
        var cardsAreaObj = new GameObject("CardsArea");
        cardsAreaObj.transform.SetParent(_panelObj.transform, false);
        var cardsAreaRect = cardsAreaObj.AddComponent<RectTransform>();
        cardsAreaRect.anchorMin = new Vector2(0, 0.14f);
        cardsAreaRect.anchorMax = new Vector2(1, 0.82f);
        cardsAreaRect.offsetMin = new Vector2(24, 0);
        cardsAreaRect.offsetMax = new Vector2(-24, 0);
        _buttonsContainer = cardsAreaObj.transform;

        // Waiting / status text (bottom 14%)
        _waitingText = CreateText("WaitingText", _panelObj.transform, "", 17f, UIConfiguration.SelectionSubtitleColor, TextAlignmentOptions.Center);
        _waitingText.fontStyle = FontStyles.Italic;
        var waitingRect = _waitingText.GetComponent<RectTransform>();
        waitingRect.anchorMin = new Vector2(0, 0);
        waitingRect.anchorMax = new Vector2(1, 0.14f);
        waitingRect.offsetMin = new Vector2(30, 0);
        waitingRect.offsetMax = new Vector2(-30, 0);
        _waitingText.gameObject.SetActive(false);
    }

    private void UpdateButtonDisplay()
    {
        if (_buttonsContainer == null) return;

        foreach (Transform child in _buttonsContainer)
            Destroy(child.gameObject);

        _waitingText.gameObject.SetActive(false);

        bool sideBySide = _currentChoices.Count == 2;
        for (int i = 0; i < _currentChoices.Count; i++)
            CreateModifierCard(_currentChoices[i], i, _currentChoices.Count, sideBySide);
    }

    private void CreateModifierCard(Modifiers.Modifier modifier, int index, int total, bool sideBySide)
    {
        var accentColor = modifier.IsDebuff ? UIConfiguration.SelectionDebuffAccent : UIConfiguration.SelectionBuffAccent;

        // Card background
        var cardObj = new GameObject($"Card_{modifier.Id}");
        cardObj.transform.SetParent(_buttonsContainer, false);
        var cardImage = cardObj.AddComponent<Image>();
        cardImage.color = UIConfiguration.SelectionCardBackground;
        var cardRect = cardObj.GetComponent<RectTransform>();

        if (sideBySide)
        {
            float gap = 0.04f;
            cardRect.anchorMin = new Vector2(index == 0 ? 0f : 0.5f + gap / 2f, 0f);
            cardRect.anchorMax = new Vector2(index == 0 ? 0.5f - gap / 2f : 1f, 1f);
        }
        else
        {
            float cardH = 1f / total;
            float padding = 0.015f;
            cardRect.anchorMin = new Vector2(0f, 1f - (index + 1) * cardH + padding);
            cardRect.anchorMax = new Vector2(1f, 1f - index * cardH - padding);
        }
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;

        // Colored accent bar along the top edge
        var accentBar = new GameObject("AccentBar");
        accentBar.transform.SetParent(cardObj.transform, false);
        var accentImage = accentBar.AddComponent<Image>();
        accentImage.color = accentColor;
        var accentRect = accentBar.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0, 1);
        accentRect.anchorMax = new Vector2(1, 1);
        accentRect.pivot = new Vector2(0.5f, 1);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(0, 5);

        // Content area with padding
        var contentObj = new GameObject("Content");
        contentObj.transform.SetParent(cardObj.transform, false);
        var contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(18, 14);
        contentRect.offsetMax = new Vector2(-18, -14);

        // BUFF / DEBUFF badge
        var badge = CreateText("Badge", contentObj.transform, modifier.IsDebuff ? "DEBUFF" : "BUFF", 12f, accentColor, TextAlignmentOptions.Left);
        badge.fontStyle = FontStyles.Bold;
        badge.characterSpacing = 3f;
        SetAnchors(badge, 0, 0.82f, 1, 1f);

        // Modifier name
        var nameText = CreateText("Name", contentObj.transform, modifier.Name, 24f, Color.white, TextAlignmentOptions.Left);
        nameText.fontStyle = FontStyles.Bold;
        SetAnchors(nameText, 0, 0.54f, 1, 0.82f);

        // Description
        var descText = CreateText("Description", contentObj.transform, modifier.Description, 14f, UIConfiguration.SelectionSubtitleColor, TextAlignmentOptions.Left);
        descText.enableWordWrapping = true;
        descText.overflowMode = TextOverflowModes.Truncate;
        SetAnchors(descText, 0, 0.20f, 1, 0.54f);

        // Severity dots
        int filled = Mathf.Clamp(Mathf.RoundToInt(Mathf.Clamp01(modifier.Severity) * 5), 1, 5);
        string accentHex = ColorUtility.ToHtmlStringRGB(accentColor);
        string dots = $"<color=#{accentHex}>{new string('●', filled)}</color><color=#444444>{new string('●', 5 - filled)}</color>  <color=#555555>intensity</color>";
        var sevText = CreateText("Severity", contentObj.transform, dots, 14f, Color.white, TextAlignmentOptions.Left);
        SetAnchors(sevText, 0, 0f, 1, 0.20f);

        // Button interaction
        var button = cardObj.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = UIConfiguration.SelectionCardBackground;
        colors.highlightedColor = UIConfiguration.SelectionCardHover;
        colors.pressedColor = UIConfiguration.SelectionCardPressed;
        colors.selectedColor = UIConfiguration.SelectionCardBackground;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.onClick.AddListener(() => VoteForModifier(modifier.Id));
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Color color, TextAlignmentOptions alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return tmp;
    }

    private static void SetAnchors(TextMeshProUGUI tmp, float xMin, float yMin, float xMax, float yMax)
    {
        var rect = tmp.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(xMin, yMin);
        rect.anchorMax = new Vector2(xMax, yMax);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
