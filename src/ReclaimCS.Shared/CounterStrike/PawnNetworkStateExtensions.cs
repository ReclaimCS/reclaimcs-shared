using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace ReclaimCS.Shared.CounterStrike;

public static class PawnNetworkStateExtensions
{
    public static void MarkHealthStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CBaseEntity", "m_iMaxHealth");
        TryMarkStateChanged(entity, "CBaseEntity", "m_iHealth");
    }

    public static void MarkMovementStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CCSPlayerPawn", "m_flVelocityModifier");
        TryMarkStateChanged(entity, "CBaseEntity", "m_flGravityScale");
    }

    public static void MarkArmorStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CCSPlayerPawn", "m_ArmorValue");
    }

    public static void MarkRenderStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CBaseModelEntity", "m_clrRender");
        TryMarkStateChanged(entity, "CBaseModelEntity", "m_nRenderMode");
        TryMarkStateChanged(entity, "CBaseModelEntity", "m_nRenderFX");
        entity.MarkEffectsStateChanged();
    }

    public static void MarkEffectsStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CBaseEntity", "m_fEffects");
    }

    public static void MarkEconStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CEconEntity", "m_AttributeManager");
        TryMarkStateChanged(entity, "CAttributeContainer", "m_Item");
        TryMarkStateChanged(entity, "CEconItemView", "m_iItemDefinitionIndex");
        TryMarkStateChanged(entity, "CEconItemView", "m_iItemIDHigh");
        TryMarkStateChanged(entity, "CEconItemView", "m_iItemIDLow");
        TryMarkStateChanged(entity, "CEconItemView", "m_bInitialized");
    }

    public static void MarkInventoryStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CCSPlayerController", "m_pInventoryServices");
        TryMarkStateChanged(entity, "CCSPlayerController_InventoryServices", "m_vecServerAuthoritativeWeaponSlots");
    }

    public static void MarkMoneyStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CCSPlayerController", "m_pInGameMoneyServices");
        TryMarkStateChanged(entity, "CCSPlayerController_InGameMoneyServices", "m_iAccount");
        TryMarkStateChanged(entity, "CCSPlayerController_InGameMoneyServices", "m_iStartAccount");
    }

    public static void MarkTeamStateChanged(this CBaseEntity entity)
    {
        TryMarkStateChanged(entity, "CBaseEntity", "m_iTeamNum");
    }

    public static void MarkPlayerStatsStateChanged(
        this CBaseEntity entity,
        bool includeArmor = true,
        bool includeRender = true)
    {
        entity.MarkHealthStateChanged();
        entity.MarkMovementStateChanged();

        if (includeArmor)
            entity.MarkArmorStateChanged();

        if (includeRender)
            entity.MarkRenderStateChanged();
    }

    private static void TryMarkStateChanged(CBaseEntity entity, string className, string fieldName)
    {
        if (!entity.IsValid)
            return;

        try
        {
            Utilities.SetStateChanged(entity, className, fieldName, 0);
        }
        catch
        {
        }
    }
}
