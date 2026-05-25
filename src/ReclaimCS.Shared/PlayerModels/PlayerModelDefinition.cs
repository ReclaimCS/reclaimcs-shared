namespace ReclaimCS.Shared.PlayerModels;

public sealed class PlayerModelDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string ModelPath { get; init; }
    public PlayerModelPack Pack { get; init; }
    public PlayerModelRole Roles { get; init; } = PlayerModelRole.Any;
    public string[] WorkshopAddonIds { get; init; } = [];
    public string SourceUrl { get; init; } = "";
    public string DateAdded { get; init; } = "";
    public string Contributor { get; init; } = "";
    public string ArmModelPath { get; init; } = "";
    public string[] AlternateModelPaths { get; init; } = [];

    public IEnumerable<string> GetResourcePaths(bool includeAlternates = true)
    {
        yield return ModelPath;

        if (!string.IsNullOrWhiteSpace(ArmModelPath))
            yield return ArmModelPath;

        if (!includeAlternates)
            yield break;

        foreach (var path in AlternateModelPaths)
            yield return path;
    }
}
