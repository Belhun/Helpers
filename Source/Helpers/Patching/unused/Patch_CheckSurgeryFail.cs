﻿//using HarmonyLib;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using UnityEngine;
//using Verse;
//using Verse.AI;

//namespace Helpers.Patching
//{
//    [HarmonyPatch]
//    public static class Patch_CheckSurgeryFail
//    {
//        static MethodBase TargetMethod()
//        {
//            //RimWorld.Recipe_Surgery.CheckSurgeryFail
//            // Locate the nested class and method generated by the compiler
//            var nestedType = typeof(Recipe_Surgery);
//            return nestedType?.GetMethod("CheckSurgeryFail", BindingFlags.Instance | BindingFlags.NonPublic);
//        }

//        public static void Postfix(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill, object __instance, bool __result)
//        {

//            //Info resaults of the surgery
//            // If resault is true the surgery failed
//            // If resault is false the surgery was a success
//            if (__result == true)
//            {
//                DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"<color=#00FF00>__result is true</color>");
//            }
//            if (__result == false)
//            {
//                DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"<color=#00FF00>__result is false</color>");
//            }
//            //if (__instance != null)
//            //{
//            //    DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $".GetT__instanceype().GetMethods(){__instance.GetType().GetMethods()}");
//            //    DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"__instance.GetType().Attributes{__instance.GetType().Attributes}");
//            //    foreach (var method in __instance.GetType().GetMethods())
//            //    {
//            //        DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"__instance.GetType().GetMethods().Length{method.Name}");
//            //        DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"__instance.GetType().GetMethods().Length{method.Attributes}");
//            //        DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"__instance.GetType().GetMethods().Length{method.GetType().Name}");
//            //    }

//            //}
//            //else
//            //{
//            //    DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"surgeon is null");
//            //}



//            DebugHelpers.DebugLog("Patch_CheckSurgeryFail", $"CheckSurgeryFail has susceffly Been added to the post");


//        }
//    }


//    public class SurgeryTesting : SurgeryOutcomeSuccess
//    {
//        public override bool Apply(float quality, RecipeDef recipe, Pawn surgeon, Pawn patient, BodyPartRecord part)
//        {
//            var test = Rand.Chance(quality);
//            Log.Message($"SurgeryOutcomeSuccess.SurgeryTesting: {test}");
//            DebugHelpers.DebugLog("GetQualityPostfix", $"Starting Postfix for SurgeryOutcomeEffectDef...");

//            return test;
//        }
//    }

//}
