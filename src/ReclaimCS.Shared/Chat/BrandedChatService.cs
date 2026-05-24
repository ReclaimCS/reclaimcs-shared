using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using ReclaimCS.Shared.CounterStrike;

namespace ReclaimCS.Shared.Chat;

public class BrandedChatService
{
    private readonly Func<ReclaimChatOptions> _options;

    public BrandedChatService(ReclaimChatOptions options)
        : this(() => options)
    {
    }

    public BrandedChatService(Func<ReclaimChatOptions> options)
    {
        _options = options;
    }

    public void SendInfo(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Info, message, moduleTag);
    public void SendSuccess(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Success, message, moduleTag);
    public void SendWarning(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Warning, message, moduleTag);
    public void SendError(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Error, message, moduleTag);
    public void SendProgress(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Progress, message, moduleTag);
    public void SendXp(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Xp, message, moduleTag);
    public void SendEconomy(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Economy, message, moduleTag);
    public void SendEvent(CCSPlayerController player, string message, string moduleTag = "") => Send(player, BrandedMessageType.Event, message, moduleTag);
    public void SendAdmin(CCSPlayerController player, string message, string moduleTag = "ADMIN") => Send(player, BrandedMessageType.Admin, message, moduleTag);

    public void ReplyInfo(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Info, message, moduleTag);
    public void ReplySuccess(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Success, message, moduleTag);
    public void ReplyWarning(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Warning, message, moduleTag);
    public void ReplyError(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Error, message, moduleTag);
    public void ReplyProgress(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Progress, message, moduleTag);
    public void ReplyXp(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Xp, message, moduleTag);
    public void ReplyEconomy(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Economy, message, moduleTag);
    public void ReplyEvent(CommandInfo command, string message, string moduleTag = "") => Reply(command, BrandedMessageType.Event, message, moduleTag);
    public void ReplyAdmin(CommandInfo command, string message, string moduleTag = "ADMIN") => Reply(command, BrandedMessageType.Admin, message, moduleTag);

    public void BroadcastInfo(string message, string moduleTag = "") => Broadcast(BrandedMessageType.Info, message, moduleTag);
    public void BroadcastWarning(string message, string moduleTag = "") => Broadcast(BrandedMessageType.Warning, message, moduleTag);
    public void BroadcastEvent(string message, string moduleTag = "") => Broadcast(BrandedMessageType.Event, message, moduleTag);

    public string FormatInfo(string message, string moduleTag = "") => Format(BrandedMessageType.Info, message, moduleTag);
    public string FormatSuccess(string message, string moduleTag = "") => Format(BrandedMessageType.Success, message, moduleTag);
    public string FormatWarning(string message, string moduleTag = "") => Format(BrandedMessageType.Warning, message, moduleTag);
    public string FormatError(string message, string moduleTag = "") => Format(BrandedMessageType.Error, message, moduleTag);
    public string FormatProgress(string message, string moduleTag = "") => Format(BrandedMessageType.Progress, message, moduleTag);
    public string FormatXp(string message, string moduleTag = "") => Format(BrandedMessageType.Xp, message, moduleTag);
    public string FormatEconomy(string message, string moduleTag = "") => Format(BrandedMessageType.Economy, message, moduleTag);
    public string FormatEvent(string message, string moduleTag = "") => Format(BrandedMessageType.Event, message, moduleTag);
    public string FormatAdmin(string message, string moduleTag = "ADMIN") => Format(BrandedMessageType.Admin, message, moduleTag);

    public string Highlight(object value, BrandedMessageType type)
    {
        return $"{ReclaimChatText.GetTypeColor(type)}{value}{CounterStrikeSharp.API.Modules.Utils.ChatColors.Default}";
    }

    public string Format(BrandedMessageType type, string message, string moduleTag = "")
    {
        message = string.IsNullOrWhiteSpace(message) ? "" : message.Trim();
        var prefix = BuildPrefix(type, moduleTag);
        var body = FormatBody(type, message);

        return string.IsNullOrWhiteSpace(prefix)
            ? body
            : $"{prefix} {body}";
    }

    private void Send(CCSPlayerController player, BrandedMessageType type, string message, string moduleTag)
    {
        if (player is not { IsValid: true })
            return;

        player.PrintToChat(Format(type, message, moduleTag));
    }

    private void Reply(CommandInfo command, BrandedMessageType type, string message, string moduleTag)
    {
        command.ReplyToCommand(Format(type, message, moduleTag));
    }

    private void Broadcast(BrandedMessageType type, string message, string moduleTag)
    {
        var options = _options();
        var formatted = Format(type, message, moduleTag);
        foreach (var player in Utilities.GetPlayers().Where(player => player.IsRealConnectedPlayer(options.IncludeBots)))
            player.PrintToChat(formatted);
    }

    private string BuildPrefix(BrandedMessageType type, string moduleTag)
    {
        var options = _options();
        var brandName = options.ShowBrandPrefix ? options.BrandName : null;
        var effectiveModuleTag = options.ShowModuleTags ? moduleTag : "";
        return ReclaimChatText.ModulePrefix(effectiveModuleTag, brandName, type);
    }

    private static string FormatBody(BrandedMessageType type, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "";

        return type switch
        {
            BrandedMessageType.Error
                or BrandedMessageType.Warning
                or BrandedMessageType.Success
                or BrandedMessageType.Xp
                or BrandedMessageType.Economy
                or BrandedMessageType.Event
                or BrandedMessageType.Admin => $"{ReclaimChatText.GetTypeColor(type)}{message}{CounterStrikeSharp.API.Modules.Utils.ChatColors.Default}",
            _ => message
        };
    }
}

public enum BrandedMessageType
{
    Info,
    Success,
    Warning,
    Error,
    Progress,
    Xp,
    Economy,
    Event,
    Admin
}
