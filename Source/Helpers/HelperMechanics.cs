using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Helpers
{
    public static class HelperMechanics
    {
        /// <summary>
        /// Calculates the contribution of helper pawns to the work speed of the main actor.
        /// </summary>
        /// <param name="actor">The primary pawn performing the job.</param>
        /// <param name="jobDriver">The job driver for the task.</param>
        /// <param name="recipeDef">The recipe definition for the task.</param>
        /// <param name="currentHelpers">A list of pawns assisting the actor.</param>
        /// <param name="workSpeedStat">The stat defining work speed for the task.</param>
        /// <returns>The total contribution from all helpers.</returns>
        public static float CalculateHelperContribution(Pawn actor,JobDriver jobDriver,RecipeDef recipeDef,List<Pawn> currentHelpers,StatDef workSpeedStat)
        {

            float helperTotal = 0f;
            var helperComp = jobDriver.pawn.GetHelperComponent();
            if (helperComp != null && helperComp.IsBeingHelped)
            {
                foreach (Pawn helper in currentHelpers)
                {
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} is assisting {actor.Name}");

                    // Apply social thoughts for this helper
                    HelperSocialMechanics.ApplySocialThoughts(helper, actor, currentHelpers);

                    // Calculate contribution based on helper skill
                    int skillLevel = helper.skills.GetSkill(recipeDef.workSkill)?.Level ?? 0;

                    float contribution = (0.5f + (skillLevel / 40f));
                    if (skillLevel < 5)
                    {
                        contribution -= 0.1f; // Penalty for low skill levels
                    }

                    // Calculate total contribution based on helper stats
                    float helperStatValue = helper.GetStatValue(workSpeedStat);
                    helperTotal += contribution * helperStatValue;

                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name}'s contribution: {contribution} (Stat Value: {helperStatValue})");

                    // Add experience to helper
                    helper.skills.Learn(recipeDef.workSkill, 0.1f * recipeDef.workSkillLearnFactor);
                    helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Learn(0.1f * recipeDef.workSkillLearnFactor);
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} gained experience in {recipeDef.workSkill.defName}");
                }
            }
            DebugHelpers.DebugLog("HelperMechanics", $"Total helper contribution to work speed: {helperTotal}");
            return helperTotal;
        }

        /// <summary>
        /// Calculates the total contribution of helpers to the construction speed.
        /// </summary>
        /// <param name="actor">The main pawn performing the task.</param>
        /// <param name="frame">The construction frame being worked on.</param>
        /// <param name="currentHelpers">List of pawns currently helping the actor.</param>
        /// <returns>The total contribution of the helpers to the construction speed.</returns>
        public static float HelperConstructionSpeed(Pawn actor, Frame frame)
        {
            var helperComp = actor.GetHelperComponent();
            List<Pawn> currentHelpers = helperComp.CurrentHelpers;
            if (helperComp.IsBeingHelped == false || currentHelpers.Count == 0)
            {
                //DebugHelpers.DebugLog("HelperMechanics", "Not Being helped.");
                return 0f;
            }

            float totalHelperContribution = 0f;

            foreach (var helper in currentHelpers)
            {
                DebugHelpers.DebugLog("HelperMechanics", $"Calculating contribution for helper: {helper.Name}");

                // Base construction speed of the helper
                float helperSpeed = helper.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f;

                // Adjust speed if the frame has material (Stuff)
                if (frame.Stuff != null)
                {
                    helperSpeed *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
                }

                DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name}'s adjusted construction speed contribution: {helperSpeed}");

                // Add the helper's speed contribution to the total
                totalHelperContribution += helperSpeed;
            }

            DebugHelpers.DebugLog("HelperMechanics", $"Total helper contribution to construction speed: {totalHelperContribution}");
            return totalHelperContribution;
        }

        
        public static void GrantSkillExperienceToHelpers(Pawn actor, SkillDef skill, float experienceAmount)
        {
            var helperComp = actor.GetHelperComponent();
            List<Pawn> currentHelpers = helperComp.CurrentHelpers;
            if (helperComp.IsBeingHelped == false || currentHelpers.Count == 0)
            {
                //DebugHelpers.DebugLog("HelperMechanics", "Not Being helped.");
                return ;
            }

            foreach (var helper in currentHelpers)
            {
                if (helper.skills != null)
                {
                    helper.skills.Learn(skill, experienceAmount);
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} gained {experienceAmount} experience in {skill.defName} while helping {actor.Name}.");
                }
            }
        }

    }
}
