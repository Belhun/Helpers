using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Helpers
{
    [StaticConstructorOnStartup]
    public static class HelpersInitializer
    {
        /// <summary>
        /// Initializes the Helpers mod by applying Harmony patches and logging success.
        /// </summary>
        static HelpersInitializer()
        {
            DebugHelpers.DebugLog("HelpersInitializer", "Assembly loaded successfully.");
            var harmony = new Harmony("Belhun.helpersmod");
            harmony.PatchAll();
            DebugHelpers.DebugLog("HelpersInitializer", "Harmony patches applied successfully.");
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        /// <summary>
        /// Adds a "Help" option to the float menu when right-clicking a target pawn.
        /// </summary>
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Define targeting parameters to filter valid target pawns
            TargetingParameters targetingParams = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetItems = false,
                validator = t =>
                {
                    if (t.Thing is Pawn targetPawn && targetPawn != pawn && targetPawn.IsColonistPlayerControlled)
                    {
                        return !targetPawn.Downed && !targetPawn.Dead;
                    }
                    return false;
                }
            };

            // Get all valid targets at the clicked position
            foreach (LocalTargetInfo target in GenUI.TargetsAt(clickPos, targetingParams, thingsOnly: true))
            {
                if (target.Thing is Pawn targetPawn)
                {
                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Found valid target: {targetPawn.LabelShortCap}");

                    // Get all selected pawns, excluding the target pawn
                    var selectedPawns = Find.Selector.SelectedObjects
                        .OfType<Pawn>()
                        .Where(p => p != targetPawn && p.IsColonistPlayerControlled)
                        .ToList();

                    if (selectedPawns.Count == 0)
                    {
                        DebugHelpers.DebugLog("FloatMenuMakerMap", "No valid helper pawns selected.");
                        continue;
                    }

                    // Define the label for the "Help" menu option
                    string label = selectedPawns.Count > 1
                        ? $"Help {targetPawn.LabelShortCap} with {selectedPawns.Count} pawns"
                        : $"Help {targetPawn.LabelShortCap}";

                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Creating menu option with label: {label}");

                    // Define the action when the menu option is selected
                    Action action = () =>
                    {
                        foreach (var helperPawn in selectedPawns)
                        {
                            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                            helperPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            DebugHelpers.DebugLog("FloatMenuMakerMap", $"{helperPawn.Name} is now helping {targetPawn.LabelShortCap}.");
                        }
                    };

                    // Create and add the menu option
                    FloatMenuOption helpOption = FloatMenuUtility.DecoratePrioritizedTask(
                        new FloatMenuOption(label, action, MenuOptionPriority.Default),
                        pawn,
                        targetPawn
                    );

                    opts.Add(helpOption);
                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Added 'Help' option to the float menu for {targetPawn.LabelShortCap}.");
                }
            }
        }
    }


    [HarmonyPatch(typeof(Toils_Recipe), "DoRecipeWork")]
    public static class DoRecipeWorkOverridePatch
    {
        /// <summary>
        /// Replaces the default DoRecipeWork toil with the custom DoRecipeWork_Helper toil.
        /// </summary>
        public static bool Prefix(ref Toil __result)
        {
            __result = Helpers.CustomToils_Recipe.DoRecipeWork_Helper();
            DebugHelpers.DebugLog("DoRecipeWorkOverridePatch", "Replaced DoRecipeWork with custom DoRecipeWork_Helper.");
            return false; // Skip the original method
        }
    }
}
