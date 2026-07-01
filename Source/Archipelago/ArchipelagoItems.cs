using System.Collections.Generic;
using Newtonsoft.Json;

namespace TBWArch.Archipelago;

public class ArchipelagoItems
{
    const long BASE_ID = 1000;
    public static Dictionary<long, string> ItemIdToUnlock = new Dictionary<long, string>
    {
        {BASE_ID + 10, "NavySeer"},
        {BASE_ID + 11, "WitchCop"},
        {BASE_ID + 12, "NecroMedic"},
        {BASE_ID + 13, "RiotPriest"},
        {BASE_ID + 14, "Druid"},
    };
}