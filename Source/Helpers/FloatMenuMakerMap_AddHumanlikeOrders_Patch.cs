using System;
using System.Collections.Generic;
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
                foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(centerCell, Rot4.North, new IntVec2(1, 1)))
                {
                    targetPawn = cell.GetFirstPawn(pawn.Map);
                    if (targetPawn != null && targetPawn != pawn) break; // Break if a valid pawn is found
                }
            }

            // Ensure the target pawn is valid and not the same as the helper pawn
            if (targetPawn != null && pawn != targetPawn && pawn.IsColonistPlayerControlled)
            {
                // Define the label for the menu option
                string label = "Help " + targetPawn.Name.ToStringShort;

                // Define the action when the option is selected
                Action action = () =>
                {
                    // Create the Helping job targeting the selected pawn
                    Log.Message($"Selection: Helper Pawn: {pawn.Name}, Helped Pawn: {targetPawn.Name}");
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc); // Start the job

                };

                // Create and add the new menu option
                opts.Add(new FloatMenuOption(label, action));
            }
        }
    }
}
