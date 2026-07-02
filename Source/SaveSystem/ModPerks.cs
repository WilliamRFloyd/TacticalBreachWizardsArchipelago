using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Wizards.Perks;
using Wizards.People;
using Wizards.SaveSystem;
using TBWArch.Utils;
using System;
using System.Reflection;
using System.IO;
using Wizards.Stages;
using System.Runtime.CompilerServices;
using TBWArch.Archipelago;

namespace TBWArch.SaveSystem
{
    [HarmonyPatch]
    internal class ModPerks
    {
        public static bool perksAdded = false;
        public static bool temp = false;

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PerkManager), "GetAcquiredPerks")]
        public static HashSet<string> CallGetAcquiredPerks(PerkManager instance, bool _temp)
        {
            throw new NotImplementedException("Failed to patch");
        }

        public static List<ProgressAddAbilityPerk> GetNewUnlockPerks()
        {
            try
            {
                List<AbilitySO> druidAbilities = Managers.Characters.GetClassData(CharacterNames.Druid).abilityList;
                druidAbilities.RemoveAll(a => a is DruidAcidDart);

                ProgressAddAbilityPerk unlockDruidAcidDart = ScriptableObject.CreateInstance<ProgressAddAbilityPerk>();
                unlockDruidAcidDart.saveName = "UnlockDruidAcidDart";
                unlockDruidAcidDart.applicableCharacter = CharacterNames.Druid;
                unlockDruidAcidDart.abilityMaster = Dev.FindObjectsOfType<DruidAcidDart>().FirstOrDefault();
                //ArchipelagoConsole.LogMessage($"Adding new perk: {unlockDruidAcidDart.saveName} for ability: {unlockDruidAcidDart.abilityMaster.AbilityClassName}");

                return new List<ProgressAddAbilityPerk> { unlockDruidAcidDart };
            }
            catch (Exception e)
            {
                //ArchipelagoConsole.LogMessage($"Failed to create new unlock perks. Exception: {e}");
                Plugin.BepinLogger.LogError(e);
                return new List<ProgressAddAbilityPerk>();
            }

        }

        [HarmonyPatch(typeof(PerkManager), "GetByCharacter", new Type[] {typeof(CharacterNames)})]
        [HarmonyPrefix]   
        internal static void GetByCharacterPatch(PerkManager __instance, ref CharacterNames _character, ref Dictionary<string, CharacterPerk> ___saveNameMap)
        {
            if (!perksAdded)
            {
                foreach (var perk in GetNewUnlockPerks())
                {
                    if (!___saveNameMap.ContainsKey(perk.saveName))
                    {
                        ___saveNameMap.Add(perk.saveName, perk);
                    }
                }
                perksAdded = true;
            }
        }

        [HarmonyPatch(typeof(PerkManager), "GetCharacterAbilities")]
        [HarmonyPostfix]
        internal static void GetCharacterAbilitiesPatch(PerkManager __instance, ref CharacterNames _character, ref Dictionary<string, CharacterPerk> ___saveNameMap, ref IEnumerable<AbilitySO> __result)
        {
            if (!temp)
            {
                ArchipelagoConsole.LogMessage(_character.ToString());
                foreach (string key in ___saveNameMap.Keys)
                {
                    ArchipelagoConsole.LogMessage($"PerkManager has perk: {key}");
                    if (___saveNameMap[key] is ProgressAddAbilityPerk abilityPerk)
                    {
                        ArchipelagoConsole.LogMessage($"PerkManager has ability perk: {abilityPerk.saveName} for ability: {abilityPerk.abilityMaster.AbilityClassName}");
                    }
                }

                foreach (AbilitySO ability in __result)
                {
                    ArchipelagoConsole.LogMessage($"PerkManager has ability: {ability.AbilityClassName}");
                }
                temp = true;

                /*IEnumerable<CharacterPerk> source = __instance.GetByCharacter(_character);
                HashSet<AbilitySO> hashSet = new HashSet<AbilitySO>(from _ability in Managers.Characters.GetClassData(_character).abilityList
                where !(_ability is TakeCover)
                select _ability);
                hashSet.UnionWith((from _perk in (from _perk in source
                select _perk as ProgressAddAbilityPerk).ExcludeNull<ProgressAddAbilityPerk>()
                select _perk.abilityMaster).Distinct<AbilitySO>());

                foreach (AbilitySO ability in hashSet)
                {
                    ArchipelagoConsole.LogMessage($"hashSet has ability: {ability.AbilityClassName}");
                }*/
            }
        }

        [HarmonyPatch(typeof(PerkManager), "Awake")]
        [HarmonyPostfix]
        internal static void AddNewUnlockPerks(PerkManager __instance, ref Dictionary<string, CharacterPerk> ___saveNameMap)
        {
            perksAdded = false;
        }
    }
}