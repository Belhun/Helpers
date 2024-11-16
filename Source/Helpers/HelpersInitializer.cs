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
            DebugHelpers.DRWLog("Assembly loaded successfully.");
            var harmony = new Harmony("Belhun.helpersmod");
            harmony.PatchAll();
            DebugHelpers.DRWLog("Harmony patches applied successfully.");
        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            IntVec3 centerCell = IntVec3.FromVector3(clickPos);
            Pawn targetPawn = centerCell.GetFirstPawn(pawn.Map);

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
                DebugHelpers.FMMLog("No valid target pawn found in the clicked area.");
                return;
            }

            var selectedPawns = Find.Selector.SelectedObjects
                .OfType<Pawn>()
                .Where(p => p != targetPawn && p.IsColonistPlayerControlled)
                .ToList();

            DebugHelpers.FMMLog($"Selected pawns: {string.Join(", ", selectedPawns.Select(p => p.Name.ToStringShort))}");

            if (selectedPawns.Count == 0)
            {
                DebugHelpers.FMMLog("No valid helper pawns selected.");
                return;
            }

            string label = selectedPawns.Count > 1
                ? $"Help {targetPawn.Name.ToStringShort} with {selectedPawns.Count} pawns"
                : $"Help {targetPawn.Name.ToStringShort}";

            DebugHelpers.FMMLog($"Creating menu option with label: {label}");

            Action action = () =>
            {
                foreach (var helperPawn in selectedPawns)
                {
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                    helperPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    DebugHelpers.FMMLog($"{helperPawn.Name} is now helping {targetPawn.Name}.");
                }
            };

            FloatMenuOption helpOption = new FloatMenuOption(label, action);
            if (helpOption != null)
            {
                opts.Add(helpOption);
                DebugHelpers.FMMLog($"Added 'Help' option to the float menu for {targetPawn.Name}.");
            }
            else
            {
                DebugHelpers.FMMLog("Failed to create a 'Help' FloatMenuOption.");
            }
        }
    }

    [HarmonyPatch(typeof(Toils_Recipe), "DoRecipeWork")]
    public static class DoRecipeWorkOverridePatch
    {
        public static bool Prefix(ref Toil __result)
        {
            __result = Helpers.CustomToils_Recipe.DoRecipeWork_Helper();
            DebugHelpers.DRWLog("Replaced DoRecipeWork with custom DoRecipeWork_Helper.");
            return false;
        }
    }
}
