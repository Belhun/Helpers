using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace Helpers
{
    /// <summary>
    /// Custom HediffCompProperties class to define stat offsets for Leader Hediff.
    /// </summary>
    public class LeaderHediffCompProperties_Stats : HediffCompProperties
    {
        // List of stat offsets to apply
        public List<StatModifier> statOffsets;


        public LeaderHediffCompProperties_Stats()
        {
            this.compClass = typeof(LeaderHediffComp_Stats);
        }
    }


    /// <summary>
    /// Custom HediffComp class for dynamically adjusting stats.
    /// </summary>
    public class LeaderHediffComp_Stats : HediffComp
    {
        // Cached reference to the properties
        public LeaderHediffCompProperties_Stats Props => (LeaderHediffCompProperties_Stats)this.props;

        // Dictionary to track contributions from helpers
        private Dictionary<Pawn, float> helperContributions = new Dictionary<Pawn, float>();

        /// <summary>
        /// Adjusts stats dynamically based on helpers' contributions.
        /// </summary>
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Props.statOffsets != null && Props.statOffsets.Count > 0)
            {
                foreach (var offset in Props.statOffsets)
                {
                    Log.Message($"[LeaderHediffComp_Stats] Stat: {offset.stat.defName}, Offset: {offset.value}");

                }
            }
            else
            {
                Log.Warning("[LeaderHediffComp_Stats] No statOffsets found in Props.");
            }
            if (Props.statOffsets != null && Props.statOffsets.Count > 0)
            {
                foreach (var offset in Props.statOffsets)
                {
                    var statValue = parent.pawn.GetStatValue(offset.stat);
                    Log.Message($"[LeaderHediffComp_Stats] Stat: {offset.stat.defName}, Original Value: {statValue}, Offset: {offset.value}, New Value: {statValue + offset.value}");

                }
            }

            // Ensure the parent pawn has a HelperComponent
            var helperComp = this.parent.pawn.GetHelperComponent();
            if (helperComp == null || !helperComp.IsBeingHelped)
            {
                ClearContributions();
                return;
            }

            // Recalculate contributions from all helpers
            UpdateContributions(helperComp.CurrentHelpers, helperComp);
        }

        /// <summary>
        /// Updates the contributions of helpers to the parent pawn's stats.
        /// </summary>
        /// <param name="helpers">List of helper pawns.</param>
        private void UpdateContributions(List<Pawn> helpers, PawnHelperComponent helperComp)
        {
            if (Props.statOffsets != null && Props.statOffsets.Count > 0)
            {
                foreach (var offset in Props.statOffsets)
                {
                    var statValue = this.parent.pawn.GetStatValue(offset.stat);
                    Log.Message($"[LeaderHediffComp_Stats] Stat: {offset.stat.defName}, Original Value: {statValue}, Offset: {offset.value}, New Value: {statValue + offset.value}");
                    
                }
            }
        }

        // Separate method for applying contributions locally
        private void ApplyContributionEffects(float totalContribution)
        {
            // Modify stats or severity locally for this pawn
            this.parent.Severity = totalContribution;
        }



        /// <summary>
        /// Clears all contributions when no helpers remain.
        /// </summary>
        private void ClearContributions()
        {
            helperContributions.Clear();
            // Optionally reset the stat adjustments to default
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look(ref helperContributions, "helperContributions", LookMode.Reference, LookMode.Value);
        }

        public override string CompLabelInBracketsExtra => helperContributions.Count > 0
            ? $"Helpers: {helperContributions.Count}"
            : null;
    }
}
