namespace Common.Application.Common.Models;

public sealed class AppFeature(string module, string name, string description,
    List<AppFeatureAction> actions)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public string Module { get; set; } = module;
    public List<AppFeatureAction> Actions { get; set; } = actions;
}

public sealed class AppFeatureAction(string name, string description)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
}