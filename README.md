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

Gameplay systems stay in their plugin repositories. COD class/loadout/perk behavior belongs in `reclaimcs-cod`; infection, zombie abilities, and human/zombie round logic belong in `reclaimcs-zombie`.

## Player Models

`ReclaimCS.Shared.PlayerModels` owns reusable model metadata for all ReclaimCS mods:

- `ReclaimPlayerModels.ReclaimCharacters` contains the mapper workshop pack `3732494585`.
- `ReclaimPlayerModels.ZombieModCharacters` contains the current zombie/human model pack paths already used by Zombie Mod.
- `ReclaimPlayerModels.ResolveModelPath(...)` accepts either a shared model ID, display name, or direct `.vmdl` path.
- `ResourceManifest.AddPlayerModelResource(...)` and `AddPlayerModelResources(...)` normalize model paths before adding them to the CounterStrikeSharp resource manifest.

## Local Workspace

Expected sibling layout:

```text
ReclaimCS/
  reclaimcs-cod/
  reclaimcs-zombie/
  reclaimcs-shared/
```

Both plugin projects reference `src/ReclaimCS.Shared/ReclaimCS.Shared.csproj` through sibling project references.
