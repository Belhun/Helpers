//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using HarmonyLib;
//using Helpers;
//using RimWorld;
//using Verse;
//using Verse.AI;
//using Verse.Noise;

//[HarmonyPatch]
//public static class Patch_PreFixMine
//{
//    static MethodBase TargetMethod()
//    {
//        // Locate the nested method using reflection
//        var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic);
//        return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
//    }

//    public static void Prefix(object __instance)
//    {
//        DebugHelpers.DebugLog("Patch_PreFixMine", "Prefix started");

//        // Access the 'mine' field
//        var mine = ReflectionHelper.GetNestedFieldOrPropertyValue(__instance, "mine") as Toil;

//        // Access the 'actor' field
//        var actor = ReflectionHelper.GetNestedFieldOrPropertyValue(mine, "actor") as Pawn;

//        // Access the 'curDriver' field
//        var curDriver = ReflectionHelper.GetNestedFieldOrPropertyValue(actor?.jobs, "curDriver") as JobDriver_Mine;

//        // Access the 'MineTarget' property
//        var mineTarget = ReflectionHelper.GetNestedFieldOrPropertyValue(curDriver, "MineTarget") as Thing;

//        // Access the 'HitPoints' field
//        var hitPoints = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "HitPoints");
//        var def = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "def");
//        var building = ReflectionHelper.GetNestedFieldOrPropertyValue(def, "building") as BuildingProperties;

//        DebugHelpers.DebugLog("Patch_PreFixMine", $"MineTarget HitPoints: {hitPoints}");

//        if (building != null)
//        {
//            bool isNaturalRock = building.isNaturalRock;
//            DebugHelpers.DebugLog("Patch_PreFixMine", $"Building found: {building.ToString()}, isNaturalRock: {isNaturalRock.ToString()}");
//        }
//        else
//        {
//            Log.Warning("Unable to retrieve 'building' field from def.");
//        }

//        IntVec3 position = (IntVec3)ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "Position");

//        MethodInfo takeDamageMethod = mineTarget.GetType().GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

//        var map = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "Map");
//        var designationManager = ReflectionHelper.GetNestedFieldOrPropertyValue(map, "designationManager");
//        MethodInfo designationAtMethod = designationManager?.GetType().GetMethod("DesignationAt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

//        var helperComp = actor.GetHelperComponent();
//        BuildingProperties buildprop = (BuildingProperties)building;
//        HelperMechanics.DistributeMiningXP(
//               actor,
//               curDriver,
//               helperComp.CurrentHelpers,
//               mineTarget,
//               building
//           );

//        // Adjust ticksToPickHit
//        FieldInfo ticksToPickHitField = typeof(JobDriver_Mine).GetField("ticksToPickHit", BindingFlags.Instance | BindingFlags.NonPublic);
//        int ticksToPickHit = (int)ticksToPickHitField.GetValue(curDriver);
//        ticksToPickHit = HelperMechanics.TranspilerMineingTTPCHC(actor, ticksToPickHit);
//        ticksToPickHitField.SetValue(curDriver, ticksToPickHit);

//        //if (helperComp != null && helperComp.IsBeingHelped)
//        //{
//        //    int amount = ((int)helperContribution);

//        //    int CurrentHitPoints = (int)hitPoints;
//        //    Map CurrentMap = (Map)map;
//        //    if (!(mineTarget is Mineable mineable2) || CurrentHitPoints > amount)
//        //    {
//        //        DamageInfo dinfo = new DamageInfo(DamageDefOf.Mining, amount, 0f, -1f, actor);
//        //        takeDamageMethod?.Invoke(mineTarget, new object[] { dinfo });
//        //    }
//        //    else
//        //    {
//        //        bool num = (bool)designationAtMethod?.Invoke(designationManager, new object[] { mineable2.Position, DesignationDefOf.MineVein });
//        //        mineable2.Notify_TookMiningDamage(CurrentHitPoints, mine.actor);
//        //        mineable2.HitPoints = 0;
//        //        mineable2.DestroyMined(actor);
//        //        if (num)
//        //        {
//        //            IntVec3[] adjacentCells = GenAdj.AdjacentCells;
//        //            foreach (IntVec3 adjacentCell in adjacentCells)
//        //            {
//        //                Designator_MineVein.FloodFillDesignations(position + adjacentCell, CurrentMap, mineable2.def);
//        //            }
//        //        }
//        //    }
//        //}
//    }
//}
