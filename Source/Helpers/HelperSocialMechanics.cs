using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Helpers
{
    public static class HelperSocialMechanics
    {
        public static void ApplySocialThoughts(Pawn helper, Pawn helped, List<Pawn> currentHelpers)
        {
            CheckRivalry(helper, helped);
            CheckRivalryAmongHelpers(helper, currentHelpers);
            CheckFriendship(helper, helped);  // New Friendship mechanic
            CheckFriendshipAmongHelpers(helper, currentHelpers); // Friendship among helpers
            CheckBeautyEffects(helper, helped);
            CheckBeautyAmongHelpers(helper, currentHelpers);
            CheckLoverRelationship(helper, helped);
            CheckLoverAmongHelpers(helper, currentHelpers);
        }

        /// <summary>
        /// Checks if a helper and helped pawn are friends and applies mood buffs.
        /// </summary>
        private static void CheckFriendship(Pawn helper, Pawn helped)
        {
            if (helper.relations != null && helped.relations != null)
            {
                int helperOpinion = helper.relations.OpinionOf(helped);
                int helpedOpinion = helped.relations.OpinionOf(helper);

                if (helperOpinion >= 40)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithFriend"));
                    DebugHelpers.HSMLog($"{helper.Name} has a positive opinion of {helped.Name} (opinion: {helperOpinion}) and gains 'WorkingWithFriend'.");
                }

                if (helpedOpinion >= 40)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithFriend"));
                    DebugHelpers.HSMLog($"{helped.Name} has a positive opinion of {helper.Name} (opinion: {helpedOpinion}) and gains 'WorkingWithFriend'.");
                }
            }
        }
        
        /// <summary>
        /// Checks for friendships among the helpers and applies mood buffs where necessary.
        /// </summary>
        private static void CheckFriendshipAmongHelpers(Pawn helper, List<Pawn> currentHelpers)
        {
            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;

                if (helper.relations != null && otherHelper.relations != null)
                {
                    int helperOpinion = helper.relations.OpinionOf(otherHelper);
                    int otherHelperOpinion = otherHelper.relations.OpinionOf(helper);

                    if (helperOpinion >= 40)
                    {
                        helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithFriend"));
                        DebugHelpers.HSMLog($"{helper.Name} has a positive opinion of {otherHelper.Name} (opinion: {helperOpinion}) and gains 'WorkingWithFriend'.");
                    }

                    if (otherHelperOpinion >= 40)
                    {
                        otherHelper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithFriend"));
                        DebugHelpers.HSMLog($"{otherHelper.Name} has a positive opinion of {helper.Name} (opinion: {otherHelperOpinion}) and gains 'WorkingWithFriend'.");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a helper and helped pawn are rivals and applies appropriate mood debuffs.
        /// </summary>
        private static void CheckRivalry(Pawn helper, Pawn helped)
        {
            if (helper.relations != null && helped.relations != null)
            {
                int helperOpinion = helper.relations.OpinionOf(helped);
                int helpedOpinion = helped.relations.OpinionOf(helper);

                if (helperOpinion <= -40)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                    DebugHelpers.HSMLog($"{helper.Name} dislikes {helped.Name} (opinion: {helperOpinion}) and gains 'WorkingWithRival'.");
                }

                if (helpedOpinion <= -40)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                    DebugHelpers.HSMLog($"{helped.Name} dislikes {helper.Name} (opinion: {helpedOpinion}) and gains 'WorkingWithRival'.");
                }
            }
        }
        /// <summary>
        /// Checks for rivalries among the helpers and applies mood debuffs where necessary.
        /// </summary>
        private static void CheckRivalryAmongHelpers(Pawn helper, List<Pawn> currentHelpers)
        {
            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;

                if (helper.relations != null && otherHelper.relations != null)
                {
                    int helperOpinion = helper.relations.OpinionOf(otherHelper);
                    int otherHelperOpinion = otherHelper.relations.OpinionOf(helper);

                    if (helperOpinion <= -40)
                    {
                        helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                        DebugHelpers.HSMLog($"{helper.Name} dislikes {otherHelper.Name} (opinion: {helperOpinion}) and gains 'WorkingWithRival'.");
                    }

                    if (otherHelperOpinion <= -40)
                    {
                        otherHelper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithRival"));
                        DebugHelpers.HSMLog($"{otherHelper.Name} dislikes {helper.Name} (opinion: {otherHelperOpinion}) and gains 'WorkingWithRival'.");
                    }
                }
            }
        }

        private static void CheckBeautyEffects(Pawn helper, Pawn helped)
        {
            int? helperBeautyDegree = HelperSocialMechanics.GetBeautyDegree(helper);
            int? helpedBeautyDegree = HelperSocialMechanics.GetBeautyDegree(helped);

            if (helperBeautyDegree != null)
            {
                if (helperBeautyDegree == -1 || helperBeautyDegree == -2)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.HSMLog($"{helper.Name} is perceived as ugly by {helped.Name}");
                }
                else if (helperBeautyDegree == 1 || helperBeautyDegree == 2)
                {
                    helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.HSMLog($"{helper.Name} is perceived as beautiful by {helped.Name}");
                }
            }

            if (helpedBeautyDegree != null)
            {
                if (helpedBeautyDegree == -1 || helpedBeautyDegree == -2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.HSMLog($"{helped.Name} is perceived as ugly by {helper.Name}");
                }
                else if (helpedBeautyDegree == 1 || helpedBeautyDegree == 2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.HSMLog($"{helped.Name} is perceived as beautiful by {helper.Name}");
                }
            }
        }

        private static void CheckBeautyAmongHelpers(Pawn helper, List<Pawn> currentHelpers)
        {
            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;

                int? otherHelperBeautyDegree = HelperSocialMechanics.GetBeautyDegree(otherHelper);
                if (otherHelperBeautyDegree == null) continue;

                if (otherHelperBeautyDegree == -1 || otherHelperBeautyDegree == -2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithUgly"));
                    DebugHelpers.HSMLog($"{otherHelper.Name} is perceived as ugly by {helper.Name}");
                }
                else if (otherHelperBeautyDegree == 1 || otherHelperBeautyDegree == 2)
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithBeautiful"));
                    DebugHelpers.HSMLog($"{otherHelper.Name} is perceived as beautiful by {helper.Name}");
                }
            }
        }

        private static void CheckLoverRelationship(Pawn helper, Pawn helped)
        {
            if (helper.relations != null && helper.relations.DirectRelationExists(PawnRelationDefOf.Lover, helped))
            {
                helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                helped.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                DebugHelpers.HSMLog($"{helper.Name} and {helped.Name} are lovers and gain 'WorkingWithLover'");
            }
        }

        private static void CheckLoverAmongHelpers(Pawn helper, List<Pawn> currentHelpers)
        {
            foreach (Pawn otherHelper in currentHelpers)
            {
                if (helper == otherHelper) continue;

                if (helper.relations != null && helper.relations.DirectRelationExists(PawnRelationDefOf.Lover, otherHelper))
                {
                    helper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                    otherHelper.needs.mood.thoughts.memories.TryGainMemory(DefDatabase<ThoughtDef>.GetNamed("WorkingWithLover"));
                    DebugHelpers.HSMLog($"{helper.Name} and {otherHelper.Name} are lovers and gain 'WorkingWithLover'");
                }
            }
        }

        public static int? GetBeautyDegree(Pawn pawn)
        {
            TraitDef beautyTrait = TraitDef.Named("Beauty");
            if (pawn.story?.traits.HasTrait(beautyTrait) == true)
            {
                int degree = pawn.story.traits.DegreeOfTrait(beautyTrait);
                DebugHelpers.HSMLog($"{pawn.Name} has Beauty trait with degree: {degree}");
                return degree;
            }

            DebugHelpers.HSMLog($"{pawn.Name} does not have the Beauty trait.");
            return null;
        }

        public static void LogBeautyTrait(Pawn pawn)
        {
            int? beautyDegree = GetBeautyDegree(pawn);
            if (beautyDegree.HasValue)
            {
                DebugHelpers.HSMLog($"{pawn.Name} has Beauty trait with degree: {beautyDegree.Value}");
            }
            else
            {
                DebugHelpers.HSMLog($"{pawn.Name} does not have the Beauty trait.");
            }
        }


    }
}
