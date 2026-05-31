using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.Movement;

public interface IAirJumpState
{
    int AirJumpsUsed { get; set; }
    PlayerButtons PreviousJumpButtons { get; set; }
    PlayerFlags PreviousJumpFlags { get; set; }
    bool WasOnGroundLastTick { get; set; }
}
