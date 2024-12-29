using System;
using System.Reflection;
using Verse;
using HarmonyLib;

namespace Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Retrieves the value of a nested field or property using reflection or Traverse.
        /// Automatically handles private/protected members.
        /// </summary>
        /// <param name="obj">The object to retrieve the value from.</param>
        /// <param name="names">The names of the nested fields or properties to retrieve.</param>
        /// <returns>The value of the nested field or property, or null if not found.</returns>
        public static object GetNestedFieldOrPropertyValue(object obj, params string[] names)
        {
            string path = obj?.GetType().Name ?? "null";

            foreach (var name in names)
            {
                if (obj == null)
                {
                    DebugHelpers.DebugLog("ReflectionHelper", $"Object is null while resolving path: {path}.{name}");
                    return null;
                }

                var type = obj.GetType();

                // Attempt direct reflection first
                while (type != null)
                {
                    // Try to get field
                    var field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (field != null)
                    {
                        obj = field.GetValue(obj);
                        path += $".{field.Name}";
                        DebugHelpers.DebugLog("ReflectionHelper", $"Found field '{field.Name}' at path: {path}");
                        break;
                    }

                    // Try to get property
                    var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (property != null)
                    {
                        try
                        {
                            // Check if the property has a getter and is accessible
                            if (property.CanRead && property.GetMethod != null)
                            {
                                obj = property.GetValue(obj);
                                path += $".{property.Name}";
                                DebugHelpers.DebugLog("ReflectionHelper", $"Found public property '{property.Name}' at path: {path}");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugHelpers.DebugLog("ReflectionHelper", $"Failed to access property '{property.Name}' via reflection: {ex.Message}. Falling back to Traverse.");
                        }
                    }

                    // Move to base type if not found
                    type = type.BaseType;
                }

                // If neither field nor property is found, fallback to Traverse
                if (type == null)
                {
                    DebugHelpers.DebugLog("ReflectionHelper", $"Direct reflection failed for '{name}', attempting Traverse.");
                    var traverse = Traverse.Create(obj).Field(name) ?? Traverse.Create(obj).Property(name);
                    if (traverse != null)
                    {
                        obj = traverse.GetValue();
                        path += $".(Traverse:{name})";
                        DebugHelpers.DebugLog("ReflectionHelper", $"Found via Traverse: '{name}' at path: {path}");
                        continue;
                    }

                    DebugHelpers.DebugLog("ReflectionHelper", $"Failed to find field or property '{name}' using reflection or Traverse at path: {path}");
                    return null;
                }
            }

            DebugHelpers.DebugLog("ReflectionHelper", $"Successfully resolved '{names[names.Length - 1]}' at path: {path}");
            return obj;
        }
    }
}
