# PlasmicTrap

PlasmicTrap is a fork of [Bloxstrap](https://github.com/bloxstraplabs/bloxstrap), an alternative launcher for Roblox with extra features. It installs and runs independently, so it can sit next to Bloxstrap or Fishstrap, though only one of them can own the `roblox://` protocol handlers at a time (whichever was installed or launched last).

## What's different from Bloxstrap

- Rebranded: own name, icon, install folder (`%LocalAppData%\PlasmicTrap`), uninstall entry, and process mutexes.
- Updates come from this repository's GitHub releases, never from Bloxstrap's.
- No telemetry. Analytics and crash-log upload to Bloxstrap's servers are removed, and the analytics settings are gone.
- Clear cache: a button in the settings page that wipes Roblox's package download cache, asset cache, and PlasmicTrap's own download cache. It refuses to run while Roblox is open.

Everything else (FastFlags, mods, custom bootstrapper themes, Discord Rich Presence, activity tracking) is inherited from Bloxstrap. `BloxstrapRPC` and `BloxstrapCustomBootstrapper` keep their names so existing games and themes keep working.

## Setup before you publish

Set your GitHub repository in `PlasmicTrap/App.xaml.cs`:

```csharp
public const string ProjectRepository = "YOUR_GITHUB_USERNAME/PlasmicTrap";
```

Until you change it, the updater does nothing. A few links in the About, Welcome, and install pages use the same `YOUR_GITHUB_USERNAME/PlasmicTrap` placeholder, so search for it and replace all occurrences.

## Building

Requires Windows and the .NET 6 SDK (`global.json` pins 6.0.428).

```
dotnet restore
dotnet build -c Release
dotnet publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false .\PlasmicTrap\PlasmicTrap.csproj
```

The `wpfui` folder is a vendored copy of [bloxstraplabs/wpfui](https://github.com/bloxstraplabs/wpfui), so there are no submodules to initialise.

## Releasing

Push a tag like `v3.0.1`. The release workflow builds on `windows-latest` and attaches `PlasmicTrap-v3.0.1.exe` to a draft release. The in-app updater downloads the first asset of the latest release, so keep that exe as the only asset.

## Credits and license

PlasmicTrap is MIT licensed. It is built on Bloxstrap by pizzaboxer and its contributors, and on [WPF-UI](https://github.com/lepoco/wpfui); see `LICENSE` and the About page.
