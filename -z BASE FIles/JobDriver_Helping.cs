using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Helpers
{

    [DefOf]
    public static class SkillDefOf
    {
        public static SkillDef Helping;
        //public static string Name = "Helping";
        static SkillDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SkillDefOf));
        }
    }


    public class JobDriver_Helping : JobDriver
    {
        private Pawn TargetPawn => (Pawn)job.targetA.Thing; // The pawn being helped
        private Hediff assistantHediff;
        private Hediff leaderHediff;
        private int tickCounter = 0; // Counter to track ticks
        private const int checkInterval = 30; // Check every 10 ticks


        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Get helper component
            var targetHelperComp = TargetPawn.GetHelperComponent();
            if (targetHelperComp != null)
            {
                targetHelperComp.AddHelper(pawn);
                DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} added as a helper to {TargetPawn.Name}.");
            }

            // Apply AssistantHediff to the helper
            assistantHediff = HediffMaker.MakeHediff(HediffDef.Named("AssistantHediff"), pawn);
            assistantHediff.TryGetComp<AssistantHediffComp_Stats>().SetLeader(TargetPawn);
            pawn.health.AddHediff(assistantHediff);

            // Apply LeaderHediff to the target pawn
            leaderHediff = TargetPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("LeaderHediff"))
                           ?? HediffMaker.MakeHediff(HediffDef.Named("LeaderHediff"), TargetPawn);
            TargetPawn.health.AddHediff(leaderHediff);

            // Fail the job if the target pawn becomes invalid
            this.FailOnDespawnedOrNull(TargetIndex.A);

            // Go to the target pawn
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            // Follow the target pawn
            Toil followToil = new Toil
            {
                tickAction = () =>{
                    tickCounter++;
                    if (tickCounter >= checkInterval)
                    {
                        tickCounter = 0; // Reset the counter
                        targetHelperComp = TargetPawn.GetHelperComponent();
                        // If the target pawn moves, adjust the path accordingly
                        if (TargetPawn.Position != pawn.Position)
                        {
                            DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} is moving to follow {TargetPawn.Name}.");
                            pawn.pather.StartPath(TargetPawn, PathEndMode.Touch);
                        }

                        // Check if the helper is still in the target pawn's component

                        if (targetHelperComp.CurrentHelpers.Contains(pawn) == false)
                        {
                            targetHelperComp.AddHelper(pawn);
                        }

                        // Check if there are duplicate pawns in CurrentHelpers
                        if (targetHelperComp.CurrentHelpers.Count(p => p == pawn) > 1)
                        {
                            DebugHelpers.DebugLog("JobDriver_Helping", $"Duplicate helper {pawn.Name} found for {TargetPawn.Name}.");
                            targetHelperComp.RemoveHelper(pawn);
                        }
                    }
                    if (targetHelperComp == null || !targetHelperComp.CurrentHelpers.Contains(pawn))
                    {
                        targetHelperComp?.AddHelper(pawn);
                        DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} re-added as helper for {TargetPawn.Name}.");
                    }

                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            followToil.AddFinishAction(CleanupHelperRelationship);
            yield return followToil;
        }

        private void CleanupHelperRelationship()
        {
            var targetHelperComp = TargetPawn.GetHelperComponent();

            // Remove LeaderHediff from the target pawn
            if (leaderHediff != null)
            {
                TargetPawn.health.RemoveHediff(leaderHediff);
                DebugHelpers.DebugLog("JobDriver_Helping", $"Removed LeaderHediff from {TargetPawn.Name}.");
            }

            // Remove AssistantHediff from the helper
            if (assistantHediff != null)
            {
                pawn.health.RemoveHediff(assistantHediff);
                DebugHelpers.DebugLog("JobDriver_Helping", $"Removed AssistantHediff from {pawn.Name}.");
            }

            // Remove the helper from the target pawn's component
            targetHelperComp?.RemoveHelper(pawn);
            DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} has stopped helping {TargetPawn.Name}.");
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed) => true;
    }

}
