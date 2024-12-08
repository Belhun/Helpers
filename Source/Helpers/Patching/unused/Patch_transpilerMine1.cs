//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;
//using HarmonyLib;
//using Helpers;
//using RimWorld;
//using Verse;
//using Verse.AI;

//[HarmonyPatch]
//public static class Patch_transpilerMine1
//{
//    static MethodBase TargetMethod()
//    {
//        // Locate the nested method using reflection
//        var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic);
//        return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
//    }

//    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, ref int ___ticksToPickHit)
//    {
//        var codes = new List<CodeInstruction>(instructions);

//        // Define the IL pattern for the target code
//        OpCode[] pattern = new[]
//        {
//            OpCodes.Ldarg_0,
//            OpCodes.Ldfld,
//            OpCodes.Ldfld,
//            OpCodes.Ldloc_0,
//            OpCodes.Call,
//            OpCodes.Ldloc_1,
//            OpCodes.Call,
//            OpCodes.Ldc_I4_M1,
//            OpCodes.Callvirt,
//            OpCodes.Pop,
//            OpCodes.Ldloc_1,
//            OpCodes.Ldfld,
//            OpCodes.Ldfld,
//            OpCodes.Ldfld,
//            OpCodes.Brtrue_S,
//            OpCodes.Ldc_I4_S,
//            OpCodes.Br_S,
//            OpCodes.Ldc_I4_S
//        };

//        // Find the index where the pattern occurs
//        int targetIndex = FindIndex(codes, pattern);

//        if (targetIndex != -1)
//        {
//            //int afterPatternIndex = targetIndex + pattern.Length;

//            //Log.Message($"[Transpiler] Found target pattern at index: {targetIndex}");

//            //// Insert custom instructions after the pattern
//            //codes.InsertRange(afterPatternIndex, new[]
//            //{
//            //    // ticksToPickHit = TranspilerMineingTTPCHC(actor, ticksToPickHit);
//            //    new CodeInstruction(OpCodes.Ldarg_0), // Load this
//            //    new CodeInstruction(OpCodes.Ldfld, typeof(JobDriver_Mine).GetField("ticksToPickHit", BindingFlags.Instance | BindingFlags.NonPublic)), // Load ticksToPickHit
//            //    new CodeInstruction(OpCodes.Ldloc_0), // Load actor (Pawn)
//            //    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HelperMechanics), nameof(HelperMechanics.TranspilerMineingTTPCHC))), // Call TranspilerMineingTTPCHC
//            //    new CodeInstruction(OpCodes.Stfld, typeof(JobDriver_Mine).GetField("ticksToPickHit", BindingFlags.Instance | BindingFlags.NonPublic)), // Store result in ticksToPickHit

//            //    // amount = TranspilerMineingDCHC(actor, ticksToPickHit, isNaturalRock, mineTarget);
//            //    new CodeInstruction(OpCodes.Ldloc_0), // Load actor (Pawn)
//            //    new CodeInstruction(OpCodes.Ldarg_0), // Load this
//            //    new CodeInstruction(OpCodes.Ldfld, typeof(JobDriver_Mine).GetField("ticksToPickHit", BindingFlags.Instance | BindingFlags.NonPublic)), // Load ticksToPickHit
//            //    new CodeInstruction(OpCodes.Ldloc_1), // Load mineTarget
//            //    new CodeInstruction(OpCodes.Ldfld, typeof(ThingDef).GetField("building", BindingFlags.Instance | BindingFlags.NonPublic)), // Load building properties
//            //    new CodeInstruction(OpCodes.Ldfld, typeof(BuildingProperties).GetField("isNaturalRock", BindingFlags.Instance | BindingFlags.Public)), // Load isNaturalRock
//            //    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HelperMechanics), nameof(HelperMechanics.TranspilerMineingDCHC))), // Call TranspilerMineingDCHC
//            //    new CodeInstruction(OpCodes.Stloc_3) // Store result in amount
//            //});
//        }
//        else
//        {
//            Log.Warning("[Transpiler] Target IL code sequence not found.");
//        }

//        return codes;
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
