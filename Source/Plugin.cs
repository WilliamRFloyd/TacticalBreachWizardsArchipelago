using BepInEx;
using BepInEx.Logging;
using TBWArch.Archipelago;
using TBWArch.Utils;
using TBWArch.UI;
using UnityEngine;
using HarmonyLib;
using Wizards.SaveSystem;
using System.Threading.Tasks;

namespace TBWArch;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInProcess("Tactical Breach Wizards.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGUID = "TBW.Bluedetroyer.Archipelago";
    public const string PluginName = "TBWArch";
    public const string PluginVersion = "0.0.1";

    public const string ModDisplayInfo = $"{PluginName} v{PluginVersion}";
    private string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
    public static ManualLogSource BepinLogger;
    public static ArchipelagoClient ArchipelagoClient;

    public Harmony harmony { get; } = new(PluginGUID);

    private void Awake()
    {
        // Plugin startup logic
        BepinLogger = Logger;
        ArchipelagoClient archipelagoClient = new ArchipelagoClient();
        ArchipelagoConsole.Awake();
        ArchipelagoClient.Instance = archipelagoClient;

        harmony.PatchAll();

        ArchipelagoConsole.LogMessage($"{ModDisplayInfo} loaded!");
    }

    private async Task OnGUI()
    {
        // show the mod is currently loaded in the corner
        GUI.Label(new Rect(16, 16, 300, 20), ModDisplayInfo);
        ArchipelagoConsole.OnGUI();

        string statusMessage;
        // show the Archipelago Version and whether we're connected or not
        if (ArchipelagoClient.Authenticated)
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = false;

            statusMessage = " Status: Connected";
            GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
        }
        else
        {
            // if your game doesn't usually show the cursor this line may be necessary
            // Cursor.visible = true;

            statusMessage = " Status: Disconnected";
            GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
            GUI.Label(new Rect(16, 70, 150, 20), "Host: ");
            GUI.Label(new Rect(16, 90, 150, 20), "Player Name: ");
            GUI.Label(new Rect(16, 110, 150, 20), "Password: ");

            ArchipelagoClient.ServerData.Uri = GUI.TextField(new Rect(150, 70, 150, 20),
                ArchipelagoClient.ServerData.Uri);
            ArchipelagoClient.ServerData.SlotName = GUI.TextField(new Rect(150, 90, 150, 20),
                ArchipelagoClient.ServerData.SlotName);
            ArchipelagoClient.ServerData.Password = GUI.TextField(new Rect(150, 110, 150, 20),
                ArchipelagoClient.ServerData.Password);

            // requires that the player at least puts *something* in the slot name
            if (GUI.Button(new Rect(16, 130, 100, 20), "Connect") &&
                !ArchipelagoClient.ServerData.SlotName.IsNullOrWhiteSpace())
            {
                await ArchipelagoClient.Instance.Connect();
            }
        }
        // this is a good place to create and add a bunch of debug buttons
    }

    private void OnDestroy()
    {
        harmony.UnpatchSelf();
         ArchipelagoConsole.LogMessage($"{ModDisplayInfo} unloaded!");
    }
}