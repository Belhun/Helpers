using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using System.Runtime.Remoting.Messaging;

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

                Log.Message($"[Helpers Mod] Initializing DoRecipeWork_Helper for pawn: {actor3.Name}");

                if (unfinishedThing2 != null && unfinishedThing2.Initialized)
                {
                    jobDriver_DoBill2.workLeft = unfinishedThing2.workLeft;
                    Log.Message($"[Helpers Mod] Using unfinished thing work left: {unfinishedThing2.workLeft}");
                }
                else
                {
                    jobDriver_DoBill2.workLeft = curJob3.bill.GetWorkAmount(thing3);
                    Log.Message($"[Helpers Mod] Calculated initial work amount: {jobDriver_DoBill2.workLeft}");

                    if (unfinishedThing2 != null)
                    {
                        if (unfinishedThing2.debugCompleted)
                        {
                            unfinishedThing2.workLeft = (jobDriver_DoBill2.workLeft = 0f);
                            Log.Message($"[Helpers Mod] Work completed (debug mode).");
                        }
                        else
                        {
                            unfinishedThing2.workLeft = jobDriver_DoBill2.workLeft;
                        }
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

                Log.Message($"[Helpers Mod] Tick action for {actor2.Name} in DoRecipeWork_Helper");

                if (unfinishedThing != null && unfinishedThing.Destroyed)
                {
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

                    float workSpeed = ((curJob2.RecipeDef.workSpeedStat == null) ? 1f : actor2.GetStatValue(curJob2.RecipeDef.workSpeedStat));
                    Log.Message($"[Helpers Mod] Base work speed: {workSpeed}");

                    if (curJob2.RecipeDef.workTableSpeedStat != null && jobDriver_DoBill.BillGiver is Building_WorkTable workTable)
                    {
                        workSpeed *= workTable.GetStatValue(curJob2.RecipeDef.workTableSpeedStat);
                        Log.Message($"[Helpers Mod] Adjusted work speed with worktable multiplier: {workSpeed}");
                    }

                    if (DebugSettings.fastCrafting)
                    {
                        workSpeed *= 30f;
                        Log.Message($"[Helpers Mod] Debug mode enabled, fast crafting speed: {workSpeed}");
                    }

                    var helperComp = jobDriver_DoBill.pawn.GetHelperComponent();
                    if (helperComp != null && helperComp.IsBeingHelped)
                    {
                        float helperTotal = 0f;
                        foreach (Pawn helper in helperComp.CurrentHelpers)
                        {
                            Log.Message($"[Helpers Mod] {helper.Name} is assisting {actor2.Name}");
                            float helperSkillContribution = 0.5f + (helper.skills.GetSkill(curJob2.RecipeDef.workSkill).Level / 40f);
                            helperTotal += helperSkillContribution * helper.GetStatValue(curJob2.RecipeDef.workSpeedStat);

                            helper.skills.Learn(curJob2.RecipeDef.workSkill, 0.1f * curJob2.RecipeDef.workSkillLearnFactor);
                            helper.skills.Learn(SkillDefOf.Helping, 0.1f); // Assuming Helping skill exists
                            Log.Message($"[Helpers Mod] Helper {helper.Name} contributed: {helperSkillContribution}");
                        }
                        workSpeed += helperTotal;
                        Log.Message($"[Helpers Mod] Total helper contribution to work speed: {helperTotal}");
                    }

                    jobDriver_DoBill.workLeft -= workSpeed;
                    Log.Message($"[Helpers Mod] Work left: {jobDriver_DoBill.workLeft}");

                    if (unfinishedThing != null)
                    {
                        if (unfinishedThing.debugCompleted)
                        {
                            unfinishedThing.workLeft = (jobDriver_DoBill.workLeft = 0f);
                            Log.Message($"[Helpers Mod] Unfinished thing work completed.");
                        }
                        else
                        {
                            unfinishedThing.workLeft = jobDriver_DoBill.workLeft;
                        }
                    }

                    actor2.GainComfortFromCellIfPossible(chairsOnly: true);

                    if (jobDriver_DoBill.workLeft <= 0f)
                    {
                        curJob2.bill.Notify_BillWorkFinished(actor2);
                        jobDriver_DoBill.ReadyForNextToil();
                        Log.Message($"[Helpers Mod] Job finished for {actor2.Name}");
                    }
                    else if (curJob2.bill.recipe.UsesUnfinishedThing)
                    {
                        int ticksPassed = Find.TickManager.TicksGame - jobDriver_DoBill.billStartTick;
                        if (ticksPassed >= 3000 && ticksPassed % 1000 == 0)
                        {
                            actor2.jobs.CheckForJobOverride();
                            Log.Message($"[Helpers Mod] Checked for job override at {ticksPassed} ticks");
                        }
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

                float workAmount;
                if (curJob.bill is Bill_Mech billMech && billMech.State == FormingState.Formed)
                {
                    workAmount = 300f;
                }
                else
                {
                    workAmount = curJob.bill.recipe.WorkAmountTotal(thing);
                }

                float progress = 1f - workLeft / workAmount;
                Log.Message($"[Helpers Mod] Progress: {progress}");
                return progress;
            });

            toil.FailOn(() =>
            {
                RecipeDef recipeDef = toil.actor.CurJob.RecipeDef;
                if (recipeDef != null && recipeDef.interruptIfIngredientIsRotting)
                {
                    LocalTargetInfo target = toil.actor.CurJob.GetTarget(TargetIndex.B);
                    if (target.HasThing && target.Thing.GetRotStage() > 0)
                    {
                        Log.Message($"[Helpers Mod] Interrupt due to ingredient rotting.");
                        return true;
                    }
                }
                return toil.actor.CurJob.bill.suspended;
            });

            toil.activeSkill = () => toil.actor.CurJob.bill.recipe.workSkill;
            return toil;
        }
    }
}
