namespace LethalRogueLike.src.UI;

/// <summary>
/// Represents a single active modifier as displayed in the overlay.
/// </summary>
public class ActiveModifierInfo
{
    public string ModifierId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDebuff { get; set; }
    public int AppliedAtLanding { get; set; }
}
