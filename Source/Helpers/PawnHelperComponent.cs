using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Helpers
{
    public static class HelpersDebug
    {
        public static bool EnableLogging = true; // Set to false to disable logging
    }
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("SpawnSetup")]
    public static class Pawn_SpawnSetup_Patch
    {
        public static void Postfix(Pawn __instance, bool respawningAfterLoad)
        {
            DebugHelpers.PSSPLog($"Pawn {__instance.Name} is spawning. Respawning after load: {respawningAfterLoad}");

            // Check if the pawn already has the PawnHelperComponent
            var existingHelperComponent = __instance.GetComp<PawnHelperComponent>();
            if (existingHelperComponent != null)
            {
                DebugHelpers.PSSPLog($"{__instance.Name} already has PawnHelperComponent attached.");
            }
            else
            {
                // Add the PawnHelperComponent dynamically
                __instance.AllComps.Add(new PawnHelperComponent { parent = __instance });
                DebugHelpers.PSSPLog($"Added PawnHelperComponent to {__instance.Name}");
            }
        }
    }


    public class PawnHelperComponent : ThingComp
    {
        public List<Pawn> CurrentHelpers = new List<Pawn>();
        public bool IsBeingHelped = false;

        public void AddHelper(Pawn helper)
        {
            if (!CurrentHelpers.Contains(helper))
            {
                CurrentHelpers.Add(helper);
                IsBeingHelped = true;
                DebugHelpers.PHCLog($"{helper.Name} is now helping {parent.LabelCap}.");
            }
        }

        public void RemoveHelper(Pawn helper)
        {
            if (CurrentHelpers.Remove(helper))
            {
                IsBeingHelped = CurrentHelpers.Count > 0;
                DebugHelpers.PHCLog($"{helper.Name} stopped helping {parent.LabelCap}. Remaining helpers: {CurrentHelpers.Count}");
            }
            else
            {
                DebugHelpers.PHCLog($"Attempted to remove {helper.Name} from helpers list, but they weren't found.");
            }
        }

        // Save and load component data
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref CurrentHelpers, "CurrentHelpers", LookMode.Reference);
            Scribe_Values.Look(ref IsBeingHelped, "IsBeingHelped", false);
            DebugHelpers.PHCLog($"Saved/loaded PawnHelperComponent for {parent.LabelCap}. IsBeingHelped: {IsBeingHelped}, CurrentHelpers count: {CurrentHelpers?.Count ?? 0}");
        }
    }


    public static class PawnHelperExtensions
    {
        public static PawnHelperComponent GetHelperComponent(this Pawn pawn)
        {
            var helperComponent = pawn.GetComp<PawnHelperComponent>();
            if (helperComponent == null)
            {
                DebugHelpers.PHExtLog($"{pawn.LabelCap} does not have a PawnHelperComponent.");
            }
            else
            {
                DebugHelpers.PHExtLog($"{pawn.LabelCap} successfully retrieved a PawnHelperComponent.");
            }
            return helperComponent;
        }
    }


    public class CompProperties_PawnHelper : CompProperties
    {
        public CompProperties_PawnHelper()
        {
            this.compClass = typeof(PawnHelperComponent);
            DebugHelpers.CPPHLog("CompProperties_PawnHelper initialized.");
        }
    }


}
