using CounterStrikeSharp.API.Core;

namespace ReclaimCS.Shared.Visibility;

public delegate bool PlayerVisibilityViewerFilter(CCSPlayerController viewer, CCSPlayerController subject);

public delegate IEnumerable<CEntityInstance?> PlayerVisibilityExtraEntityProvider(CCSPlayerController subject);

public sealed class PlayerVisibilityOptions
{
    public bool HidePlayerPawn { get; init; } = true;

    public bool HideWeapons { get; init; } = true;

    public bool HideFromSelf { get; init; }

    public float NearbyEquipmentRadius { get; init; } = 96f;

    public PlayerVisibilityViewerFilter? ShouldHideFromViewer { get; init; }

    public PlayerVisibilityExtraEntityProvider? ExtraEntities { get; init; }
}
