using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace Helpers
{

    // TODO Rival Dose not work, Needs to be fixed

    //[DefOf]
    //public static class HelpersDefOf
    //{
    //    // Relations
    //    public static PawnRelationDef Rival;

    //    // Ensure all defs are resolved
    //    static HelpersDefOf()
    //    {
    //        DefOfHelper.EnsureInitializedInCtor(typeof(HelpersDefOf));
    //    }
    //}

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

                DebugHelpers.CTRLog($"Initializing DoRecipeWork_Helper for pawn: {actor.Name}");

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
                    DebugHelpers.CTRLog($"Unfinished thing destroyed, ending job for {actor.Name}");
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
                        DebugHelpers.CTRLog($"{helper.Name} is assisting {actor.Name}");

                        ApplySocialThoughts(helper, actor, helperComp.CurrentHelpers);

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
                    DebugHelpers.CTRLog($"Total helper contribution to work speed: {helperTotal}");
                }

                jobDriver.workLeft -= workSpeed;
                DebugHelpers.CTRLog($"Work left for {actor.Name}: {jobDriver.workLeft}");

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

        private static void ApplySocialThoughts(Pawn helper, Pawn helped, List<Pawn> currentHelpers)
        {
            // Check opinion-based rivalry between helper and helped
            if (helper.relations != null && helped.relations != null)
            {
                int helperOpinion = helper.relations.OpinionOf(helped);
                int helpedOpinion = helped.relations.OpinionOf(helper);

                // If the helper dislikes the helped, apply the debuff to the helper
                if (helperOpinion <= -40)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                    DebugHelpers.CTRLog($"{helper.Name} dislikes {helped.Name} (opinion: {helperOpinion}) and gains 'WorkingWithRival'.");
                }

                // If the helped dislikes the helper, apply the debuff to the helped
                if (helpedOpinion <= -40)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                    DebugHelpers.CTRLog($"{helped.Name} dislikes {helper.Name} (opinion: {helpedOpinion}) and gains 'WorkingWithRival'.");
                }
            }

            // Check rivalry among helpers
            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue; // Skip self-check

                if (helper.relations != null && otherHelper.relations != null)
                {
                    int helperOpinion = helper.relations.OpinionOf(otherHelper);
                    int otherHelperOpinion = otherHelper.relations.OpinionOf(helper);

                    // If the helper dislikes the other helper, apply the debuff to the helper
                    if (helperOpinion <= -40)
                    {
                        helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                        DebugHelpers.CTRLog($"{helper.Name} dislikes {otherHelper.Name} (opinion: {helperOpinion}) and gains 'WorkingWithRival'.");
                    }

                    // If the other helper dislikes the helper, apply the debuff to the other helper
                    if (otherHelperOpinion <= -40)
                    {
                        otherHelper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                        DebugHelpers.CTRLog($"{otherHelper.Name} dislikes {helper.Name} (opinion: {otherHelperOpinion}) and gains 'WorkingWithRival'.");
                    }
                }
            }

            // Check Beauty for helper and helped
            int? helperBeautyDegree = PawnBeautyChecker.GetBeautyDegree(helper);
            int? helpedBeautyDegree = PawnBeautyChecker.GetBeautyDegree(helped);

            if (helperBeautyDegree != null)
            {
                if (helperBeautyDegree == -1 || helperBeautyDegree == -2)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.CTRLog($"{helper.Name} is perceived as ugly by {helped.Name}");
                }
                else if (helperBeautyDegree == 1 || helperBeautyDegree == 2)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.CTRLog($"{helper.Name} is perceived as beautiful by {helped.Name}");
                }
            }

            if (helpedBeautyDegree != null)
            {
                if (helpedBeautyDegree == -1 || helpedBeautyDegree == -2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.CTRLog($"{helped.Name} is perceived as ugly by {helper.Name}");
                }
                else if (helpedBeautyDegree == 1 || helpedBeautyDegree == 2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.CTRLog($"{helped.Name} is perceived as beautiful by {helper.Name}");
                }
            }

            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;

                int? otherHelperBeautyDegree = PawnBeautyChecker.GetBeautyDegree(otherHelper);
                if (otherHelperBeautyDegree == null) continue;

                if (otherHelperBeautyDegree == -1 || otherHelperBeautyDegree == -2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.CTRLog($"{otherHelper.Name} is perceived as ugly by {helper.Name}");
                }
                else if (otherHelperBeautyDegree == 1 || otherHelperBeautyDegree == 2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.CTRLog($"{otherHelper.Name} is perceived as beautiful by {helper.Name}");
                }
            }

            // Check Lovers
            if (helper.relations != null && helper.relations.DirectRelationExists(PawnRelationDefOf.Lover, helped))
            {
                helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                DebugHelpers.CTRLog($"{helper.Name} and {helped.Name} are lovers and gain 'WorkingWithLover'");
            }

            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;
                if (helper.relations != null && helper.relations.DirectRelationExists(PawnRelationDefOf.Lover, otherHelper))
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                    otherHelper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                    DebugHelpers.CTRLog($"{helper.Name} and {otherHelper.Name} are lovers and gain 'WorkingWithLover'");
                }
            }
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
                DebugHelpers.PBCLog($"{pawn.Name} has Beauty trait with degree: {degree}");
                return degree;
            }

            // Log and return null if the pawn does not have the Beauty trait
            DebugHelpers.PBCLog($"{pawn.Name} does not have the Beauty trait.");
            return null;
        }

        public static void LogBeautyTrait(Pawn pawn)
        {
            int? beautyDegree = GetBeautyDegree(pawn);
            if (beautyDegree.HasValue)
            {
                DebugHelpers.PBCLog($"{pawn.Name} has Beauty trait with degree: {beautyDegree.Value}");
            }
            else
            {
                DebugHelpers.PBCLog($"{pawn.Name} does not have the Beauty trait.");
            }
        }
    }


}

