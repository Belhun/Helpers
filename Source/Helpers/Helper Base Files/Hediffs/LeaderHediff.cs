using Verse;
using RimWorld;
using System.Runtime.Remoting.Messaging;
using System;
using UnityEngine;

namespace Helpers
{
    

    public class StatPart_HelpersMovementSpeed : StatPart
    {
        private const float MovementSpeedPerHelper = 10f; // Define the speed boost per helper

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

            var helperComp = pawn.GetHelperComponent();
            helperComp?.ValidateHelpers();
            if (helperComp == null || !helperComp.IsBeingHelped) return;
            
            val += helperComp.CurrentHelpers.Count * MovementSpeedPerHelper;

            DebugHelpers.DebugLog("StatPart_MovementSpeed", $"Adjusted MoveSpeed for {pawn.Name}: {val} (Helpers: {helperComp.CurrentHelpers.Count})");
        }
        

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

            // Get the helper component from the pawn
            var helperComp = pawn.GetHelperComponent();
            helperComp?.ValidateHelpers();
            if (helperComp == null || !helperComp.IsBeingHelped) return null;
            // Return the explanation for the movement speed adjustment
            return $"Helpers: +{helperComp.CurrentHelpers.Count * MovementSpeedPerHelper} movement speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersMedicalTendSpeed : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {

            if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

            var helperComp = pawn.GetHelperComponent();
            helperComp?.ValidateHelpers();
            if (helperComp == null || !helperComp.IsBeingHelped) return;

            float totalContribution = 0f;
            foreach (var helper in helperComp.CurrentHelpers)
            {
                // Get the helper's MedicalTendSpeed stat
                float helperTendSpeed = helper.GetStatValue(StatDefOf.MedicalTendSpeed);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperTendSpeed * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - MedicalTendSpeed: {helperTendSpeed * 100:F1}%");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - Helping Skill: {helpingSkill}, Contribution Factor: {contributionFactor:F2}");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - Contributed: {helperTendSpeed * contributionFactor * 100:F1}%");
            }   


            // Add the total contribution to the primary pawn's stat value
            val += totalContribution;

            DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $" Adjusted MedicalTendSpeed for {pawn.Name}: {val} (Helpers: {helperComp.CurrentHelpers.Count})");
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;


            var helperComp = pawn.GetHelperComponent();
            helperComp?.ValidateHelpers();
            if (helperComp == null || !helperComp.IsBeingHelped) return null;
            float totalContribution = 0f;
            foreach (var helper in helperComp.CurrentHelpers)
            {
                // Get the helper's MedicalTendSpeed stat
                float helperTendSpeed = helper.GetStatValue(StatDefOf.MedicalTendSpeed);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperTendSpeed * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - MedicalTendSpeed: {helperTendSpeed * 100:F1}%");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - Helping Skill: {helpingSkill}, Contribution Factor: {contributionFactor:F2}");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"Helper {helper.Name} - Contributed: {helperTendSpeed * contributionFactor * 100:F1}%");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendSpeed", $"");
            }


            return $"Helpers: +{totalContribution * 100:F1}% Medical Tend Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }


}
