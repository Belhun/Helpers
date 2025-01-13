using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Helpers
{
    public static class DebugHelpers
    {
        public static bool OverallLogging = true; // Enable/Disable all logging globally

        // Dictionary to manage logging per class/feature
        private static readonly Dictionary<string, bool> ClassLoggingFlags = new Dictionary<string, bool>
        {
            { "PawnHelperComponent", false }, // Active for helper component logic
            { "FloatMenuMakerMap", false }, // Used for adding the "Help" option
            { "JobDriver_Helping", false }, // Relevant for the Helping job driver
            { "HelperSocialMechanics", false }, // Handles social thoughts logic
            { "HelperMechanics", false }, // Core mechanics for helper contributions
            { "Patch_PostFixCunstruction", false }, // Construction contribution patch
            { "Patch_PostToilsRecipe", true }, // Recipe work contribution patch
            { "SurgeryOutcomeSuccess", false }, // Handles surgery outcome success
            { "HelpersInitializer", false }, // Logs initialization details
            { "Patch_PlantSow", false },
            { "Patch_PlantWork", false },
            { "Patch_PreFixMine", false },
            { "Patch_transpiler", false },
            { "LeaderHediff", false },
            { "LeaderHediffComp_Stats", false },
            { "SkillRecord_Learn_Patch", true },
            { "StatPart_MovementSpeed", false },
            { "StatPart_HelpersMedicalTendSpeed", false },
            { "AssignIdlePawnsToHelp", true },
            { "ReflectionHelper", false }
        };
        

        /// <summary>
        /// Logs a message if logging is enabled for the specified class/feature.
        /// </summary>
        /// <param name="className">The name of the class/feature.</param>
        /// <param name="message">The message to log.</param>
        public static void DebugLog(string className, string message)
        {
            if (OverallLogging && ClassLoggingFlags.TryGetValue(className, out bool isEnabled) && isEnabled)
            {
                Verse.Log.Message($"<color=#059EDC>[Helpers Mod][{className}]</color> {message}");
            }
        }

        /// <summary>
        /// Dynamically enables or disables logging for a specific class/feature.
        /// </summary>
        /// <param name="className">The name of the class/feature.</param>
        /// <param name="isEnabled">Whether logging should be enabled or disabled.</param>
        public static void SetLogging(string className, bool isEnabled)
        {
            if (ClassLoggingFlags.ContainsKey(className))
            {
                ClassLoggingFlags[className] = isEnabled;
            }
            else
            {
                Verse.Log.Warning($"<color=#00FF00>[Helpers Mod] Attempted to set logging for unknown class/feature:</color> {className}");
            }
        }

        /// <summary>
        /// Lists all class/feature logging statuses for debugging purposes.
        /// </summary>
        public static void PrintLoggingStatus()
        {
            foreach (var entry in ClassLoggingFlags)
            {
                Verse.Log.Message($"[Helpers Mod] Logging for {entry.Key}: {(entry.Value ? "Enabled" : "Disabled")}");
            }
        }

        /// <summary>
        /// Logs the value of 'num' for debugging purposes.
        /// </summary>
        /// <param name="num">The value to log.</param>
        public static void LogNumValue(int num)
        {
            if (OverallLogging && ClassLoggingFlags.TryGetValue("Patch_transpiler", out bool isEnabled) && isEnabled)
            {
                return;
            }
            Log.Message("[Helpers] num value: " + num);
        }


    }
}
