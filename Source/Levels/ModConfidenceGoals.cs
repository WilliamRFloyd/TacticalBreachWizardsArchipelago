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
using Wizards.LevelBuilding;
using TBWArch.Archipelago;

namespace TBWArch.Levels
{
    [HarmonyPatch]
    internal class ModConfidenceGoals
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(ConfidencePointManager), "GetSaveString")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string CallGetSaveString(LevelFile _file, ConfidenceGoal _goal)
        {
            throw new NotImplementedException("Failed to patch");
        }

        [HarmonyPatch(typeof(ConfidencePointManager), "ProcessGoal", new Type[] {typeof(ConfidenceGoal)})]
        [HarmonyPrefix]
        public static void ProcessGoalPatch(ConfidencePointManager __instance, ref ConfidenceGoal _goal, ref HashSet<string> ___completedGoals)
        {
			if (!Managers.Level.confidenceGoals.Contains(_goal))
			{
				throw new ArgumentOutOfRangeException(string.Format("Trying to process Confidence Goal {0} not present in current level: {1}.  Cannot process goals without a level containing them.", _goal.goalName, Managers.Level.currentFile));
			}
			if (_goal.IsObjectiveComplete())
			{
				if (!_goal.PreviouslyCompleted())
				{
                    string saveName = CallGetSaveString(Managers.Level.currentFile, _goal);
					___completedGoals.Add(saveName);
                    ArchipelagoClient.Instance.AddLocationByName(saveName);
				}
			}
            
            return;
        }
    }
}