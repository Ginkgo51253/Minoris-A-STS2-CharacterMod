# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Minoris is an original character mod for **Slay the Spire 2**, built on Godot 4.5.1 with C# / .NET 9.0. The character "Minoris" is a boy blessed by the Sun God, featuring multi-hit attack mechanics. Author: 银杏 (Yinxing).

## Build Commands

```bash
# Debug build (default) - also auto-copies to ST2 mods folder
build.bat

# Release build
build.bat release

# Export as PCK package (requires GODOT_PATH env var)
build.bat pack

# Build without copying to mods folder
build.bat nocopy
```

Direct dotnet build:
```bash
dotnet build ./Minoris.csproj -c Debug
```

Environment variables for configuring paths: `GODOT_PATH`, `STS2_PATH`, `STS2_DATA_DIR`, `STS2_MODS_DIR`.

## Architecture

### Entry Point
`MainFile.cs` — `[ModInitializer]` attribute marks the `Initialize()` method. Creates a Harmony instance and patches all `[HarmonyPatch]` classes in the assembly. Also defines the `GRIP` custom keyword.

### Code Organization (`Minoris/MinorisCode/`)
- **Character/** — `Minoris.cs` (character model, starting deck/HP/relic), `MinorisCardPool.cs`, `MinorisRelicPool.cs`, `MinorisPotionPool.cs`
- **Cards/** — Split into `Cards_StarterAndSamples.cs` and `Cards_AncientAndMultiplayer.cs`. All cards extend `MinorisCard` which extends `CustomCardModel`.
- **Relics/** — `Relics_All.cs`, extend `MinorisRelic` → `CustomRelicModel`
- **Powers/** — Extend `MinorisPower`
- **Potions/** — Extend `MinorisPotion`
- **Extensions/** — `StringExtensions.cs` provides resource path helpers (`ImagePath()`, `CardImagePath()`, etc.)

### Localization (`Minoris/localization/`)
JSON files per language (`eng/`, `zhs/`) for cards, relics, powers, characters, and keywords. The `Alchyr.Sts2.ModAnalyzers` NuGet package validates these at build time. Localization files are declared in `.csproj` as `<AdditionalFiles>`.

### Harmony Patches (in `MainFile.cs`)
- **CreatureCmdPatches** — Wraps creature death to trigger win condition checks; handles player kill when combat state is null.
- **ProgressSaveManagerPatches** — Bypasses unlock/progress checks for custom characters via `BypassProgressSaveManagerEpochChecks` flag.
- **NRelicModelGetterPatch** — Fallback to Akabeko relic model when `_model` field is null.

### Key Dependencies
- **Alchyr.Sts2.BaseLib v0.1.7** — Modding framework (custom card/relic/power models, localization pipeline)
- **0Harmony** — Runtime patching (loaded from ST2 game directory, not NuGet)
- **sts2.dll / SmartFormat.dll** — Game engine references (loaded from ST2 data directory)

### Resource Conventions
- Character asset prefix: `minoris_`
- Character color theme: yellow/amber (`#ffe100`)
- Starting HP: 70, starting relic: Small Bow Tie
- Custom keyword: `GRIP` (registered via `[CustomEnum]` + `[KeywordProperties]`)

## Development Notes

- No test framework; testing is manual via in-game runs.
- The `.csproj` auto-detects platform (Windows/Linux/macOS) and resolves ST2 game paths from Steam registry or standard directories.
- `Nullable` is enabled project-wide.
- Godot export uses "mobile" rendering method. Export configuration is in `export_presets.cfg`.
