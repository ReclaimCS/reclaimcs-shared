using CounterStrikeSharp.API;

namespace ReclaimCS.Shared.Movement;

public interface IAirJumpState
{
    int AirJumpsUsed { get; set; }
    PlayerButtons PreviousJumpButtons { get; set; }
    bool WasOnGroundLastTick { get; set; }
}
