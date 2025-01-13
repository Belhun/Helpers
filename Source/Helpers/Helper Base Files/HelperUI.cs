using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Reflection;
using LudeonTK;
using RimWorld.Utility;
using Verse.AI;

namespace Helpers
{
    public static class PawnAssignmentSelector
    {
        public static void DrawPawnAssignmentSelector(Rect rect, ref Vector2 scrollPosition)
        {
            float listHeight = Find.CurrentMap.mapPawns.FreeColonistsCount * 35f; // Calculate total height dynamically

            Widgets.BeginScrollView(rect, ref scrollPosition, new Rect(0f, 0f, rect.width - 16f, listHeight));
            float curY = 0f;

            foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonists)
            {
                Rect pawnRect = new Rect(0f, curY, rect.width - 16f, 50f);

                // Draw pawn name
                Widgets.Label(new Rect(pawnRect.x + 5f, pawnRect.y, 150f, pawnRect.height / 2), pawn.LabelShortCap);

                // Check helper component
                var helperComponent = pawn.GetHelperComponent();

                // Draw "Being Helped" status
                bool isBeingHelped = helperComponent?.IsBeingHelped ?? false;
                string status = isBeingHelped ? "Being Helped" : "Not Helped";
                Color statusColor = isBeingHelped ? Color.green : Color.red;
                GUI.color = statusColor;
                Widgets.Label(new Rect(pawnRect.x + 160f, pawnRect.y, 100f, pawnRect.height / 2), status);
                GUI.color = Color.white;

                // Draw number of helpers
                if (helperComponent != null && helperComponent.CurrentHelpers != null)
                {
                    Widgets.Label(new Rect(pawnRect.x + 270f, pawnRect.y, 200f, pawnRect.height / 2),
                        $"Helpers: {helperComponent.CurrentHelpers.Count}");
                }

                //// Draw current job and progress
                //if (pawn.CurJob != null)
                //{
                //    Widgets.Label(new Rect(pawnRect.x + 5f, pawnRect.y + 20f, 200f, pawnRect.height / 2),
                //        $"Job: {pawn.CurJob.def.label.CapitalizeFirst()}");
                    
                //    if (helperComponent != null && helperComponent.CurrentHelpers.Count > 0)
                //    {
                //        float totalHelpingSkill = helperComponent.CurrentHelpers.Sum(helper =>
                //            helper.skills?.GetSkill(helperComponent.CurreentSkillDef).Level ?? 0);

                //        float jobHelpPercentage = Mathf.Clamp(totalHelpingSkill / (pawn.skills.GetSkill(helperComponent.CurreentSkillDef).Level + 1), 0f, 1f) * 100;
                //        Widgets.Label(new Rect(pawnRect.x + 210f, pawnRect.y + 20f, 200f, pawnRect.height / 2),
                //            $"Helped: {jobHelpPercentage:F1}%");
                //    }
                //}

                curY += 55f; // Adjust for two lines of information
            }

            Widgets.EndScrollView();
        }
    }



    public class Dialog_PawnAssignment : Window
    {
        private Vector2 scrollPosition = Vector2.zero;
        private Pawn selectedPawnToHelp;

        public Dialog_PawnAssignment()
        {
            this.draggable = true;
            this.doCloseX = true;
            this.closeOnAccept = false;
            this.closeOnCancel = false;
            this.preventCameraMotion = false;
        }

        public override Vector2 InitialSize => new Vector2(400f, 600f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "Colonist Pawn Assignments");

            // Dropdown for selecting the colonist to help
            Rect dropdownRect = new Rect(0f, 40f, 300f, 30f);
            Widgets.Label(new Rect(0f, 40f, 100f, 30f), "Select Colonist:");
            Widgets.Dropdown(
                dropdownRect,
                selectedPawnToHelp,
                pawn => pawn,
                GenerateColonistOptions,
                selectedPawnToHelp?.LabelShortCap ?? "Select..."
                );

            // Button for assigning idle pawns to help
            if (Widgets.ButtonText(new Rect(0f, 80f, 150f, 30f), "Assign Idle Pawns"))
            {
                AssignPawnsToHelp();
            }

            // Scrollable list for all pawns
            Rect listRect = new Rect(0f, 120f, inRect.width, inRect.height - 120f);
            PawnAssignmentSelector.DrawPawnAssignmentSelector(listRect, ref scrollPosition);
        }

        private IEnumerable<Widgets.DropdownMenuElement<Pawn>> GenerateColonistOptions(Pawn currentSelection)
        {
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonists)
            {
                yield return new Widgets.DropdownMenuElement<Pawn>
                {
                    payload = pawn,
                    option = new FloatMenuOption(pawn.LabelShortCap, () => selectedPawnToHelp = pawn)
                };
            }
        }


        private void AssignPawnsToHelp()
        {
            if (selectedPawnToHelp == null)
            {
                Messages.Message("No colonist selected to help!", MessageTypeDefOf.RejectInput, historical: false);
                return;
            }

            // Filter pawns doing joy activities or wandering
            List<Pawn> joyOrWanderingPawns = Find.CurrentMap.mapPawns.FreeColonists
                .Where(pawn =>
                    pawn.CurJob != null &&
                    (pawn.CurJob.def == JobDefOf.Wait_MaintainPosture ||
                     pawn.CurJob.def == JobDefOf.GotoWander ||
                     pawn.CurJob.def.joyKind != null) &&
                    pawn != selectedPawnToHelp)
                .ToList();

            // Log all colonists' current jobs
            DebugHelpers.DebugLog("AssignPawnsToHelp", "Logging all colonists and their jobs:");
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonists)
            {
                string jobLabel = pawn.CurJob?.def.label ?? "No job";
                DebugHelpers.DebugLog("AssignPawnsToHelp", $"{pawn.LabelShortCap}: {jobLabel}");
            }

            if (!joyOrWanderingPawns.Any())
            {
                Messages.Message("No suitable pawns available to assign.", MessageTypeDefOf.RejectInput, historical: false);
                DebugHelpers.DebugLog("AssignPawnsToHelp", "No pawns found performing joy activities or wandering.");
                return;
            }

            // Assign jobs to pawns performing joy or wandering
            foreach (Pawn pawn in joyOrWanderingPawns)
            {
                DebugHelpers.DebugLog("AssignPawnsToHelp", $"Attempting to assign {pawn.LabelShortCap} to help {selectedPawnToHelp.LabelShortCap}.");

                Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), selectedPawnToHelp);

                if (job == null)
                {
                    DebugHelpers.DebugLog("AssignPawnsToHelp", $"Failed to create a Helping job for {pawn.LabelShortCap}.");
                    continue;
                }

                DebugHelpers.DebugLog("AssignPawnsToHelp", $"Job created successfully for {pawn.LabelShortCap}: Target is {selectedPawnToHelp.LabelShortCap}");

                if (!pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
                {
                    DebugHelpers.DebugLog("AssignPawnsToHelp", $"Failed to assign job to {pawn.LabelShortCap}. Check pawn's state and eligibility.");
                    continue;
                }

                DebugHelpers.DebugLog("AssignPawnsToHelp", $"{pawn.LabelShortCap} successfully assigned to help {selectedPawnToHelp.LabelShortCap}.");
            }

            Messages.Message($"{joyOrWanderingPawns.Count} pawns are now helping {selectedPawnToHelp.LabelShortCap}.", MessageTypeDefOf.TaskCompletion, historical: false);
        }
    }
}
