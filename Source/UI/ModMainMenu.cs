using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.SaveSystem;
using Wizards.UI;
using TBWArch.Utils;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TBWArch.UI
{
    [HarmonyPatch]
    internal class ModMainMenu
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(MainMenuScenePanel), "OnChangeSaveSlot")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void CallOnChangeSaveSlot()
        {
            throw new NotImplementedException("Failed to patch");
        }

        [HarmonyPatch(typeof(MainMenuScenePanel), "Update")]
        [HarmonyPostfix]
        public static void MainMenuScenePanelPatch(MainMenuScenePanel __instance)
        {            
            __instance.playButton.SetLabel("Connect");
            //__instance.changeSaveSlotButton.SetButtonEnabled(false);
            //__instance.playButton.AssignAction(new MainMenuButton.OnMenuButtonClicked(ModMainMenu.CallOnChangeSaveSlot)); 
        }

        /*[HarmonyPatch(typeof(PerksPanel), "UpdatePanel")]
        [HarmonyPostfix]
        public static void PerksPanelPatch(PerksPanel __instance)
        {
            __instance.perkCommitButton.myText.text = "Test";
        }*/

        /*[HarmonyPatch(typeof(PerkManager), "LoadData", new Type[] {typeof(DataBlock)})]
        [HarmonyPrefix]
        public static void PerkManagerPatch(PerkManager __instance, DataBlock _data)
        {
            ArchipelagoConsole.LogMessage($"Perk Manager Loading Data");
			if (_data.ContainsKey("acquiredPerk"))
			{
                ArchipelagoConsole.LogMessage($"Has acquiredPerk");
				foreach (string item2 in _data.FindList("acquiredPerk"))
				{
					ArchipelagoConsole.LogMessage(item2);
				}
			}
            ArchipelagoConsole.LogMessage($"Finished Checking");
        }*/

        /*[HarmonyPatch(typeof(PerkManager), "Awake")]
        [HarmonyPrefix]
        public static void PerkManagerPatch(PerkManager __instance)
        {
            ArchipelagoConsole.LogMessage($"Check perkList");
	        foreach (CharacterPerk characterPerk in __instance.perkList.perks)
			{
                ArchipelagoConsole.LogMessage($"{characterPerk.SaveName}: {characterPerk.displayName}");
			}
            ArchipelagoConsole.LogMessage($"Finished Checking");
        }*/

        [HarmonyPatch(typeof(PerkPurchaseButton), "PerkIsStoryPerk", MethodType.Getter)]
        [HarmonyPrefix]
        static bool HideInnatePerkMessagePatch(ref bool __result)
        {
			__result = false;
            return false;
        }

        [HarmonyPatch(typeof(PerkPurchaseButton), "PerkConfigAllowsRefund", MethodType.Getter)]
        [HarmonyPrefix]
        static bool DisallowRefundsPatch(ref bool __result)
        {
			__result = false;
            return false;
        }
    }
}