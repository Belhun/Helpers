//using HarmonyLib;
//using RimWorld;
//using System.Collections.Generic;
//using System.Reflection;
//using Verse;
//using Verse.AI;

//namespace Helpers.Patching
//{
//    /// <summary>
//    /// Works with the SurgeryOutcomeEffectDef class to log information about surgery outcomes.
//    /// </summary>
//    [StaticConstructorOnStartup]
//    public static class SurgeryOutcomeEffectDefPatches
//    {
//        static SurgeryOutcomeEffectDefPatches()
//        {
//            var harmonys = new Harmony("Belhun.Surgeryimplamentation");
//            harmonys.Patch(
//                original: AccessTools.Method(typeof(SurgeryOutcomeEffectDef), nameof(SurgeryOutcomeEffectDef.GetQuality)),
//                prefix: new HarmonyMethod(typeof(SurgeryOutcomeEffectDefPatches), nameof(GetQualityPostfix))
//            );
//            harmonys.Patch(
//                original: AccessTools.Method(typeof(SurgeryOutcomeEffectDef), nameof(SurgeryOutcomeEffectDef.GetOutcome)),
//                prefix: new HarmonyMethod(typeof(SurgeryOutcomeEffectDefPatches), nameof(GetOutcomePostfix))
//            );
//        }

//        public static void GetQualityPostfix(
//            RecipeDef recipe,
//            Pawn surgeon,
//            Pawn patient,
//            List<Thing> ingredients,
//            BodyPartRecord part,
//            Bill bill,
//            ref float __result, object __instance)
//        {

//            DebugHelpers.DebugLog("GetQualityPostfix", $"Starting Postfix for SurgeryOutcomeEffectDef...");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"RecipeDef recipe: {recipe.defName}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"Pawn surgeon: {surgeon.Name}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"Pawn patient: {patient.Name}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"List<Thing> ingredients: {ingredients.Count}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"BodyPartRecord part: {part.def.label}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"Bill bill: {bill.recipe.defName}");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"<color=#00FF00>float __result: {__result}</color>");
//            //DebugHelpers.DebugLog("GetQualityPostfix", $"object __instance: {__instance.GetType()}");
//        }

//        public static void GetOutcomePostfix(
//            RecipeDef recipe,
//            Pawn surgeon,
//            Pawn patient,
//            List<Thing> ingredients,
//            BodyPartRecord part,
//            Bill bill,
//            ref SurgeryOutcome __result)
//        {
//            string outcome = __result != null ? __result.ToString() : "null";
//            DebugHelpers.DebugLog("GetOutcomePostfix", $" GetOutcome called with surgeon: {surgeon.Name}" +
//                $",patient: {patient.Name}, recipe: {recipe.defName}, <color=#00FF00>outcome: {outcome}</color>");
//        }
//    }
//}
