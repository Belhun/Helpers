//using System.Collections.Generic;
//using RimWorld;
//using Verse;
//using Verse.AI;
//using UnityEngine;

//namespace Helpers
//{
//    public static class CustomToils_Recipe
//    {
//        /// <summary>
//        /// Custom toil for recipe work, incorporating helper contributions.
//        /// </summary>
//        public static Toil DoRecipeWork_Helper()
//        {
//            Toil toil = ToilMaker.MakeToil("DoRecipeWork");

//            // Initialization action for the toil
//            toil.initAction = delegate
//            {
//                Pawn actor = toil.actor;
//                Job curJob = actor.jobs.curJob;
//                JobDriver_DoBill jobDriver = (JobDriver_DoBill)actor.jobs.curDriver;
//                Thing targetThing = curJob.GetTarget(TargetIndex.B).Thing;
//                UnfinishedThing unfinishedThing = targetThing as UnfinishedThing;

//                DebugHelpers.DebugLog("CustomToils_Recipe", $"Initializing DoRecipeWork_Helper for pawn: {actor.Name}");

//                if (unfinishedThing != null && unfinishedThing.Initialized)
//                {
//                    jobDriver.workLeft = unfinishedThing.workLeft;
//                }
//                else
//                {
//                    jobDriver.workLeft = curJob.bill.GetWorkAmount(targetThing);
//                    if (unfinishedThing != null)
//                    {
//                        unfinishedThing.workLeft = jobDriver.workLeft;
//                    }
//                }

//                jobDriver.billStartTick = Find.TickManager.TicksGame;
//                jobDriver.ticksSpentDoingRecipeWork = 0;
//                curJob.bill.Notify_BillWorkStarted(actor);
//            };

//            // Main tick action for the toil
//            toil.tickAction = delegate
//            {
//                Pawn actor = toil.actor;
//                Job curJob = actor.jobs.curJob;
//                JobDriver_DoBill jobDriver = (JobDriver_DoBill)actor.jobs.curDriver;
//                UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;

//                if (unfinishedThing != null && unfinishedThing.Destroyed)
//                {
//                    DebugHelpers.DebugLog("CustomToils_Recipe", $"Unfinished thing destroyed, ending job for {actor.Name}");
//                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
//                    return;
//                }

//                jobDriver.ticksSpentDoingRecipeWork++;
//                curJob.bill.Notify_PawnDidWork(actor);

//                if (curJob.GetTarget(TargetIndex.A).Thing is IBillGiverWithTickAction billGiverWithTickAction)
//                {
//                    billGiverWithTickAction.UsedThisTick();
//                }

//                if (curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing && actor.skills != null)
//                {
//                    actor.skills.Learn(curJob.RecipeDef.workSkill, 0.1f * curJob.RecipeDef.workSkillLearnFactor);
//                }

//                // Base work speed calculation
//                float workSpeed = curJob.RecipeDef.workSpeedStat == null
//                    ? 1f
//                    : actor.GetStatValue(curJob.RecipeDef.workSpeedStat);
                
//                if (curJob.RecipeDef.workTableSpeedStat != null && jobDriver.BillGiver is Building_WorkTable workTable)
//                {
//                    workSpeed *= workTable.GetStatValue(curJob.RecipeDef.workTableSpeedStat);
//                }
                
//                if (DebugSettings.fastCrafting)
//                {
//                    workSpeed *= 30f;
//                }

//                // Helper Logic
//                var helperComp = jobDriver.pawn.GetHelperComponent();
//                if (helperComp != null && helperComp.IsBeingHelped)
//                {
//                    workSpeed += HelperMechanics.CalculateHelperContribution(
//                        actor,
//                        jobDriver,
//                        curJob.RecipeDef,
//                        helperComp.CurrentHelpers,
//                        curJob.RecipeDef.workSpeedStat
//                    );
//                }

//                jobDriver.workLeft -= workSpeed;
//                DebugHelpers.DebugLog("CustomToils_Recipe", $"Work left for {actor.Name}: {jobDriver.workLeft}");

//                if (unfinishedThing != null)
//                {
//                    unfinishedThing.workLeft = jobDriver.workLeft;
//                }

//                actor.GainComfortFromCellIfPossible(chairsOnly: true);

//                if (jobDriver.workLeft <= 0f)
//                {
//                    curJob.bill.Notify_BillWorkFinished(actor);
//                    jobDriver.ReadyForNextToil();
//                }
//            };

//            // Toil setup
//            toil.defaultCompleteMode = ToilCompleteMode.Never;
//            toil.WithEffect(() => toil.actor.CurJob.bill.recipe.effectWorking, TargetIndex.A);
//            toil.PlaySustainerOrSound(() => toil.actor.CurJob.bill.recipe.soundWorking);
//            toil.WithProgressBar(TargetIndex.A, () =>
//            {
//                Pawn actor = toil.actor;
//                Job curJob = actor.CurJob;
//                Thing targetThing = curJob.GetTarget(TargetIndex.B).Thing;
//                float workLeft = ((JobDriver_DoBill)actor.jobs.curDriver).workLeft;
//                float workAmount = curJob.bill.recipe.WorkAmountTotal(targetThing);
//                return 1f - workLeft / workAmount;
//            });

//            toil.FailOn(() =>
//            {
//                RecipeDef recipeDef = toil.actor.CurJob.RecipeDef;
//                if (recipeDef != null && recipeDef.interruptIfIngredientIsRotting)
//                {
//                    LocalTargetInfo target = toil.actor.CurJob.GetTarget(TargetIndex.B);
//                    return target.HasThing && target.Thing.GetRotStage() > 0;
//                }
//                return toil.actor.CurJob.bill.suspended;
//            });

//            toil.activeSkill = () => toil.actor.CurJob.bill.recipe.workSkill;
//            return toil;
//        }
//    }
//}
