namespace ReclaimCS.Shared.PlayerModels;

public static class ReclaimPlayerModels
{
    public const string ReclaimCharactersWorkshopAddonId = "3732494585";
    public const string ZombiePlayerModelsWorkshopAddonId = "3170427476";

    public static readonly string[] ZombieModWorkshopAddonIds =
    [
        ZombiePlayerModelsWorkshopAddonId,
        "3730088795",
        "3730813775"
    ];

    public static class ModelIds
    {
        public const string BatmanClassic = "batman_classic";
        public const string Cj = "cj";
        public const string DeadpoolWorkshop = "deadpool_workshop";
        public const string MasterChief = "master_chief";
        public const string Hunk = "hunk";
        public const string MaalikInfernal = "maalik_infernal";
        public const string MetroCop = "metro_cop";

        public const string ClassicZombie = "classic_zombie";
        public const string Molong = "molong";
        public const string RunnerZombie = "runner_zombie";
        public const string BruteZombie = "brute_zombie";
        public const string CultistZombie = "cultist_zombie";
        public const string FrozenZombie = "frozen_zombie";
        public const string Lurker = "lurker";

        public const string EarthGovSecurity = "earthgov_security";
        public const string TacticalTrooper = "tactical_trooper";
        public const string VectorHunter = "vector_hunter";
        public const string DeadpoolReborn = "deadpool_reborn";
        public const string GhostTactical = "ghost_tactical";
        public const string MasterChiefNoHitbox = "master_chief_nohitbox";
    }

    public static class ModelPaths
    {
        public const string BatmanClassic = "characters/models/night_fighter/fortnite/batman_classic/batman_classic_fix.vmdl";
        public const string BatmanClassicArms = "characters/models/night_fighter/fortnite/batman_classic/batman_classic_arms.vmdl";
        public const string Cj = "characters/models/nuclearsilo/gta/cj/cj.vmdl";
        public const string CjNoHitbox = "characters/models/nuclearsilo/gta/cj/cj_nohitbox.vmdl";
        public const string DeadpoolWorkshop = "characters/models/kolka/deadpool/deadpool.vmdl";
        public const string DeadpoolWorkshopArms = "characters/models/kolka/deadpool/deadpool_arms.vmdl";
        public const string MasterChief = "characters/models/kolka/master_chief/master_chief.vmdl";
        public const string Hunk = "characters/models/darnias/hunk/hunk.vmdl";
        public const string MaalikInfernal = "characters/models/nozb1/maalik_infernal_player_model/maalik_infernal_player_model.vmdl";
        public const string MaalikInfernalArms = "characters/models/nozb1/maalik_infernal_player_model/maalik_infernal_arm.vmdl";
        public const string MetroCop = "characters/models/kolka/metro_cop/metro_cop.vmdl";

        public const string ClassicZombie = "agents/models/gxp/classic_zombie/classic_zombie.vmdl";
        public const string Molong = "agents/models/han/molong/molong.vmdl";
        public const string RunnerZombie = "agents/models/s2ze/zombie_basic/zombie_basic.vmdl";
        public const string BruteZombie = "agents/models/s2ze/zombie_chris_walker/zombie_chris_walker.vmdl";
        public const string CultistZombie = "agents/models/s2ze/zombie_cultist/zombie_cultist.vmdl";
        public const string FrozenZombie = "agents/models/s2ze/zombie_frozen/zombie_frozen.vmdl";
        public const string Lurker = "characters/models/kolka/2025/lurker/lurker.vmdl";

        public const string EarthGovSecurity = "agents/models/s2ze/earthgovsol/deadspace_earthgovsol_hitbox.vmdl";
        public const string TacticalTrooper = "agents/models/s2ze/hd2_b01_tac/hd2_b01_tac_nohitbox.vmdl";
        public const string VectorHunter = "agents/models/apple/vector/vector.vmdl";
        public const string DeadpoolReborn = "agents/models/reborn/deadpool/deadpool.vmdl";
        public const string GhostTactical = "characters/models/kolka/ghost/ghost.vmdl";
        public const string MasterChiefNoHitbox = "agents/models/s2ze/master_chief/master_chief_nohitbox.vmdl";
    }

    public static IReadOnlyList<PlayerModelDefinition> All { get; } =
    [
        ReclaimCharacter(ModelIds.BatmanClassic, "Batman Classic", ModelPaths.BatmanClassic, "https://gamebanana.com/mods/475754", armModelPath: ModelPaths.BatmanClassicArms),
        ReclaimCharacter(ModelIds.Cj, "CJ", ModelPaths.Cj, "https://gamebanana.com/mods/519998", alternateModelPaths: [ModelPaths.CjNoHitbox]),
        ReclaimCharacter(ModelIds.DeadpoolWorkshop, "Deadpool", ModelPaths.DeadpoolWorkshop, "https://gamebanana.com/mods/483043", armModelPath: ModelPaths.DeadpoolWorkshopArms),
        ReclaimCharacter(ModelIds.MasterChief, "Master Chief", ModelPaths.MasterChief, "https://gamebanana.com/mods/464875"),
        ReclaimCharacter(ModelIds.Hunk, "HUNK", ModelPaths.Hunk, "https://gamebanana.com/mods/464551"),
        ReclaimCharacter(ModelIds.MaalikInfernal, "Maaalik Infernal", ModelPaths.MaalikInfernal, "https://gamebanana.com/mods/482660", armModelPath: ModelPaths.MaalikInfernalArms),
        ReclaimCharacter(ModelIds.MetroCop, "Metrocop", ModelPaths.MetroCop, "https://gamebanana.com/mods/470173"),

        ZombieMod(ModelIds.ClassicZombie, "Classic Zombie", ModelPaths.ClassicZombie, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.Molong, "VIP Molong", ModelPaths.Molong, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.RunnerZombie, "Runner", ModelPaths.RunnerZombie, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.BruteZombie, "Brute", ModelPaths.BruteZombie, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.CultistZombie, "Cultist", ModelPaths.CultistZombie, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.FrozenZombie, "Frozen Zombie", ModelPaths.FrozenZombie, PlayerModelRole.Zombie),
        ZombieMod(ModelIds.Lurker, "Lurker", ModelPaths.Lurker, PlayerModelRole.Zombie),

        ZombieMod(ModelIds.EarthGovSecurity, "EarthGov Security", ModelPaths.EarthGovSecurity, PlayerModelRole.Human),
        ZombieMod(ModelIds.TacticalTrooper, "Tactical Trooper", ModelPaths.TacticalTrooper, PlayerModelRole.Human),
        ZombieMod(ModelIds.VectorHunter, "Vector Hunter", ModelPaths.VectorHunter, PlayerModelRole.Human),
        ZombieMod(ModelIds.DeadpoolReborn, "VIP Heavy Deadpool", ModelPaths.DeadpoolReborn, PlayerModelRole.Human),
        ZombieMod(ModelIds.GhostTactical, "VIP Tactical Ghost", ModelPaths.GhostTactical, PlayerModelRole.Human),
        ZombieMod(ModelIds.MasterChiefNoHitbox, "Master Chief No-Hitbox", ModelPaths.MasterChiefNoHitbox, PlayerModelRole.Human)
    ];

    public static IEnumerable<PlayerModelDefinition> ReclaimCharacters =>
        All.Where(model => model.Pack == PlayerModelPack.ReclaimCharacters);

    public static IEnumerable<PlayerModelDefinition> ZombieModCharacters =>
        All.Where(model => model.Pack == PlayerModelPack.ZombieMod);

    public static IEnumerable<PlayerModelDefinition> Humans =>
        All.Where(model => model.Roles.HasFlag(PlayerModelRole.Human));

    public static IEnumerable<PlayerModelDefinition> Zombies =>
        All.Where(model => model.Roles.HasFlag(PlayerModelRole.Zombie));

    public static bool TryFind(string? idOrNameOrPath, out PlayerModelDefinition model)
    {
        model = null!;
        var normalized = NormalizeLookupValue(idOrNameOrPath);
        if (normalized.Length == 0)
            return false;

        var found = All.FirstOrDefault(candidate =>
            NormalizeLookupValue(candidate.Id) == normalized
            || NormalizeLookupValue(candidate.Name) == normalized
            || NormalizeLookupValue(candidate.ModelPath) == normalized);

        if (found == null)
            return false;

        model = found;
        return true;
    }

    public static string ResolveModelPath(string? idOrPath, string? fallbackIdOrPath = null)
    {
        var primary = ResolveModelPathCore(idOrPath);
        if (!string.IsNullOrWhiteSpace(primary))
            return primary;

        return ResolveModelPathCore(fallbackIdOrPath);
    }

    public static string NormalizeModelPath(string? modelPath)
    {
        return string.IsNullOrWhiteSpace(modelPath)
            ? ""
            : modelPath.Trim().Replace('\\', '/');
    }

    public static IEnumerable<string> GetWorkshopAddonIds(IEnumerable<PlayerModelDefinition> models)
    {
        return models
            .SelectMany(model => model.WorkshopAddonIds)
            .Where(addonId => !string.IsNullOrWhiteSpace(addonId))
            .Select(addonId => addonId.Trim())
            .Distinct(StringComparer.Ordinal);
    }

    private static string ResolveModelPathCore(string? idOrPath)
    {
        var normalized = NormalizeModelPath(idOrPath);
        if (normalized.Length == 0)
            return "";

        if (normalized.EndsWith(".vmdl", StringComparison.OrdinalIgnoreCase) || normalized.Contains('/'))
            return normalized;

        return TryFind(normalized, out var model) ? model.ModelPath : "";
    }

    private static string NormalizeLookupValue(string? value)
    {
        return NormalizeModelPath(value)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();
    }

    private static PlayerModelDefinition ReclaimCharacter(
        string id,
        string name,
        string modelPath,
        string sourceUrl,
        string armModelPath = "",
        string[]? alternateModelPaths = null)
    {
        return new PlayerModelDefinition
        {
            Id = id,
            Name = name,
            ModelPath = modelPath,
            Pack = PlayerModelPack.ReclaimCharacters,
            Roles = PlayerModelRole.Human,
            WorkshopAddonIds = [ReclaimCharactersWorkshopAddonId],
            SourceUrl = sourceUrl,
            DateAdded = "2026-05-25",
            Contributor = "dmac",
            ArmModelPath = armModelPath,
            AlternateModelPaths = alternateModelPaths ?? []
        };
    }

    private static PlayerModelDefinition ZombieMod(
        string id,
        string name,
        string modelPath,
        PlayerModelRole roles)
    {
        return new PlayerModelDefinition
        {
            Id = id,
            Name = name,
            ModelPath = modelPath,
            Pack = PlayerModelPack.ZombieMod,
            Roles = roles,
            WorkshopAddonIds = [..ZombieModWorkshopAddonIds]
        };
    }
}
