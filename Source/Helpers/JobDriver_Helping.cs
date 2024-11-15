﻿using System.Collections.Generic;
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
            Log.Message($"[Helpers Mod] {pawn.Name} is starting to help {TargetPawn.Name}.");

            // Attach the helper to the target pawn's component
            var targetHelperComp = TargetPawn.GetHelperComponent();
            if (targetHelperComp != null)
            {
                targetHelperComp.AddHelper(pawn);
                Log.Message($"[Helpers Mod] {pawn.Name} added as a helper to {TargetPawn.Name}.");
            }

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
                        tickCounter = 0; // Reset the counter

                        // Check if target pawn has moved and start a new path if necessary
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
                // Remove the helper from the target pawn's component upon finishing
                if (targetHelperComp != null)
                {
                    targetHelperComp.RemoveHelper(pawn);
                    Log.Message($"[Helpers Mod] {pawn.Name} has stopped helping {TargetPawn.Name}.");
                }
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
