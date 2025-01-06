using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Helpers
{
    /// <summary>
    /// Initializes the Helpers mod by applying Harmony patches.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HelpersInitializer
    {
        static HelpersInitializer()
        {
            DebugHelpers.DebugLog("HelpersInitializer", "Assembly loaded successfully.");
            var harmony = new Harmony("Belhun.helpersmod");
            harmony.PatchAll();
            DebugHelpers.DebugLog("HelpersInitializer", "Harmony patches applied successfully.");
            Harmony.DEBUG = true;

        }
    }
    /// <summary>
    /// Adds a "Help" option to the float menu when right-clicking a target pawn.
    /// </summary>
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Define targeting parameters to filter valid target pawns
            TargetingParameters targetingParams = new TargetingParameters
            {
                canTargetPawns = true,
                canTargetItems = false,
                validator = t =>
                {
                    if (t.Thing is Pawn targetPawn && targetPawn != pawn && targetPawn.IsColonistPlayerControlled)
                    {
                        return !targetPawn.Downed && !targetPawn.Dead;
                    }
                    return false;
                }
            };

            // Get all valid targets at the clicked position
            foreach (LocalTargetInfo target in GenUI.TargetsAt(clickPos, targetingParams, thingsOnly: true))
            {
                if (target.Thing is Pawn targetPawn)
                {
                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Found valid target: {targetPawn.LabelShortCap}");

                    // Get all selected pawns, excluding the target pawn
                    var selectedPawns = Find.Selector.SelectedObjects
                        .OfType<Pawn>()
                        .Where(p => p != targetPawn && p.IsColonistPlayerControlled)
                        .ToList();

                    if (selectedPawns.Count == 0)
                    {
                        DebugHelpers.DebugLog("FloatMenuMakerMap", "No valid helper pawns selected.");
                        continue;
                    }

                    // Define the label for the "Help" menu option
                    string label = selectedPawns.Count > 1
                        ? $"Help {targetPawn.LabelShortCap} with {selectedPawns.Count} pawns"
                        : $"Help {targetPawn.LabelShortCap}";

                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Creating menu option with label: {label}");

                    // Define the action when the menu option is selected
                    Action action = () =>
                    {
                        foreach (var helperPawn in selectedPawns)
                        {
                            Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("Helping"), targetPawn);
                            helperPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                            DebugHelpers.DebugLog("FloatMenuMakerMap", $"{helperPawn.Name} is now helping {targetPawn.LabelShortCap}.");
                        }
                    };

                    // Create and add the menu option
                    FloatMenuOption helpOption = FloatMenuUtility.DecoratePrioritizedTask(
                        new FloatMenuOption(label, action, MenuOptionPriority.Default),
                        pawn,
                        targetPawn
                    );

                    opts.Add(helpOption);
                    DebugHelpers.DebugLog("FloatMenuMakerMap", $"Added 'Help' option to the float menu for {targetPawn.LabelShortCap}.");
                }
            }
        }
    }
    /// <summary>
    /// When a pawn gains XP, check if they have Helpers.
    /// If so, mirror the XP to those Helpers.
    /// </summary>
    [HarmonyPatch(typeof(SkillRecord))]
    [HarmonyPatch("Learn")]
    public static class SkillRecord_Learn_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SkillRecord __instance, Pawn ___pawn, float xp, bool direct)
        {
            // check if there is a helper component and if not then skip the rest
            var leaderHelperComp = ___pawn.GetHelperComponent();
            if (leaderHelperComp == null)
            {
              
                return;
            }

            // 2) Get the list of helpers and if there are no helpers then skip the rest
            var helpers = leaderHelperComp.CurrentHelpers;
            if (helpers == null || helpers.Count == 0)
            {
                // No helpers => Nothing to do
                return;
            }

            // check if the xp is an negtive number if it is then skip the rest
            if (xp < 0)
            {
                return;
            }

            //give XP to the helpers for helping the pawn
            foreach (var helper in helpers)
            {
                if (helper.skills == null) continue;

                var skill = helper.skills.GetSkill(__instance.def);
                if (skill == null) continue;

                // Log or debug
                DebugHelpers.DebugLog("SkillRecord_Learn_Patch",
                    $"{helper.LabelShortCap} receives {xp} XP in {__instance.def.label} for helping {___pawn.LabelShortCap}."
                );

                skill.Learn(xp, direct: true);
            }
        }
    }
}