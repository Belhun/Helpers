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
            { "CustomToils_Recipe", false },
            { "Pawn_SpawnSetup_Patch", false },
            { "PawnHelperComponent", false },
            { "CompProperties_PawnHelper", false },
            { "PawnHelperExtensions", false },
            { "FloatMenuMakerMap", false },
            { "JobDriver_Helping", false },
            { "HelperSocialMechanics", false },
            { "JobDriver_ConstructFinishFrame", false },
            { "HelperMechanics", true },
            { "Patch_CheckSurgeryFail", true },
            { "Patch_PostFixCunstruction", false },
            { "Patch_PostToilsRecipe", false },
            { "GetQualityPostfix", true },
            { "GetOutcomePostfix", true },
            { "SurgeryOutcomeSuccess", true },
            { "HelpersInitializer", true }
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


    }

    public static class ReflectionDebugger
    {
        public static void DebugInstanceFieldsAndProperties(object instance, string targetName, int maxDepth = 4)
        {
            if (instance == null)
            {
                Log.Warning("ReflectionDebugger: Instance is null.");
                return;
            }

            var visited = new HashSet<object>();
            var result = FindFieldOrPropertyPathRecursive(instance, targetName, "", maxDepth, visited);

            if (string.IsNullOrEmpty(result))
                Log.Warning($"ReflectionDebugger: Target variable '{targetName}' not found.");
            else
                Log.Message($"ReflectionDebugger: Found '{targetName}' at path: {result}");
        }

        private static string FindFieldOrPropertyPathRecursive(object instance, string targetName, string path, int depth, HashSet<object> visited)
        {
            if (depth == 0 || instance == null || visited.Contains(instance))
                return null;

            visited.Add(instance);

            var type = instance.GetType();
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try
                {
                    var value = field.GetValue(instance);
                    if (field.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                        return $"{path}.{field.Name}";

                    if (value != null)
                    {
                        var recursiveResult = FindFieldOrPropertyPathRecursive(value, targetName, $"{path}.{field.Name}", depth - 1, visited);
                        if (!string.IsNullOrEmpty(recursiveResult))
                            return recursiveResult;
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"ReflectionDebugger: Exception accessing field '{field.Name}': {e.Message}");
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.GetIndexParameters().Length > 0) continue; // Skip indexed properties

                try
                {
                    var value = property.GetValue(instance);
                    if (property.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                        return $"{path}.{property.Name}";

                    if (value != null)
                    {
                        var recursiveResult = FindFieldOrPropertyPathRecursive(value, targetName, $"{path}.{property.Name}", depth - 1, visited);
                        if (!string.IsNullOrEmpty(recursiveResult))
                            return recursiveResult;
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"ReflectionDebugger: Exception accessing property '{property.Name}': {e.Message}");
                }
            }

            if (instance is IEnumerable enumerable)
            {
                var index = 0;
                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    var recursiveResult = FindFieldOrPropertyPathRecursive(item, targetName, $"{path}[{index}]", depth - 1, visited);
                    if (!string.IsNullOrEmpty(recursiveResult))
                        return recursiveResult;

                    index++;
                }
            }

            return null;
        }

        public static void InspectObject(object obj, string context, int depth = 0)
        {
            if (obj == null || depth > 5) // Prevent infinite recursion
            {
                Log.Message($"{context}: (null or max depth reached)");
                return;
            }

            var type = obj.GetType();
            Log.Message($"{new string('-', depth)}Inspecting {context} (Type: {type.Name})");

            // Log fields
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                try
                {
                    var fieldValue = field.GetValue(obj);
                    Log.Message($"{new string(' ', depth)}Field: {field.Name}, Type: {fieldValue?.GetType().Name ?? "null"}, Value: {fieldValue}");
                    InspectObject(fieldValue, $"{context}.{field.Name}", depth + 1); // Recurse into the field
                }
                catch (Exception e)
                {
                    Log.Warning($"Could not inspect field {field.Name}: {e.Message}");
                }
            }

            // Log properties
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                try
                {
                    var propertyValue = property.GetValue(obj, null);
                    Log.Message($"{new string(' ', depth)}Property: {property.Name}, Type: {propertyValue?.GetType().Name ?? "null"}, Value: {propertyValue}");
                    InspectObject(propertyValue, $"{context}.{property.Name}", depth + 1); // Recurse into the property
                }
                catch (Exception e)
                {
                    Log.Warning($"Could not inspect property {property.Name}: {e.Message}");
                }
            }

            // Log methods (not recursing into methods, just logging their names)
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                Log.Message($"{new string(' ', depth)}Method: {method.Name}");
            }
        }

    }


}
