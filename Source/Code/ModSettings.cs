using UnityEngine;
using Verse;

namespace BoneMod
{

    public class BoneModSettings : ModSettings
    {
        public static float boneFactor = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref boneFactor, "boneFactor", 1f);
        }

    }

    public class BoneModMod : Mod
    {
        BoneModSettings settings;

        public BoneModMod(ModContentPack con) : base(con)
        {
            settings = GetSettings<BoneModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new();
            listing.Begin(inRect);
            // Use Widgets.Label with explicit rects to avoid relying on Listing_Standard.Label overloads that differ across RimWorld versions
            Widgets.Label(listing.GetRect(Text.LineHeight), "Multiplier");
            BoneModSettings.boneFactor = listing.Slider(BoneModSettings.boneFactor, 0f, 10f);
            Widgets.Label(listing.GetRect(Text.LineHeight), "ROM_SettingsBoneMultiplier_Num".Translate(BoneModSettings.boneFactor));
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Rim of Madness - Bones";
        }
    }
}