using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace Helpers
{
    public class SurgeryOutcomeComp_Helpers : SurgeryOutcomeComp
    {
        public override bool Affects(RecipeDef recipe, Pawn surgeon, Pawn patient, BodyPartRecord part)
        {
            // Check if the surgeon is being helped
            return surgeon.GetHelperComponent()?.IsBeingHelped == true;
        }

        public override void AffectQuality(RecipeDef recipe, Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill, ref float quality)
        {
            var helperComp = surgeon.GetHelperComponent();
            if (helperComp == null || !helperComp.IsBeingHelped) return;

            float totalContribution = 0f;

            // Calculate contributions from each helper
            foreach (var helper in helperComp.CurrentHelpers)
            {
                
                int helperSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Medicine")).Level;

                if (helperSkill >= 5)
                {
                    // Bonus for skilled helpers
                    totalContribution += helperSkill * 0.05f;
                }
                else
                {
                    // Penalty for unskilled helpers
                    totalContribution -= (5 - helperSkill) * 0.05f;
                }
            }

            // Clamp total contribution to avoid extreme values
            totalContribution = Mathf.Clamp(totalContribution, -0.5f, 0.5f);

            // Adjust surgery quality
            quality += totalContribution;

            // Debug log for tracking
            Log.Message($"[Helpers] Surgery quality adjusted by {totalContribution}. Final quality: {quality}");
        }
    }
}