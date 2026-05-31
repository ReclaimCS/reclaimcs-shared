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
        float upForce,
        bool allowInstantJump = true)
    {
        if (player.PlayerPawn.Value is not { IsValid: true } pawn)
            return;

        if (!enabled || !player.PawnIsAlive)
        {
            Reset(state, player.Buttons);
            return;
        }

        var currentFlags = (PlayerFlags)pawn.Flags;
        var currentButtons = player.Buttons;
        var wasOnGround = (state.PreviousJumpFlags & PlayerFlags.FL_ONGROUND) != 0;
        var isOnGround = (currentFlags & PlayerFlags.FL_ONGROUND) != 0;
        var wasJumping = (state.PreviousJumpButtons & PlayerButtons.Jump) != 0;
        var isJumping = (currentButtons & PlayerButtons.Jump) != 0;

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
            if (allowInstantJump || CanForceAirJump(pawn))
                ApplyAirJumpVelocity(pawn, upForce);
        }

        state.PreviousJumpButtons = currentButtons;
        state.PreviousJumpFlags = currentFlags;
        state.WasOnGroundLastTick = isOnGround;
    }

    public static void Reset(IAirJumpState state, PlayerButtons currentButtons)
    {
        state.AirJumpsUsed = 0;
        state.PreviousJumpButtons = currentButtons;
        state.PreviousJumpFlags = PlayerFlags.FL_ONGROUND;
        state.WasOnGroundLastTick = true;
    }

    public static void ApplyAirJumpVelocity(CCSPlayerPawn pawn, float upForce)
    {
        var effectiveUpForce = Math.Max(1.0f, upForce);
        pawn.AbsVelocity.Z = effectiveUpForce;
    }

    public static bool CanForceAirJump(CCSPlayerPawn pawn)
    {
        return pawn.AbsVelocity.Z < 0.0f;
    }

    public static bool IsActuallyGrounded(CCSPlayerPawn pawn)
    {
        return (pawn.Flags & OnGroundFlag) == OnGroundFlag;
    }
}
