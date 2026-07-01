using System;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using TBWArch.Utils;
using TBWArch.SaveSystem;
using Wizards.SaveSystem;
using Wizards.Perks;
using System.Threading.Tasks;

namespace TBWArch.Archipelago;

public class ArchipelagoClient
{
    public static readonly Version APVersion = new(0, 6, 8);
    private const string Game = "Tactical Breach Wizards";

    public static bool Authenticated;
    private bool attemptingConnection;

    public static ArchipelagoData ServerData = new();
    private DeathLinkHandler DeathLinkHandler;
    private static ArchipelagoSession session;

    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public async Task Connect()
    {
        if (Authenticated || attemptingConnection) return;

        ConnectionManager connectionManager = SaveArchipelagoBehavior.connectionManager;

        if (connectionManager != null)
        {
            connectionManager.serverName = ServerData.Uri;
            connectionManager.slotName = ServerData.SlotName;
            Managers.Save.SaveArchipelagoData();
        }


        try
        {
            session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
            SetupSession();
        }
        catch (Exception e)
        {
            ArchipelagoConsole.LogMessage($"Failed to create session for {ServerData.Uri} as {ServerData.SlotName}. Exception: {e}");
            Plugin.BepinLogger.LogError(e);
        }

        await TryConnect();
    }

    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private void SetupSession()
    {
        session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message.ToString());
        session.Items.ItemReceived += OnItemReceived;
        session.Socket.ErrorReceived += OnSessionErrorReceived;
        session.Socket.SocketClosed += OnSessionSocketClosed;
    }

    /// <summary>
    /// attempt to connect to the server with our connection info
    /// </summary>
    private async Task TryConnect()
    {
        try
        {
            await session.ConnectAsync();
        }
        catch (Exception e)
        {
            ArchipelagoConsole.LogMessage($"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}. Exception: {e}");
            Plugin.BepinLogger.LogError(e);
            attemptingConnection = false;
            Disconnect();
            return;
        }

        var result = await session.LoginAsync(
            Game,
            ServerData.SlotName,
            ItemsHandlingFlags.AllItems,
            APVersion,
            uuid: Guid.NewGuid().ToString(),
            password: ServerData.Password == "" ? null : ServerData.Password
        );

        HandleConnectResult(result);

        /*try
        {
            // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
            //ArchipelagoConsole.LogMessage($"Game: {Game}\nURI: {ServerData.Uri}\nSlot Name: {ServerData.SlotName}\nPassword: {ServerData.Password}");
            ThreadPool.QueueUserWorkItem(
                _ => HandleConnectResult(
                    session.TryConnectAndLogin(
                        Game,
                        ServerData.SlotName,
                        ItemsHandlingFlags.AllItems,
                        new Version(APVersion),
                        password: ServerData.Password == "" ? null : ServerData.Password,
                        requestSlotData: ServerData.NeedSlotData
                    )));
        }
        catch (Exception e)
        {
            Plugin.BepinLogger.LogError(e);
            HandleConnectResult(new LoginFailure(e.ToString()));
            attemptingConnection = false;
        }*/
    }

    /// <summary>
    /// handle the connection result and do things
    /// </summary>
    /// <param name="result"></param>
    private void HandleConnectResult(LoginResult result)
    {
        string outText;
        if (result.Successful)
        {
            var success = (LoginSuccessful)result;

            ServerData.SetupSession(success.SlotData, session.RoomState.Seed);
            Authenticated = true;

            DeathLinkHandler = new(session.CreateDeathLinkService(), ServerData.SlotName);
            session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
            
            ArchipelagoConsole.LogMessage(outText);
        }
        else
        {
            var failure = (LoginFailure)result;
            outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
            outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

            Plugin.BepinLogger.LogError(outText);

            Authenticated = false;
            Disconnect();
        }

        ArchipelagoConsole.LogMessage(outText);
        attemptingConnection = false;
    }

    /// <summary>
    /// something went wrong, or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    private void Disconnect()
    {
        Plugin.BepinLogger.LogDebug("disconnecting from server...");
        session?.Socket.DisconnectAsync();
        session = null;
        Authenticated = false;
    }

    public void SendMessage(string message)
    {
        session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private void OnItemReceived(ReceivedItemsHelper helper)
    {
        if (helper.Index <= ServerData.Index) return;

        ServerData.Index++;

        // TODO reward the item here
        // if items can be received while in an invalid state for actually handling them, they can be placed in a local
        // queue/collection to be handled later
        ItemInfo nextItem = helper.DequeueItem();
        ArchipelagoConsole.LogMessage($"Recieved item {nextItem.ItemDisplayName}");

        long baseId = 1000;

        switch(nextItem.ItemId)
        {
            case long id when id >= baseId + 10 && id <= baseId + 14:
                string unlockWizardName = ArchipelagoItems.ItemIdToUnlock[id];
                ArchipelagoConsole.LogMessage($"Recieved item {nextItem.ItemDisplayName} which is a wizard class with unlock name {unlockWizardName}. Unlocking it.");

                UnlockItemByName(unlockWizardName);
                break;

            case long id when id >= baseId + 50 && id <= baseId + 100:
                string unlockPerkName = ArchipelagoItems.ItemIdToUnlock[id];
                ArchipelagoConsole.LogMessage($"Recieved item {nextItem.ItemDisplayName} which is a perk with unlock name {unlockPerkName}. Unlocking it.");

                UnlockPerkByName(unlockPerkName);
                break;
        }
    }

    public static void SendChecks()
    {
        if (session == null || !Authenticated) return;
        session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());
    }

    /// <summary>
    /// something went wrong with our socket connection
    /// </summary>
    /// <param name="e">thrown exception from our socket</param>
    /// <param name="message">message received from the server</param>
    private void OnSessionErrorReceived(Exception e, string message)
    {
        ArchipelagoConsole.LogMessage($"Error received from Archipelago server: {message}. Exception: {e}");
        Plugin.BepinLogger.LogError(e);
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private void OnSessionSocketClosed(string reason)
    {
        Plugin.BepinLogger.LogError($"Connection to Archipelago lost: {reason}");
        Disconnect();
    }

    private void UnlockItemByName(string unlockName)
    {
        Unlockable unlock = Managers.Progress.GetUnlockableByName(unlockName);
        if (unlock != null)
        {
            Managers.Save.LoadProgressData();
            unlock.unlocked = true;
            Managers.Save.SaveProgressData();
        }
        else
        {
            Plugin.BepinLogger.LogError($"Failed to unlock perk {unlockName} because it was not found in the game's unlockables.");
        }
    }

    private void UnlockPerkByName(string perkName)
    {
        CharacterPerk perk = Managers.Perks.GetByName(perkName);
        if (perk != null)
        {
            Managers.Perks.AcquirePerk(perk);
            ArchipelagoConsole.LogMessage($"Unlocked perk {perkName}.");
        }
        else
        {
            ArchipelagoConsole.LogMessage($"Failed to unlock perk {perkName} because it was not found in the game's perks.");
        }
    }
}