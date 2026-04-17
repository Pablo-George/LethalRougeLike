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
        Plugin.Logger.LogInfo("[UI] Awake: starting.");
        try
        {
            if (Instance != null)
            {
                Plugin.Logger.LogWarning("[UI] Awake: instance already exists! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            gameObject.SetActive(false);
            Plugin.Logger.LogInfo("[UI] Awake: calling CreateUIElements.");
            CreateUIElements();
            Plugin.Logger.LogInfo("[UI] Awake: CreateUIElements done. _buttonsContainer=" + (_buttonsContainer != null ? "OK" : "NULL"));
            DontDestroyOnLoad(gameObject);
            Plugin.Logger.LogInfo("[UI] ModifierSelectionUI initialized.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] Error in Awake: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void OnDestroy()
    {
        Plugin.Logger.LogInfo("[UI] OnDestroy: clearing Instance.");
        if (ReferenceEquals(Instance, this))
        {
            Instance = null!;
        }
    }

    public void ShowSelection(Modifiers.Modifier[] choices)
    {
        try
        {
            System.Console.WriteLine("DEBUG: ShowSelection ENTERED");
            System.Console.WriteLine("DEBUG: choices=" + (choices != null ? choices.Length.ToString() : "NULL"));
            
            bool thisIsNull = ReferenceEquals(null, this);
            System.Console.WriteLine("DEBUG: thisIsNull=" + thisIsNull);
            
            bool instIsNull = ReferenceEquals(null, ModifierSelectionUI.Instance);
            System.Console.WriteLine("DEBUG: instIsNull=" + instIsNull);
            
            if (instIsNull)
            {
                System.Console.WriteLine("DEBUG: Instance is null, recreating UI...");
                Plugin.Logger.LogInfo("[UI] ShowSelection: Instance is null, recreating UI...");
                
                var newGo = new UnityEngine.GameObject("ModifierSelectionUI");
                newGo.AddComponent<ModifierSelectionUI>();
                
                instIsNull = ReferenceEquals(null, ModifierSelectionUI.Instance);
                System.Console.WriteLine("DEBUG: After recreation, instIsNull=" + instIsNull);
                
                if (instIsNull)
                {
                    System.Console.WriteLine("DEBUG: Failed to recreate UI!");
                    Plugin.Logger.LogError("[UI] ShowSelection: Failed to recreate UI!");
                    return;
                }
            }
            
            System.Console.WriteLine("DEBUG: about to access base.gameObject...");
            bool goIsNull = ReferenceEquals(null, base.gameObject);
            System.Console.WriteLine("DEBUG: base.gameObject access succeeded, goIsNull=" + goIsNull);
            
            Plugin.Logger.LogInfo("[UI] ShowSelection: START. this=" + (thisIsNull ? "NULL" : "OK") + ", Instance=" + (instIsNull ? "NULL" : "OK") + ", gameObject=" + (goIsNull ? "NULL" : "OK"));
            if (thisIsNull || instIsNull || goIsNull)
            {
                Plugin.Logger.LogError("[UI] ShowSelection: this, Instance, or gameObject is null! Cannot show selection.");
                return;
            }

            if (choices == null)
            {
                Plugin.Logger.LogWarning("[UI] ShowSelection called with null choices!");
                return;
            }

            if (choices.Length < 2)
            {
                Plugin.Logger.LogWarning($"[UI] ShowSelection called with insufficient choices: {choices.Length}");
                return;
            }

            Plugin.Logger.LogInfo("[UI] ShowSelection: updating choices list.");
            _currentChoices.Clear();
            _currentChoices.AddRange(choices);
            _isVisible = true;

            Plugin.Logger.LogInfo("[UI] ShowSelection: calling UpdateButtonDisplay.");
            try
            {
                UpdateButtonDisplay();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError("[UI] UpdateButtonDisplay threw: " + ex.Message);
            }
            Plugin.Logger.LogInfo("[UI] ShowSelection: UpdateButtonDisplay done.");

            Plugin.Logger.LogInfo("[UI] ShowSelection: activating gameObject.");
            if (gameObject == null)
            {
                Plugin.Logger.LogError("[UI] ShowSelection: gameObject became null before SetActive!");
                return;
            }
            System.Console.WriteLine("DEBUG: About to call SetActive(true)");
            gameObject.SetActive(true);
            System.Console.WriteLine("DEBUG: SetActive(true) called");
            Plugin.Logger.LogInfo("[UI] ShowSelection: gameObject activated.");
            Plugin.Logger.LogInfo($"[UI] Showing selection UI with {choices.Length} choices: {string.Join(", ", choices.Select(c => $"{c.Name} ({c.Id})"))}");

            if (Config.ModConfig.SelectionMethod.Value == Config.ModConfig.SelectionMethodType.RandomAuto)
            {
                Plugin.Logger.LogInfo("[UI] RandomAuto selection enabled. Auto-selecting...");
                var randomChoice = choices[UnityEngine.Random.Range(0, choices.Length)];
                Plugin.Logger.LogInfo($"[UI] Auto-selected: {randomChoice.Name}");
                SelectModifier(randomChoice.Id);
            }
            else
            {
                Plugin.Logger.LogInfo("[UI] HostOnly selection - waiting for host to choose.");
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] EXCEPTION in ShowSelection: {ex.GetType().Name}: {ex.Message}");
            Plugin.Logger.LogError($"[UI] Stack: {ex.StackTrace}");
            HideSelection();
        }
    }

    public void SelectModifier(string modifierId)
    {
        try
        {
            if (string.IsNullOrEmpty(modifierId))
            {
                Plugin.Logger.LogWarning("[UI] SelectModifier called with null/empty modifierId!");
                return;
            }

            if (!_isVisible)
            {
                Plugin.Logger.LogWarning($"[UI] SelectModifier called but UI not visible! (modifierId: {modifierId})");
                return;
            }

            if (Networking.NetworkHandler.Instance != null && !Networking.NetworkHandler.Instance.IsHost)
            {
                Plugin.Logger.LogWarning("[UI] Non-host attempted to select modifier! Ignored.");
                return;
            }

            Plugin.Logger.LogInfo($"[UI] Player selecting modifier: {modifierId}");
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
            if (string.IsNullOrEmpty(modifierId))
            {
                Plugin.Logger.LogWarning("[UI] VoteForModifier called with null/empty modifierId!");
                return;
            }

            if (!_isVisible)
            {
                Plugin.Logger.LogWarning($"[UI] VoteForModifier called but UI not visible! (modifierId: {modifierId})");
                return;
            }

            if (Networking.NetworkHandler.Instance != null)
            {
                if (Networking.NetworkHandler.Instance.IsHost)
                {
                    RunState.Instance.RecordVote("host", modifierId);
                    Plugin.Logger.LogInfo($"[UI] Host recorded vote locally: {modifierId}");
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
            {
                child.gameObject.SetActive(false);
            }
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
            if (gameObject == null)
            {
                Plugin.Logger.LogWarning("[UI] HideSelection: gameObject is null, skipping.");
                _isVisible = false;
                _currentChoices.Clear();
                return;
            }
            _isVisible = false;
            _currentChoices.Clear();
            gameObject.SetActive(false);
            Plugin.Logger.LogDebug("[UI] Selection UI hidden.");
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError($"[UI] EXCEPTION in HideSelection: {ex.GetType().Name}: {ex.Message}");
            Plugin.Logger.LogError($"[UI] Stack: {ex.StackTrace}");
        }
    }

    private void CreateUIElements()
    {
        Plugin.Logger.LogInfo("[UI] CreateUIElements: creating canvas.");
        _canvasObj = new GameObject("ModifierSelectionCanvas");
        var canvas = _canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var scaler = _canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        _canvasObj.AddComponent<GraphicRaycaster>();
        _canvasObj.transform.SetParent(gameObject.transform, false);
        Plugin.Logger.LogInfo("[UI] CreateUIElements: canvas created.");

        _panelObj = new GameObject("SelectionPanel");
        _panelObj.transform.SetParent(_canvasObj.transform, false);
        var panelImage = _panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f);
        var panelRect = _panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(600, 400);
        Plugin.Logger.LogInfo("[UI] CreateUIElements: panel created.");

        var contentObj = new GameObject("Content");
        contentObj.AddComponent<RectTransform>();
        contentObj.transform.SetParent(_panelObj.transform, false);
        var contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(20, 20);
        contentRect.offsetMax = new Vector2(-20, -20);

        var buttonsContainerObj = new GameObject("ButtonsContainer");
        buttonsContainerObj.AddComponent<RectTransform>();
        buttonsContainerObj.transform.SetParent(contentObj.transform, false);
        var bcRect = buttonsContainerObj.GetComponent<RectTransform>();
        bcRect.anchorMin = new Vector2(0, 0.1f);
        bcRect.anchorMax = new Vector2(1, 0.8f);
        bcRect.offsetMin = Vector2.zero;
        bcRect.offsetMax = Vector2.zero;
        _buttonsContainer = buttonsContainerObj.transform;
        Plugin.Logger.LogInfo("[UI] CreateUIElements: buttonsContainer set.");

        _titleText = CreateTextObj("Title", contentObj.transform, new Vector2(0, 160), new Vector2(0, 50), "Select a Modifier");
        _titleText.fontSize = 36;
        _titleText.alignment = TextAlignmentOptions.Center;

        _waitingText = CreateTextObj("WaitingText", contentObj.transform, new Vector2(0, -120), new Vector2(0, 40), "Waiting for host to choose...");
        _waitingText.fontSize = 24;
        _waitingText.alignment = TextAlignmentOptions.Center;
        _waitingText.color = Color.gray;
        _waitingText.gameObject.SetActive(false);
        Plugin.Logger.LogInfo("[UI] CreateUIElements: all elements created.");
    }

    private TextMeshProUGUI CreateTextObj(string name, Transform parent, Vector2 anchorPos, Vector2 sizeDelta, string text)
    {
        var textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        var textMesh = textObj.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.fontSize = 28;
        textMesh.color = Color.white;
        var rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchorPos;
        rect.sizeDelta = sizeDelta;
        return textMesh;
    }

    private void UpdateButtonDisplay()
    {
        Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: gameObject={gameObject}, _buttonsContainer={_buttonsContainer}");
        if (gameObject == null)
        {
            Plugin.Logger.LogError("[UI] UpdateButtonDisplay: gameObject is NULL!");
            return;
        }
        if (_buttonsContainer == null)
        {
            Plugin.Logger.LogError("[UI] UpdateButtonDisplay: _buttonsContainer is null, aborting!");
            return;
        }

        Plugin.Logger.LogInfo("[UI] UpdateButtonDisplay: clearing old buttons.");
        foreach (Transform child in _buttonsContainer)
            Destroy(child.gameObject);

        var buttonHeight = 60f;
        var spacing = 10f;
        var totalHeight = _currentChoices.Count * (buttonHeight + spacing) - spacing;
        var startY = totalHeight / 2;

        Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: creating {_currentChoices.Count} buttons.");
        for (int i = 0; i < _currentChoices.Count; i++)
        {
            var modifier = _currentChoices[i];
            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 1 - new GameObject for button {i} ({modifier.Name}).");
            var buttonObj = new GameObject($"Button_{modifier.Id}");

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 2 - SetParent for button {i}.");
            buttonObj.transform.SetParent(_buttonsContainer, false);

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 3 - AddComponent<Image> for button {i}.");
            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 4 - RectTransform for button {i}.");
            var rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            var yPos = startY - i * (buttonHeight + spacing) - buttonHeight / 2;
            rect.anchoredPosition = new Vector2(0, yPos);
            rect.sizeDelta = new Vector2(0, buttonHeight);

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 5 - TextMeshProUGUI for button {i}.");
            var txtObj = new GameObject("Text");
            txtObj.transform.SetParent(buttonObj.transform, false);
            var txt = txtObj.AddComponent<TextMeshProUGUI>();
            txt.text = modifier.Name;
            txt.fontSize = 24;
            txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.Center;
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: step 6 - AddComponent<Button> for button {i}.");
            var button = buttonObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() => VoteForModifier(modifier.Id));

            Plugin.Logger.LogInfo($"[UI] UpdateButtonDisplay: button {i} done.");
        }
        Plugin.Logger.LogInfo("[UI] UpdateButtonDisplay: all buttons created.");
    }
}
