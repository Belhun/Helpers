using LudeonTK;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Helpers.Testing
{
    public static class TestHelpers
    {

        [DebugAction("Helpers Debug", "Check Helper Component", allowedGameStates = AllowedGameStates.Playing)]
        public static void CheckHelperComponent()
        {
            Pawn selectedPawn = Find.Selector.SingleSelectedThing as Pawn;
            if (selectedPawn != null)
            {
                var helperComp = selectedPawn.GetHelperComponent();
                if (helperComp == null)
                {
                    Log.Message($"[Debug] {selectedPawn.Name} does not have a Helper Component.");
                    return;
                }

                string message = $"{selectedPawn.Name}:\n";
                message += $"Is Being Helped: {helperComp.IsBeingHelped}\n";
                message += $"Helpers ({helperComp.CurrentHelpers.Count}):\n";

                foreach (var helper in helperComp.CurrentHelpers)
                {
                    message += $"- {helper.Name}\n";
                }

                Log.Message(message);
            }
            else
            {
                Log.Warning("[Debug] No pawn selected.");
            }
        }

        [DebugAction("Helpers Debug", "Apply Leader Hediff", allowedGameStates = AllowedGameStates.Playing)]
        public static void ApplyLeaderHediff()
        {
            Pawn selectedPawn = Find.Selector.SingleSelectedThing as Pawn;
            if (selectedPawn != null)
            {
                Hediff leaderHediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("LeaderHediff"), selectedPawn);
                selectedPawn.health.AddHediff(leaderHediff);
                Log.Message($"[Debug] Leader Hediff applied to {selectedPawn.Name}");
            }
            else
            {
                Log.Warning("[Debug] No pawn selected to apply Leader Hediff.");
            }
        }
        [DebugAction("Helpers Debug", "Apply Test Work Speed Boost", allowedGameStates = AllowedGameStates.Playing)]
        public static void ApplyTestWorkSpeedBoost()
        {
            // Ensure the selected thing is a pawn
            Pawn selectedPawn = Find.Selector.SingleSelectedThing as Pawn;
            if (selectedPawn != null)
            {
                float test = selectedPawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
                Log.Message($"[Debug] WorkSpeedGlobal value for {selectedPawn.Name}: {test}");
                // Define the test Hediff
                HediffDef testBoostHediff = DefDatabase<HediffDef>.GetNamed("CustomStatBoost");

                // Apply the Hediff
                selectedPawn.health.AddHediff(testBoostHediff);
                Log.Message($"[Debug] Applied Work Speed Test Boost to {selectedPawn.Name}.");
                float workSpeed = selectedPawn.GetStatValue(StatDefOf.WorkSpeedGlobal);
                Log.Message($"[Debug] WorkSpeedGlobal value for {selectedPawn.Name}: {workSpeed}");

            }
            else
            {
                Log.Warning("[Debug] No pawn selected. Please select a pawn to apply the Work Speed Test Boost.");
            }
        }


        //[DebugAction("Helpers Debug", "Test SurgeryOutcomeComp_Helpers", allowedGameStates = AllowedGameStates.Playing)]
        //public static void TestSurgeryOutcomeCompHelpers()
        //{
        //    // Create a fake surgeon and patient
        //    Pawn surgeon = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
        //    Pawn patient = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

        //    // Add the PawnHelperComponent to the surgeon
        //    var helperComp = new PawnHelperComponent { parent = surgeon };
        //    surgeon.AllComps.Add(helperComp);

        //    // Create helper pawns
        //    var helper1 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
        //    var helper2 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

        //    // Assign skill levels to helpers
        //    helper1.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Medicine")).Level = 8; // High skill
        //    helper2.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Medicine")).Level = 3; // Low skill

        //    // Add helpers to the surgeon's component
        //    helperComp.AddHelper(helper1);
        //    helperComp.AddHelper(helper2);

        //    // Simulate surgery
        //    float quality = 1.0f; // Base quality
        //    var surgeryComp = new SurgeryOutcomeComp_Helpers();
        //    surgeryComp.AffectQuality(null, surgeon, patient, new List<Thing>(), null, null, ref quality);

        //    // Log the final result
        //    Log.Message($"[Test] Final surgery quality: {quality}");
        //}

        [DebugAction("Helpers Debug", "Check Stat Values", allowedGameStates = AllowedGameStates.Playing)]
        public static void CheckStatValues()
        {
            Pawn selectedPawn = Find.Selector.SingleSelectedThing as Pawn;
            if (selectedPawn != null)
            {
                StatDef[] statsToCheck = {
            StatDefOf.WorkSpeedGlobal,
            StatDefOf.MedicalTendSpeed,
            StatDefOf.MedicalTendQuality,
            StatDefOf.MedicalSurgerySuccessChance,
            StatDefOf.GeneralLaborSpeed,
            StatDefOf.MoveSpeed,
            StatDefOf.FoodPoisonChance,
            StatDefOf.MiningSpeed,
            StatDefOf.MiningYield,
            StatDefOf.PlantWorkSpeed,
            StatDefOf.PlantHarvestYield,
            StatDefOf.ConstructionSpeed,
            StatDefOf.ConstructSuccessChance
        };

                foreach (var stat in statsToCheck)
                {
                    float statValue = selectedPawn.GetStatValue(stat);
                    Log.Message($"[Debug] {stat.defName} value for {selectedPawn.Name}: {statValue}");
                }
            }
            else
            {
                Log.Warning("[Debug] No pawn selected.");
            }
        }

    }

}
