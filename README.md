# RepoCoins

**Get 1 cosmetic coin every time you complete a level in R.E.P.O.** Client-side: only you need it, host or guest.

## Install (players)
1. Install **r2modman**: https://thunderstore.io/package/ebkr/r2modman/
2. Download **`RepoCoins-x.y.z.zip`** from the [latest release](https://github.com/Flelk/RepoCoins/releases/latest). Don't unzip it.
3. In r2modman: pick **R.E.P.O.** → a profile → **Settings → Import local mod** → choose the zip.
4. Click **Start modded**.

Details, settings and how to undo: [thunderstore/README.md](thunderstore/README.md).

---

## Source

A client-side BepInEx 5 mod for R.E.P.O. It gives 1 cosmetic coin (`MetaManager.CosmeticTokenAdd`) per completed level.
It's detected by Harmony postfixes on `RunManager.ChangeLevel` (host/solo) and `RunManager.UpdateLevel` (guests), when the run goes from a level to the shop and `levelsCompleted` goes up by 1.
The player-facing docs are in [thunderstore/README.md](thunderstore/README.md).

## Build it yourself
Needs the .NET SDK 8+ and R.E.P.O. installed. The game's DLLs are referenced from your install and never committed.

```powershell
.\build.ps1                 # -> dist\RepoCoins-<version>.zip  (Thunderstore format)
.\build.ps1 -Install        # also copies the DLL into <game>\BepInEx\plugins\RepoCoins
.\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\REPO"
```
Or double-click `build.cmd`. If you have no mod manager, run `tools\install-bepinex.ps1` once first.

## Share it
- **Friends:** send them `dist\RepoCoins-x.y.z.zip`. In r2modman/Gale they use *Import local mod*.
- **Public:** upload the same zip at https://thunderstore.io/c/repo/create/. Bump `version_number` in `thunderstore/manifest.json` (and add to CHANGELOG) for each release. The DLL version follows it automatically.
- **Source:** this folder is a git repo. Push it to GitHub so others can build or fork it.

## Layout
| Path | What |
| --- | --- |
| `src/Plugin.cs` | The mod |
| `RepoCoins.csproj` | References BepInEx (NuGet) and the game's own DLLs (publicized) |
| `thunderstore/` | manifest, README, CHANGELOG, icon that go into the zip |
| `build.ps1` / `build.cmd` | Build + package (+ optional install) |
| `tools/install-bepinex.ps1` | Puts BepInExPack in the game folder for testing |
