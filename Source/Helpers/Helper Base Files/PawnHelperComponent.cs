using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Helpers
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("SpawnSetup")]
    public static class Pawn_SpawnSetup_Patch
    {
        /// <summary>
        /// Adds the PawnHelperComponent dynamically during the pawn's spawn setup.
        /// </summary>
        /// <param name="__instance">The pawn instance being set up.</param>
        /// <param name="respawningAfterLoad">Indicates if the pawn is being respawned after loading.</param>
        public static void Postfix(Pawn __instance, bool respawningAfterLoad)
        {
            DebugHelpers.DebugLog("Pawn_SpawnSetup", $"Pawn {__instance.Name} is spawning. Respawning after load: {respawningAfterLoad}");

            // Check if the pawn already has the PawnHelperComponent
            var existingHelperComponent = __instance.GetComp<PawnHelperComponent>();
            if (existingHelperComponent != null)
            {
                DebugHelpers.DebugLog("Pawn_SpawnSetup", $"{__instance.Name} already has PawnHelperComponent attached.");
            }
            else
            {
                // Add the PawnHelperComponent dynamically
                __instance.AllComps.Add(new PawnHelperComponent { parent = __instance });
                DebugHelpers.DebugLog("Pawn_SpawnSetup", $"Added PawnHelperComponent to {__instance.Name}");
            }
        }
    }

    public class PawnHelperComponent : ThingComp
    {
        public List<Pawn> CurrentHelpers = new List<Pawn>();
        public bool IsBeingHelped => CurrentHelpers.Count > 0;

        /// <summary>
        /// Adds a helper to the pawn's helper list.
        /// </summary>
        public void AddHelper(Pawn helper)
        {
            if (!CurrentHelpers.Contains(helper))
            {
                CurrentHelpers.Add(helper);
                DebugHelpers.DebugLog("PawnHelperComponent", $"{helper.Name} is now helping {parent.LabelCap}.");
            }
        }

        /// <summary>
        /// Removes a helper from the pawn's helper list.
        /// </summary>
        public void RemoveHelper(Pawn helper)
        {
            if (CurrentHelpers.Remove(helper))
            {
                DebugHelpers.DebugLog("PawnHelperComponent", $"{helper.Name} stopped helping {parent.LabelCap}. Remaining helpers: {CurrentHelpers.Count}");
            }
        }

        /// <summary>
        /// Clears all helpers from the list (e.g., when the job ends).
        /// </summary>
        public void ClearHelpers()
        {
            DebugHelpers.DebugLog("PawnHelperComponent", $"{parent.LabelCap} cleared all helpers.");
            CurrentHelpers.Clear();
        }

        /// <summary>
        /// Saves and loads the component data.
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref CurrentHelpers, "CurrentHelpers", LookMode.Reference);
        }


        /// <summary>
        /// Validates the CurrentHelpers list by removing invalid helpers.
        /// </summary>
        public void ValidateHelpers()
        {
            if (CurrentHelpers.Count == 0)
            {
                DebugHelpers.DebugLog("PawnHelperComponent", $"{parent.LabelCap} has no helpers. Validation not needed.");
                return;
            }

            // Track helpers to remove
            var helpersToRemove = new List<Pawn>();

            foreach (var helper in CurrentHelpers)
            {
                // Check if the helper is doing the Helping job
                if (helper.jobs?.curJob?.def != DefDatabase<JobDef>.GetNamed("Helping"))
                {
                    DebugHelpers.DebugLog("PawnHelperComponent", $"{helper.Name} is not doing the Helping job and will be removed.");
                    helpersToRemove.Add(helper);
                    continue;
                }

                // Check if the helper's target is this pawn
                var targetPawn = helper.jobs.curJob.targetA.Thing as Pawn;
                if (targetPawn != parent)
                {
                    DebugHelpers.DebugLog("PawnHelperComponent", $"{helper.Name} is not helping the expected pawn {parent.LabelCap} and will be removed.");
                    helpersToRemove.Add(helper);
                }
            }

            // Remove invalid helpers
            foreach (var helper in helpersToRemove)
            {
                RemoveHelper(helper);
                DebugHelpers.DebugLog("PawnHelperComponent", $"{helper.Name} has been removed from {parent.LabelCap}'s helper list.");
            }
        }
    }


    public static class PawnHelperExtensions
    {
        /// <summary>
        /// Retrieves the PawnHelperComponent for a pawn.
        /// </summary>
        public static PawnHelperComponent GetHelperComponent(this Pawn pawn)
        {
            var helperComponent = pawn.GetComp<PawnHelperComponent>();
            if (helperComponent == null)
            {
                DebugHelpers.DebugLog("PawnHelperExtensions", $"{pawn.LabelCap} does not have a PawnHelperComponent.");
            }
            else
            {
                DebugHelpers.DebugLog("PawnHelperExtensions", $"{pawn.LabelCap} successfully retrieved a PawnHelperComponent.");
            }
            return helperComponent;
        }
    }

    public class CompProperties_PawnHelper : CompProperties
    {
        /// <summary>
        /// Initializes the component properties for the PawnHelperComponent.
        /// </summary>
        public CompProperties_PawnHelper()
        {
            this.compClass = typeof(PawnHelperComponent);
            DebugHelpers.DebugLog("CompProperties_PawnHelper", "CompProperties_PawnHelper initialized.");
        }
    }
}
