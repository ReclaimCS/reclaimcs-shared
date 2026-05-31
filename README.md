# ReclaimCS Shared

Shared .NET foundations for ReclaimCS CounterStrikeSharp plugins.

This repository intentionally contains only code that is generic across game modes:

- chat color formatting and branded message helpers
- center HTML output and interactive center-menu primitives
- CounterStrikeSharp player identity/runtime-key helpers
- CounterStrikeSharp network state-change helpers
- shared air-jump/double-jump movement primitives
- small SQLite primitives shared by plugin persistence layers
- dashboard/web integration primitives for authenticated JSON APIs, per-player polling, and push de-duplication
- shared player-model catalog entries, model-path resolution, workshop-pack IDs, and resource-manifest helpers
- shared kill feed icon config, matching, resource precache, and `EventPlayerDeath` rewriting

Gameplay systems stay in their plugin repositories. COD class/loadout/perk behavior belongs in `reclaimcs-cod`; infection, zombie abilities, and human/zombie round logic belong in `reclaimcs-zombie`.

## Player Models

`ReclaimCS.Shared.PlayerModels` owns reusable model metadata for all ReclaimCS mods:

- `ReclaimPlayerModels.ReclaimCharacters` contains the mapper workshop pack `3732494585`.
- `ReclaimPlayerModels.ZombieModCharacters` contains the current zombie/human model pack paths already used by Zombie Mod.
- `ReclaimPlayerModels.ResolveModelPath(...)` accepts either a shared model ID, display name, or direct `.vmdl` path.
- `ResourceManifest.AddPlayerModelResource(...)` and `AddPlayerModelResources(...)` normalize model paths before adding them to the CounterStrikeSharp resource manifest.

## Kill Feed Icons

`ReclaimCS.Shared.KillFeed` owns the reusable kill feed icon layer. A plugin config can expose:

```csharp
public KillFeedIconOptions KillFeedIcons { get; set; } = new();
```

Register it from a CounterStrikeSharp plugin with:

```csharp
RegisterEventHandler<EventPlayerDeath>((@event, info) =>
{
    KillFeedIconService.Apply(Config.KillFeedIcons, @event);
    return HookResult.Continue;
}, HookMode.Pre);

RegisterListener<Listeners.OnServerPrecacheResources>(manifest =>
    KillFeedIconService.PrecacheResources(Config.KillFeedIcons, manifest));
```

Config example:

```json
"KillFeedIcons": {
  "Enabled": true,
  "Icons": {
    "knife": { "Icon": "reclaim_claws" },
    "hegrenade": { "Icon": "reclaim_explosion" },
    "awp": { "Icon": "dronegun", "Permission": ["#css/vip"], "Team": "T" },
    "*": { "Icon": "movelinear" }
  }
}
```

`Icon` is the kill feed equipment icon key written into `EventPlayerDeath.Weapon`. Custom SVGs should be mounted by an addon under `panorama/images/icons/equipment/<icon>.svg`. ReclaimCS-shared also keeps canonical source assets under `assets/killfeed-icons/panorama/images/icons/equipment/` so future servers can copy or package the same icon layout. `Resource` can be set when the resource to precache differs from `Icon`; otherwise the icon key itself is added to the resource manifest.

## Local Workspace

Expected sibling layout:

```text
ReclaimCS/
  reclaimcs-cod/
  reclaimcs-zombie/
  reclaimcs-shared/
```

Both plugin projects reference `src/ReclaimCS.Shared/ReclaimCS.Shared.csproj` through sibling project references.
