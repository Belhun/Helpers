//using HarmonyLib;
//using RimWorld;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;
//using Verse;
//using Verse.AI;

//namespace Helpers
//{
//    [HarmonyPatch]
//    public static class Patch_Toils_Recipe
//    {
//        static MethodBase TargetMethod()
//        {
//            return typeof(Verse.AI.Toils_Recipe)
//                .GetNestedType("<>c__DisplayClass2_0", BindingFlags.NonPublic)
//                ?.GetMethod("<DoRecipeWork>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
//        }

//        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
//        {
//            var codes = new List<CodeInstruction>(instructions);
//            int insertionIndex = -1;

//            LocalBuilder helperCompLocal = generator.DeclareLocal(typeof(PawnHelperComponent));
//            LocalBuilder workSpeedLocal = generator.DeclareLocal(typeof(float));

//            // Find the location of `jobDriver.workLeft -= workSpeed;`
//            for (int i = 0; i < codes.Count - 2; i++)
//            {
//                if (codes[i].opcode == OpCodes.Ldfld && // Accessing a field
//                    codes[i].operand is FieldInfo field && field.Name == "workLeft" && // Ensure it's `workLeft`
//                    codes[i + 1].opcode == OpCodes.Ldloc_S && // Loading `workSpeed`
//                    codes[i + 2].opcode == OpCodes.Sub) // Subtracting `workSpeed` from `workLeft`
//                {
//                    insertionIndex = i;
//                    break;
//                }
//            }

//            if (insertionIndex != -1)
//            {
//                Log.Message($"Found insertion point at index {insertionIndex}. Injecting helper contribution logic.");

//                // Insert the new code before `jobDriver.workLeft -= workSpeed;`
//                codes.InsertRange(insertionIndex, new[]
//                {
//                    // Load arguments for `HelperMechanics.CalculateHelperContribution()`
//                    new CodeInstruction(OpCodes.Ldarg_0), // Load `actor`
//                    new CodeInstruction(OpCodes.Ldarg_0), // Load `jobDriver`
//                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "curJob")), // Access `curJob`
//                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Job), "RecipeDef")), // Get `RecipeDef`
//                    new CodeInstruction(OpCodes.Ldloc_S, helperCompLocal), // Load `helperCompLocal`
//                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PawnHelperComponent), "CurrentHelpers")), // Access `CurrentHelpers`
//                    new CodeInstruction(OpCodes.Ldarg_0), // Load `jobDriver`
//                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "curJob")), // Access `curJob`
//                    new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Job), "RecipeDef")), // Get `RecipeDef`
//                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RecipeDef), "workSpeedStat")), // Access `workSpeedStat`

//                    // Call `HelperMechanics.CalculateHelperContribution()`
//                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HelperMechanics), "CalculateHelperContribution")),

//                    // Add the result to `workSpeed`
//                    new CodeInstruction(OpCodes.Ldloc_S, workSpeedLocal), // Load `workSpeed`
//                    new CodeInstruction(OpCodes.Add), // Add helper contribution
//                    new CodeInstruction(OpCodes.Stloc_S, workSpeedLocal) // Store back into `workSpeed`
//                });

//                Log.Message("Helper contribution logic successfully injected.");
//            }
//            else
//            {
//                Log.Warning("Target IL code sequence for `jobDriver.workLeft -= workSpeed` not found. Transpiler injection failed.");
//            }

//            return codes;
//        }
//    }
//}
