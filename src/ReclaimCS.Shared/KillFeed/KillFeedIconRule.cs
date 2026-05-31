namespace ReclaimCS.Shared.KillFeed;

public sealed class KillFeedIconRule
{
    public string Icon { get; set; } = "";
    public string Resource { get; set; } = "";
    public string[] ExtraResources { get; set; } = [];
    public string[] Permission { get; set; } = [];
    public string[] Permissions { get; set; } = [];
    public string Team { get; set; } = "";
    public bool Precache { get; set; } = true;

    public void Normalize()
    {
        Icon = Icon?.Trim() ?? "";
        Resource = Resource?.Trim() ?? "";
        Team = Team?.Trim() ?? "";
        ExtraResources = NormalizeValues(ExtraResources);
        Permission = NormalizeValues(Permission);
        Permissions = NormalizeValues(Permissions);
    }

    public IEnumerable<string> GetPermissions()
    {
        foreach (var permission in Permission)
            yield return permission;

        foreach (var permission in Permissions)
            yield return permission;
    }

    public IEnumerable<string> GetResources()
    {
        if (!Precache)
            yield break;

        if (IsPrecacheableResource(Resource))
            yield return Resource;

        foreach (var resource in ExtraResources)
        {
            if (IsPrecacheableResource(resource))
                yield return resource;
        }
    }

    private static bool IsPrecacheableResource(string? resource)
    {
        if (string.IsNullOrWhiteSpace(resource))
            return false;

        var normalized = resource.Trim().Replace('\\', '/');
        return !normalized.StartsWith("panorama/", StringComparison.OrdinalIgnoreCase)
            && !normalized.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            && !normalized.EndsWith(".vsvg", StringComparison.OrdinalIgnoreCase)
            && !normalized.EndsWith(".vsvg_c", StringComparison.OrdinalIgnoreCase);
    }

    private static string[] NormalizeValues(IEnumerable<string?>? values)
    {
        return (values ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .ToArray();
    }
}
