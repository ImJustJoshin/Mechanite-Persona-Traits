using UnityEngine;
using Verse;

namespace MechanitePersonaTraits
{
    public class MechanitePersonaTraitsSettings : ModSettings
    {
        public float burstingFallSetting, burstingFallDefault = 6.5f;
        public float overflowingFallSetting, overflowingFallDefault = 2.5f;
        public float swellingFallSetting, swellingFallDefault = 0.65f;
        public float normalFallSetting, normalFallDefault = 0.35f;

        public float mechaniteRecovery, mechaniteDefault = 0.048f;
        public float plaguelustRecovery, plaguelustDefault = 0.085f;

        public bool defaultSettings = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref burstingFallSetting, "MPT_MechaniteCapacity.Bursting", 6.5f);
            Scribe_Values.Look(ref overflowingFallSetting, "MPT_MechaniteCapacity.Overflowing", 2.5f);
            Scribe_Values.Look(ref swellingFallSetting, "MPT_MechaniteCapacity.Swelling", 0.65f);
            Scribe_Values.Look(ref normalFallSetting, "MPT_MechaniteCapacity.Normal", 0.35f);

            Scribe_Values.Look(ref mechaniteRecovery, "MPT_MechaniteCapacity.Recovery", 0.048f);
            Scribe_Values.Look(ref plaguelustRecovery, "MPT_Need_MechaniteFactory.Recovery", 0.085f);
            
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            //Begin Settings
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            //----Notice Start
            Text.Font = GameFont.Medium;
            listingStandard.Label("MPT_SettingRestart".Translate().Colorize(Color.yellow), -1f, null);
            listingStandard.Label("MPT_SettingNoticeTop".Translate().Colorize(Color.yellow), -1f, "MPT_SettingNoticeTopTooltip".Translate());
            listingStandard.Gap();
            //----Notice End

            //----Mechanite Capacity Thresholds Start
            listingStandard.Label("MPT_SettingThresholdLabel".Translate(), -1, "MPT_SettingThresholdLabelTooltip".Translate());
            Text.Font = GameFont.Small;

            //Bursting - Stage 4 -  Highest Limit
            listingStandard.Label("MPT_SettingThresholdHighest".Translate(decimal.Round((decimal)burstingFallSetting, 2).ToString().Colorize(Color.green)), -1f, "MPT_SettingThresholdHighestTooltip".Translate((DefDatabase<HediffDef>.GetNamed("MPT_MechaniteCapacity").stages[5].minSeverity).ToString()));
            burstingFallSetting = listingStandard.Slider(burstingFallSetting, 0.1f, 10f);

            //Overflowing - Stage 3 - High
            listingStandard.Label("MPT_SettingThresholdHigh".Translate(decimal.Round((decimal)overflowingFallSetting, 2).ToString().Colorize(Color.green)), -1f, "MPT_SettingThresholdHighTooltip".Translate((DefDatabase<HediffDef>.GetNamed("MPT_MechaniteCapacity").stages[4].minSeverity).ToString()));
            overflowingFallSetting = listingStandard.Slider(overflowingFallSetting, 0.1f, 10f);

            //Swelling - Stage 2 - Medium
            listingStandard.Label("MPT_SettingThresholdMedium".Translate(decimal.Round((decimal)swellingFallSetting, 2).ToString().Colorize(Color.green)), -1f, "MPT_SettingThresholdMediumTooltip".Translate((DefDatabase<HediffDef>.GetNamed("MPT_MechaniteCapacity").stages[3].minSeverity).ToString()));
            swellingFallSetting = listingStandard.Slider(swellingFallSetting, 0.1f, 10f);

            //Normal - Stage 1 - Low
            listingStandard.Label("MPT_SettingThresholdLow".Translate(decimal.Round((decimal)normalFallSetting, 2).ToString().Colorize(Color.green)), -1f, "MPT_SettingThresholdLowTooltip".Translate((DefDatabase<HediffDef>.GetNamed("MPT_MechaniteCapacity").stages[2].minSeverity).ToString()));
            normalFallSetting = listingStandard.Slider(normalFallSetting, 0.1f, 10f);

            //Diminished and Depleted - Stage 0 - Nothing
            //I might add settings for them but in my eyes at this level, Plaguelust fallPerDay should be 0.
            //Ensures that players don't have to keep fighting non-stop to keep Plaguelust at bay. Think luciferium.
            //Nothing happens to the pawn until the need hits zero. THAT's when the urges hit.

            //----Mechanite Capacity Thresholds End

            //----Recovery Settings Start
            listingStandard.Gap();

            Text.Font = GameFont.Medium;
            listingStandard.Label("MPT_SettingRecoveryLabel".Translate(), -1, "MPT_SettingRecoveryLabelTooltip".Translate());
            Text.Font = GameFont.Small;

            listingStandard.Label("MPT_SettingRecoveryCapacity".Translate(mechaniteRecovery.ToStringPercent()), -1f, "MPT_SettingRecoveryCapacityTooltip".Translate());
            mechaniteRecovery = listingStandard.Slider(mechaniteRecovery, 0.025f, 0.100f);

            listingStandard.Label("MPT_SettingRecoveryPlaguelust".Translate(plaguelustRecovery.ToStringPercent()), -1f, "MPT_SettingRecoveryPlaguelustTooltip".Translate());
            plaguelustRecovery = listingStandard.Slider(plaguelustRecovery, 0.025f, 0.100f);

            listingStandard.Gap();
            Text.Font = GameFont.Medium;
            listingStandard.Label("MPT_SettingNoticeBottom".Translate(), -1, "MPT_SettingNoticeBottomTooltip".Translate());
            listingStandard.Label("MPT_SettingNoticeInfester".Translate().Colorize(Color.yellow), -1, "MPT_SettingNoticeInfesterTooltip".Translate("MPT_SettingNoticeHelper".Translate()));
            Text.Font = GameFont.Small;

            //----Recovery Settings End

            defaultSettings = listingStandard.ButtonText("MPT_SettingReset".Translate(), "MPT_SettingResetTooltip".Translate(), 0.2f);
            if (defaultSettings == true)
            {
                burstingFallSetting = listingStandard.Slider(burstingFallDefault, 0.1f, 10f);
                overflowingFallSetting = listingStandard.Slider(overflowingFallDefault, 0.1f, 10f);
                swellingFallSetting = listingStandard.Slider(swellingFallDefault, 0.1f, 10f);
                normalFallSetting = listingStandard.Slider(normalFallDefault, 0.1f, 10f);

                mechaniteRecovery = listingStandard.Slider(mechaniteDefault, 0.025f, 0.100f);
                plaguelustRecovery = listingStandard.Slider(plaguelustDefault, 0.025f, 0.100f);

                defaultSettings = false;
            }

            //----Other Settings End

            //End Settings
            listingStandard.End();
        }
    }
    //Load settings in-game :)
    public class MechanitePersonaTraits : Mod
    {
        //A reference to settings above.
        public static MechanitePersonaTraitsSettings settings;

        //Resolves the refernce to the settings above.
        public MechanitePersonaTraits(ModContentPack content) : base(content)
        {
            settings = GetSettings<MechanitePersonaTraitsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "MPT_SettingCatagory".Translate();
        }
    }
}