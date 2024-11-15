using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Helpers
{
    public static class CustomToils_Recipe
    {
        public static Toil DoRecipeWork_Helper()
        {

            Toil toil = ToilMaker.MakeToil("DoRecipeWork");
            toil.initAction = delegate
            {
                Pawn actor3 = toil.actor;
                Job curJob3 = actor3.jobs.curJob;
                JobDriver_DoBill jobDriver_DoBill2 = (JobDriver_DoBill)actor3.jobs.curDriver;
                Thing thing3 = curJob3.GetTarget(TargetIndex.B).Thing;
                UnfinishedThing unfinishedThing2 = thing3 as UnfinishedThing;

                if (HelpersDebug.EnableLogging)
                    Log.Message($"[Helpers Mod] Initializing DoRecipeWork_Helper for pawn: {actor3.Name}");

                if (unfinishedThing2 != null && unfinishedThing2.Initialized)
                {
                    jobDriver_DoBill2.workLeft = unfinishedThing2.workLeft;
                }
                else
                {
                    jobDriver_DoBill2.workLeft = curJob3.bill.GetWorkAmount(thing3);
                    if (unfinishedThing2 != null)
                    {
                        unfinishedThing2.workLeft = jobDriver_DoBill2.workLeft;
                    }
                }
                jobDriver_DoBill2.billStartTick = Find.TickManager.TicksGame;
                jobDriver_DoBill2.ticksSpentDoingRecipeWork = 0;
                curJob3.bill.Notify_BillWorkStarted(actor3);
            };

            toil.tickAction = delegate
            {
                Pawn actor2 = toil.actor;
                Job curJob2 = actor2.jobs.curJob;
                JobDriver_DoBill jobDriver_DoBill = (JobDriver_DoBill)actor2.jobs.curDriver;
                UnfinishedThing unfinishedThing = curJob2.GetTarget(TargetIndex.B).Thing as UnfinishedThing;

                if (unfinishedThing != null && unfinishedThing.Destroyed)
                {
                    if (HelpersDebug.EnableLogging)
                        Log.Message($"[Helpers Mod] Unfinished thing destroyed, ending job.");
                    actor2.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                    jobDriver_DoBill.ticksSpentDoingRecipeWork++;
                    curJob2.bill.Notify_PawnDidWork(actor2);

                    if (toil.actor.CurJob.GetTarget(TargetIndex.A).Thing is IBillGiverWithTickAction billGiverWithTickAction)
                    {
                        billGiverWithTickAction.UsedThisTick();
                    }

                    if (curJob2.RecipeDef.workSkill != null && curJob2.RecipeDef.UsesUnfinishedThing && actor2.skills != null)
                    {
                        actor2.skills.Learn(curJob2.RecipeDef.workSkill, 0.1f * curJob2.RecipeDef.workSkillLearnFactor);
                    }

                    // Base work speed
                    float workSpeed = ((curJob2.RecipeDef.workSpeedStat == null) ? 1f : actor2.GetStatValue(curJob2.RecipeDef.workSpeedStat));

                    if (curJob2.RecipeDef.workTableSpeedStat != null && jobDriver_DoBill.BillGiver is Building_WorkTable workTable)
                    {
                        workSpeed *= workTable.GetStatValue(curJob2.RecipeDef.workTableSpeedStat);
                    }

                    if (DebugSettings.fastCrafting)
                    {
                        workSpeed *= 30f;
                    }

                    // Helper Skill Dependency Logic
                    var helperComp = jobDriver_DoBill.pawn.GetHelperComponent();
                    if (helperComp != null && helperComp.IsBeingHelped)
                    {
                        float helperTotal = 0f;
                        foreach (Pawn helper in helperComp.CurrentHelpers)
                        {
                            if (HelpersDebug.EnableLogging)
                                Log.Message($"[Helpers Mod] {helper.Name} is assisting {actor2.Name}");

                            // Helper's skill check
                            int skillLevel = helper.skills.GetSkill(curJob2.RecipeDef.workSkill)?.Level ?? 0;
                            if (skillLevel < 1)
                            {
                                // Penalize if skill level is below 1
                                float penalty = -0.1f * workSpeed;
                                helperTotal += penalty;
                                if (HelpersDebug.EnableLogging)
                                    Log.Message($"[Helpers Mod] {helper.Name} negatively affected speed due to insufficient skill: {penalty}");
                            }
                            else
                            {
                                float helperSkillContribution = 0.5f + (skillLevel / 40f);
                                helperTotal += helperSkillContribution * helper.GetStatValue(curJob2.RecipeDef.workSpeedStat);
                                if (HelpersDebug.EnableLogging)
                                    Log.Message($"[Helpers Mod] {helper.Name} contributed: {helperSkillContribution}");
                            }

                            // Helper XP Gain
                            helper.skills.Learn(curJob2.RecipeDef.workSkill, 0.1f * curJob2.RecipeDef.workSkillLearnFactor);
                            helper.skills.Learn(SkillDefOf.Helping, 0.1f); // Assuming Helping skill exists
                        }
                        workSpeed += helperTotal;
                        if (HelpersDebug.EnableLogging)
                            Log.Message($"[Helpers Mod] Total helper contribution to work speed: {helperTotal}");
                    }

                    jobDriver_DoBill.workLeft -= workSpeed;
                    if (HelpersDebug.EnableLogging)
                        Log.Message($"[Helpers Mod] Work left: {jobDriver_DoBill.workLeft}");

                    if (unfinishedThing != null)
                    {
                        unfinishedThing.workLeft = jobDriver_DoBill.workLeft;
                    }

                    actor2.GainComfortFromCellIfPossible(chairsOnly: true);

                    if (jobDriver_DoBill.workLeft <= 0f)
                    {
                        curJob2.bill.Notify_BillWorkFinished(actor2);
                        jobDriver_DoBill.ReadyForNextToil();
                    }
                }
            };

            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect(() => toil.actor.CurJob.bill.recipe.effectWorking, TargetIndex.A);
            toil.PlaySustainerOrSound(() => toil.actor.CurJob.bill.recipe.soundWorking);
            toil.WithProgressBar(TargetIndex.A, delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                Thing thing = curJob.GetTarget(TargetIndex.B).Thing;
                float workLeft = ((JobDriver_DoBill)actor.jobs.curDriver).workLeft;
                float workAmount = curJob.bill.recipe.WorkAmountTotal(thing);
                return 1f - workLeft / workAmount;
            });

            toil.FailOn(() =>
            {
                RecipeDef recipeDef = toil.actor.CurJob.RecipeDef;
                if (recipeDef != null && recipeDef.interruptIfIngredientIsRotting)
                {
                    LocalTargetInfo target = toil.actor.CurJob.GetTarget(TargetIndex.B);
                    return target.HasThing && target.Thing.GetRotStage() > 0;
                }
                return toil.actor.CurJob.bill.suspended;
            });

            toil.activeSkill = () => toil.actor.CurJob.bill.recipe.workSkill;
            return toil;
        }
    }
}
