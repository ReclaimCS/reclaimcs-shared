using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace ReclaimCS.Shared.Movement;

public sealed class AirJumpService
{
    private const uint OnGroundFlag = 1u;

    public void UpdateMovementState(
        CCSPlayerController player,
        IAirJumpState state,
        bool enabled,
        int additionalJumps,
        float upForce)
    {
        if (player.PlayerPawn.Value is not { IsValid: true } pawn)
            return;

        if (!enabled || !player.PawnIsAlive)
        {
            Reset(state, player.Buttons);
            return;
        }

        var currentButtons = player.Buttons;
        var wasJumping = state.PreviousJumpButtons.HasFlag(PlayerButtons.Jump);
        var isJumping = currentButtons.HasFlag(PlayerButtons.Jump);
        var wasOnGround = state.WasOnGroundLastTick;
        var isOnGround = IsActuallyGrounded(pawn);

        if (isOnGround)
        {
            state.AirJumpsUsed = 0;
        }
        else if (state.AirJumpsUsed < 1)
        {
            state.AirJumpsUsed = 1;
        }

        var maxTotalJumps = Math.Max(1, additionalJumps + 1);
        if (maxTotalJumps > 1
            && !wasJumping
            && isJumping
            && !wasOnGround
            && !isOnGround
            && state.AirJumpsUsed < maxTotalJumps)
        {
            state.AirJumpsUsed++;
            ApplyAirJumpVelocity(pawn, upForce);
        }

        state.PreviousJumpButtons = currentButtons;
        state.WasOnGroundLastTick = isOnGround;
    }

    public static void Reset(IAirJumpState state, PlayerButtons currentButtons)
    {
        state.AirJumpsUsed = 0;
        state.PreviousJumpButtons = currentButtons;
        state.WasOnGroundLastTick = true;
    }

    public static void ApplyAirJumpVelocity(CCSPlayerPawn pawn, float upForce)
    {
        var currentVelocity = pawn.AbsVelocity;
        var effectiveUpForce = Math.Max(1.0f, upForce);
        pawn.Teleport(velocity: new Vector(currentVelocity.X, currentVelocity.Y, effectiveUpForce));
    }

    public static bool IsActuallyGrounded(CCSPlayerPawn pawn)
    {
        return (pawn.Flags & OnGroundFlag) == OnGroundFlag;
    }
}
