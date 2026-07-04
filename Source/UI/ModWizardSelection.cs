using HarmonyLib;
using System.Linq;
using Wizards.UI;

namespace TBWArch.UI
{
    [HarmonyPatch]
    internal class ModWizardSelection
    {
        [HarmonyPatch(typeof(MissionCharacterSelectPanel), "OnActivatePanel")]
        [HarmonyPostfix]
        public static void MissionDisableDefaultPatch(MissionCharacterSelectPanel __instance)
        {
            __instance.DefaultButton.SetButtonEnabled(false);
        }

        [HarmonyPatch(typeof(MissionCharacterSelectSlot), "Initialize")]
        [HarmonyPostfix]
        public static void MissionSetDefaultToOwnedPatch(MissionCharacterSelectSlot __instance)
        {
            __instance.selectedCharacter = Managers.Characters.UnlockedCharacterList.FirstOrDefault();
        }

    }
}