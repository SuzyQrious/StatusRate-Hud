# Status Rate HUD

A compact HUD mod for **The Long Dark** that shows your remaining survival stats and how quickly they are changing **per real-world second**.

[日本語の説明](README.ja.md) · [Download](https://github.com/SuzyQrious/StatusRate-Hud/releases/latest)

## Features

- Remaining warmth, energy, hydration, satiety and condition as percentages.
- Actual changes measured approximately every 0.5 seconds, shown as percentage points per real-world second.
- Muted colors, shadowed text and slim gauges; configurable size and position.
- Hidden in pause, inventory and other non-gameplay menus.
- Live position/size preview when **this mod's own settings tab** is selected.
- HUD labels, settings and numeric formatting follow the game's selected language.
- 19 supported languages, with English as the fallback.
- Read-only: does not change player stats or save data.

For example, `80.0%  -0.025%/s` means the value is falling by **0.025 percentage points per real-world second**. Food, drink, damage and time acceleration affect the measurement. This is a recent measurement, not a prediction. A value capped at full or empty reports zero once it stops changing.

## Requirements

Tested by the author/user on Windows, in Survival mode:

| Component | Version |
| --- | --- |
| The Long Dark | 2.60 |
| [MelonLoader](https://github.com/LavaGang/MelonLoader) | 0.7.3 |
| [ModSettings](https://github.com/DigitalzombieTLD/ModSettings) | 2.2.5 |

MelonLoader and ModSettings must be installed separately. They are not bundled. Other versions, operating systems and Wintermute have not been tested.

## Installation

1. Close the game.
2. Install MelonLoader for the game's `tld.exe`, then install `ModSettings.dll` in the game's `Mods` folder.
3. Download **StatusRateHud.dll** from [Releases](https://github.com/SuzyQrious/StatusRate-Hud/releases).
4. Place `StatusRateHud.dll` directly in the game installation folder's `Mods` folder.

```text
TheLongDark/
└── Mods/
    ├── ModSettings.dll
    └── StatusRateHud.dll
```

Steam: right-click the game → **Manage → Browse local files** to find the installation folder.

Download `StatusRateHud.dll`, not GitHub's automatically generated **Source code** archives. The source archives contain code, not an installable DLL.

## Settings and preview

Open **Options → Mod Settings → Status Rate HUD** to enable the HUD or adjust text size, horizontal position and vertical position.

The preview follows sliders immediately, including when the gameplay HUD is disabled. It uses frozen values (75% if no live sample has been acquired) and zero change rates. Select another mod or leave the settings screen to hide it. Confirm settings to save changes.

## Languages

English, Japanese, German, Russian, French, Spanish, Finnish, Korean, Norwegian, Portuguese (Portugal), Portuguese (Brazil), Simplified Chinese, Traditional Chinese, Swedish, Turkish, Dutch, Italian, Polish and Ukrainian.

Language selection follows the game, not Windows. The mod name **Status Rate HUD** stays unchanged. Reopen the settings screen if it was already open when changing language. Translated text adjusts its column width automatically. The game HUD font is preferred, with Windows font fallbacks.

Translations have not all been reviewed by native speakers or visually tested. Corrections are welcome through [Issues](https://github.com/SuzyQrious/StatusRate-Hud/issues).

## Updating / uninstalling

Close the game before replacing or removing `Mods/StatusRateHud.dll`. Do not keep old DLL copies under different filenames inside `Mods`. Existing settings are retained on upgrade.

Versions **1.3.0 and 1.3.1** fail to load under MelonLoader's invariant-globalization runtime. Use **1.3.2 or later**.

## Build from source

Install a .NET SDK capable of targeting .NET 6. A local game installation with MelonLoader-generated interop assemblies and ModSettings is required. No game assemblies are included in this repository.

```powershell
dotnet build StatusRateHud.csproj -c Release -p:GameDir="D:\SteamLibrary\steamapps\common\TheLongDark"
```

The resulting DLL is in `bin/Release/net6.0/`. Building does not install the mod automatically.

The pure calculation/localization tests do not require the game:

```powershell
dotnet run --project StatusRateHud.Tests.csproj -c Release
dotnet run --project StatusRateHud.Tests.csproj -c Release -p:InvariantGlobalization=true
```

Tests cover real-time sampling, pause/recovery/acceleration, language aliases, fallback and numeric formatting. Version 1.3.2 was also tested in-game by the user. Full visual QA in every language is still pending.

See [CHANGELOG](CHANGELOG.md) for version history. This is an unofficial mod, not affiliated with Hinterland.


