# ReclaimCS Shared

Shared .NET foundations for ReclaimCS CounterStrikeSharp plugins.

This repository intentionally contains only code that is generic across game modes:

- chat color formatting and branded message helpers
- center HTML output and interactive center-menu primitives
- CounterStrikeSharp player identity/runtime-key helpers
- CounterStrikeSharp network state-change helpers
- shared air-jump/double-jump movement primitives
- small SQLite primitives shared by plugin persistence layers

Gameplay systems stay in their plugin repositories. COD class/loadout/perk behavior belongs in `reclaimcs-cod`; infection, zombie abilities, and human/zombie round logic belong in `reclaimcs-zombie`.

## Local Workspace

Expected sibling layout:

```text
ReclaimCS/
  reclaimcs-cod/
  reclaimcs-zombie/
  reclaimcs-shared/
```

Both plugin projects reference `src/ReclaimCS.Shared/ReclaimCS.Shared.csproj` through sibling project references.
