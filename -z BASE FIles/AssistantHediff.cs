using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Helpers
{
    /// <summary>
    /// Custom HediffCompProperties class to define stat offsets for Assistant Hediff.
    /// </summary>
    public class AssistantHediffCompProperties_Stats : HediffCompProperties
    {
        // List of stat offsets to apply
        public List<StatModifier> statOffsets;

        public AssistantHediffCompProperties_Stats()
        {
            this.compClass = typeof(AssistantHediffComp_Stats);
        }
    }

    /// <summary>
    /// Custom HediffComp class for dynamically adjusting stats for the assistant.
    /// </summary>
    public class AssistantHediffComp_Stats : HediffComp
    {
        private Pawn leaderPawn;

        public void SetLeader(Pawn leader)
        {
            leaderPawn = leader;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (leaderPawn == null || leaderPawn.DestroyedOrNull() || leaderPawn.Dead)
            {
                ClearLeader();
                return;
            }

            // Ensure recursive calls are avoided
            if (IsRecursionGuardActive)
            {
                DebugHelpers.DebugLog("AssistantHediffComp_Stats", "Skipping CompPostTick due to recursion guard.");
                return;
            }

            try
            {
                ActivateRecursionGuard();
                UpdateAssistanceStats();
            }
            finally
            {
                DeactivateRecursionGuard();
            }
        }

        private void UpdateAssistanceStats()
        {
            if (leaderPawn == null) return;

            float leaderTendSpeed = leaderPawn.GetStatValue(StatDefOf.MedicalTendSpeed);
            float adjustment = leaderTendSpeed * 0.1f;

            this.parent.Severity = adjustment;
            DebugHelpers.DebugLog("AssistantHediffComp_Stats", $"Updated stats for {parent.pawn.Name} based on leader {leaderPawn.Name}: {adjustment}");
        }

        private void ClearLeader()
        {
            leaderPawn = null;
            this.parent.Severity = 0f;
            DebugHelpers.DebugLog("AssistantHediffComp_Stats", "Cleared leader from AssistantHediff.");
        }

        private static bool IsRecursionGuardActive => recursionGuard > 0;
        private static int recursionGuard = 0;

        private static void ActivateRecursionGuard() => recursionGuard++;

        private static void DeactivateRecursionGuard() => recursionGuard--;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref leaderPawn, "leaderPawn");
        }

        public override string CompLabelInBracketsExtra => leaderPawn != null ? $"Helping {leaderPawn.Name}" : "No Leader";
    }
}
