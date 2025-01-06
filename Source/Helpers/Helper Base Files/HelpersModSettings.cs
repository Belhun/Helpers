//using UnityEngine;
//using Verse;

//namespace Helpers
//{
//    public class HelpersModSettings : ModSettings
//    {
//        public bool LogUnknownSurgeries = true;
//        public bool ProcessUnknownSurgeries = false;
//        public bool DisableUnknownSurgeryNotifications = false; // New setting
//        public bool HasNotifiedUnknownSurgery = false; // Session-based flag

//        public override void ExposeData()
//        {
//            base.ExposeData();
//            Scribe_Values.Look(ref LogUnknownSurgeries, "LogUnknownSurgeries", true);
//            Scribe_Values.Look(ref ProcessUnknownSurgeries, "ProcessUnknownSurgeries", false);
//            Scribe_Values.Look(ref DisableUnknownSurgeryNotifications, "DisableUnknownSurgeryNotifications", false);
//        }
//    }



//    public class HelpersMod : Mod
//    {
//        public static HelpersModSettings Settings;

//        public HelpersMod(ModContentPack content) : base(content)
//        {
//            Settings = GetSettings<HelpersModSettings>();
//        }

//        public override string SettingsCategory()
//        {
//            return "Helpers Mod";
//        }

//        public override void DoSettingsWindowContents(Rect inRect)
//        {
//            var listing = new Listing_Standard();
//            listing.Begin(inRect);

//            listing.Label("Settings for unknown surgeries:");

//            // Toggle for logging unknown surgeries
//            listing.CheckboxLabeled("Log Unknown Surgeries", ref Settings.LogUnknownSurgeries);

//            // Toggle for processing unknown surgeries
//            listing.CheckboxLabeled("Process Unknown Surgeries", ref Settings.ProcessUnknownSurgeries);

//            // Toggle for disabling notifications
//            listing.CheckboxLabeled("Disable Unknown Surgery Notifications", ref Settings.DisableUnknownSurgeryNotifications);

//            listing.End();
//            base.DoSettingsWindowContents(inRect);
//        }
//    }


//}
