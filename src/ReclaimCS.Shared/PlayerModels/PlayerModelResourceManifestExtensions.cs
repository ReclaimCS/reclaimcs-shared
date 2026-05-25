using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.PlayerModels;

public static class PlayerModelResourceManifestExtensions
{
    public static void AddPlayerModelResources(this ResourceManifest manifest, IEnumerable<PlayerModelDefinition> models, bool includeAlternates = true)
    {
        foreach (var model in models)
            manifest.AddPlayerModelResources(model, includeAlternates);
    }

    public static void AddPlayerModelResources(this ResourceManifest manifest, PlayerModelDefinition model, bool includeAlternates = true)
    {
        foreach (var path in model.GetResourcePaths(includeAlternates))
            manifest.AddPlayerModelResource(path);
    }

    public static void AddPlayerModelResource(this ResourceManifest manifest, string? modelPath)
    {
        var normalized = ReclaimPlayerModels.NormalizeModelPath(modelPath);
        if (!string.IsNullOrWhiteSpace(normalized))
            manifest.AddResource(normalized);
    }
}
