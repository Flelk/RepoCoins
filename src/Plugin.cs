using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RepoCoins;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    internal static ConfigEntry<SemiFunc.Rarity> CoinRarity;
    internal static ConfigEntry<bool> ShowMessage;
    internal static ConfigEntry<bool> BackupSave;

    void Awake()
    {
        Log = Logger;

        CoinRarity = Config.Bind("General", "Rarity", SemiFunc.Rarity.Common,
            "Rarity of the coin you get for each completed level: Common, Uncommon, Rare or UltraRare.");
        ShowMessage = Config.Bind("General", "ShowMessage", true,
            "Show a short message on screen when you get a coin.");
        BackupSave = Config.Bind("Safety", "BackupSaveFirst", true,
            "Copy MetaSave.es3 to MetaSave.es3.repocoins-backup before the first coin (only if no backup exists yet).");

        // Hidden object that draws the on-screen message and survives scene changes.
        var go = new GameObject("RepoCoins");
        go.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(go);
        go.AddComponent<CoinMessage>();

        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll(typeof(LevelCompletePatches));
        Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} loaded: 1 {CoinRarity.Value} coin per completed level.");
    }

    /// <summary>Gives one coin. Called once per completed level on this player's machine.</summary>
    internal static void GiveCoin(int levelsCompleted)
    {
        try
        {
            var meta = MetaManager.instance;
            if (meta == null || !meta.saveReady)
            {
                Log.LogWarning("Level completed but the cosmetic save isn't ready; no coin given.");
                return;
            }

            if (BackupSave.Value) TryBackup(meta);

            var rarity = CoinRarity.Value;
            if (CosmeticTokenUI.instance != null)
            {
                meta.CosmeticTokenAdd(rarity); // adds, refreshes the coin UI and saves
            }
            else
            {
                // Coin UI isn't loaded in this scene: add and save directly.
                meta.cosmeticTokens.Add((int)rarity);
                meta.Save();
            }

            Log.LogInfo($"Level {levelsCompleted} completed: +1 {rarity} coin (total {meta.cosmeticTokens.Count}).");
            if (ShowMessage.Value) CoinMessage.Show($"+1 {rarity} coin  (level {levelsCompleted} complete)");
        }
        catch (Exception e)
        {
            Log.LogError($"Failed to give coin: {e}");
        }
    }

    static void TryBackup(MetaManager meta)
    {
        try
        {
            string save = meta.SavePathFull;
            string backup = save + ".repocoins-backup";
            if (File.Exists(save) && !File.Exists(backup))
            {
                File.Copy(save, backup);
                Log.LogInfo($"Backed up save to {backup}");
            }
        }
        catch (Exception e)
        {
            Log.LogWarning($"Save backup failed: {e.Message}");
        }
    }
}

/// <summary>
/// A level counts as completed when the run goes from a real level to the shop and
/// levelsCompleted goes up by exactly one.
/// Host / single player: RunManager.ChangeLevel does this (it returns early on everyone else).
/// Other players: the host's UpdateLevelRPC calls RunManager.UpdateLevel with the new count.
/// The two never both run on the same machine, so the coin can't be paid twice.
/// </summary>
[HarmonyPatch]
internal static class LevelCompletePatches
{
    internal struct Before
    {
        public bool InRunLevel;
        public int Completed;
    }

    static Before Capture()
    {
        var rm = RunManager.instance;
        return rm == null
            ? default
            : new Before { InRunLevel = SemiFunc.RunIsLevel(), Completed = rm.levelsCompleted };
    }

    static void CheckCompleted(Before before)
    {
        var rm = RunManager.instance;
        if (rm == null || !before.InRunLevel) return;
        if (rm.levelsCompleted == before.Completed + 1 && SemiFunc.RunIsShop())
            Plugin.GiveCoin(rm.levelsCompleted);
    }

    [HarmonyPatch(typeof(RunManager), nameof(RunManager.ChangeLevel))]
    [HarmonyPrefix]
    static void ChangeLevelPrefix(out Before __state) => __state = Capture();

    [HarmonyPatch(typeof(RunManager), nameof(RunManager.ChangeLevel))]
    [HarmonyPostfix]
    static void ChangeLevelPostfix(Before __state) => CheckCompleted(__state);

    [HarmonyPatch(typeof(RunManager), nameof(RunManager.UpdateLevel))]
    [HarmonyPrefix]
    static void UpdateLevelPrefix(out Before __state) => __state = Capture();

    [HarmonyPatch(typeof(RunManager), nameof(RunManager.UpdateLevel))]
    [HarmonyPostfix]
    static void UpdateLevelPostfix(Before __state, bool _gameOver)
    {
        if (!_gameOver) CheckCompleted(__state);
    }
}

internal class CoinMessage : MonoBehaviour
{
    static string message;
    static float until;

    internal static void Show(string text)
    {
        message = text;
        until = Time.unscaledTime + 8f; // long enough to outlast the level-change loading screen
    }

    void OnGUI()
    {
        if (message == null || Time.unscaledTime > until) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        var rect = new Rect(20, 20, 900, 40);
        GUI.color = Color.black;
        GUI.Label(new Rect(rect.x + 2, rect.y + 2, rect.width, rect.height), message, style);
        GUI.color = new Color(1f, 0.85f, 0.2f);
        GUI.Label(rect, message, style);
    }
}
