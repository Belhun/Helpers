using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Helpers
{
    public static class DebugHelpers
    {
        public static bool OverallLogging = true; // Enable/Disable all logging globally
        public static bool CustomToils_Recipe_Logging = true; // Enable/Disable logging for CustomToils_Recipe
        public static bool PawnBeautyChecker_Logging = true; // Enable/Disable logging for PawnBeautyChecker
        public static bool Pawn_SpawnSetup_Patch_Logging = true; // Enable/Disable logging for Pawn_SpawnSetup_Patch
        public static bool PawnHelperComponent_Logging = true; // Enable/Disable logging for PawnHelperComponent
        public static bool CompProperties_PawnHelper_Logging = true; // Enable/Disable logging for CompProperties_PawnHelper
        public static bool PawnHelperExtensions_Logging = true; // Enable/Disable logging for PawnHelperExtensions
        public static bool FloatMenuMakerMap_Logging = true; // Enable/Disable logging for FloatMenuMakerMap
        public static bool DoRecipeWorkOverride_Logging = true; // Enable/Disable logging for DoRecipeWorkOverridePatch
        public static bool JobDriver_Helping_Logging = true; // Enable/Disable logging for JobDriver_Helping

        public static void CTRLog(string message)
        {
            if (OverallLogging && CustomToils_Recipe_Logging)
            {
                Log.Message($"[Helpers Mod][CustomToils_Recipe] {message}");
            }
        }

        public static void PBCLog(string message)
        {
            if (OverallLogging && PawnBeautyChecker_Logging)
            {
                Log.Message($"[Helpers Mod][PawnBeautyChecker] {message}");
            }
        }

        public static void PSSPLog(string message)
        {
            if (OverallLogging && Pawn_SpawnSetup_Patch_Logging)
            {
                Log.Message($"[Helpers Mod][Pawn_SpawnSetup_Patch] {message}");
            }
        }

        public static void PHCLog(string message)
        {
            if (OverallLogging && PawnHelperComponent_Logging)
            {
                Log.Message($"[Helpers Mod][PawnHelperComponent] {message}");
            }
        }

        public static void CPPHLog(string message)
        {
            if (OverallLogging && CompProperties_PawnHelper_Logging)
            {
                Log.Message($"[Helpers Mod][CompProperties_PawnHelper] {message}");
            }
        }

        public static void PHExtLog(string message)
        {
            if (OverallLogging && PawnHelperExtensions_Logging)
            {
                Log.Message($"[Helpers Mod][PawnHelperExtensions] {message}");
            }
        }

        public static void FMMLog(string message)
        {
            if (OverallLogging && FloatMenuMakerMap_Logging)
            {
                Log.Message($"[Helpers Mod][FloatMenuMakerMap] {message}");
            }
        }

        public static void DRWLog(string message)
        {
            if (OverallLogging && DoRecipeWorkOverride_Logging)
            {
                Log.Message($"[Helpers Mod][DoRecipeWorkOverridePatch] {message}");
            }
        }

        public static void JDHLog(string message)
        {
            if (OverallLogging && JobDriver_Helping_Logging)
            {
                Log.Message($"[Helpers Mod][JobDriver_Helping] {message}");
            }
        }
    }
}
