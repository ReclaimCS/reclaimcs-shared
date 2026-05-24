namespace ReclaimCS.Shared.Chat;

public sealed class ReclaimChatOptions
{
    public string BrandName { get; set; } = "ReclaimCS";
    public bool ShowBrandPrefix { get; set; } = true;
    public bool ShowModuleTags { get; set; }
    public bool IncludeBots { get; set; }
}
