using RimWorld;
using Verse;

namespace MechanitePersonaTraits
{
    //The Big One!
    class Hediff_MechaniteCapacity : HediffWithComps
    {
        private readonly NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");

        private readonly float burstingFall = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().burstingFallSetting;
        private readonly float overflowingFall = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().overflowingFallSetting;
        private readonly float swellingFall = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().swellingFallSetting;
        private readonly float normalFall = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().normalFallSetting;

        public void PlagueLustTick()
        {
            //Check if pawn has Mechanite Capacity...
            Hediff mechaniteCapacity = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            //This works..? 
            if (mechaniteCapacity != null)
            {
                if (mechaniteCapacity.Severity >= 0.90f)
                {
                    //Overflowing rises to Bursting :(
                    //Capacity limit reached. Plaguelust, if its not 0 by now... it soon will be.
                    plaguelust.fallPerDay = burstingFall;
                }
                else if (mechaniteCapacity.Severity >= 0.75f && mechaniteCapacity.Severity < 0.90f)
                {
                    //Swelling rising to Overflowing
                    //Urges begin to spiral out of control as mechanite reserves greatly exceed "normal" levels.
                    plaguelust.fallPerDay = overflowingFall;
                }
                else if (mechaniteCapacity.Severity >= 0.50f && mechaniteCapacity.Severity < 0.75f)
                {
                    //Normal rising to Swelling
                    //Urges escalate as mechanite reserves begin to exceed "normal" levels.
                    plaguelust.fallPerDay = swellingFall;
                }
                else if (mechaniteCapacity.Severity >= 0.25f && mechaniteCapacity.Severity < 0.50f)
                {
                    //Diminished rising to Normal
                    //Urges return to normal as mechanite reserves return to "normal".
                    plaguelust.fallPerDay = normalFall;
                }
                else
                {
                    //Diminished AND Depleted
                    //Urges are satisfied due to diminished mechanite reserves. How nice.
                    plaguelust.fallPerDay = 0f;
                }
            }
        }

        //This little guy gives the MPT_MechaniteCapacity hediff a neat little bar
        //To visually show how high/low it is! :)
        public override string SeverityLabel
        {
            get
            {
                if (this.Severity <= 0f)
                {
                    return null;
                }
                return this.Severity.ToStringPercent("F0");
            }
        }

        //Final result in-game
        public override void Tick()
        {
            //Call PlagueLustTick - Gotta update that plaguelust fallPerDay for the pawn.
            base.Tick();
            PlagueLustTick();
        }
    }
}
