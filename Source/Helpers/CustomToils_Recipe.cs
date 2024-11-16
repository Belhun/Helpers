using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Helpers
{
    [DefOf]
    public static class HelpersDefOf
    {
        // Traits
        public static TraitDef Ugly;
        public static TraitDef Beautiful;

        // Relations
        public static PawnRelationDef Rival;

        // Ensure all defs are resolved
        static HelpersDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HelpersDefOf));
        }
    }

    public static class DebugHelpers
    {
        public static bool EnableLogging = true;

        public static void Log(string message)
        {
            if (EnableLogging)
            {
                Log.Message($"[Helpers Mod] {message}");
            }
        }
    }

    public static class CustomToils_Recipe
    {
        public static Toil DoRecipeWork_Helper()
        {
            Toil toil = ToilMaker.MakeToil("DoRecipeWork");
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBill jobDriver = (JobDriver_DoBill)actor.jobs.curDriver;
                Thing targetThing = curJob.GetTarget(TargetIndex.B).Thing;
                UnfinishedThing unfinishedThing = targetThing as UnfinishedThing;

                DebugHelpers.Log($"Initializing DoRecipeWork_Helper for pawn: {actor.Name}");

                if (unfinishedThing != null && unfinishedThing.Initialized)
                {
                    jobDriver.workLeft = unfinishedThing.workLeft;
                }
                else
                {
                    jobDriver.workLeft = curJob.bill.GetWorkAmount(targetThing);
                    if (unfinishedThing != null)
                    {
                        unfinishedThing.workLeft = jobDriver.workLeft;
                    }
                }
                jobDriver.billStartTick = Find.TickManager.TicksGame;
                jobDriver.ticksSpentDoingRecipeWork = 0;
                curJob.bill.Notify_BillWorkStarted(actor);
            };

            toil.tickAction = delegate
            {
                Pawn actor = toil.actor;
                Job curJob = actor.jobs.curJob;
                JobDriver_DoBill jobDriver = (JobDriver_DoBill)actor.jobs.curDriver;
                UnfinishedThing unfinishedThing = curJob.GetTarget(TargetIndex.B).Thing as UnfinishedThing;

                if (unfinishedThing != null && unfinishedThing.Destroyed)
                {
                    DebugHelpers.Log($"Unfinished thing destroyed, ending job for {actor.Name}");
                    actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                    return;
                }

                jobDriver.ticksSpentDoingRecipeWork++;
                curJob.bill.Notify_PawnDidWork(actor);

                if (curJob.GetTarget(TargetIndex.A).Thing is IBillGiverWithTickAction billGiverWithTickAction)
                {
                    billGiverWithTickAction.UsedThisTick();
                }

                if (curJob.RecipeDef.workSkill != null && curJob.RecipeDef.UsesUnfinishedThing && actor.skills != null)
                {
                    actor.skills.Learn(curJob.RecipeDef.workSkill, 0.1f * curJob.RecipeDef.workSkillLearnFactor);
                }

                // Base work speed
                float workSpeed = curJob.RecipeDef.workSpeedStat == null
                    ? 1f
                    : actor.GetStatValue(curJob.RecipeDef.workSpeedStat);

                if (curJob.RecipeDef.workTableSpeedStat != null && jobDriver.BillGiver is Building_WorkTable workTable)
                {
                    workSpeed *= workTable.GetStatValue(curJob.RecipeDef.workTableSpeedStat);
                }

                if (DebugSettings.fastCrafting)
                {
                    workSpeed *= 30f;
                }

                // Helper Logic
                var helperComp = jobDriver.pawn.GetHelperComponent();
                if (helperComp != null && helperComp.IsBeingHelped)
                {
                    float helperTotal = 0f;

                    foreach (Pawn helper in helperComp.CurrentHelpers)
                    {
                        DebugHelpers.Log($"{helper.Name} is assisting {actor.Name}");

                        ApplySocialThoughts(helper, actor);

                        int skillLevel = helper.skills.GetSkill(curJob.RecipeDef.workSkill)?.Level ?? 0;

                        float contribution = (0.5f + (skillLevel / 40f));
                        if (skillLevel < 5)
                        {
                            contribution -= 0.1f;
                        }

                        helperTotal += contribution * helper.GetStatValue(curJob.RecipeDef.workSpeedStat);
                        helper.skills.Learn(curJob.RecipeDef.workSkill, 0.1f * curJob.RecipeDef.workSkillLearnFactor);
                    }

                    workSpeed += helperTotal;
                    DebugHelpers.Log($"Total helper contribution to work speed: {helperTotal}");
                }

                jobDriver.workLeft -= workSpeed;
                DebugHelpers.Log($"Work left for {actor.Name}: {jobDriver.workLeft}");

                if (unfinishedThing != null)
                {
                    unfinishedThing.workLeft = jobDriver.workLeft;
                }

                actor.GainComfortFromCellIfPossible(chairsOnly: true);

                if (jobDriver.workLeft <= 0f)
                {
                    curJob.bill.Notify_BillWorkFinished(actor);
                    jobDriver.ReadyForNextToil();
                }
            };

            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.WithEffect(() => toil.actor.CurJob.bill.recipe.effectWorking, TargetIndex.A);
            toil.PlaySustainerOrSound(() => toil.actor.CurJob.bill.recipe.soundWorking);
            toil.WithProgressBar(TargetIndex.A, () =>
            {
                Pawn actor = toil.actor;
                Job curJob = actor.CurJob;
                Thing targetThing = curJob.GetTarget(TargetIndex.B).Thing;
                float workLeft = ((JobDriver_DoBill)actor.jobs.curDriver).workLeft;
                float workAmount = curJob.bill.recipe.WorkAmountTotal(targetThing);
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

        private static void ApplySocialThoughts(Pawn helper, Pawn helped)
        {
            if (helper.relations != null && helper.relations.DirectRelationExists(HelpersDefOf.Rival, helped))
            {
                helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                DebugHelpers.Log($"{helper.Name} gained 'WorkingWithRival' thought for working with {helped.Name}");
            }

            if (helped.story != null && helper.needs != null)
            {
                if (helped.story.traits.HasTrait(HelpersDefOf.Ugly))
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.Log($"{helper.Name} gained 'WorkingWithUgly' thought for working with {helped.Name}");
                }
                else if (helped.story.traits.HasTrait(HelpersDefOf.Beautiful))
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.Log($"{helper.Name} gained 'WorkingWithBeautiful' thought for working with {helped.Name}");
                }
            }

            if (helper.relations != null && helper.relations.DirectRelationExists(PawnRelationDefOf.Lover, helped))
            {
                helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                DebugHelpers.Log($"{helper.Name} gained 'WorkingWithLover' thought for working with {helped.Name}");
            }
        }

    public static class PawnBeautyChecker
    {
        public static int? GetBeautyDegree(Pawn pawn)
        {
            // Check if the pawn has the Beauty trait
            TraitDef beautyTrait = TraitDef.Named("Beauty");
            if (pawn.story?.traits.HasTrait(beautyTrait) == true)
            {
                // Get the degree of the Beauty trait
                int degree = pawn.story.traits.DegreeOfTrait(beautyTrait);
                return degree;
            }

            // Return null if the pawn does not have the Beauty trait
            return null;
        }

        public static void LogBeautyTrait(Pawn pawn)
        {
            int? beautyDegree = GetBeautyDegree(pawn);
            if (beautyDegree.HasValue)
            {
                Log.Message($"{pawn.Name} has Beauty trait with degree: {beautyDegree.Value}");
            }
            else
            {
                Log.Message($"{pawn.Name} does not have the Beauty trait.");
            }
        }
    }

}
}
