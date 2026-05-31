using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.KillFeed;

public static class KillFeedIconService
{
    public static void PrecacheResources(KillFeedIconOptions? options, ResourceManifest manifest)
    {
        if (options is not { Enabled: true })
            return;

        options.Normalize();
        if (options.Icons.Count == 0)
            return;

        foreach (var resource in options.Icons.Values.SelectMany(rule => rule.GetResources()).Distinct(StringComparer.OrdinalIgnoreCase))
            manifest.AddResource(resource);
    }

    public static bool Apply(KillFeedIconOptions? options, EventPlayerDeath gameEvent)
    {
        if (options is not { Enabled: true })
            return false;

        options.Normalize();
        ApplyConfiguredDeathFlags(options, gameEvent);

        var attacker = gameEvent.Attacker;
        if (!IsEligibleAttacker(attacker))
            return false;

        var rule = FindBestRule(options, attacker!, gameEvent.Weapon);
        if (rule == null || string.IsNullOrWhiteSpace(rule.Icon))
            return false;

        gameEvent.Weapon = rule.Icon;
        return true;
    }

    private static KillFeedIconRule? FindBestRule(KillFeedIconOptions options, CCSPlayerController attacker, string eventWeapon)
    {
        var normalizedEventWeapon = NormalizeWeaponKey(eventWeapon);
        KillFeedIconRule? bestRule = null;
        var bestScore = 0;

        foreach (var (rawKey, rule) in options.Icons)
        {
            if (rule == null || string.IsNullOrWhiteSpace(rule.Icon) || !IsRuleAllowedForPlayer(rule, attacker))
                continue;

            var score = GetMatchScore(rawKey, normalizedEventWeapon);
            if (score > bestScore)
            {
                bestScore = score;
                bestRule = rule;
            }
        }

        return bestRule;
    }

    private static int GetMatchScore(string ruleKey, string eventWeapon)
    {
        var normalizedRuleKey = NormalizeWeaponKey(ruleKey);
        if (normalizedRuleKey == "*")
            return 1;

        if (normalizedRuleKey.Equals(eventWeapon, StringComparison.OrdinalIgnoreCase))
            return 4;

        if (normalizedRuleKey.Equals("knife", StringComparison.OrdinalIgnoreCase) && IsKnifeFamily(eventWeapon))
            return 3;

        return 0;
    }

    private static bool IsRuleAllowedForPlayer(KillFeedIconRule rule, CCSPlayerController player)
    {
        return IsTeamAllowed(rule.Team, player.Team) && HasPermission(player, rule.GetPermissions());
    }

    private static bool HasPermission(CCSPlayerController player, IEnumerable<string> permissions)
    {
        var required = permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .ToArray();

        if (required.Length == 0)
            return true;

        foreach (var permission in required)
        {
            if (permission.StartsWith('@') && AdminManager.PlayerHasPermissions(player, permission))
                return true;

            if (permission.StartsWith('#') && AdminManager.PlayerInGroup(player, permission))
                return true;
        }

        return false;
    }

    private static bool IsTeamAllowed(string team, CsTeam playerTeam)
    {
        return NormalizeTeam(team) switch
        {
            "" or "all" or "both" or "*" => true,
            "t" or "terrorist" or "terrorists" => playerTeam == CsTeam.Terrorist,
            "ct" or "counterterrorist" or "counterterrorists" or "counter_terrorist" or "counter_terrorists" => playerTeam == CsTeam.CounterTerrorist,
            _ => false
        };
    }

    private static bool IsEligibleAttacker(CCSPlayerController? attacker)
    {
        return attacker is { IsValid: true }
            && (attacker.IsBot || attacker.Connected == PlayerConnectedState.Connected);
    }

    private static void ApplyConfiguredDeathFlags(KillFeedIconOptions options, EventPlayerDeath gameEvent)
    {
        ApplyBooleanMode(options.ThroughSmoke, value => gameEvent.Thrusmoke = value);
        ApplyBooleanMode(options.NoScope, value => gameEvent.Noscope = value);
        ApplyBooleanMode(options.Headshot, value => gameEvent.Headshot = value);
        ApplyBooleanMode(options.AssistedFlash, value => gameEvent.Assistedflash = value);
        ApplyBooleanMode(options.AttackerBlind, value => gameEvent.Attackerblind = value);
        ApplyBooleanMode(options.AttackerInAir, value => gameEvent.Attackerinair = value);
        ApplyIntMode(options.Penetrated, value => gameEvent.Penetrated = value);
        ApplyIntMode(options.Dominated, value => gameEvent.Dominated = value);
        ApplyIntMode(options.Revenge, value => gameEvent.Revenge = value);
        ApplyIntMode(options.SquadWipe, value => gameEvent.Wipe = value);
    }

    private static void ApplyBooleanMode(int mode, Action<bool> apply)
    {
        if (mode == 1)
            apply(true);
        else if (mode == 2)
            apply(false);
    }

    private static void ApplyIntMode(int mode, Action<int> apply)
    {
        if (mode == 1)
            apply(1);
        else if (mode == 2)
            apply(0);
    }

    private static string NormalizeWeaponKey(string value)
    {
        var normalized = (value ?? "").Trim().ToLowerInvariant();
        return normalized.StartsWith("weapon_", StringComparison.Ordinal)
            ? normalized["weapon_".Length..]
            : normalized;
    }

    private static string NormalizeTeam(string value)
    {
        return (value ?? "").Trim().ToLowerInvariant().Replace("-", "_").Replace(" ", "_");
    }

    private static bool IsKnifeFamily(string weapon)
    {
        return weapon.Contains("knife", StringComparison.OrdinalIgnoreCase)
            || weapon.Equals("bayonet", StringComparison.OrdinalIgnoreCase);
    }
}
