namespace LethalRogueLike.src.Modifiers;

public class AppliedModifier
{
    public string ModifierId { get; set; } = string.Empty;
    public int AppliedAtLanding { get; set; }
    public float Severity { get; set; }
}
