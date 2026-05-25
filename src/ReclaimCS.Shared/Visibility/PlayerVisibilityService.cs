using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace ReclaimCS.Shared.Visibility;

public sealed class PlayerVisibilityService
{
    private readonly Dictionary<int, PlayerVisibilityState> _states = [];

    public void SetHidden(
        CCSPlayerController player,
        string source,
        bool hidden,
        PlayerVisibilityOptions? options = null)
    {
        if (hidden)
        {
            HidePlayer(player, source, options);
            return;
        }

        ShowPlayer(player, source);
    }

    public void HidePlayer(
        CCSPlayerController player,
        string source,
        PlayerVisibilityOptions? options = null)
    {
        if (!TryGetPlayerSlot(player, out var slot))
            return;

        var normalizedSource = NormalizeSource(source);
        var state = GetOrCreateState(slot);
        state.HiddenRequests[normalizedSource] = new PlayerVisibilityRequest(
            normalizedSource,
            options ?? new PlayerVisibilityOptions());
        state.TemporaryReveals.Remove(normalizedSource);
    }

    public void ShowPlayer(CCSPlayerController player, string source)
    {
        if (!TryGetPlayerSlot(player, out var slot))
            return;

        ClearSource(slot, source);
    }

    public void RevealTemporarily(CCSPlayerController player, string source, TimeSpan duration)
    {
        if (!TryGetPlayerSlot(player, out var slot))
            return;

        var normalizedSource = NormalizeSource(source);
        var state = GetOrCreateState(slot);

        if (duration <= TimeSpan.Zero)
        {
            state.TemporaryReveals.Remove(normalizedSource);
            RemoveStateIfEmpty(slot, state);
            return;
        }

        state.TemporaryReveals[normalizedSource] = DateTime.UtcNow.Add(duration);
    }

    public void ClearTemporaryReveal(CCSPlayerController player, string source)
    {
        if (!TryGetPlayerSlot(player, out var slot))
            return;

        if (!_states.TryGetValue(slot, out var state))
            return;

        state.TemporaryReveals.Remove(NormalizeSource(source));
        RemoveStateIfEmpty(slot, state);
    }

    public void ClearPlayer(CCSPlayerController player)
    {
        if (player.IsValid)
            ClearPlayer(player.Slot);
    }

    public void ClearPlayer(int slot)
    {
        _states.Remove(slot);
    }

    public void ClearAll()
    {
        _states.Clear();
    }

    public bool IsHidden(CCSPlayerController player)
    {
        return player.IsValid
            && _states.TryGetValue(player.Slot, out var state)
            && state.HiddenRequests.Count > 0;
    }

    public bool IsHiddenForSource(CCSPlayerController player, string source)
    {
        return player.IsValid
            && _states.TryGetValue(player.Slot, out var state)
            && state.HiddenRequests.ContainsKey(NormalizeSource(source));
    }

    public IReadOnlyCollection<string> GetHiddenSources(CCSPlayerController player)
    {
        if (!player.IsValid || !_states.TryGetValue(player.Slot, out var state))
            return [];

        return state.HiddenRequests.Keys.ToArray();
    }

    public void OnCheckTransmit(CCheckTransmitInfoList infoList)
    {
        if (_states.Count == 0)
            return;

        var players = Utilities.GetPlayers()
            .Where(player => player is { IsValid: true })
            .ToArray();

        PruneDisconnectedSlots(players);

        var hiddenPlayers = players
            .Where(player => player.PawnIsAlive && _states.ContainsKey(player.Slot))
            .ToArray();

        if (hiddenPlayers.Length == 0)
            return;

        var now = DateTime.UtcNow;
        foreach (var (info, viewer) in infoList)
        {
            if (viewer is not { IsValid: true })
                continue;

            foreach (var subject in hiddenPlayers)
            {
                var mask = BuildTransmitMask(viewer, subject, now);
                if (!mask.ShouldHide)
                    continue;

                if (mask.HidePlayerPawn)
                    RemoveEntity(info, subject.PlayerPawn.Value);

                if (mask.HideWeapons)
                    RemovePlayerWeapons(info, subject);

                foreach (var entity in mask.ExtraEntities)
                    RemoveEntity(info, entity);
            }
        }
    }

    private PlayerVisibilityState GetOrCreateState(int slot)
    {
        if (!_states.TryGetValue(slot, out var state))
        {
            state = new PlayerVisibilityState();
            _states[slot] = state;
        }

        return state;
    }

    private void ClearSource(int slot, string source)
    {
        if (!_states.TryGetValue(slot, out var state))
            return;

        var normalizedSource = NormalizeSource(source);
        state.HiddenRequests.Remove(normalizedSource);
        state.TemporaryReveals.Remove(normalizedSource);
        RemoveStateIfEmpty(slot, state);
    }

    private PlayerVisibilityTransmitMask BuildTransmitMask(
        CCSPlayerController viewer,
        CCSPlayerController subject,
        DateTime now)
    {
        if (!_states.TryGetValue(subject.Slot, out var state) || state.HiddenRequests.Count == 0)
            return PlayerVisibilityTransmitMask.Empty;

        var mask = new PlayerVisibilityTransmitMask();
        foreach (var request in state.HiddenRequests.Values)
        {
            if (!RequestAppliesToViewer(state, request, viewer, subject, now))
                continue;

            mask.HidePlayerPawn |= request.Options.HidePlayerPawn;
            mask.HideWeapons |= request.Options.HideWeapons;

            var extraEntities = request.Options.ExtraEntities;
            if (extraEntities == null)
                continue;

            foreach (var entity in GetExtraEntities(extraEntities, subject, request.Source))
                mask.ExtraEntities.Add(entity);
        }

        return mask;
    }

    private static IEnumerable<CEntityInstance?> GetExtraEntities(
        PlayerVisibilityExtraEntityProvider provider,
        CCSPlayerController subject,
        string source)
    {
        try
        {
            return provider(subject);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ReclaimCS.Shared] Visibility extra entity provider failed for '{source}': {ex.Message}");
            return [];
        }
    }

    private bool RequestAppliesToViewer(
        PlayerVisibilityState state,
        PlayerVisibilityRequest request,
        CCSPlayerController viewer,
        CCSPlayerController subject,
        DateTime now)
    {
        if (viewer.Slot == subject.Slot && !request.Options.HideFromSelf)
            return false;

        if (IsTemporarilyRevealed(state, request.Source, now))
            return false;

        if (request.Options.ShouldHideFromViewer == null)
            return true;

        try
        {
            return request.Options.ShouldHideFromViewer(viewer, subject);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ReclaimCS.Shared] Visibility viewer filter failed for '{request.Source}': {ex.Message}");
            return true;
        }
    }

    private static bool IsTemporarilyRevealed(PlayerVisibilityState state, string source, DateTime now)
    {
        if (!state.TemporaryReveals.TryGetValue(source, out var revealedUntil))
            return false;

        if (revealedUntil > now)
            return true;

        state.TemporaryReveals.Remove(source);
        return false;
    }

    private void PruneDisconnectedSlots(IReadOnlyCollection<CCSPlayerController> players)
    {
        var connectedSlots = players
            .Select(player => player.Slot)
            .ToHashSet();

        foreach (var slot in _states.Keys.ToArray())
        {
            if (!connectedSlots.Contains(slot))
                _states.Remove(slot);
        }
    }

    private static void RemovePlayerWeapons(CCheckTransmitInfo info, CCSPlayerController player)
    {
        if (player.PlayerPawn.Value is not { IsValid: true } pawn)
            return;

        var weaponServices = pawn.WeaponServices;
        if (weaponServices == null)
            return;

        var activeWeapon = weaponServices.ActiveWeapon.Value;
        if (activeWeapon is { IsValid: true })
            RemoveEntity(info, activeWeapon);

        if (weaponServices.MyWeapons == null)
            return;

        foreach (var weaponHandle in weaponServices.MyWeapons)
            RemoveEntity(info, weaponHandle.Value);
    }

    private static void RemoveEntity(CCheckTransmitInfo info, CEntityInstance? entity)
    {
        if (entity is { IsValid: true })
            info.TransmitEntities.Remove(entity);
    }

    private static bool TryGetPlayerSlot(CCSPlayerController player, out int slot)
    {
        slot = default;
        if (!player.IsValid)
            return false;

        slot = player.Slot;
        return true;
    }

    private static string NormalizeSource(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("Visibility source must not be empty.", nameof(source));

        return source.Trim();
    }

    private void RemoveStateIfEmpty(int slot, PlayerVisibilityState state)
    {
        if (state.HiddenRequests.Count == 0 && state.TemporaryReveals.Count == 0)
            _states.Remove(slot);
    }

    private sealed class PlayerVisibilityState
    {
        public Dictionary<string, PlayerVisibilityRequest> HiddenRequests { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, DateTime> TemporaryReveals { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    private sealed record PlayerVisibilityRequest(string Source, PlayerVisibilityOptions Options);

    private sealed class PlayerVisibilityTransmitMask
    {
        public static PlayerVisibilityTransmitMask Empty { get; } = new();

        public bool HidePlayerPawn { get; set; }

        public bool HideWeapons { get; set; }

        public List<CEntityInstance?> ExtraEntities { get; } = [];

        public bool ShouldHide => HidePlayerPawn || HideWeapons || ExtraEntities.Count > 0;
    }
}
