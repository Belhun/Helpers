using LudeonTK;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Helpers.Testing
{
    public static class TestHelpers
    {
        [DebugAction("Helpers Debug", "Test SurgeryOutcomeComp_Helpers", allowedGameStates = AllowedGameStates.Playing)]
        public static void TestSurgeryOutcomeCompHelpers()
        {
            // Create a fake surgeon and patient
            Pawn surgeon = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
            Pawn patient = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

            // Add the PawnHelperComponent to the surgeon
            var helperComp = new PawnHelperComponent { parent = surgeon };
            surgeon.AllComps.Add(helperComp);

            // Create helper pawns
            var helper1 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
            var helper2 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

            // Assign skill levels to helpers
            helper1.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Medicine")).Level = 8; // High skill
            helper2.skills.GetSkill(DefDatabase<SkillDef>.GetNamed("Medicine")).Level = 3; // Low skill

            // Add helpers to the surgeon's component
            helperComp.AddHelper(helper1);
            helperComp.AddHelper(helper2);

            // Simulate surgery
            float quality = 1.0f; // Base quality
            var surgeryComp = new SurgeryOutcomeComp_Helpers();
            surgeryComp.AffectQuality(null, surgeon, patient, new List<Thing>(), null, null, ref quality);

            // Log the final result
            Log.Message($"[Test] Final surgery quality: {quality}");
        }
    }
}
