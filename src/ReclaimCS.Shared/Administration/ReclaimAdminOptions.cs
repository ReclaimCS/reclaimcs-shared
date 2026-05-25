namespace ReclaimCS.Shared.Administration;

public sealed class ReclaimAdminOptions
{
    public bool Enabled { get; set; } = true;
    public bool RequireAdminPermissions { get; set; } = true;
    public string[] RequiredPermissions { get; set; } = ["@css/root"];
}
