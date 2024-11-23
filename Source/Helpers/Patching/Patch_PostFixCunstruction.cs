using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Helpers
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
                //Log.Message($"GetMembers :{__instance.GetType().GetMembers()}");
                //Log.Message($"GetMethods :{__instance.GetType().GetMethods()}");
                //Log.Message($"GetFields :{__instance.GetType().GetFields()}");
                //Log.Message($"GetFields :{__instance.GetType().GetFields()[0]}");
                //Log.Message($"GetFields :{__instance.GetType().GetFields()[1]}");
                //Log.Message($"GetFields :{__instance.GetType().GetFields()[0].Attributes}");
                //Log.Message($"GetFields :{__instance.GetType().GetFields()[1]}");

                var test = __instance.GetType().GetFields()[0].Attributes;
                var buildarray = __instance.GetType().GetField("build", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
                if (buildarray == null)
                {
                    Log.Warning("Helpers: Unable to retrieve var 'buildarray'.");
                    return;
                }
                //Log.Message($"buildarray.Attributes :{buildarray.Attributes}");
                //Log.Message($"buildarray.Name :{buildarray.Name}");
                //Log.Message($"buildarray :{buildarray}");
                //Log.Message($"buildarray?.GetValue(__instance) :{buildarray?.GetValue(__instance)}");


                Toil build = buildarray?.GetValue(__instance) as Toil;
                if (build == null)
                {
                    Log.Warning("Helpers: Unable to retrieve Toil 'build'.");
                    return;
                }
                //Log.Message($"build :{build.GetActor()}");
                //Log.Message($"build.actor :{build.actor}");
                //Log.Message($"build :{build}");
                //Log.Message($"build.GetType() :{build.GetType()}");
                //Log.Message($"build.GetType().GetFields() :{build.GetType().GetFields()}");

                Pawn actor = build?.GetType().GetField("actor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(build) as Pawn;
                if (actor == null)
                {
                    Log.Warning("Helpers: Unable to retrieve Pawn 'actor'.");
                    return;
                }
                //Log.Message($"actor :{actor}");
                //Log.Message($"actor :{actor.abilities}");
                //Log.Message($"actor :{actor.CurJob.GetTarget(TargetIndex.A)}");

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
