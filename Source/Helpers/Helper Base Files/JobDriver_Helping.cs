using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Helpers
{
    public class JobDriver_Helping : JobDriver
    {
        private Pawn TargetPawn => (Pawn)job.targetA.Thing; // The pawn being helped
        private int tickCounter = 0; // Counter to track ticks
        private const int checkInterval = 10; // Check every 10 ticks

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} is starting to help {TargetPawn.Name}.");

            // Attach the helper to the target pawn's component
            var targetHelperComp = TargetPawn.GetHelperComponent();
            if (targetHelperComp != null)
            {
                targetHelperComp.AddHelper(pawn);
                DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} added as a helper to {TargetPawn.Name}.");
            }

            // Fail the job if the target pawn becomes invalid
            this.FailOnDespawnedOrNull(TargetIndex.A);

            // Go to the target pawn
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            // Follow the target pawn
            Toil followToil = new Toil
            {
                tickAction = () =>
                {
                    tickCounter++;
                    if (tickCounter >= checkInterval)
                    {
                        tickCounter = 0; // Reset the counter

                        // If the target pawn moves, adjust the path accordingly
                        if (TargetPawn.Position != pawn.Position)
                        {
                            DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} is moving to follow {TargetPawn.Name}.");
                            pawn.pather.StartPath(TargetPawn, PathEndMode.Touch);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never // Keeps the pawn in this toil indefinitely
            };

            followToil.AddFinishAction(() =>
            {
                // Remove the helper from the target pawn's component upon finishing
                if (targetHelperComp != null)
                {
                    targetHelperComp.RemoveHelper(pawn);
                    DebugHelpers.DebugLog("JobDriver_Helping", $"{pawn.Name} has stopped helping {TargetPawn.Name}.");
                }
            });

            yield return followToil;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Allow multiple helpers by skipping reservations
            return true;
        }
    }
}
