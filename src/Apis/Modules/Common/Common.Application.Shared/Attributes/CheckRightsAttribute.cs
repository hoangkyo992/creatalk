namespace Common.Application.Shared.Attributes;

/// <summary>
///
/// </summary>
/// <seealso cref="CheckRightsAttribute" />
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class CheckRightsAttribute : Attribute
{
    public string[] Actions { get; private set; }

    public string Feature { get; private set; }

    public CheckRightsAttribute(string feature, params string[] actions)
    {
        Feature = feature;
        Actions = actions;
    }
}