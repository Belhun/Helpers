using System.Collections.Generic;
using Verse;

namespace Helpers
{
    public static class DebugHelpers
    {
        public static bool OverallLogging = true; // Enable/Disable all logging globally

        // Dictionary to manage logging per class/feature
        private static readonly Dictionary<string, bool> ClassLoggingFlags = new Dictionary<string, bool>
        {
            { "CustomToils_Recipe", true },
            { "Pawn_SpawnSetup_Patch", false },
            { "PawnHelperComponent", false },
            { "CompProperties_PawnHelper", false },
            { "PawnHelperExtensions", false },
            { "FloatMenuMakerMap", false },
            { "DoRecipeWorkOverridePatch", false },
            { "JobDriver_Helping", false },
            { "HelperSocialMechanics", false },
            { "JobDriver_ConstructFinishFrame_[MakeNewToils]", false },
            { "HelperMechanics", true }
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
                Verse.Log.Message($"[Helpers Mod][{className}] {message}");
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
                Verse.Log.Warning($"[Helpers Mod] Attempted to set logging for unknown class/feature: {className}");
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
    }
}
