using HarmonyLib;
using Wizards.Steam;

namespace TBWArch.Misc
{
    [HarmonyPatch]
    internal class ModAchievements
    {
        [HarmonyPatch(typeof(SteamManager), "SetAchievement")]
        [HarmonyPrefix]
        public static void DisableAchievementsPatch()
        {
            return;
        }
    }
}