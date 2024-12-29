//using HarmonyLib;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Verse.AI;
//using Verse;
//using System.Diagnostics;

//namespace Helpers.Plants
//{
//    [HarmonyPatch]
//    public static class Patch_PlantSow
//    {
//        static MethodBase TargetMethod()
//        {

//            return typeof(JobDriver_PlantSow)
//                .GetNestedType("<>c__DisplayClass5_0", BindingFlags.NonPublic)
//                ?.GetMethod("<MakeNewToils>b__4", BindingFlags.Instance | BindingFlags.NonPublic);
//        }

//        public static void Postfix(object __instance)
//        {
//            DebugHelpers.DebugLog("Patch_PlantSow", $" Postfix started");
//            Toil toil = Traverse.Create(__instance).Field("sowToil").GetValue<Toil>();
//            if (toil != null)
//            {
//                DebugHelpers.DebugLog("Patch_PlantSow", $"Toil: {toil.ToString()}");
//            }
//            else
//            {
//                DebugHelpers.DebugLog("Patch_PlantSow", $" Toil is null.");
//            }
//            Pawn actor = toil.actor;
//            if (actor != null)
//            {
//                DebugHelpers.DebugLog("Patch_PlantSow", $" Pawn: {actor.Name}");
//            }
//            else
//            {
//                DebugHelpers.DebugLog("Patch_PlantSow", $" Pawn is null.");
//            }
//            var helperComp = actor.GetHelperComponent();
//            var curJob = actor.jobs?.curJob;
//            var jobDriver = actor.jobs?.curDriver as JobDriver_PlantSow;
//            FieldInfo workDoneField = typeof(JobDriver_PlantSow).GetField("sowWorkDone", BindingFlags.Instance | BindingFlags.NonPublic);
//            float currentWork = (float)workDoneField.GetValue(jobDriver);
//            float helperContribution = HelperMechanics.PlantsCHC(
//                    actor,
//                    jobDriver,
//                    helperComp.CurrentHelpers,
//                    StatDefOf.PlantWorkSpeed
//                );

//        }

//        [HarmonyPatch]
//        public static class Patch_PlantWork
//        {
//            static MethodBase TargetMethod()
//            {
//                try
//                {
//                    // Step 1: Get the nested class
//                    var nestedType = typeof(JobDriver_PlantWork)
//                        .GetNestedType("<>c__DisplayClass11_0", BindingFlags.NonPublic);
//                    // Step 2: Get the method within the nested class
//                    var targetMethod = nestedType
//                        .GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
//                    return targetMethod;
//                }
//                catch (Exception ex)
//                {
//                    Log.Error($"[Helpers] Exception while resolving TargetMethod: {ex.Message}\n{ex.StackTrace}");
//                    return null;
//                }
//            }


//            public static void Postfix(object __instance)
//            {
//                DebugHelpers.DebugLog("Patch_PlantWork", $" Postfix started");

//                Toil toil = Traverse.Create(__instance).Field("cut").GetValue<Toil>();
//                if (toil != null)
//                {
//                    DebugHelpers.DebugLog("Patch_PlantWork", $"Toil: {toil.ToString()}");
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" Toil: {toil.ToString()}");
//                }
//                else
//                {
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" Toil is null.");
//                }
//                Pawn actor = toil.actor;
//                if (actor != null)
//                {
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" Pawn: {actor.Name}");
//                }
//                else
//                {
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" Pawn is null.");
//                }
//                var helperComp = actor.GetHelperComponent();
//                var curJob = actor.jobs?.curJob;
//                var jobDriver = actor.jobs?.curDriver as JobDriver_PlantWork;
//                if (helperComp != null && helperComp.IsBeingHelped)
//                {
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" helperComp is not null && pawn IsBeingHelped ");

//                    // Calculate helper contribution
//                    float helperContribution = HelperMechanics.PlantsCHC(
//                        actor,
//                        jobDriver,
//                        helperComp.CurrentHelpers,
//                        StatDefOf.PlantWorkSpeed
//                    );
//                    FieldInfo workDoneField = typeof(JobDriver_PlantWork).GetField("workDone", BindingFlags.Instance | BindingFlags.NonPublic);
//                    if (workDoneField == null)
//                    {
//                        DebugHelpers.DebugLog("Patch_PlantWork", $" Unable to find 'workDone' field in JobDriver_PlantWork.");
//                        return;
//                    }
//                    float currentWorkDone = (float)workDoneField.GetValue(jobDriver);
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" workDone. Old Value: {currentWorkDone}");

//                    // Add helper contribution
//                    currentWorkDone += helperContribution;

//                    // Set the updated value back to the field
//                    workDoneField.SetValue(jobDriver, currentWorkDone);
//                    DebugHelpers.DebugLog("Patch_PlantWork", $" Adjusted workDone. New value: {currentWorkDone}");

//                    FieldInfo testFeild = typeof(JobDriver_PlantWork).GetField("workDone", BindingFlags.Instance | BindingFlags.NonPublic);

//                    DebugHelpers.DebugLog("Patch_PlantWork", $"if it worked the above should be {(float)testFeild.GetValue(jobDriver)}");
//                }
//                DebugHelpers.DebugLog("Patch_PlantWork", $" Postfix End");

//            }
//        }
//    }
//}
