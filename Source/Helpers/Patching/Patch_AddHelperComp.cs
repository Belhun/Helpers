using HarmonyLib;
using Helpers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Helpers
{
    [HarmonyPatch(typeof(SurgeryOutcomeEffectDef), "GetQuality")]
    public static class Patch_AddHelperComp
    {
        private static readonly HashSet<System.Type> KnownSurgeryWorkerClasses = new HashSet<System.Type>
        {
            typeof(Recipe_Surgery),
            typeof(Recipe_InstallArtificialBodyPart),
            typeof(Recipe_RemoveBodyPart)
        };
        public static void Postfix(SurgeryOutcomeEffectDef __instance, RecipeDef recipe)
        {
            Log.Message($"[Helpers] Postfix triggered for {recipe?.defName ?? "Unknown Recipe"}");


            if (recipe.surgerySuccessChanceFactor <= 0f)
            {
                Log.Message($"[Helpers] Skipping {recipe.defName}: surgerySuccessChanceFactor is <= 0.");
                return;
            }

            if (recipe.workerClass == null || !Patch_AddHelperComp.KnownSurgeryWorkerClasses.Contains(recipe.workerClass))
            {
                // Check if Process Unknown Surgeries is disabled
                if (!HelpersMod.Settings.ProcessUnknownSurgeries)
                {
                    // Log the issue for devs or players in dev mode
                    if (Prefs.DevMode)
                    {
                        Log.Warning($"[Helpers] Unknown surgery detected: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name ?? "NULL"}).");
                        Log.Warning("COPY AND REPORT THIS:");
                        Log.Warning($" - Surgery Name: {recipe.defName}");
                        Log.Warning($" - Worker Class: {recipe.workerClass?.Name ?? "NULL"}");
                        Log.Warning($" - Mod Responsible: {recipe.modContentPack?.Name ?? "Unknown"}");
                    }
                    else if (!HelpersMod.Settings.DisableUnknownSurgeryNotifications && !HelpersMod.Settings.HasNotifiedUnknownSurgery)
                    {
                        // Show a one-time notification
                        var letter = LetterMaker.MakeLetter(
                            "Unknown Surgery Detected",
                            $"The surgery '{recipe.defName}' has been detected but is not recognized by the Helpers mod. This could be from a modded recipe.\n\n" +
                            "COPY AND REPORT THIS:\n" +
                            $"- Surgery Name: {recipe.defName}\n" +
                            $"- Worker Class: {recipe.workerClass?.Name ?? "NULL"}\n" +
                            $"- Mod Responsible: {recipe.modContentPack?.Name ?? "Unknown"}\n\n" +
                            "Once reported, you can enable 'Process Unknown Surgeries' in settings to allow contributions for this surgery. If you don't want to see this message again, you can disable it in the Helpers mod settings.",
                            LetterDefOf.NegativeEvent);

                        Find.LetterStack.ReceiveLetter(letter);
                        HelpersMod.Settings.HasNotifiedUnknownSurgery = true; // Ensure this only happens once per session
                    }

                    return; // Skip processing the surgery
                }
                else
                {
                    // Log or notify about the surgery being processed
                    if (HelpersMod.Settings.LogUnknownSurgeries)
                    {
                        Log.Warning($"[Helpers] Unknown surgery processed: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name ?? "NULL"}).");
                        Log.Warning("This surgery was processed because 'Process Unknown Surgeries' is enabled in settings. Report any unexpected behavior.");
                    }
                }
            }

            Log.Message($"[Helpers] Processing surgery recipe: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name})");

            if (__instance.comps == null)
            {
                __instance.comps = new List<SurgeryOutcomeComp>();
                Log.Message($"[Helpers] Initialized comps list for {recipe.defName}");
            }

            if (!__instance.comps.OfType<SurgeryOutcomeComp_Helpers>().Any())
            {
                __instance.comps.Add(new SurgeryOutcomeComp_Helpers());
                Log.Message($"[Helpers] Added SurgeryOutcomeComp_Helpers to {recipe.defName}");
            }
            else
            {
                Log.Message($"[Helpers] SurgeryOutcomeComp_Helpers already exists for {recipe.defName}");
            }


        }
        //public static class HelpersSettings
        //{
        //    public static bool LogUnknownSurgeries = true; // Default to logging unknown surgeries
        //}
    }

}
