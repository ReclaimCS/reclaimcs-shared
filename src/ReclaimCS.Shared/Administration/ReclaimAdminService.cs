using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;

namespace ReclaimCS.Shared.Administration;

public static class ReclaimAdminService
{
    public static bool CanUseAdminFeature(
        CCSPlayerController? player,
        ReclaimAdminOptions? options,
        bool allowConsole = true)
    {
        options ??= new ReclaimAdminOptions();
        if (!options.Enabled)
            return false;

        if (player == null)
            return allowConsole;

        if (!options.RequireAdminPermissions || options.RequiredPermissions.Length == 0)
            return true;

        return HasRequiredPermissions(player, options);
    }

    public static bool HasRequiredPermissions(CCSPlayerController? player, ReclaimAdminOptions? options)
    {
        options ??= new ReclaimAdminOptions();
        if (!options.Enabled || player is not { IsValid: true } || player.IsBot)
            return false;

        return options.RequiredPermissions.Length > 0
            && AdminManager.PlayerHasPermissions(player, options.RequiredPermissions);
    }
}
