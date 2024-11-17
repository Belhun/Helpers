using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Helpers
{
    public static class HelperMechanics
    {
        public static float CalculateHelperContribution(
            Pawn actor,
            JobDriver jobDriver,
            RecipeDef recipeDef,
            List<Pawn> currentHelpers,
            StatDef workSpeedStat)
        {
            float helperTotal = 0f;

            foreach (Pawn helper in currentHelpers)
            {
                DebugHelpers.HMLog($"{helper.Name} is assisting {actor.Name}");

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
                helperTotal += contribution * helper.GetStatValue(workSpeedStat);

                // Add experience to helper
                helper.skills.Learn(recipeDef.workSkill, 0.1f * recipeDef.workSkillLearnFactor);
            }

            DebugHelpers.HMLog($"Total helper contribution to work speed: {helperTotal}");
            return helperTotal;
        }
    }
}
