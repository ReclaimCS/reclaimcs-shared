using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using ReclaimCS.Shared.CounterStrike;

namespace ReclaimCS.Shared.Administration;

public sealed class PlayerNameTagService
{
    private const string AdminClanTag = "[ADMIN]";
    private const string VipClanTag = "[VIP]";

    private readonly Func<ReclaimAdminOptions> _adminOptions;
    private readonly Func<CCSPlayerController, bool>? _hasVipBenefits;

    public PlayerNameTagService(
        ReclaimAdminOptions adminOptions,
        Func<CCSPlayerController, bool>? hasVipBenefits = null)
        : this(() => adminOptions, hasVipBenefits)
    {
    }

    public PlayerNameTagService(
        Func<ReclaimAdminOptions> adminOptions,
        Func<CCSPlayerController, bool>? hasVipBenefits = null)
    {
        _adminOptions = adminOptions;
        _hasVipBenefits = hasVipBenefits;
    }

    public void Apply(CCSPlayerController player)
    {
        var tag = ResolveTag(player);
        if (tag == null)
        {
            if (IsManagedClanTag(player.Clan))
                SetClanTag(player, "");

            return;
        }

        if (!string.Equals(player.Clan, tag.Value.ClanTag, StringComparison.Ordinal))
            SetClanTag(player, tag.Value.ClanTag);
    }

    public bool HasManagedTag(CCSPlayerController player)
    {
        return ResolveTag(player) != null;
    }

    public void PrintTaggedChatMessage(CCSPlayerController player, string message, bool teamChat, bool includeBots)
    {
        var tag = ResolveTag(player);
        if (tag == null)
            return;

        var formattedMessage = FormatTaggedChatMessage(player, message, teamChat, tag.Value);
        foreach (var recipient in Utilities.GetPlayers().Where(recipient =>
            recipient.IsRealConnectedPlayer(includeBots)
            && (!teamChat || recipient.Team == player.Team)))
        {
            recipient.PrintToChat(formattedMessage);
        }
    }

    private PlayerNameTag? ResolveTag(CCSPlayerController player)
    {
        if (ReclaimAdminService.HasRequiredPermissions(player, _adminOptions()))
            return new PlayerNameTag(AdminClanTag, ChatColors.Red);

        if (_hasVipBenefits?.Invoke(player) == true)
            return new PlayerNameTag(VipClanTag, ChatColors.Gold);

        return null;
    }

    private static void SetClanTag(CCSPlayerController player, string clanTag)
    {
        player.Clan = clanTag;
        TryMarkClanStateChanged(player);
    }

    private static bool IsManagedClanTag(string clanTag)
    {
        return string.Equals(clanTag, AdminClanTag, StringComparison.Ordinal)
            || string.Equals(clanTag, VipClanTag, StringComparison.Ordinal);
    }

    private static string FormatTaggedChatMessage(CCSPlayerController player, string message, bool teamChat, PlayerNameTag tag)
    {
        var deadPrefix = player.PawnIsAlive ? "" : "*DEAD* ";
        var teamPrefix = teamChat ? "(TEAM) " : "";
        var colorPrimer = player.PawnIsAlive && !teamChat ? " " : "";
        return $"{ChatColors.Default}{deadPrefix}{teamPrefix}{colorPrimer}{tag.ChatColor}{tag.ClanTag} {ChatColors.ForPlayer(player)}{player.PlayerName}{ChatColors.Default}: {message}";
    }

    private static void TryMarkClanStateChanged(CCSPlayerController player)
    {
        try
        {
            Utilities.SetStateChanged(player, "CBasePlayerController", "m_szClan");
        }
        catch
        {
        }

        try
        {
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
        }
        catch
        {
        }
    }

    private readonly record struct PlayerNameTag(string ClanTag, char ChatColor);
}
