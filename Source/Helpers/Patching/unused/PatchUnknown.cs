﻿//using HarmonyLib;
//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Verse;

//namespace Helpers.Patching
//{
//    [HarmonyPatch]
//    public static class Patch_Uknown
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


//            DebugHelpers.DebugLog("Patch_Uknown", $"Patch_Uknown has susceffly Been added to the post");


//        }
//    }
//}
