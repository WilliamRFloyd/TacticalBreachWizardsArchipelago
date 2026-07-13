using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TBWArch.Utils;

namespace TBWArch.Archipelago;

public class ArchipelagoData
{
    public string Uri;
    public string SlotName;
    public string Password;
    public int Index;

    public int RequiredConfidence;

    public string GoalMission;

    public List<long> CheckedLocations;

    private const int TOTAL_CONFIDENCE = 232;

    private readonly List<string> OPTION_TO_MISSION_NAME = ["Finale - Roof", "Dream Heatsig", "Proving Ground 4"];

    /// <summary>
    /// seed for this archipelago data. Can be used when loading a file to verify the session the player is trying to
    /// load is valid to the room it's connecting to.
    /// </summary>
    private string seed;

    private Dictionary<string, object> slotData;

    public bool NeedSlotData => slotData == null;

    public ArchipelagoData()
    {
        Uri = "localhost";
        SlotName = "Player1";
        CheckedLocations = new();
    }

    public ArchipelagoData(string uri, string slotName, string password)
    {
        Uri = uri;
        SlotName = slotName;
        Password = password;
        CheckedLocations = new();
    }

    /// <summary>
    /// assigns the slot data and seed to our data handler. any necessary setup using this data can be done here.
    /// </summary>
    /// <param name="roomSlotData">slot data of your slot from the room</param>
    /// <param name="roomSeed">seed name of this session</param>
    public void SetupSession(Dictionary<string, object> roomSlotData, string roomSeed)
    {
        slotData = roomSlotData;
        seed = roomSeed;

        object value;

        float confidencePercentage = Convert.ToSingle(slotData.TryGetValue("confidence_required_percentage", out value) ? value : 75) / 100;
        int missionNum = Convert.ToInt32(slotData.TryGetValue("goal_mission", out value) ? value : 0);

        RequiredConfidence = (int) (confidencePercentage * TOTAL_CONFIDENCE);
        GoalMission = OPTION_TO_MISSION_NAME[missionNum];
        
    }

    /// <summary>
    /// returns the object as a json string to be written to a file which you can then load
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}