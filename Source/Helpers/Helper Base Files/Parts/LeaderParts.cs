using RimWorld;
using Verse;

namespace Helpers
{


    public class StatPart_HelpersMovementSpeed : StatPart
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
                float helperMovementSpeed = helper.GetStatValue(StatDefOf.MoveSpeed);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperMovementSpeed * contributionFactor;

                DebugHelpers.DebugLog("StatPart_MovementSpeed", $"Helper {helper.Name} - MovementSpeed: {helperMovementSpeed * 100:F1}%");
                DebugHelpers.DebugLog("StatPart_MovementSpeed", $"Helper {helper.Name} - Helping Skill: {helpingSkill}, Contribution Factor: {contributionFactor:F2}");
                DebugHelpers.DebugLog("StatPart_MovementSpeed", $"Helper {helper.Name} - Contributed: {helperMovementSpeed * contributionFactor * 100:F1}%");
            }


            // Add the total contribution to the primary pawn's stat value
            val += totalContribution;

            DebugHelpers.DebugLog("StatPart_MovementSpeed", $" Adjusted MedicalTendSpeed for {pawn.Name}: {val} (Helpers: {helperComp.CurrentHelpers.Count})");
        }


        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

            // Get the helper component from the pawn
            var helperComp = pawn.GetHelperComponent();
            helperComp?.ValidateHelpers();
            if (helperComp == null || !helperComp.IsBeingHelped) return null;

            float totalContribution = 0f;
            foreach (var helper in helperComp.CurrentHelpers)
            {
                // Get the helper's MedicalTendSpeed stat
                float helperMovementSpeed = helper.GetStatValue(StatDefOf.MoveSpeed);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperMovementSpeed * contributionFactor;
            }


            // Return the explanation for the movement speed adjustment
            return $"Helpers: +{totalContribution} movement speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }
    // disabled for now, due to overpowerness, as this stacks with all the other work speed bonuses
    public class StatPart_HelpersWorkSpeed : StatPart
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
            float helperMovementSpeed = helper.GetStatValue(StatDefOf.WorkSpeedGlobal);

            // Scale contribution based on the helper's Helping skill level
            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
            float contributionFactor = 0.2f + (helpingSkill / 40f);

            totalContribution += helperMovementSpeed * contributionFactor;

            DebugHelpers.DebugLog("StatPart_HelpersWorkSpeed", $"Helper {helper.Name} - WorkSpeedGlobal: {helperMovementSpeed * 100:F1}%");
            DebugHelpers.DebugLog("StatPart_HelpersWorkSpeed", $"Helper {helper.Name} - Helping Skill: {helpingSkill}, Contribution Factor: {contributionFactor:F2}");
            DebugHelpers.DebugLog("StatPart_HelpersWorkSpeed", $"Helper {helper.Name} - Contributed: {helperMovementSpeed * contributionFactor * 100:F1}%");
        }


        // Add the total contribution to the primary pawn's stat value
        val += totalContribution;

        DebugHelpers.DebugLog("StatPart_HelpersWorkSpeed", $" Adjusted MedicalTendSpeed for {pawn.Name}: {val} (Helpers: {helperComp.CurrentHelpers.Count})");
    }



    public override string ExplanationPart(StatRequest req)
    {
        if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

        // Get the helper component from the pawn
        var helperComp = pawn.GetHelperComponent();
        helperComp?.ValidateHelpers();
        if (helperComp == null || !helperComp.IsBeingHelped) return null;

        float totalContribution = 0f;
        foreach (var helper in helperComp.CurrentHelpers)
        {
            // Get the helper's MedicalTendSpeed stat
            float helperMovementSpeed = helper.GetStatValue(StatDefOf.WorkSpeedGlobal);

            // Scale contribution based on the helper's Helping skill level
            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
            float contributionFactor = 0.5f + (helpingSkill / 40f);

            totalContribution += helperMovementSpeed * contributionFactor;
        }


        // Return the explanation for the movement speed adjustment
        return $"Helpers: +{totalContribution * 100:F1}% Global Work Speed ({helperComp.CurrentHelpers.Count} helpers)";
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
            }


            return $"Helpers: +{totalContribution * 100:F1}% Medical Tend Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersMedicalTendQuality : StatPart
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
                // Get the helper's MedicalTendQuality stat
                float helperTendSpeed = helper.GetStatValue(StatDefOf.MedicalTendQuality);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperTendSpeed * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendQuality", $"Helper {helper.Name} - MedicalTendQuality: {helperTendSpeed * 100:F1}%");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendQuality", $"Helper {helper.Name} - Helping Skill: {helpingSkill}, Contribution Factor: {contributionFactor:F2}");
                DebugHelpers.DebugLog("StatPart_HelpersMedicalTendQuality", $"Helper {helper.Name} - Contributed: {helperTendSpeed * contributionFactor * 100:F1}%");
            }


            // Add the total contribution to the primary pawn's stat value
            val += totalContribution;

            DebugHelpers.DebugLog("StatPart_HelpersMedicalTendQuality", $" Adjusted MedicalTendSpeed for {pawn.Name}: {val} (Helpers: {helperComp.CurrentHelpers.Count})");
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
                // Get the helper's MedicalTendQuality stat
                float helperTendSpeed = helper.GetStatValue(StatDefOf.MedicalTendQuality);

                // Scale contribution based on the helper's Helping skill level
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperTendSpeed * contributionFactor;
            }


            return $"Helpers: +{totalContribution * 100:F1}% Medical Tend Quaility ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }


    public class StatPart_HelpersMedicalSurgerySuccessChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersMedicalSurgerySuccessChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to MedicalSurgerySuccessChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersMedicalSurgerySuccessChance", $"Adjusted MedicalSurgerySuccessChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.MedicalSurgerySuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Surgery Success Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersGeneralLaborSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.GeneralLaborSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersGeneralLaborSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to GeneralLaborSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersGeneralLaborSpeed", $"Adjusted GeneralLaborSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.GeneralLaborSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% General Labor Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }


    //public class StatPart_HelpersFoodPoisonChance : StatPart
    //{
    //    public override void TransformValue(StatRequest req, ref float val)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.GetStatValue(StatDefOf.FoodPoisonChance);
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;

    //            DebugHelpers.DebugLog("StatPart_HelpersFoodPoisonChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to FoodPoisonChance.");
    //        }

    //        val += totalContribution;
    //        DebugHelpers.DebugLog("StatPart_HelpersFoodPoisonChance", $"Adjusted FoodPoisonChance for {pawn.Name}: {val}");
    //    }

    //    public override string ExplanationPart(StatRequest req)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return null;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.GetStatValue(StatDefOf.FoodPoisonChance);
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;
    //        }

    //        return $"Helpers: +{totalContribution * 100:F1}% Food Poison Chance ({helperComp.CurrentHelpers.Count} helpers)";
    //    }
    //}

    public class StatPart_HelpersMiningSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.MiningSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersMiningSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to MiningSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersMiningSpeed", $"Adjusted MiningSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.MiningSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Mining Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersPlantWorkSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.PlantWorkSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersPlantWorkSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to PlantWorkSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersPlantWorkSpeed", $"Adjusted PlantWorkSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.PlantWorkSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Plant Work Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersConstructionSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.ConstructionSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersConstructionSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ConstructionSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersConstructionSpeed", $"Adjusted ConstructionSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.ConstructionSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Construction Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersConstructSuccessChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.ConstructSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersConstructSuccessChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ConstructSuccessChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersConstructSuccessChance", $"Adjusted ConstructSuccessChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.ConstructSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Construct Success Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersCarryingCapacity : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.CarryingCapacity);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersCarryingCapacity", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ConstructSuccessChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersCarryingCapacity", $"Adjusted HelpersCarryingCapacity for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.CarryingCapacity);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution:F1}% Carrying Capacity ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersCookingSpeed : StatPart
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
                float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Cooking")).Level;
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);
                float multiplier = helperStat * 10f;

                totalContribution += multiplier * contributionFactor;
                DebugHelpers.DebugLog("StatPart_HelpersCarryingCapacity", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ConstructSuccessChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersCarryingCapacity", $"Adjusted HelpersCarryingCapacity for {pawn.Name}: {val}");
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
                float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Cooking")).Level;
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);
                float multiplier = helperStat * 10f;

                totalContribution += multiplier * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Cooking Speed  ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }


    //The Values that are Shown to the user are wrong, but i belive thye acctual ones are working, 
    //Will need to make sure that the User is awear of how much it helpes to have a pawn helping arrest someone
    public class StatPart_HelpersArrestSuccessChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.ArrestSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersArrestSuccessChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ArrestSuccessChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersArrestSuccessChance", $"Adjusted ArrestSuccessChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.ArrestSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Arrest Success Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }
    // test all Below

    //The Values that are Shown to the user are wrong, but i belive thye acctual ones are working, 
    //Will need to make sure that the User is awear of how much it helpes to have a pawn helping Fixing chance
    public class StatPart_HelpersFixConstructionChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.FixBrokenDownBuildingSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersFixConstructionChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to FixConstructionChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersFixConstructionChance", $"Adjusted FixConstructionChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.FixBrokenDownBuildingSuccessChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Fix Construction Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersHackingSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.HackingSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersHackingSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to HackingSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersHackingSpeed", $"Adjusted HackingSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.HackingSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Hacking Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersAnimalGatherSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.AnimalGatherSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersAnimalGatherSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to AnimalGatherSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersAnimalGatherSpeed", $"Adjusted AnimalGatherSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.AnimalGatherSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Animal Gather Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersSmoothingSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.SmoothingSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersSmoothingSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to SmoothingSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersSmoothingSpeed", $"Adjusted SmoothingSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.SmoothingSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Smoothing Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }


    // this is consisdered a phycic task, maby in the future we can limit help to those who are phycic
    public class StatPart_HelpersPruningSpeed : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.PruningSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersPruningSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to PruningSpeed.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersPruningSpeed", $"Adjusted PruningSpeed for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.PruningSpeed);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution:F1}% Pruning Speed ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersTameAnimalChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.TameAnimalChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersTameAnimalChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to TameAnimalChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersTameAnimalChance", $"Adjusted TameAnimalChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.TameAnimalChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Tame Animal Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersTrainAnimalChance : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.TrainAnimalChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersTrainAnimalChance", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to TrainAnimalChance.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersTrainAnimalChance", $"Adjusted TrainAnimalChance for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.TrainAnimalChance);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Train Animal Chance ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }

    public class StatPart_HelpersSuppressionPower : StatPart
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
                float helperStat = helper.GetStatValue(StatDefOf.SuppressionPower);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;

                DebugHelpers.DebugLog("StatPart_HelpersSuppressionPower", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to SuppressionPower.");
            }

            val += totalContribution;
            DebugHelpers.DebugLog("StatPart_HelpersSuppressionPower", $"Adjusted SuppressionPower for {pawn.Name}: {val}");
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
                float helperStat = helper.GetStatValue(StatDefOf.SuppressionPower);
                int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
                float contributionFactor = 0.5f + (helpingSkill / 40f);

                totalContribution += helperStat * contributionFactor;
            }

            return $"Helpers: +{totalContribution * 100:F1}% Suppression Power ({helperComp.CurrentHelpers.Count} helpers)";
        }
    }
    //public class StatPart_HelpersButcherySpeed : StatPart
    //{
    //    public override void TransformValue(StatRequest req, ref float val)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Cooking")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;
    //            DebugHelpers.DebugLog("StatPart_HelpersButcherySpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to ButcherySpeed.");
    //        }

    //        val += totalContribution;
    //        DebugHelpers.DebugLog("StatPart_HelpersButcherySpeed", $"Adjusted ButcherySpeed for {pawn.Name}: {val}");
    //    }

    //    public override string ExplanationPart(StatRequest req)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return null;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Cooking")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;
    //        }

    //        return $"Helpers: +{totalContribution:F1}% Butchery Speed ({helperComp.CurrentHelpers.Count} helpers)";
    //    }
    //}

    //public class StatPart_HelpersMechanoidShreddingSpeed : StatPart
    //{
    //    public override void TransformValue(StatRequest req, ref float val)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Crafting")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;
    //            DebugHelpers.DebugLog("StatPart_HelpersMechanoidShreddingSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to MechanoidShreddingSpeed.");
    //        }

    //        val += totalContribution;
    //        DebugHelpers.DebugLog("StatPart_HelpersMechanoidShreddingSpeed", $"Adjusted MechanoidShreddingSpeed for {pawn.Name}: {val}");
    //    }

    //    public override string ExplanationPart(StatRequest req)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return null;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Crafting")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);

    //            totalContribution += helperStat * contributionFactor;
    //        }

    //        return $"Helpers: +{totalContribution:F1}% Mechanoid Shredding Speed ({helperComp.CurrentHelpers.Count} helpers)";
    //    }
    //}
    //// since am useing 
    //public class StatPart_HelperSmeltingSpeed : StatPart
    //{
    //    public StatPart_HelperSmeltingSpeed() { } // Ensure this constructor exists
    //    public override void TransformValue(StatRequest req, ref float val)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Crafting")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);
    //            totalContribution += helperStat * contributionFactor;

    //            DebugHelpers.DebugLog("StatPart_HelperSmeltingSpeed", $"{helper.Name} contributed {helperStat * contributionFactor:F2} to SmeltingSpeed.");
    //        }

    //        val += totalContribution;
    //        DebugHelpers.DebugLog("StatPart_HelperSmeltingSpeed", $"Adjusted SmeltingSpeed for {pawn.Name}: {val}");
    //    }

    //    public override string ExplanationPart(StatRequest req)
    //    {
    //        if (!req.HasThing || !(req.Thing is Pawn pawn)) return null;

    //        var helperComp = pawn.GetHelperComponent();
    //        helperComp?.ValidateHelpers();
    //        if (helperComp == null || !helperComp.IsBeingHelped) return null;

    //        float totalContribution = 0f;
    //        foreach (var helper in helperComp.CurrentHelpers)
    //        {
    //            float helperStat = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Crafting")).Level;
    //            int helpingSkill = helper.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Helping")).Level;
    //            float contributionFactor = 0.5f + (helpingSkill / 40f);
    //            totalContribution += helperStat * contributionFactor;
    //        }

    //        return $"Helpers: +{totalContribution:F1}% Smelting Speed ({helperComp.CurrentHelpers.Count} helpers)";
    //    }
    //}
}


