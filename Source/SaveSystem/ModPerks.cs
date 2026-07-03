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
    public class JunkPerk : CharacterPerk
    {
        public string saveName;

        public string targetAbilityClassName;
        public override CharacterNames ApplicableCharacter { get {return CharacterNames.None;} }

		public override string SaveName
		{
			get
			{
				return this.saveName;
			}
		}

        protected sealed override void ApplyPerk(Person _user)
        {
            return;
        }
    }

    [HarmonyPatch]
    internal class ModPerks
    {
        public static bool perksAdded = false;
        public static bool temp = false;

        public static Dictionary<string, string> abilityToUnlockPerk = new Dictionary<string, string>
        {
            {"SeerShot", "UnlockArcaneBurst"},

            {"WandShot", "UnlockWitchBolt"},

            {"ThrowInstantGrenade", "UnlockSedativeGrenade"},
            {"Resurrect", "UnlockResurrect"},
            {"DeathsDoor", "UnlockDeathsDoor"},

            {"Charge", "UnlockCharge"},
            {"CenserSmash", "UnlockCenserSmash"},
            {"Swap", "UnlockSwap"},
            {"ThrowCover", "UnlockThrowCover"},

            {"DogPounce", "UnlockRabidBite"},
            {"PullAttack", "UnlockPullAttack"},
            {"DruidAcidDart", "UnlockDruidAcidDart"},
            {"StealMana", "UnlockStealMana"},
        };

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(PerkManager), "GetAcquiredPerks")]
        public static HashSet<string> CallGetAcquiredPerks(PerkManager instance, bool _temp)
        {
            throw new NotImplementedException("Failed to patch");
        }

        [HarmonyPatch(typeof(PerkManager), "GetCharacterAbilities")]
        [HarmonyPostfix]   
        internal static void GetCharacterAbilitiesPatch(PerkManager __instance, ref IEnumerable<AbilitySO> __result, ref Dictionary<string, CharacterPerk> ___saveNameMap)
        {
            //AddNewUnlockPerks(__instance, ref ___saveNameMap);
            foreach (AbilitySO ability in __result)
            {
                if (abilityToUnlockPerk.ContainsKey(ability.AbilityClassName))
                {
                    CharacterPerk perk = __instance.GetByName(abilityToUnlockPerk[ability.AbilityClassName]);
                    if (!__instance.IsAcquired(perk))
                    {
                        __result = __result.Where(a => a != ability);
                    }
                }
                //ArchipelagoConsole.LogMessage(ability.AbilityClassName);
            }
        }

        [HarmonyPatch(typeof(Person), "CreateAbilities")]
        [HarmonyPostfix]
        internal static void CreateAbilitiesPatch(Person __instance)
        {
            List<AbilitySO> abilitiesToRemove = new List<AbilitySO>();
            foreach (AbilitySO ability in __instance.abilityInstanceList)
            {
                if (abilityToUnlockPerk.ContainsKey(ability.AbilityClassName))
                {
                    PerkManager perkManager = Managers.Perks;
                    CharacterPerk perk = perkManager.GetByName(abilityToUnlockPerk[ability.AbilityClassName]);
                    if (!perkManager.IsAcquired(perk))
                    {
                        abilitiesToRemove.Add(ability);
                    }
                }
            }

            foreach (AbilitySO ability in abilitiesToRemove)
            {
                __instance.RemoveAbility(ability);
            }
        }

        [HarmonyPatch(typeof(PerkManager), "GetByName")]
        [HarmonyPrefix]   
        internal static void GetByNamePatch(PerkManager __instance, ref Dictionary<string, CharacterPerk> ___saveNameMap)
        {
            //AddNewUnlockPerks(__instance, ref ___saveNameMap);
        }

        /*[HarmonyPatch(typeof(PerkManager), "GetCharacterAbilities")]
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
                temp = true;
            }
        }*/

        [HarmonyPatch(typeof(PerkManager), "Awake")]
        [HarmonyPostfix]
        internal static void ReadyPerkAdd(PerkManager __instance, ref Dictionary<string, CharacterPerk> ___saveNameMap)
        {
            foreach (string abilityName in abilityToUnlockPerk.Keys)
            {
                JunkPerk perk = ScriptableObject.CreateInstance<JunkPerk>();
                string saveName = abilityToUnlockPerk[abilityName];
                perk.saveName = saveName;
                perk.targetAbilityClassName = abilityName;
                ___saveNameMap.Add(saveName, perk);
            }
        }
    }
}