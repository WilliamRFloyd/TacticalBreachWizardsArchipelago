using System.Collections.Generic;
using Newtonsoft.Json;

namespace TBWArch.Archipelago;

public class ArchipelagoItems
{
    const long BASE_ID = 1000;
    public static Dictionary<long, string> ItemIdToUnlock = new Dictionary<long, string>
    {
        //Wizards
        {BASE_ID + 10, "NavySeer"},
        {BASE_ID + 11, "WitchCop"},
        {BASE_ID + 12, "NecroMedic"},
        {BASE_ID + 13, "RiotPriest"},
        {BASE_ID + 14, "Druid"},

        //Ability unlocks (which are technically perks too but whatever)
        {BASE_ID + 20, "UnlockArcaneBurst"},
        {BASE_ID + 21, "UnlockTimeBoost"},
        {BASE_ID + 22, "UnlockFalseProphet"},
        {BASE_ID + 23, "seerOverwatch"}, //I'm still very upset this ability specifically doesn't have the same unlock name.
        {BASE_ID + 24, "UnlockWitchBolt"},
        {BASE_ID + 25, "UnlockChainShock"},
        {BASE_ID + 26, "UnlockBroomBreach"},
        {BASE_ID + 27, "UnlockGaleGrenade"},
        {BASE_ID + 28, "UnlockSedativeGrenade"},
        {BASE_ID + 29, "UnlockGhostShot"},
        {BASE_ID + 30, "UnlockResurrect"},
        {BASE_ID + 31, "UnlockDeathsDoor"},
        {BASE_ID + 32, "UnlockTransference"},
        {BASE_ID + 33, "UnlockCharge"},
        {BASE_ID + 34, "UnlockCenserSmash"},
        {BASE_ID + 35, "UnlockSwap"},
        {BASE_ID + 36, "UnlockThrowCover"},
        {BASE_ID + 37, "UnlockRabidBite"},
        {BASE_ID + 38, "UnlockPullAttack"},
        {BASE_ID + 39, "UnlockDruidAcidDart"},
        {BASE_ID + 40, "UnlockDruidPlantGrenade"},
        //Perks
        {BASE_ID + 50, "ArcaneBurstDamage"},
        {BASE_ID + 51, "SupportingFire"},
        {BASE_ID + 52, "InfiniteSupportingFire"},

        {BASE_ID + 53, "TimeBoostBigTime"},

        {BASE_ID + 54, "FalseProphetAttack"},
        {BASE_ID + 55, "FalseProphetHealth"},
        {BASE_ID + 56, "FalseProphetInteract"},

        {BASE_ID + 57, "PredictiveShotDamage"},
        {BASE_ID + 58, "PredictiveShotRefresh"},
        {BASE_ID + 59, "PredictiveShotManaGain"},

        {BASE_ID + 60, "WitchBoltMelee"},
        {BASE_ID + 61, "WitchBoltGrantsMove"},
        {BASE_ID + 62, "InfiniteRefreshingJolt"},

        {BASE_ID + 63, "ChainShockTargets"},
        {BASE_ID + 64, "ChainShockConduits"},
        {BASE_ID + 65, "ChainShockForce"},
        {BASE_ID + 66, "ChainShockSuperchain"},

        {BASE_ID + 67, "BroomPush"},

        {BASE_ID + 68, "GaleGrenadeUses"},
        {BASE_ID + 69, "GaleSecondWind"},
        {BASE_ID + 70, "GaleStorm"},

        {BASE_ID + 71, "SedativeGrenadePotency"},
        {BASE_ID + 72, "SedativeGrenadeUnsteady"},
        {BASE_ID + 73, "SedativeGrenadeShards"},

        {BASE_ID + 74, "GhostSkullDamage"},
        {BASE_ID + 75, "GhostSkullWidth"},
        {BASE_ID + 76, "GhostSkullDizzy"},

        {BASE_ID + 77, "DeathsDoorUses"},
        {BASE_ID + 78, "DeathsFloor"},
        {BASE_ID + 79, "DeathsDoorRange"},

        {BASE_ID + 80, "TransferenceGenius"},
        {BASE_ID + 81, "TransferenceUnsteady"},

        {BASE_ID + 82, "ChargeExtraKnockback"},
        {BASE_ID + 83, "ChargeRampage"},
        {BASE_ID + 84, "ChargeRampageInfinite"},
        {BASE_ID + 85, "ChargeSelective"},

        {BASE_ID + 86, "CenserSlamDamage"},

        {BASE_ID + 87, "SwapWithoutLOS"},
        {BASE_ID + 88, "SwapIsFreeOnFriends"},
        {BASE_ID + 89, "SwapConfusion"},
        {BASE_ID + 90, "SwapWithObjects"},

        {BASE_ID + 91, "ThrowCoverRetrieve"},
        {BASE_ID + 92, "ThrowCoverHeight"},

        {BASE_ID + 93, "BiteSedative"},
    
        {BASE_ID + 94, "PullBrittle"},
        {BASE_ID + 95, "PullGround"},
        {BASE_ID + 96, "PullFriends"},

        {BASE_ID + 97, "DartGasmask"},

        {BASE_ID + 98, "SporeUses"},
        {BASE_ID + 99, "SporeIntelligent"},
        {BASE_ID + 100, "SporeKnockback"},
    };
}