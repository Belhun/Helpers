using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Noise;

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
        public static float CalculateHelperContribution(Pawn actor, JobDriver jobDriver, RecipeDef recipeDef, List<Pawn> currentHelpers, StatDef workSpeedStat)
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
                    float contribution = 0.5f + (skillLevel / 40f);
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
        /// Calculates the total contribution of helpers to the cutting speed and awards experience to the helpers.
        /// </summary>
        /// <param name="actor">Main Pawn</param>
        /// <param name="jobDriver">The Cutting JobDriver</param>
        /// <param name="currentHelpers">A list of pawns that are helping the main pawn</param>
        /// <param name="workSpeedStat">The stat defining work speed for the task</param>
        /// <returns>The total contribution from all helpers</returns>
        public static float PlantsCHC(Pawn actor, JobDriver jobDriver, List<Pawn> currentHelpers, StatDef workSpeedStat)
        {
            SkillDef plantsSkillDef = DefDatabase<SkillDef>.GetNamed("Plants");
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
                    int skillLevel = helper.skills.GetSkill(plantsSkillDef)?.Level ?? 0;
                    float contribution = 0.5f + (skillLevel / 40f);
                    if (skillLevel < 5)
                    {
                        contribution -= 0.1f; // Penalty for low skill levels
                    }

                    // Calculate total contribution based on helper stats
                    float helperStatValue = helper.GetStatValue(workSpeedStat);
                    helperTotal += contribution * helperStatValue;

                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name}'s contribution: {contribution} (Stat Value: {helperStatValue})");

                    // Add experience to helper
                    helper.skills.Learn(plantsSkillDef, 0.1f);
                    helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Learn(0.1f);
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} gained experience in {plantsSkillDef.defName}");
                }
            }
            DebugHelpers.DebugLog("HelperMechanics", $"Total helper contribution to work speed: {helperTotal}");
            return helperTotal;
        }

        /// <summary>
        /// Calculates the total contribution of helpers to the mining speed and awards experience to the helpers.
        /// </summary>
        /// <param name="actor">Main Pawn</param>
        /// <param name="jobDriver">The Mining JobDriver</param>
        /// <param name="currentHelpers">A list of pawns that are helping the main pawn</param>
        /// <param name="workSpeedStat">The stat defining work speed for the task</param>
        /// <param name="mineTarget">The target being mined</param>
        /// <param name="buildingProp">The properties of the building being mined</param>
        /// <returns>The total contribution from all helpers</returns>
        public static float MineingCHC(Pawn actor, JobDriver_Mine jobDriver, List<Pawn> currentHelpers, StatDef workSpeedStat, Thing mineTarget, BuildingProperties buildingProp)
        {
            SkillDef mineSkillDef = DefDatabase<SkillDef>.GetNamed("Mining");
            float helperTotal = 0f;

            DebugHelpers.DebugLog("HelperMechanics", $"MineTarget: {mineTarget}");

            bool rockIsNaturalRock = buildingProp.isNaturalRock;
            DebugHelpers.DebugLog("HelperMechanics", $"Is Natural Rock: {rockIsNaturalRock}");

            int baseDamage = rockIsNaturalRock ? 80 : 40;
            var helperComp = jobDriver.pawn.GetHelperComponent();

            if (helperComp != null && helperComp.IsBeingHelped)
            {
                foreach (Pawn helper in currentHelpers)
                {
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} is assisting {actor.Name}");

                    // Apply social thoughts for this helper
                    HelperSocialMechanics.ApplySocialThoughts(helper, actor, currentHelpers);

                    // Calculate contribution based on helper skill
                    int skillLevel = helper.skills.GetSkill(mineSkillDef)?.Level ?? 0;

                    float contribution = 0.5f + (skillLevel / 40f);
                    if (skillLevel < 5)
                    {
                        contribution -= 0.1f; // Penalty for low skill levels
                    }

                    // Calculate total contribution based on helper stats
                    float helperStatValue = helper.GetStatValue(workSpeedStat);
                    helperTotal += contribution * helperStatValue;

                    int helperLevel = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                    int adjustedAmount = (int)(baseDamage * (0.5f + (helperLevel / 40f)));

                    Log.Message($"[Patch_PreFixMine] Base amount: {baseDamage}, Helper Level: {helperLevel}, Adjusted amount: {adjustedAmount}");

                    // Add experience to helper
                    helper.skills.Learn(mineSkillDef, 0.07f);
                    helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Learn(0.1f);
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} gained experience in {mineSkillDef.defName}");
                    DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name} gained experience in {DefDatabase<SkillDef>.GetNamed("Helping").defName}");
                }
            }
            DebugHelpers.DebugLog("HelperMechanics", $"Total helper contribution to work speed: {helperTotal}");
            return helperTotal;
        }

        public static int TranspilerMineingTTPCHC(Pawn actor, int ticksToPickHit)
        {
            var helperComp = actor.GetHelperComponent();
            if (helperComp != null && helperComp.IsBeingHelped)
            {
                foreach (Pawn helper in helperComp.CurrentHelpers)
                {
                    ticksToPickHit--;
                }
            }
            return ticksToPickHit;
        }

        /// <summary>
        /// Calculates the total contribution of helpers to the construction speed.
        /// </summary>
        /// <param name="actor">The main pawn performing the task.</param>
        /// <param name="frame">The construction frame being worked on.</param>
        /// <returns>The total contribution of the helpers to the construction speed.</returns>
        public static float HelperConstructionSpeed(Pawn actor, Frame frame)
        {
            var helperComp = actor.GetHelperComponent();
            List<Pawn> currentHelpers = helperComp.CurrentHelpers;
            if (!helperComp.IsBeingHelped || currentHelpers.Count == 0)
            {
                return 0f;
            }

            float totalHelperContribution = 0f;

            foreach (var helper in currentHelpers)
            {
                DebugHelpers.DebugLog("HelperMechanics", $"Calculating contribution for helper: {helper.Name}");

                float helperSpeed = helper.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f;

                if (frame.Stuff != null)
                {
                    helperSpeed *= frame.Stuff.GetStatValueAbstract(StatDefOf.ConstructionSpeedFactor);
                }

                DebugHelpers.DebugLog("HelperMechanics", $"{helper.Name}'s adjusted construction speed contribution: {helperSpeed}");
                totalHelperContribution += helperSpeed;
            }

            DebugHelpers.DebugLog("HelperMechanics", $"Total helper contribution to construction speed: {totalHelperContribution}");
            return totalHelperContribution;
        }

        public static void GrantSkillExperienceToHelpers(Pawn actor, SkillDef skill, float experienceAmount)
        {
            var helperComp = actor.GetHelperComponent();
            List<Pawn> currentHelpers = helperComp.CurrentHelpers;
            if (!helperComp.IsBeingHelped || currentHelpers.Count == 0)
            {
                return;
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
