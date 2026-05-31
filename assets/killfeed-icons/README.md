# ReclaimCS Killfeed Icons Addon

This folder is the source layout for the client-mounted CS2 addon that carries
custom killfeed SVGs.

Published Workshop item: `3733924238`.

Expected icon paths:

```text
panorama/images/icons/equipment/kamikaze.svg
panorama/images/icons/equipment/thorns.svg
panorama/images/icons/equipment/firebrand.svg
```

Before publishing the addon through CS2 Workshop Tools, add this include to the
addon build `gameinfo.gi` `AddonConfig/VpkDirectories` list:

```text
"include" "panorama/images/icons/equipment"
```

After the workshop item is published, mount that workshop ID for clients with
MultiAddonManager, then set the server killfeed config back to:

```json
"KillFeedIcons": {
  "Enabled": true,
  "Icons": {
    "kamikaze": { "Icon": "kamikaze" },
    "thorns": { "Icon": "thorns" },
    "firebrand": { "Icon": "firebrand" }
  }
}
```
