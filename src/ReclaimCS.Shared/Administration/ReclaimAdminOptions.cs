namespace ReclaimCS.Shared.Administration;

public sealed class ReclaimAdminOptions
{
    public bool Enabled { get; set; } = true;
    public bool RequireAdminPermissions { get; set; } = true;
    public bool EnableScoreboardTags { get; set; } = false;
    public string[] RequiredPermissions { get; set; } = ["@css/root"];
}
