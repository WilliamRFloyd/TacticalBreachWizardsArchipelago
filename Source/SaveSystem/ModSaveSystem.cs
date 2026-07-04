using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.SaveSystem;
using TBWArch.Archipelago;
using TBWArch.Utils;
using System;
using System.Reflection;
using Wizards.Stages;
using Wizards.UI;
using System.Threading;

namespace TBWArch.SaveSystem
{
    [HarmonyPatch]
    internal class ModSaveSystem
    {
        private static readonly List<string> allStages = [
            "Text_Prologue_Location_Card",
            "Game_Prologue",
            "Intro_Title_Logo_Screen",
            "ChapterScreen_1",
            "Text_Rushwater_PD_Location_Card",
            "Dlog_WitchStatementPI",
            "Dlog_WitchAndChiefCoda",
            "Game_Witch_Intro",
            "Text_Rushwater_Cells_Fading_Title_Card",
            "Dlog_WitchMeetsNecro",
            "Mission_End_Screen_Witch_Intro",
            "Dlog_SubwayDebrief",
            "Perk_Witch_Intro",
            "Text_Ma's_Location_Card",
            "Dlog_WitchAndSeerAtMas",
            "Map_MapStageWitch1",
            "Dlog_MasPostMap",
            "Text_Evidence_Lockup_Location_Card",
            "Game_Evidence_Lockup",
            "Dlog_ArchivesFoundFile",
            "Dlog_Morgue",
            "Dlog_Archives_Boss_Chat",
            "Game_Evidence_Boss",
            "Mission_End_Screen_Archives",
            "Perk_Evidence_Boss",
            "Dlog_SubwayArchivesDebrief",
            "Text_Ma's_Location_Card_2",
            "Dlog_WitchMeetsMa",
            "Map_MapStageWitch2",
            "Dlog_WitchMeetsMaCoda",
            "ChapterScreen_2",
            "Text_Liboli_Ferry_Location_Card",
            "Dlog_FerryBrief",
            "Dlog_FerryBriefCoda",
            "Chat_MemorialFlashback",
            "Text_Dream_Intro_fading_title_card",
            "Game_Dream_Intro",
            "SdMsn_Dream2A",
            "Text_Blacksite_Location_Card",
            "Game_Liboli_Intro",
            "Dlog_BlacksiteCell",
            "Text_Liboli_Bar_Night_Fading_Title_Card",
            "Dlog_NecroBar",
            "Mission_End_Screen_Liboli_Intro",
            "SdMsn_Dream2B",
            "Perk_Liboli_Intro",
            "Dlog_NecroBarCoda",
            "Text_Liboli_Bar_Morning_Fading_Title_Card_Variant",
            "Dlog_NecroIntroBrief",
            "Text_Blacksite_Armoury_Location_Card",
            "Game_Necro_Intro",
            "Mission_End_Screen_Necro_Intro",
            "Perk_Necro_Intro",
            "Text_Liboli_Bar_Location_Card",
            "Dlog_MeetingBriefing",
            "Text_Blacksite_Meet_Location_Card",
            "Game_The_Meet",
            "Dlog_TheMeet",
            "Mission_End_Screen_The_Meet",
            "Perk_The_Meet",
            "SdMsn_Dream2C",
            "Map_MapStageLiboli1",
            "Dlog_PyroBriefing",
            "Text_Blacksite_Exit_Location_Card",
            "Game_Exit_Strategy",
            "Dlog_PyroAndWarlock",
            "Game_The_Pyromancer",
            "Mission_End_Screen_The_Pyromancer",
            "Perk_The_Pyromancer",
            "Text_Liboli_Bar_Again_Location_Card",
            "Dlog_Act2Debrief",
            "Text_Jen_Dream_fading_title_card",
            "Game_Lucid_Dream_Jen",
            "Unlock_ChainShockSuperchain",
            "SdMsn_Dream2D",
            "ChapterScreen_3",
            "Dlog_TrainBrief",
            "Text_Kalan_train_Location_Card",
            "Game_Train",
            "Mission_End_Screen_Kalan_Arrival",
            "Text_Kalan_Streets_Location_Card",
            "Game_Streets",
            "Mission_End_Screen_Kalan_Streets",
            "Text_Kalan_Underground_fading_title_card",
            "Dlog_StreetsDebrief",
            "Perk_Streets",
            "Dlog_TheRecordingBrief1",
            "Dlog_TheRecordingBrief2",
            "Dlog_TheRecordingBrief3",
            "Dlog_TheRecordingBrief4",
            "Text_The_Recording_Mission_Location_Card",
            "Game_The_Recording",
            "Mission_End_Screen_The_Recording",
            "Perk_The_Recording",
            "Text_The_Recording_itself_Location_Card",
            "Dlog_TheRecordingItself",
            "Dlog_TheRecordingDebrief",
            "Dlog_TheRecordingCheers",
            "Chat_BanksKalan",
            "SdMsn_Dream3B",
            "Map_MapStageKalan1",
            "ChapterScreen_4",
            "Text_Broadcast_Briefing_Location_Card",
            "Dlog_BroadcastBrief",
            "Text_Broadcast_Location_Card",
            "Dlog_BroadcastTeamsMeet",
            "Dlog_BroadcastTeamsPairOff",
            "Text_Broadcast_fading_title_card",
            "Game_The_Broadcast",
            "Dlog_BroadcastBeforeEavesdrop",
            "Text_Eavesdrop_Fading_Title_Card",
            "Dlog_BroadcastJenEavesdrops",
            "Text_After_Eavesdrop_Fading_Title_Card",
            "Game_Siege_Cleric",
            "Mission_End_Screen_The_Broadcast",
            "Perk_Siege_Cleric",
            "Dlog_BroadcastDebrief",
            "SdMsn_Dream3C",
            "Dlog_ZanDecidesToMeetLiv",
            "Dlog_ZanMeetsLiv",
            "Game_Flashback",
            "Dlog_ZanMeetsLivAfterFlashback",
            "Text_Ambush_Location_Card",
            "Game_Kalan_Ambush",
            "Dlog_AmbushInterlude",
            "Game_Kalan_Ambush_2",
            "Text_Banks_Dream_fading_title_card",
            "Game_Lucid_Dream_Banks",
            "Unlock_DeathsFloor",
            "Dlog_BanksEmerges",
            "Perk_Lucid_Dream_Banks",
            "Text_Jen_Solves_Sickness_Location_Card",
            "Dlog_JenSolvesSickness",
            "Map_MapStageKalan2",
            "Dlog_Act3Debrief",
            "Act_3_Beta_End_Screen",
            "ChapterScreen_5",
            "Text_Medil_Boat_Location_Card",
            "Dlog_1BoatToMedil",
            "Text_Medil_TRAIN_Location_Card",
            "Dlog_1MeetMilitia",
            "Dlog_1TwoTrainsBrief",
            "Game_Two_Trains",
            "Mission_End_Screen_Two_Trains",
            "Dlog_1TwoTrainsDebrief",
            "Perk_Two_Trains",
            "Text_Dall_Dream_fading_title_card",
            "Game_Lucid_Dream_Dall",
            "Unlock_SwapWithoutLOS",
            "Text_Medil_OSPREY_location_card",
            "Dlog_2OspreyBrief",
            "Game_Fort_Osprey",
            "Mission_End_Screen_Fort_Osprey",
            "Perk_Fort_Osprey",
            "Dlog_2OspreyDebrief",
            "Map_MapStageMedil1",
            "Text_Rion_Dream_fading_title_card",
            "Game_Lucid_Dream_Rion",
            "Unlock_SporeIntelligent",
            "Chat_RionDog",
            "Text_Medil_VILLA_location_card",
            "Dlog_3VillaBrief",
            "Game_Villa_Medil",
            "Mission_End_Screen_Villa_Medil",
            "Perk_Villa_Medil",
            "Text_Zan_Dream_fading_title_card",
            "Game_Lucid_Dream_Zan",
            "Unlock_SeerFinaleKnockBackDummy",
            "Chat_ZanDreamDebrief",
            "Text_Medil_VILLA_planning_location_card",
            "Dlog_4FinaleBrief",
            "Perk_Lucid_Dream_Zan",
            "Text_Medil_MINES_location_card",
            "Finale_Overview_Vault",
            "Game_Finale_Vault",
            "Finale_Overview_Mines",
            "Game_Finale_Mines",
            "Finale_Overview_Roof",
            "Game_Finale_Roof",
            "Dlog_4FinaleBanksShoots",
            "ChapterScreen_6",
            "Dlog_4FinaleDebrief",
            "Credits_Roll",
            "Dlog_5Ending2JenBanks",
            "Map_MapCredits1",
            "Credits_Roll_2",
            "Dlog_5Ending3DallRion",
            "Map_MapCredits2",
            "Credits_Roll_3",
            "Dlog_5Ending5ZanLivDead",
            "Dlog_5Ending5ZanLivAlive",
            "Map_MapCredits3",
            "Credits_Roll_4",
            "Dlog_5Ending4Steve",
            "Credits_Roll_5",
            "SdMsn_3ProvingGroundsUnlock"
        ];

        private static readonly List<string> allWantedUnlockables = [
            "Mission_Select_Panel",
            "MissionDream2A",
            "MissionDream2B",
            "MissionDream2C",
            "MissionDream2D",
            "MissionDream3B",
            "MissionDream3C",
            "Mission3ProvingGroundsMissionUnlock",
        ];

        public static void AddAllStages()
        {
            HashSet<string> allStagesSet = [.. allStages];
            StageManager stageManager = Managers.Stage;
            stageManager.ResetProgress();
            Traverse.Create(stageManager).Field("completedStageIDs").SetValue(allStagesSet);
            Managers.Save.SaveProgressData();
        }

        public static void AddDreamsPanel()
        {
            Managers.Save.LoadProgressData();
            foreach (string unlockableName in allWantedUnlockables)
            {
                Unlockable unlock = Managers.Progress.GetUnlockableByName(unlockableName);
                if (unlock != null)
                {
                    unlock.unlocked = true;
                }
                else
                {
                    ArchipelagoConsole.LogMessage($"No unlockable {unlockableName}");
                }
            }
            Managers.Save.SaveProgressData();
        }

        [HarmonyPatch(typeof(PlayPanel), "UpdatePanel")]
        [HarmonyPrefix]
        public static void UpdatePanelPatch(PlayPanel __instance)
        {
            if (Managers.Stage.NumberOfCompletedStages < allStages.Count)
            {
                AddAllStages();
            }
            if (!Managers.Progress.GetUnlockableByName("MissionAct4Tests").unlocked)
            {
                AddDreamsPanel();
            }
        }

        [HarmonyPatch(typeof(SaveManager), "Awake")]
        [HarmonyPostfix]
        public static void AddSaveComponentsPatch(SaveManager __instance)
        {
            if (__instance.GetComponent<ConnectionManager>() == null)
            {
                __instance.gameObject.AddComponent<ConnectionManager>();
            }

            if (__instance.GetComponent<SaveArchipelagoBehavior>() == null)
            {
                __instance.gameObject.AddComponent<SaveArchipelagoBehavior>();
            }

            if (__instance.GetComponent<LevelSaveManager>() == null)
            {
                __instance.gameObject.AddComponent<LevelSaveManager>();
            }

            if (__instance.GetComponent<MissionUnlockManager>() == null)
            {
                __instance.gameObject.AddComponent<MissionUnlockManager>();
            }
        }

        public static string GetArchipelagoSaveSlotDirectory()
        {
            return SaveManager.saveDirectory + "Archipelago/";
        }

        [HarmonyPatch(typeof(SaveManager), "GetSaveSlotDirectory", new Type[] {typeof(int)})]
        [HarmonyPostfix]
        public static void SaveSlotPatch(SaveManager __instance, int _slot, ref string __result)
        {
            __result = GetArchipelagoSaveSlotDirectory();
        }


        [HarmonyPatch(typeof(SaveManager), "LoadInitialData")]
        [HarmonyPostfix]
        public static void LoadInitialDataPatch(SaveManager __instance)
        {
            if (!__instance.LoadArchipelagoData())
            {
                __instance.SaveArchipelagoData();
            }
        }

        [HarmonyPatch(typeof(MainMenuPanel), "OnQuitClick")]
        [HarmonyPrefix]
        public static void OnQuitClickPatch()
        {
            if (ArchipelagoClient.Instance != null)
            {
                Plugin.BepinLogger.LogError("Disconnecting from Archipelago server on quit...");
                ArchipelagoClient.Instance.Disconnect();
            }
        }
    }
}