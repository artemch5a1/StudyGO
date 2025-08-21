namespace StudyGO.API.CustomAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisabledAttribute : Attribute
{
    public string? Description { get; }

    public DisabledAttribute(string? description = null)
    {
        Description = description;
    }
}