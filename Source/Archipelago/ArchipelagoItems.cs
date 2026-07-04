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
        {BASE_ID + 40, "UnlockCrowdGrenade"}, //Spore grenade
        {BASE_ID + 41, "UnlockStealMana"},

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

        //Missions
        {BASE_ID + 300, "Dream 2A"},
        {BASE_ID + 301, "Dream 2B"},
        {BASE_ID + 302, "Dream 2C"},
        {BASE_ID + 303, "Dream 2D"},
        {BASE_ID + 304, "Dream 3B"},
        {BASE_ID + 305, "Dream 3C"},
        {BASE_ID + 306, "Dream 4A"},
        {BASE_ID + 307, "Dream Heatsig"},
        {BASE_ID + 308, "Dream Intro"},
        {BASE_ID + 309, "Evidence Boss"},
        {BASE_ID + 310, "Evidence Lockup"},
        {BASE_ID + 311, "Exit Strategy"},
        {BASE_ID + 312, "Finale - Mines"},
        {BASE_ID + 313, "Finale - Roof"},
        {BASE_ID + 314, "Finale - Vault"},
        {BASE_ID + 315, "Flashback"},
        {BASE_ID + 316, "Fort Osprey"},
        {BASE_ID + 317, "Kalan Ambush"},
        {BASE_ID + 318, "Kalan Ambush 2"},
        {BASE_ID + 319, "Liboli Intro"},
        {BASE_ID + 320, "Lucid Dream - Banks"},
        {BASE_ID + 321, "Lucid Dream - Dall"},
        {BASE_ID + 322, "Lucid Dream - Jen"},
        {BASE_ID + 323, "Lucid Dream - Rion"},
        {BASE_ID + 324, "Lucid Dream - Zan"},
        {BASE_ID + 325, "Prologue"},
        {BASE_ID + 326, "Proving Ground 1"},
        {BASE_ID + 327, "Proving Ground 2"},
        {BASE_ID + 328, "Proving Ground 3"},
        {BASE_ID + 329, "Proving Ground 4"},
        {BASE_ID + 330, "Siege Cleric"},
        {BASE_ID + 331, "Streets"},
        {BASE_ID + 332, "The Broadcast"},
        {BASE_ID + 333, "The Meet"},
        {BASE_ID + 334, "The Pyromancer"},
        {BASE_ID + 335, "The Recording"},
        {BASE_ID + 336, "Train"},
        {BASE_ID + 337, "Two Trains"},
        {BASE_ID + 338, "Villa Medil"},
        {BASE_ID + 339, "Witch Intro"},
        {BASE_ID + 340, "Necro Intro"},
    };
}