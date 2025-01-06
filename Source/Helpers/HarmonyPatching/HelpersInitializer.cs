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
    [StaticConstructorOnStartup]
    public static class HelpersInitializer
    {
        /// <summary>
        /// Initializes the Helpers mod by applying Harmony patches and logging success.
        /// </summary>
        static HelpersInitializer()
        {
            DebugHelpers.DebugLog("HelpersInitializer", "Assembly loaded successfully.");
            var harmony = new Harmony("Belhun.helpersmod");
            harmony.PatchAll();
            DebugHelpers.DebugLog("HelpersInitializer", "Harmony patches applied successfully.");
            Harmony.DEBUG = true;

        }
    }

    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public static class FloatMenuMakerMap_AddHumanlikeOrders_Patch
    {
        /// <summary>
        /// Adds a "Help" option to the float menu when right-clicking a target pawn.
        /// </summary>
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

    //[HarmonyPatch(typeof(SurgeryOutcomeEffectDef), "GetQuality")]
    //public static class Patch_AddHelperComp
    //{
    //    private static readonly HashSet<System.Type> KnownSurgeryWorkerClasses = new HashSet<System.Type>
    //    {
    //        typeof(Recipe_Surgery),
    //        typeof(Recipe_InstallArtificialBodyPart),
    //        typeof(Recipe_RemoveBodyPart)
    //    };
    //    public static void Postfix(SurgeryOutcomeEffectDef __instance, RecipeDef recipe)
    //    {
    //        Log.Message($"[Helpers] Postfix triggered for {recipe?.defName ?? "Unknown Recipe"}");


    //        if (recipe.surgerySuccessChanceFactor <= 0f)
    //        {
    //            Log.Message($"[Helpers] Skipping {recipe.defName}: surgerySuccessChanceFactor is <= 0.");
    //            return;
    //        }

    //        if (recipe.workerClass == null || !Patch_AddHelperComp.KnownSurgeryWorkerClasses.Contains(recipe.workerClass))
    //        {
    //            // Check if Process Unknown Surgeries is disabled
    //            if (!HelpersMod.Settings.ProcessUnknownSurgeries)
    //            {
    //                // Log the issue for devs or players in dev mode
    //                if (Prefs.DevMode)
    //                {
    //                    Log.Warning($"[Helpers] Unknown surgery detected: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name ?? "NULL"}).");
    //                    Log.Warning("COPY AND REPORT THIS:");
    //                    Log.Warning($" - Surgery Name: {recipe.defName}");
    //                    Log.Warning($" - Worker Class: {recipe.workerClass?.Name ?? "NULL"}");
    //                    Log.Warning($" - Mod Responsible: {recipe.modContentPack?.Name ?? "Unknown"}");
    //                }
    //                else if (!HelpersMod.Settings.DisableUnknownSurgeryNotifications && !HelpersMod.Settings.HasNotifiedUnknownSurgery)
    //                {
    //                    // Show a one-time notification
    //                    var letter = LetterMaker.MakeLetter(
    //                        "Unknown Surgery Detected",
    //                        $"The surgery '{recipe.defName}' has been detected but is not recognized by the Helpers mod. This could be from a modded recipe.\n\n" +
    //                        "COPY AND REPORT THIS:\n" +
    //                        $"- Surgery Name: {recipe.defName}\n" +
    //                        $"- Worker Class: {recipe.workerClass?.Name ?? "NULL"}\n" +
    //                        $"- Mod Responsible: {recipe.modContentPack?.Name ?? "Unknown"}\n\n" +
    //                        "Once reported, you can enable 'Process Unknown Surgeries' in settings to allow contributions for this surgery. If you don't want to see this message again, you can disable it in the Helpers mod settings.",
    //                        LetterDefOf.NegativeEvent);

    //                    Find.LetterStack.ReceiveLetter(letter);
    //                    HelpersMod.Settings.HasNotifiedUnknownSurgery = true; // Ensure this only happens once per session
    //                }

    //                return; // Skip processing the surgery
    //            }
    //            else
    //            {
    //                // Log or notify about the surgery being processed
    //                if (HelpersMod.Settings.LogUnknownSurgeries)
    //                {
    //                    Log.Warning($"[Helpers] Unknown surgery processed: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name ?? "NULL"}).");
    //                    Log.Warning("This surgery was processed because 'Process Unknown Surgeries' is enabled in settings. Report any unexpected behavior.");
    //                }
    //            }
    //        }

    //        Log.Message($"[Helpers] Processing surgery recipe: {recipe.defName} (WorkerClass: {recipe.workerClass?.Name})");

    //        if (__instance.comps == null)
    //        {
    //            __instance.comps = new List<SurgeryOutcomeComp>();
    //            Log.Message($"[Helpers] Initialized comps list for {recipe.defName}");
    //        }

    //        if (!__instance.comps.OfType<SurgeryOutcomeComp_Helpers>().Any())
    //        {
    //            __instance.comps.Add(new SurgeryOutcomeComp_Helpers());
    //            Log.Message($"[Helpers] Added SurgeryOutcomeComp_Helpers to {recipe.defName}");
    //        }
    //        else
    //        {
    //            Log.Message($"[Helpers] SurgeryOutcomeComp_Helpers already exists for {recipe.defName}");
    //        }


    //    }
    //    //public static class HelpersSettings
    //    //{
    //    //    public static bool LogUnknownSurgeries = true; // Default to logging unknown surgeries
    //    //}
    //}

    //[HarmonyPatch]
    //public static class Patch_ConstructionFrameWork
    //{
    //    static MethodBase TargetMethod()
    //    {

    //        return typeof(JobDriver_ConstructFinishFrame)
    //            .GetNestedType("<>c__DisplayClass6_0", BindingFlags.NonPublic)
    //            ?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
    //    }

    //    public static void Postfix(object __instance)
    //    {
    //        try
    //        {

    //            Log.Message($"__instance.GetType().GetFields :{__instance.GetType().GetFields()}");

    //            var test = __instance.GetType().GetFields()[0].Attributes;
    //            var buildarray = __instance.GetType().GetField("build", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //            if (buildarray == null)
    //            {
    //                Log.Warning("Helpers: Unable to retrieve var 'buildarray'.");
    //                return;
    //            }



    //            Toil build = buildarray?.GetValue(__instance) as Toil;
    //            if (build == null)
    //            {
    //                Log.Warning("Helpers: Unable to retrieve Toil 'build'.");
    //                return;
    //            }


    //            Pawn actor = build?.GetType().GetField("actor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(build) as Pawn;
    //            if (actor == null)
    //            {
    //                Log.Warning("Helpers: Unable to retrieve Pawn 'actor'.");
    //                return;
    //            }


    //            //// Retrieve the current job's target frame
    //            Frame frame = actor.CurJob.GetTarget(TargetIndex.A).Thing as Frame;
    //            if (frame == null)
    //            {
    //                Log.Warning("Helpers: Unable to retrieve construction frame from job target.");
    //                return;
    //            }

    //            // Calculate helper contribution
    //            float helperContribution = HelperMechanics.HelperConstructionSpeed(actor, frame);

    //            // Adjust frame.workDone with helper contribution
    //            frame.workDone += helperContribution;

    //            DebugHelpers.DebugLog("Patch_PostFixCunstruction", $"Helpers: Added {helperContribution} work to frame. Total work done: {frame.workDone}");
    //        }
    //        catch (System.Exception ex)
    //        {
    //            Log.Error($"Helpers: Exception in construction postfix - {ex.Message}\n{ex.StackTrace}");
    //        }
    //    }
    //}

    //[HarmonyPatch]
    //public static class Patch_PostToilsRecipe
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        // Locate the nested class and method generated by the compiler
    //        var nestedType = typeof(Toils_Recipe).GetNestedType("<>c__DisplayClass2_0", BindingFlags.NonPublic);
    //        return nestedType?.GetMethod("<DoRecipeWork>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
    //    }

    //    public static void Postfix(object __instance)
    //    {
    //        //DebugHelpers.DebugLog("Patch_PostToilsRecipe", $"<color=#00FF00>{__result}");

    //        // Inspect closure fields
    //        var fields = __instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //        DebugHelpers.DebugLog("Patch_PostToilsRecipe", $"Closure fields: {string.Join(", ", fields.Select(f => f.Name))}");

    //        // Try retrieving the Toil field
    //        var toilField = __instance.GetType().GetField("toil", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //        if (toilField == null)
    //        {
    //            Log.Warning("Failed to retrieve 'toil' field from closure.");
    //            return;
    //        }

    //        var toil = toilField.GetValue(__instance) as Toil;
    //        if (toil == null)
    //        {
    //            Log.Warning("Failed to cast 'toil' to Toil.");
    //            return;
    //        }

    //        DebugHelpers.DebugLog("Patch_PostToilsRecipe", $"Toil retrieved: {toil}");

    //        // Get the actor (main pawn performing the job)
    //        Pawn actor = toil?.GetType().GetField("actor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(toil) as Pawn;
    //        if (actor == null)
    //        {
    //            Log.Warning("Helpers: Unable to retrieve Pawn 'actor'.");
    //            return;
    //        }

    //        // Retrieve the current job and job driver
    //        var curJob = actor.jobs?.curJob;

    //        if (curJob == null)
    //        {
    //            //Job is done or cancelled
    //            return;
    //        }
    //        else
    //        {
    //            var jobDriver = actor.jobs?.curDriver as JobDriver_DoBill;
    //            if (jobDriver == null)
    //            {
    //                return;
    //            }

    //            // Get the helper component
    //            var helperComp = actor.GetHelperComponent();
    //            if (helperComp == null || !helperComp.IsBeingHelped)
    //            {
    //                // No helpers active, nothing to adjust
    //                return;
    //            }

    //            // Adjust workLeft using the helper contribution
    //            float helperContribution = HelperMechanics.CalculateHelperContribution(
    //                actor,
    //                jobDriver,
    //                curJob.RecipeDef,
    //                helperComp.CurrentHelpers,
    //                curJob.RecipeDef.workSpeedStat
    //            );

    //            jobDriver.workLeft -= helperContribution;

    //            DebugHelpers.DebugLog("Patch_PostToilsRecipe", $" Adjusted workLeft by {helperContribution}. Remaining work: {jobDriver.workLeft}");

    //            // Log experience updates handled within HelperMechanics (if applicable)
    //            foreach (var helper in helperComp.CurrentHelpers)
    //            {
    //                DebugHelpers.DebugLog("Patch_PostToilsRecipe", ": {helper.Name} assisted {actor.Name} and contributed.");
    //            }
    //        }
    //    }
    //}

    //[HarmonyPatch]
    //public static class Patch_PreFixMine
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        // Locate the nested method using reflection
    //        var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic);
    //        return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
    //    }

    //    public static void Prefix(object __instance)
    //    {
    //        DebugHelpers.DebugLog("Patch_PreFixMine", "Prefix started");

    //        // Access the 'mine' field
    //        var mine = ReflectionHelper.GetNestedFieldOrPropertyValue(__instance, "mine") as Toil;

    //        // Access the 'actor' field
    //        var actor = ReflectionHelper.GetNestedFieldOrPropertyValue(mine, "actor") as Pawn;

    //        // Access the 'curDriver' field
    //        var curDriver = ReflectionHelper.GetNestedFieldOrPropertyValue(actor?.jobs, "curDriver") as JobDriver_Mine;

    //        // Access the 'MineTarget' property
    //        var mineTarget = ReflectionHelper.GetNestedFieldOrPropertyValue(curDriver, "MineTarget") as Thing;

    //        // Access the 'HitPoints' field
    //        var hitPoints = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "HitPoints");
    //        var def = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "def");
    //        var building = ReflectionHelper.GetNestedFieldOrPropertyValue(def, "building") as BuildingProperties;

    //        DebugHelpers.DebugLog("Patch_PreFixMine", $"MineTarget HitPoints: {hitPoints}");

    //        if (building != null)
    //        {
    //            bool isNaturalRock = building.isNaturalRock;
    //            DebugHelpers.DebugLog("Patch_PreFixMine", $"Building found: {building.ToString()}, isNaturalRock: {isNaturalRock.ToString()}");
    //        }
    //        else
    //        {
    //            Log.Warning("Unable to retrieve 'building' field from def.");
    //        }

    //        IntVec3 position = (IntVec3)ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "Position");

    //        MethodInfo takeDamageMethod = mineTarget.GetType().GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    //        var map = ReflectionHelper.GetNestedFieldOrPropertyValue(mineTarget, "Map");
    //        var designationManager = ReflectionHelper.GetNestedFieldOrPropertyValue(map, "designationManager");
    //        MethodInfo designationAtMethod = designationManager?.GetType().GetMethod("DesignationAt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    //        var helperComp = actor.GetHelperComponent();
    //        BuildingProperties buildprop = (BuildingProperties)building;
    //        HelperMechanics.DistributeMiningXP(
    //               actor,
    //               curDriver,
    //               helperComp.CurrentHelpers,
    //               mineTarget,
    //               building
    //           );

    //        // Adjust ticksToPickHit
    //        FieldInfo ticksToPickHitField = typeof(JobDriver_Mine).GetField("ticksToPickHit", BindingFlags.Instance | BindingFlags.NonPublic);
    //        int ticksToPickHit = (int)ticksToPickHitField.GetValue(curDriver);
    //        ticksToPickHit = HelperMechanics.TranspilerMineingTTPCHC(actor, ticksToPickHit);
    //        ticksToPickHitField.SetValue(curDriver, ticksToPickHit);

    //        //if (helperComp != null && helperComp.IsBeingHelped)
    //        //{
    //        //    int amount = ((int)helperContribution);

    //        //    int CurrentHitPoints = (int)hitPoints;
    //        //    Map CurrentMap = (Map)map;
    //        //    if (!(mineTarget is Mineable mineable2) || CurrentHitPoints > amount)
    //        //    {
    //        //        DamageInfo dinfo = new DamageInfo(DamageDefOf.Mining, amount, 0f, -1f, actor);
    //        //        takeDamageMethod?.Invoke(mineTarget, new object[] { dinfo });
    //        //    }
    //        //    else
    //        //    {
    //        //        bool num = (bool)designationAtMethod?.Invoke(designationManager, new object[] { mineable2.Position, DesignationDefOf.MineVein });
    //        //        mineable2.Notify_TookMiningDamage(CurrentHitPoints, mine.actor);
    //        //        mineable2.HitPoints = 0;
    //        //        mineable2.DestroyMined(actor);
    //        //        if (num)
    //        //        {
    //        //            IntVec3[] adjacentCells = GenAdj.AdjacentCells;
    //        //            foreach (IntVec3 adjacentCell in adjacentCells)
    //        //            {
    //        //                Designator_MineVein.FloodFillDesignations(position + adjacentCell, CurrentMap, mineable2.def);
    //        //            }
    //        //        }
    //        //    }
    //        //}
    //    }
    //}

    //[HarmonyPatch]
    //public static class Patch_TranspilerMine
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        // Locate the nested method using reflection
    //        var nestedType = typeof(JobDriver_Mine).GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic | BindingFlags.Instance);
    //        return nestedType?.GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
    //    }

    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var codes = new List<CodeInstruction>(instructions);
    //        var calculateMethod = AccessTools.Method(typeof(HelperMechanics), nameof(HelperMechanics.CalculateAdjustedMiningDamage));
    //        bool injected = false;

    //        for (int i = 0; i < codes.Count - 5; i++)
    //        {
    //            // Locate the code assigning to 'num'
    //            if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i + 1].opcode == OpCodes.Stloc_3)
    //            {
    //                // Add logging before
    //                codes.Insert(i + 2, new CodeInstruction(OpCodes.Ldloc_3)); // Load baseAmount
    //                codes.Insert(i + 3, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DebugHelpers), nameof(DebugHelpers.LogNumValue))));

    //                // Replace with custom logic
    //                codes.Insert(i + 4, new CodeInstruction(OpCodes.Ldloc_0)); // Load 'actor'
    //                codes.Insert(i + 5, new CodeInstruction(OpCodes.Ldloc_1)); // Load 'mineTarget'
    //                codes.Insert(i + 6, new CodeInstruction(OpCodes.Ldloc_3)); // Load baseAmount
    //                codes.Insert(i + 7, new CodeInstruction(OpCodes.Call, calculateMethod)); // Call the custom method
    //                codes.Insert(i + 8, new CodeInstruction(OpCodes.Stloc_3)); // Store adjusted value

    //                // Add logging after
    //                codes.Insert(i + 9, new CodeInstruction(OpCodes.Ldloc_3)); // Load adjusted num
    //                codes.Insert(i + 10, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DebugHelpers), nameof(DebugHelpers.LogNumValue))));

    //                injected = true;
    //                break;
    //            }
    //        }

    //        if (!injected)
    //        {
    //            Log.Warning("[Patch_TranspilerMine] Failed to inject custom mining damage logic.");
    //        }

    //        return codes.AsEnumerable();
    //    }


    //}

    //[HarmonyPatch]
    //public static class Patch_PlantSow
    //{
    //    static MethodBase TargetMethod()
    //    {

    //        return typeof(JobDriver_PlantSow)
    //            .GetNestedType("<>c__DisplayClass5_0", BindingFlags.NonPublic)
    //            ?.GetMethod("<MakeNewToils>b__4", BindingFlags.Instance | BindingFlags.NonPublic);
    //    }

    //    public static void Postfix(object __instance)
    //    {
    //        DebugHelpers.DebugLog("Patch_PlantSow", $" Postfix started");
    //        Toil toil = Traverse.Create(__instance).Field("sowToil").GetValue<Toil>();
    //        if (toil != null)
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantSow", $"Toil: {toil.ToString()}");
    //        }
    //        else
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantSow", $" Toil is null.");
    //        }
    //        Pawn actor = toil.actor;
    //        if (actor != null)
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantSow", $" Pawn: {actor.Name}");
    //        }
    //        else
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantSow", $" Pawn is null.");
    //        }
    //        var helperComp = actor.GetHelperComponent();
    //        var curJob = actor.jobs?.curJob;
    //        var jobDriver = actor.jobs?.curDriver as JobDriver_PlantSow;
    //        FieldInfo workDoneField = typeof(JobDriver_PlantSow).GetField("sowWorkDone", BindingFlags.Instance | BindingFlags.NonPublic);
    //        float currentWork = (float)workDoneField.GetValue(jobDriver);
    //        float helperContribution = HelperMechanics.PlantsCHC(
    //                actor,
    //                jobDriver,
    //                helperComp.CurrentHelpers,
    //                StatDefOf.PlantWorkSpeed
    //            );

    //    }

    //[HarmonyPatch]
    //public static class Patch_PlantWork
    //{
    //    static MethodBase TargetMethod()
    //    {
    //        try
    //        {
    //            // Step 1: Get the nested class
    //            var nestedType = typeof(JobDriver_PlantWork)
    //                .GetNestedType("<>c__DisplayClass11_0", BindingFlags.NonPublic);
    //            // Step 2: Get the method within the nested class
    //            var targetMethod = nestedType
    //                .GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic);
    //            return targetMethod;
    //        }
    //        catch (Exception ex)
    //        {
    //            Log.Error($"[Helpers] Exception while resolving TargetMethod: {ex.Message}\n{ex.StackTrace}");
    //            return null;
    //        }
    //    }


    //    public static void Postfix(object __instance)
    //    {
    //        DebugHelpers.DebugLog("Patch_PlantWork", $" Postfix started");

    //        Toil toil = Traverse.Create(__instance).Field("cut").GetValue<Toil>();
    //        if (toil != null)
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantWork", $"Toil: {toil.ToString()}");
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" Toil: {toil.ToString()}");
    //        }
    //        else
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" Toil is null.");
    //        }
    //        Pawn actor = toil.actor;
    //        if (actor != null)
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" Pawn: {actor.Name}");
    //        }
    //        else
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" Pawn is null.");
    //        }
    //        var helperComp = actor.GetHelperComponent();
    //        var curJob = actor.jobs?.curJob;
    //        var jobDriver = actor.jobs?.curDriver as JobDriver_PlantWork;
    //        if (helperComp != null && helperComp.IsBeingHelped)
    //        {
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" helperComp is not null && pawn IsBeingHelped ");

    //            // Calculate helper contribution
    //            float helperContribution = HelperMechanics.PlantsCHC(
    //                actor,
    //                jobDriver,
    //                helperComp.CurrentHelpers,
    //                StatDefOf.PlantWorkSpeed
    //            );
    //            FieldInfo workDoneField = typeof(JobDriver_PlantWork).GetField("workDone", BindingFlags.Instance | BindingFlags.NonPublic);
    //            if (workDoneField == null)
    //            {
    //                DebugHelpers.DebugLog("Patch_PlantWork", $" Unable to find 'workDone' field in JobDriver_PlantWork.");
    //                return;
    //            }
    //            float currentWorkDone = (float)workDoneField.GetValue(jobDriver);
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" workDone. Old Value: {currentWorkDone}");

    //            // Add helper contribution
    //            currentWorkDone += helperContribution;

    //            // Set the updated value back to the field
    //            workDoneField.SetValue(jobDriver, currentWorkDone);
    //            DebugHelpers.DebugLog("Patch_PlantWork", $" Adjusted workDone. New value: {currentWorkDone}");

    //            FieldInfo testFeild = typeof(JobDriver_PlantWork).GetField("workDone", BindingFlags.Instance | BindingFlags.NonPublic);

    //            DebugHelpers.DebugLog("Patch_PlantWork", $"if it worked the above should be {(float)testFeild.GetValue(jobDriver)}");
    //        }
    //        DebugHelpers.DebugLog("Patch_PlantWork", $" Postfix End");

    //    }
//}
    //}
}
