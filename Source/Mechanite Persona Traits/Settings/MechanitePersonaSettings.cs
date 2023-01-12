using RimWorld;
using UnityEngine;
using Verse;

namespace MechanitePersonaTraits
{
    public class MechanitePersonaSettings : ModSettings
    {
        public float burstingFallSetting, burstingFallDefault = 6.5f;
        public float overflowingFallSetting, overflowingFallDefault = 2.5f;
        public float swellingFallSetting, swellingFallDefault = 0.65f;
        public float normalFallSetting, normalFallDefault = 0.35f;

        public float mechaniteRecovery, mechaniteDefault = 0.048f;
        public float plaguelustRecovery, plaguelustDefault = 0.085f;

        public bool infectorTraitSpawn = true;
        public bool injectorTraitSpawn = true;
        public bool infesterTraitSpawn = true;

        public bool defaultSettings = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref infectorTraitSpawn, "MPT_OnHit_Mechanite", true);
            Scribe_Values.Look(ref injectorTraitSpawn, "MPT_OnHit_MechaniteSelf", true);
            Scribe_Values.Look(ref infesterTraitSpawn, "MPT_Unique_MechaniteInfester", true);

            base.ExposeData();
        }

        //The probably important part that allows us to set our settings through GUI.
        public void DoSettingsWindowContents(Rect inRect)
        {
            //Begin Settings
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            //----Notice Start

            Text.Font = GameFont.Medium;
            listingStandard.Label("SETTINGS REQUIRE RESTART TO TAKE EFFECT!".Colorize(Color.yellow), -1f, null);

            //----Mechanite Capacity Thresholds Start
            listingStandard.Gap();
            listingStandard.Label("Mechanite Capacity Thresholds", -1, "Settings below determine how Mechanite Capacity works on a Plague Lich once certain thresholds are reached.");
            Text.Font = GameFont.Small;

            //Bursting - Stage 4 -  Highest Limit
            listingStandard.Label("Mechanite Capacity (Bursting): " + decimal.Round((decimal)burstingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", + - 1f, "Highest limit of Mechanite Capacity (Severity >= 0.90). How fast should Plaguelust fall per day once this threshold is reached?");
            burstingFallSetting = listingStandard.Slider(burstingFallSetting, 0.1f, 10f);

            //Overflowing - Stage 3 - High
            listingStandard.Label("Mechanite Capacity (Overflowing): " + decimal.Round((decimal)overflowingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "High limit of Mechanite Capacity (Severity >= 0.75). How fast should Plaguelust fall per day once this threshold is reached?");
            overflowingFallSetting = listingStandard.Slider(overflowingFallSetting, 0.1f, 10f);

            //Swelling - Stage 2 - Medium
            listingStandard.Label("Mechanite Capacity (Swelling): " + decimal.Round((decimal)swellingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "Medium limit of Mechanite Capacity (Severity >= 0.50). How fast should Plaguelust fall per day once this threshold is reached?");
            swellingFallSetting = listingStandard.Slider(swellingFallSetting, 0.1f, 10f);

            //Normal - Stage 1 - Low
            listingStandard.Label("Mechanite Capacity (Normal): " + decimal.Round((decimal)normalFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "Low limit of Mechanite Capacity (Severity >= 0.25). How fast should Plaguelust fall per day once this threshold is reached?");
            normalFallSetting = listingStandard.Slider(normalFallSetting, 0.1f, 10f);

            //Diminished and Depleted - Stage 0 - Nothing
            //I might add settings for them but in my eyes at this level, Plaguelust fallPerDay should be 0.
            //Ensures that players don't have to keep fighting non-stop to keep Plaguelust at bay. Think luciferium.
            //Nothing happens to the pawn until the need hits zero. THAT's when the urges hit.

            //----Mechanite Capacity Thresholds End

            //----Recovery Settings Start
            listingStandard.Gap();

            Text.Font = GameFont.Medium;
            listingStandard.Label("Recovery Settings", -1, "Settings below determine the recovery rates for the trait, Mechanite Infester.");
            Text.Font = GameFont.Small;

            listingStandard.Label("Mechanite Capacity recovery: -" + decimal.Round((decimal)mechaniteRecovery * 100, 2).ToString() + "% severity", -1f, "After successfully infecting a target with the Mechanite Plague, Mechanite Capacity severity is reduced.\n\nThis number determines how much Mechanite Capacity will be reduced by.");
            mechaniteRecovery = listingStandard.Slider(mechaniteRecovery, 0.025f, 0.100f);

            listingStandard.Label("Plaguelust recovery: " + decimal.Round((decimal)plaguelustRecovery * 100, 2).ToString() + "%", -1f, "After successfully infecting a target with the Mechanite Plague, Plaguelust need is increased. This satisfies the urges of a Mechanite Plague Lich.\n\nThis number determines how much Plaguelust will be increased by.");
            plaguelustRecovery = listingStandard.Slider(plaguelustRecovery, 0.025f, 0.100f);

            defaultSettings = listingStandard.ButtonText("Reset settings", null, 0.2f);
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

            //----Recovery Settings End

            //----Other Settings Start
            listingStandard.Gap();

            Text.Font = GameFont.Medium;
            listingStandard.Label("Other Settings", -1, "Settings below are other, more miscellaneous changes for this mod.");
            Text.Font = GameFont.Small;

            //Trait Spawn Checks
            listingStandard.CheckboxLabeled("Enable/Disable - " + "Positive".Colorize(Color.green) + " weapon trait: Mechanite Infector.        Default: " + "Enabled".Colorize(Color.green), ref infectorTraitSpawn, "This setting, if disabled, will prevent the weapon trait, Mechanite Infector, from spawning. This does not remove this trait from any existing weapons to prevent errors.", 0f, 0.58f);
            listingStandard.CheckboxLabeled("Enable/Disable - " + "Negative".Colorize(Color.red) + " weapon trait: Mechanite Injector.      Default: " + "Enabled".Colorize(Color.green), ref injectorTraitSpawn, "This setting, if disabled, will prevent the weapon trait, Mechanite Injector, from spawning. This does not remove this trait from any existing weapons to prevent errors.", 0f, 0.58f);
            listingStandard.CheckboxLabeled("Enable/Disable - " + "Positive...?".Colorize(Color.yellow) + " weapon trait: Mechanite Infester.   Default: " + "Enabled".Colorize(Color.green), ref infesterTraitSpawn, "This setting, if disabled, will prevent the weapon trait, Mechanite Infester, from spawning. This does not remove this trait from any existing weapons to prevent errors.", 0f, 0.58f);
            
            //Can't set commonality to zero because hardcoded error.
            //However, ensuring next to impossible chances is basically the same thing.
            if (infectorTraitSpawn == false)
            {
                WeaponTraitDef mechaniteInfector = DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite");
                mechaniteInfector.commonality = 0.0000001f;
            }

            if (injectorTraitSpawn == false)
            {
                WeaponTraitDef mechaniteInjector = DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_MechaniteSelf");
                mechaniteInjector.commonality = 0.0000001f;
            }

            if (infesterTraitSpawn == false)
            {
                WeaponTraitDef mechaniteInfester = DefDatabase<WeaponTraitDef>.GetNamed("MPT_Unique_MechaniteInfester");
                mechaniteInfester.commonality = 0.0000001f;
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
        public static MechanitePersonaSettings settings;

        //Resolves the refernce to the settings above.
        public MechanitePersonaTraits(ModContentPack content) : base(content)
        {
            MechanitePersonaTraits.settings = GetSettings<MechanitePersonaSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            MechanitePersonaTraits.settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Mechanite Persona Traits";
        }
    }
}