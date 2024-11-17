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
            IntVec3 centerCell = IntVec3.FromVector3(clickPos);
            Pawn targetPawn = centerCell.GetFirstPawn(pawn.Map);

            // Search for a valid target pawn in adjacent cells if none found at the clicked position
            if (targetPawn == null)
            {
                foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(centerCell, Rot4.North, new IntVec2(3, 3)))
                {
                    targetPawn = cell.GetFirstPawn(pawn.Map);
                    if (targetPawn != null && targetPawn != pawn) break;
                }
            }

            if (targetPawn == null)
            {
                DebugHelpers.DebugLog("FloatMenuMakerMap", "No valid target pawn found in the clicked area.");
                return;
            }

            // Get all selected pawns, excluding the target pawn
            var selectedPawns = Find.Selector.SelectedObjects
                .OfType<Pawn>()
                .Where(p => p != targetPawn && p.IsColonistPlayerControlled)
                .ToList();

            DebugHelpers.DebugLog("FloatMenuMakerMap", $"Selected pawns: {string.Join(", ", selectedPawns.Select(p => p.Name.ToStringShort))}");

            if (selectedPawns.Count == 0)
            {
                DebugHelpers.DebugLog("FloatMenuMakerMap", "No valid helper pawns selected.");
                return;
            }

            // Define the label for the "Help" menu option
            string label = selectedPawns.Count > 1
                ? $"Help {targetPawn.Name.ToStringShort} with {selectedPawns.Count} pawns"
                : $"Help {targetPawn.Name.ToStringShort}";

            DebugHelpers.DebugLog("FloatMenuMakerMap", $"Creating menu option with label: {label}");

            // Define the action when the menu option is selected
            Action action = () =>
            {
                foreach (var helperPawn in selectedPawns)
                {
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                    helperPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"{helperPawn.Name} is now helping {targetPawn.Name}.");
                }
            };

            // Create and add the menu option
            FloatMenuOption helpOption = new FloatMenuOption(label, action);
            if (helpOption != null)
            {
                opts.Add(helpOption);
                DebugHelpers.DebugLog("FloatMenuMakerMap", $"Added 'Help' option to the float menu for {targetPawn.Name}.");
            }
            else
            {
                DebugHelpers.DebugLog("FloatMenuMakerMap", "Failed to create a 'Help' FloatMenuOption.");
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
