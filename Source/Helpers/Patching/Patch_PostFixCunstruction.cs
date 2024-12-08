using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Helpers.Patching
{
    [HarmonyPatch]
    public static class Patch_ConstructionFrameWork
    {
        static MethodBase TargetMethod()
        {

            return typeof(JobDriver_ConstructFinishFrame)
                .GetNestedType("<>c__DisplayClass6_0", BindingFlags.NonPublic)
                ?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static void Postfix(object __instance)
        {
            try
            {

                Log.Message($"__instance.GetType().GetFields :{__instance.GetType().GetFields()}");

                var test = __instance.GetType().GetFields()[0].Attributes;
                var buildarray = __instance.GetType().GetField("build", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                if (buildarray == null)
                {
                    Log.Warning("Helpers: Unable to retrieve var 'buildarray'.");
                    return;
                }



                Toil build = buildarray?.GetValue(__instance) as Toil;
                if (build == null)
                {
                    Log.Warning("Helpers: Unable to retrieve Toil 'build'.");
                    return;
                }


                Pawn actor = build?.GetType().GetField("actor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(build) as Pawn;
                if (actor == null)
                {
                    Log.Warning("Helpers: Unable to retrieve Pawn 'actor'.");
                    return;
                }


                //// Retrieve the current job's target frame
                Frame frame = actor.CurJob.GetTarget(TargetIndex.A).Thing as Frame;
                if (frame == null)
                {
                    Log.Warning("Helpers: Unable to retrieve construction frame from job target.");
                    return;
                }

                // Calculate helper contribution
                float helperContribution = HelperMechanics.HelperConstructionSpeed(actor, frame);

                // Adjust frame.workDone with helper contribution
                frame.workDone += helperContribution;

                DebugHelpers.DebugLog("Patch_PostFixCunstruction", $"Helpers: Added {helperContribution} work to frame. Total work done: {frame.workDone}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"Helpers: Exception in construction postfix - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
