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
            // Log the initiation of the helping job
            Log.Message($"[Helpers Mod] {pawn.Name} is starting to help {TargetPawn.Name}.");

            // Fail if the target pawn becomes invalid
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
                        tickCounter = 3; // Reset the counter
                        // If the target pawn has moved, move the helper pawn to follow
                        if (TargetPawn.Position != pawn.Position)
                        {
                            Log.Message($"[Helpers Mod] {pawn.Name} is moving to follow {TargetPawn.Name}.");
                            pawn.pather.StartPath(TargetPawn, PathEndMode.Touch);
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never // Keeps the pawn in this toil indefinitely
            };
            followToil.AddFinishAction(() =>
            {
                // Log the completion of the follow toil
                Log.Message($"[Helpers Mod] {pawn.Name} has stopped helping {TargetPawn.Name}.");
            });
            yield return followToil;
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Skip reservation to allow multiple helpers
            return true;
        }
    }
}
