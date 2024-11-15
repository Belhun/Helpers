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
        static HelpersInitializer()
        {
            // Log message to confirm assembly loading
            Log.Message("[Helpers Mod] Assembly loaded successfully.");

            // Initialize Harmony
            var harmony = new Harmony("Belhun.helpersmod");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Convert the clicked position to an IntVec3 cell
            IntVec3 centerCell = IntVec3.FromVector3(clickPos);
            Pawn targetPawn = centerCell.GetFirstPawn(pawn.Map);

            // If no pawn is found at the clicked cell, search a 3x3 area around it
            if (targetPawn == null)
            {
                foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(centerCell, Rot4.North, new IntVec2(3, 3)))
                {
                    targetPawn = cell.GetFirstPawn(pawn.Map);
                    if (targetPawn != null && targetPawn != pawn) break; // Break if a valid pawn is found
                }
            }

            // Log whether a valid target pawn was found
            if (targetPawn != null)
            {
                
                //Log.Message($"[Helpers Mod] Target pawn found: {targetPawn.Name}");
            }
            else
            {
                //Log.Message("[Helpers Mod] No valid target pawn found in the clicked area.");
                return; // Exit if no valid target pawn
            }

            // Get selected pawns, excluding the right-clicked target pawn
            var selectedPawns = Find.Selector.SelectedObjects
                                     .OfType<Pawn>()
                                     .Where(p => p != targetPawn && p.IsColonistPlayerControlled)
                                     .ToList();

            // Log the selected pawns
            Log.Message($"[Helpers Mod] Selected pawns for help action: {string.Join(", ", selectedPawns.Select(p => p.Name.ToStringShort))}");

            // If no valid helper pawns are selected, return
            if (selectedPawns.Count == 0)
            {
                //Log.Message("[Helpers Mod] No valid helper pawns selected.");
                return;
            }

            // Define the label for the menu option based on how many pawns will help
            string label = selectedPawns.Count > 1
                ? $"Help {targetPawn.Name.ToStringShort} with {selectedPawns.Count} pawns"
                : $"Help {targetPawn.Name.ToStringShort}";

            Log.Message($"[Helpers Mod] Creating menu option with label: {label}");

            // Define the action when the option is selected
            Action action = () =>
            {
                foreach (var helperPawn in selectedPawns)
                {
                    // Create and assign the Helping job to each selected pawn
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                    helperPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc); // Start the job
                    Log.Message($"[Helpers Mod] {helperPawn.Name} is now helping {targetPawn.Name}.");
                }
            };

            // Create and add the new menu option, ensuring it's not null
            FloatMenuOption helpOption = new FloatMenuOption(label, action);
            if (helpOption != null)
            {
                opts.Add(helpOption);
                Log.Message($"[Helpers Mod] Successfully added 'Help' option to the float menu for {targetPawn.Name}.");
            }
            else
            {
                Log.Message("[Helpers Mod] Failed to create a 'Help' FloatMenuOption.");
            }
        }
    }

    [HarmonyPatch(typeof(Toils_Recipe), "DoRecipeWork")]
    public static class DoRecipeWorkOverridePatch
    {
        public static bool Prefix(ref Toil __result)
        {
            // Replace the original DoRecipeWork with the custom DoRecipeWork_Helper
            __result = Helpers.CustomToils_Recipe.DoRecipeWork_Helper();
            Log.Message("[Helpers Mod] Replaced DoRecipeWork with custom DoRecipeWork_Helper.");
            return false; // Skip the original method
        }
    }
}
