using HarmonyLib;
using System.Linq;
using TBWArch.Utils;
using TBWArch.SaveSystem;
using Wizards.UI;
using Wizards.SaveSystem;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;

namespace TBWArch.UI
{
    [HarmonyPatch]
    internal class ModMissionSelection
    {
        [HarmonyPatch(typeof(ChapterLineBehaviour), "AppearInstant")]
        [HarmonyPostfix]
        public static void MissionSelectPanelPatch(ChapterLineBehaviour __instance)
        {
            foreach (MissionPolaroid polaroid in __instance.polaroids)
            {
                string folderName;
                if (polaroid.myStage == null) //Dream missions store their information differently
                {
                    folderName = polaroid.myDreamMission.folder;
                }
                else
                {
                    folderName = polaroid.myStage.StageData;
                }
                //ArchipelagoConsole.LogMessage(folderName);
                bool unlockedMission = !SaveArchipelagoBehavior.missionUnlockManager.unlockedMissions.Contains(folderName);
                if (unlockedMission)
                {
                    Traverse.Create(polaroid).Field("isShown").SetValue(false);
                    polaroid.actualPolaroid.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(ChapterSelectLevelLine), "SetLevel")]
        [HarmonyPostfix]
        public static void MarkLevelCompletedVisualPatch(ChapterSelectLevelLine __instance, ref LevelFile ___level)
        {
            string saveString = ___level.GetSaveDataString();
            if (SaveArchipelagoBehavior.levelSaveManager.completedLevels.Contains(saveString))
            {
                string originalText = __instance.playLevelButton.myText.text;
                __instance.playLevelButton.myText.text = $"<color=yellow>{originalText}</color>";
            }
        }
    }
}