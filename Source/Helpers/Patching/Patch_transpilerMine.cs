using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using RimWorld;
using Verse;
using Verse.AI;


namespace Helpers
{
    [HarmonyPatch]
    public static class Patch_TranspilerMine
    {
        static MethodBase TargetMethod()
        {
            // Locate the nested method using reflection
            var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic | BindingFlags.Instance);
            return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var calculateMethod = AccessTools.Method(typeof(HelperMechanics), nameof(HelperMechanics.CalculateAdjustedMiningDamage));
            bool injected = false;

            for (int i = 0; i < codes.Count - 5; i++)
            {
                // Locate the code assigning to 'num'
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i + 1].opcode == OpCodes.Stloc_3)
                {
                    // Add logging before
                    codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_3)); // Load baseAmount
                    codes.Insert(i + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DebugHelpers), nameof(DebugHelpers.LogNumValue))));

                    // Replace with custom logic
                    codes.Insert(i + 4, new CodeInstruction(OpCodes.Ldloc_0)); // Load 'actor'
                    codes.Insert(i + 5, new CodeInstruction(OpCodes.Ldloc_1)); // Load 'mineTarget'
                    codes.Insert(i + 6, new CodeInstruction(OpCodes.Ldloc_3)); // Load baseAmount
                    codes.Insert(i + 7, new CodeInstruction(OpCodes.Call, calculateMethod)); // Call the custom method
                    codes.Insert(i + 8, new CodeInstruction(OpCodes.Stloc_3)); // Store adjusted value

                    // Add logging after
                    codes.Insert(i + 9, new CodeInstruction(OpCodes.Ldloc_3)); // Load adjusted num
                    codes.Insert(i + 10, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DebugHelpers), nameof(DebugHelpers.LogNumValue))));

                    injected = true;
                    break;
                }
            }

            if (!injected)
            {
                Log.Warning("[Patch_TranspilerMine] Failed to inject custom mining damage logic.");
            }

            return codes.AsEnumerable();
        }


    }
}