# REPO Coins

Client-side mod for R.E.P.O.: you get **1 cosmetic coin each time you complete a level**, meaning the truck leaves and you go to the shop.
It works whether you're hosting or joining. Only you need it, and it doesn't affect anyone else in the lobby.

No coin for failed levels, the shop, the arena, the lobby, or for joining a lobby mid-run.

## Install

**With r2modman or Gale (easiest)**
1. Select R.E.P.O. and make or pick a profile.
2. Settings → *Import local mod* → choose `RepoCoins-x.y.z.zip`. BepInExPack is installed for you if it's missing.
3. Start the game with *Start modded*.

**Manual**
1. Install [BepInExPack](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/) into the game folder (Steam → R.E.P.O. → Manage → Browse local files).
2. Put `RepoCoins.dll` in `BepInEx\plugins\`.

## Settings
These are in `BepInEx\config\gabe.repocoins.cfg`, or the config editor in r2modman/Gale:
- `Rarity`: Common (default), Uncommon, Rare or UltraRare
- `ShowMessage`: shows a "+1 coin" message on screen (on by default)
- `BackupSaveFirst`: on by default

## Safety
Before the first coin, the mod copies your save to
`%USERPROFILE%\AppData\LocalLow\semiwork\Repo\MetaSave.es3.repocoins-backup`.
To undo everything, close the game, delete `MetaSave.es3` and rename the backup to `MetaSave.es3`.
