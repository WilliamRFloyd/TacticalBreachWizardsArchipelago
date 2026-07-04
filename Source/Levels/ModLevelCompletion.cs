using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.SaveSystem;
using Wizards.UI;
using TBWArch.Utils;
using TBWArch.SaveSystem;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wizards.LevelBuilding;
using TBWArch.Archipelago;

namespace TBWArch.Levels
{
    [HarmonyPatch]
    internal class ModLevelCompletion
    {
        [HarmonyPatch(typeof(LevelManager), "MarkLevelComplete")]
        [HarmonyPrefix]
        public static void MarkLevelCompletePatch(LevelManager __instance)
        {
            string saveName = __instance.currentFile.GetSaveDataString();
            ArchipelagoConsole.LogMessage(saveName);
            Managers.Save.LoadArchipelagoData();

            LevelSaveManager levelSaveManager = SaveArchipelagoBehavior.levelSaveManager;
            if (!levelSaveManager.completedLevels.Contains(saveName))
            {
                levelSaveManager.completedLevels.Add(saveName);
                ArchipelagoClient.Instance.AddLocationByName(saveName);
                Managers.Save.SaveArchipelagoData();
            }
        }
    }
}