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
//public static class Patch_transpilerMine
//{
//    static MethodBase TargetMethod()
//    {
//        // Locate the nested method using reflection
//        var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic);
//        return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
//    }

//    //public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, ref int ___amounts)
//    //{
//    //    var codes = new List<CodeInstruction>(instructions);

//    //    OpCode[] pattern = new[]
//    //    {
//    //        OpCodes.Ldloc_1,    // Load mineTarget
//    //        OpCodes.Ldfld,      // Access Thing.def
//    //        OpCodes.Ldfld,      // Access ThingDef.building
//    //        OpCodes.Ldfld,      // Access BuildingProperties.isNaturalRock
//    //        OpCodes.Brtrue_S,   // Branch if true
//    //        OpCodes.Ldc_I4_S,   // Push 40
//    //        OpCodes.Br_S,       // Branch
//    //        OpCodes.Ldc_I4_S,   // Push 80
//    //        OpCodes.Stloc_3     // Store in local variable (amount)
//    //    };


//    //    int targetIndex = FindIndex(codes, pattern);



//    //    codes.InsertRange(targetIndex, new[]
//    //    {
//    //        new CodeInstruction(OpCodes.Ldloc_3), // Load the value of `amount`
//    //        new CodeInstruction(OpCodes.Ldc_I4, 10), // Push 10 onto the stack
//    //        new CodeInstruction(OpCodes.Add),    // Add 10 to `amount`
//    //        new CodeInstruction(OpCodes.Stloc_3) // Store the updated value back in `amount`
//    //    });


//    //    return codes;
//    //}


//    public static void Prefix(object __instance)
//    {
//        DebugHelpers.DebugLog("Patch_PreFixMine", "Prefix started");

//        // Access the 'mine' field from '__instance'
//        var mine = ReflectionHelper.GetNestedFieldOrPropertyValue(__instance, "mine") as Toil;

//        // Access the 'actor' field from 'mine'
//        var actor = ReflectionHelper.GetNestedFieldOrPropertyValue(mine, "actor") as Pawn;

//        // Access the 'curDriver' field from 'actor.jobs'
//        var curDriver = ReflectionHelper.GetNestedFieldOrPropertyValue(actor?.jobs, "curDriver") as JobDriver_Mine;

//        // Access the 'MineTarget' property from 'curDriver'
//        var mineTarget = ReflectionHelper.GetNestedFieldOrPropertyValue(curDriver, "MineTarget");

//        Log.Message($"MineTarget found: {mineTarget}");

//        // Inspect the base properties of 'mineTarget'
//        var def = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "def");

//        DebugHelpers.DebugLog("Patch_PreFixMine", $"Def found: {def}");

//        // Access the 'building' field from 'def'
//        var building = ReflectionHelper.GetNestedFieldOrPropertyValue(def, "building") as BuildingProperties;

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
//        int hitPoints = (int)ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "HitPoints");

//        MethodInfo takeDamageMethod = mineTarget.GetType().GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

//        var map = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "Map");
//        var designationManager = ReflectionHelper.GetNestedFieldOrPropertyValue(map, "designationManager");
//        MethodInfo designationAtMethod = designationManager?.GetType().GetMethod("DesignationAt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

//        var helperComp = actor.GetHelperComponent();

//        float helperContribution = HelperMechanics.MineingCHC(
//               actor,
//               curDriver,
//               helperComp.CurrentHelpers,
//               StatDefOf.MiningSpeed
//           );
//        DebugHelpers.DebugLog("Patch_PreFixMine", $" Prefix end");
//        //var defPositiontype = mineTarget?.GetType().BaseType?.GetProperties("Position", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(mineTarget);

//        //IntVec3 position = mineTarget.Position;
//        //int amount = (building.isNaturalRock ? 80 : 40);
//        //if (!(mineTarget is Mineable mineable2) || mineTarget.HitPoints > amount)
//        //{
//        //    DamageInfo dinfo = new DamageInfo(DamageDefOf.Mining, amount, 0f, -1f, actor);
//        //    mineTarget.TakeDamage(dinfo);
//        //}
//        //else
//        //{
//        //    bool num = <> 4__this.Map.designationManager.DesignationAt(mineable2.Position, DesignationDefOf.MineVein) != null;
//        //    mineable2.Notify_TookMiningDamage(mineTarget.HitPoints, mine.actor);
//        //    mineable2.HitPoints = 0;
//        //    mineable2.DestroyMined(actor);
//        //    if (num)
//        //    {
//        //        IntVec3[] adjacentCells = GenAdj.AdjacentCells;
//        //        foreach (IntVec3 adjacentCell in adjacentCells)
//        //        {
//        //            Designator_MineVein.FloodFillDesignations(position + adjacentCell, <> 4__this.Map, mineable2.def);
//        //        }
//        //    }
//        //}



//    }

//    private static int FindIndex(List<CodeInstruction> codes, OpCode[] pattern)
//    {
//        for (int i = 0; i <= codes.Count - pattern.Length; i++)
//        {
//            bool match = true;
//            for (int j = 0; j < pattern.Length; j++)
//            {
//                if (codes[i + j].opcode != pattern[j])
//                {
//                    match = false;
//                    break;
//                }
//            }
//            if (match)
//            {
//                return i; // Return the index of the first matched instruction
//            }
//        }
//        return -1; // Pattern not found
//    }
//}
