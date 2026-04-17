using UnityEngine;

namespace LethalRogueLike.src.UI;

/// <summary>
/// Centralized styling and behavior configuration for the in-game active modifiers overlay.
/// </summary>
public static class UIConfiguration
{
    // Colors - In-game Overlay
    public static Color OverlayBackground { get; } = new Color(0, 0, 0, 0.70f);
    public static Color OverlayDebuffIndicator { get; } = new Color(0.906f, 0.298f, 0.235f, 1f);  // #E74C3C
    public static Color OverlayBuffIndicator { get; } = new Color(0.180f, 0.800f, 0.443f, 1f);    // #2ECC71
    
    // Layout
    public static float OverlayMaxWidth { get; } = 300f;
    public static float OverlayMargin { get; } = 10f;
    public static int MaxDisplayCount { get; } = 8;  // Before scroll
    
    // Text
    public static float OverlayFontSize { get; } = 16f;
    public static float OverlayTitleFontSize { get; } = 18f;
    
    // Animation
    public static float FadeInDuration { get; } = 0.2f;
    public static float FadeOutDuration { get; } = 0.15f;
}
