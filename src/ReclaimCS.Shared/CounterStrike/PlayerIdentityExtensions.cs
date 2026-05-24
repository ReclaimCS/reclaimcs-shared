using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.CounterStrike;

public static class PlayerIdentityExtensions
{
    public const ulong DefaultBotStateKeyPrefix = 0xC500_0000_0000_0000;

    public static bool IsRealConnectedPlayer(this CCSPlayerController? player, bool includeBots = false)
    {
        if (player is not { IsValid: true })
            return false;

        if (player.IsBot)
            return includeBots;

        return player.Connected == PlayerConnectedState.Connected && player.SteamID != 0;
    }

    public static ulong GetRuntimeKey(this CCSPlayerController player)
    {
        return player.GetRuntimeKey(DefaultBotStateKeyPrefix);
    }

    public static ulong GetRuntimeKey(this CCSPlayerController player, ulong botStateKeyPrefix)
    {
        if (!player.IsValid)
            throw new InvalidOperationException("Invalid player.");

        if (!player.IsBot && player.SteamID != 0)
            return player.SteamID;

        return botStateKeyPrefix | Convert.ToUInt64(player.UserId);
    }

    public static List<CCSPlayerController> GetPlayersInProximity(
        this CCSPlayerController player,
        IEnumerable<CCSPlayerController> allPlayers,
        float radius)
    {
        var result = new List<CCSPlayerController>();
        var pawn = player.PlayerPawn.Value;
        if (pawn?.AbsOrigin == null)
            return result;

        var origin = pawn.AbsOrigin;

        foreach (var other in allPlayers)
        {
            if (other == null || !other.IsValid || other == player)
                continue;

            var otherPawn = other.PlayerPawn.Value;
            if (otherPawn?.AbsOrigin == null)
                continue;

            var otherOrigin = otherPawn.AbsOrigin;
            var dx = otherOrigin.X - origin.X;
            var dy = otherOrigin.Y - origin.Y;
            var dz = otherOrigin.Z - origin.Z;
            var distance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);

            if (distance <= radius)
                result.Add(other);
        }

        return result;
    }

    public static void ForceTeamState(this CCSPlayerController player, CsTeam team)
    {
        if (!player.IsValid)
            return;

        var teamNum = (byte)team;
        player.TeamNum = teamNum;
        player.InitialTeamNum = (int)team;
        player.MarkTeamStateChanged();

        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid)
            return;

        pawn.TeamNum = teamNum;
        pawn.InitialTeamNum = (int)team;
        pawn.MarkTeamStateChanged();
    }
}
