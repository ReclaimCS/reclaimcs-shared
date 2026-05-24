using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.Chat;

public static class ReclaimChatText
{
    public static string Brand(object? value) => $"{ChatColors.LightPurple}{value}{ChatColors.Default}";
    public static string Good(object? value) => $"{ChatColors.Lime}{value}{ChatColors.Default}";
    public static string Warn(object? value) => $"{ChatColors.Yellow}{value}{ChatColors.Default}";
    public static string Bad(object? value) => $"{ChatColors.Red}{value}{ChatColors.Default}";
    public static string Class(object? value) => $"{ChatColors.LightBlue}{value}{ChatColors.Default}";
    public static string Link(object? value) => $"{ChatColors.LightBlue}{value}{ChatColors.Default}";
    public static string BrightLink(object? value) => $"{ChatColors.LightRed}{value}{ChatColors.Default}";
    public static string Command(object? value) => $"{ChatColors.Lime}{value}{ChatColors.Default}";
    public static string Number(object? value) => $"{ChatColors.Lime}{value}{ChatColors.Default}";
    public static string Player(object? value) => $"{ChatColors.Gold}{value}{ChatColors.Default}";
    public static string Money(int amount) => $"{ChatColors.Lime}${amount}{ChatColors.Default}";

    public static string ModulePrefix(string moduleTag, string? brandName = null, BrandedMessageType type = BrandedMessageType.Info)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(brandName))
            parts.Add($" {ChatColors.Red}[{brandName.Trim()}]{ChatColors.Default}{ChatColors.Grey} |{ChatColors.Default}");

        var tag = FormatModuleTag(type, moduleTag);
        if (!string.IsNullOrWhiteSpace(tag))
            parts.Add(tag);

        return string.Join(" ", parts);
    }

    public static string FormatModuleTag(BrandedMessageType type, string moduleTag)
    {
        if (string.IsNullOrWhiteSpace(moduleTag))
            return "";

        var normalized = moduleTag.Trim();
        if (normalized.StartsWith("[", StringComparison.Ordinal) && normalized.EndsWith("]", StringComparison.Ordinal))
            normalized = normalized[1..^1].Trim();

        return string.IsNullOrWhiteSpace(normalized)
            ? ""
            : $"{GetTypeColor(type)}[{normalized.ToUpperInvariant()}]{ChatColors.Default}";
    }

    public static string GetTypeColor(BrandedMessageType type)
    {
        return type switch
        {
            BrandedMessageType.Success => ChatColors.Lime.ToString(),
            BrandedMessageType.Warning => ChatColors.Yellow.ToString(),
            BrandedMessageType.Error => ChatColors.Red.ToString(),
            BrandedMessageType.Progress => ChatColors.LightBlue.ToString(),
            BrandedMessageType.Xp => ChatColors.LightPurple.ToString(),
            BrandedMessageType.Economy => ChatColors.Gold.ToString(),
            BrandedMessageType.Event => ChatColors.LightRed.ToString(),
            BrandedMessageType.Admin => ChatColors.Gold.ToString(),
            _ => ChatColors.LightBlue.ToString()
        };
    }
}
