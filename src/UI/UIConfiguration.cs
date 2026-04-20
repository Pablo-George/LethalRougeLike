using UnityEngine;

namespace LethalRogueLike.src.UI;

public static class UIConfiguration
{
    // Colors - In-game Overlay
    public static Color OverlayBackground { get; } = new Color(0, 0, 0, 0.70f);
    public static Color OverlayDebuffIndicator { get; } = new Color(0.906f, 0.298f, 0.235f, 1f);  // #E74C3C
    public static Color OverlayBuffIndicator { get; } = new Color(0.180f, 0.800f, 0.443f, 1f);    // #2ECC71

    // Colors - Selection Screen
    public static Color SelectionBackground { get; } = new Color(0.07f, 0.07f, 0.09f, 0.93f);
    public static Color SelectionCardBackground { get; } = new Color(0.13f, 0.13f, 0.16f, 1f);
    public static Color SelectionCardHover { get; } = new Color(0.20f, 0.20f, 0.25f, 1f);
    public static Color SelectionCardPressed { get; } = new Color(0.26f, 0.26f, 0.32f, 1f);
    public static Color SelectionBuffAccent { get; } = new Color(0.180f, 0.800f, 0.443f, 1f);    // #2ECC71
    public static Color SelectionDebuffAccent { get; } = new Color(0.906f, 0.298f, 0.235f, 1f);  // #E74C3C
    public static Color SelectionTitleColor { get; } = new Color(0.92f, 0.87f, 0.72f, 1f);
    public static Color SelectionSubtitleColor { get; } = new Color(0.55f, 0.55f, 0.60f, 1f);

    // Layout - Overlay
    public static float OverlayMaxWidth { get; } = 300f;
    public static float OverlayMargin { get; } = 10f;
    public static int MaxDisplayCount { get; } = 8;

    // Text - Overlay
    public static float OverlayFontSize { get; } = 16f;
    public static float OverlayTitleFontSize { get; } = 18f;

    // Animation
    public static float FadeInDuration { get; } = 0.2f;
    public static float FadeOutDuration { get; } = 0.15f;
}
