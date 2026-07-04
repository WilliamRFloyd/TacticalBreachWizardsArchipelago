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
        /*[HarmonyPatch(typeof(ChapterSelectPanel), "OnActivatePanel")]
        [HarmonyPostfix]
        public static void MissionSelectPanelPatch(ChapterSelectPanel __instance)
        {
            foreach (ChapterLineBehaviour chapter in __instance.chapterLines)
            {
                foreach (MissionPolaroid polaroid in chapter.polaroids)
                {
                    string folderName = polaroid.myStage.StageData;
                    if (!SaveArchipelagoBehavior.missionUnlockManager.unlockedMissions.Contains(folderName))
                    {
                        Traverse.Create(polaroid).Field("isShown").SetValue(false);
                        GameObject actualPolaroid = polaroid.actualPolaroid;
                        polaroid.actualPolaroid.SetActive(false);
                    }
                }
            }
        }*/

        [HarmonyPatch(typeof(ChapterLineBehaviour), "AppearInstant")]
        [HarmonyPostfix]
        public static void MissionSelectPanelPatch(ChapterLineBehaviour __instance)
        {
            foreach (MissionPolaroid polaroid in __instance.polaroids)
            {
                string folderName;
                ArchipelagoConsole.LogMessage("Test");
                if (polaroid.myStage == null) //Dream missions store their information differently
                {
                    folderName = polaroid.myDreamMission.folder;
                }
                else
                {
                    folderName = polaroid.myStage.StageData;
                }
                ArchipelagoConsole.LogMessage(folderName);
                if (!SaveArchipelagoBehavior.missionUnlockManager.unlockedMissions.Contains(folderName))
                {
                    Traverse.Create(polaroid).Field("isShown").SetValue(false);
                    polaroid.actualPolaroid.SetActive(false);
                }
            }
        }
    }
}