using MP_MechanitePlague;
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

        readonly bool mechaniteModSettingInsect = LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns;
        readonly bool mechaniteModSettingAnimal = LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns;

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
            listingStandard.Label("SETTINGS REQUIRE RESTART TO TAKE EFFECT!".Colorize(Color.yellow), -1f, null);
            listingStandard.Label("Hover over labels for additional information.".Colorize(Color.yellow), -1f, "Hey look! A Tooltip!");
            listingStandard.Gap();

            //----Mechanite Capacity Thresholds Start
            listingStandard.Label("Mechanite Capacity Thresholds", -1, "Settings below determine how Mechanite Capacity works once certain thresholds are reached.");
            Text.Font = GameFont.Small;

            //Bursting - Stage 4 -  Highest Limit
            listingStandard.Label("Mechanite Capacity (Bursting): " + decimal.Round((decimal)burstingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", +-1f, "Highest limit of Mechanite Capacity (Severity >= " + ("0.90").Colorize(Color.black) + "). How fast should Plaguelust fall per day once this threshold is reached?");
            burstingFallSetting = listingStandard.Slider(burstingFallSetting, 0.1f, 10f);

            //Overflowing - Stage 3 - High
            listingStandard.Label("Mechanite Capacity (Overflowing): " + decimal.Round((decimal)overflowingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "High limit of Mechanite Capacity (Severity >= " + ("0.75").Colorize(Color.grey) + "). How fast should Plaguelust fall per day once this threshold is reached?");
            overflowingFallSetting = listingStandard.Slider(overflowingFallSetting, 0.1f, 10f);

            //Swelling - Stage 2 - Medium
            listingStandard.Label("Mechanite Capacity (Swelling): " + decimal.Round((decimal)swellingFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "Medium limit of Mechanite Capacity (Severity >= " + ("0.50").Colorize(Color.yellow) + "). How fast should Plaguelust fall per day once this threshold is reached?");
            swellingFallSetting = listingStandard.Slider(swellingFallSetting, 0.1f, 10f);

            //Normal - Stage 1 - Low
            listingStandard.Label("Mechanite Capacity (Normal): " + decimal.Round((decimal)normalFallSetting, 2).ToString().Colorize(Color.green) + " Fall per Day.", -1f, "Low limit of Mechanite Capacity (Severity >= " + ("0.25").Colorize(Color.green) + "). How fast should Plaguelust fall per day once this threshold is reached?");
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

            listingStandard.Label("Mechanite Capacity recovery: -" + mechaniteRecovery.ToStringPercent() + " severity", -1f, "After successfully infecting a target with the Mechanite Plague, Mechanite Capacity severity is reduced.\n\nThis number determines how much Mechanite Capacity will be reduced by.");
            mechaniteRecovery = listingStandard.Slider(mechaniteRecovery, 0.025f, 0.100f);

            listingStandard.Label("Plaguelust recovery: " + plaguelustRecovery.ToStringPercent(), -1f, "After successfully infecting a target with the Mechanite Plague, Plaguelust need is increased. This satisfies the urges for pawns bonded to a Mechanite Infester persona weapon.\n\nThis number determines how much Plaguelust will be increased by.");
            plaguelustRecovery = listingStandard.Slider(plaguelustRecovery, 0.025f, 0.100f);

            listingStandard.Gap();
            Text.Font = GameFont.Medium;
            listingStandard.Label("Trait spawn settings are now handled by More Persona Traits - Trait Spawn mod settings!", -1, "THIS WILL NOT REMOVE THE TRAIT FROM THE GAME. ONLY PREVENT THEM FROM BEING ROLLED.\n\nIn the time I was gone, Arquebus made a whole thing for it. So just use that instead please!");
            listingStandard.Label("Mechanite Infester requires humanlike targets to purge mechanites! - Hover for more info!".Colorize(Color.yellow), -1, "Mechanite Purge is needed for Plaguelust recovery and Mechanite recovery to take effect when using the Mechanite Infester persona trait.\n\nThis behavior can be altered inside " + ("The Mechanite Plague").Colorize(Color.green) + " mod settings by changing allowed spawns for insects and animals.\n\nDoing so will make Mechanite Infester work normally even against non-humanlike pawns. However, settings may still need a restart to take effect.");
            Text.Font = GameFont.Small;

            //----Recovery Settings End

            defaultSettings = listingStandard.ButtonText("Reset settings", "All settings will revert to default values", 0.2f);
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
            return "Mechanite Persona Traits";
        }
    }
}