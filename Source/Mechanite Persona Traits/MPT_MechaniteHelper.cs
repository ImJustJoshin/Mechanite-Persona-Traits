using RimWorld;
using Verse;

namespace MPT_MechaniteHelper
{
    //The Big One!
    class Hediff_MechaniteCapacity : HediffWithComps
    {
        private readonly NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");

        //TODO: Expose these values in a Mod Options menu for players.
        public float burstingFall = 6.5f;
        public float overflowingFall = 2.5f;
        public float swellingFall = 0.65f;
        public float normalFall = 0.35f;

        public void PlagueLustTick()
        {
            //Check if pawn has Mechanite Capacity...
            Hediff mechaniteCapacity = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            //This works...?
            if (mechaniteCapacity != null)
            {
                if (mechaniteCapacity.Severity >= 0.90f)
                {
                    //Overflowing rises to Bursting :(
                    //Plaguelust if its not 0 by now... it soon will be.
                    plaguelust.fallPerDay = burstingFall;
                }
                else if (mechaniteCapacity.Severity >= 0.75f && mechaniteCapacity.Severity < 0.90f)
                {
                    //Swelling rising to Overflowing
                    //Urges begin to spiral out of control as mechanite reserves greatly exceed "normal" capacity.
                    plaguelust.fallPerDay = overflowingFall;
                }
                else if (mechaniteCapacity.Severity >= 0.50f && mechaniteCapacity.Severity < 0.75f)
                {
                    //Normal rising to Swelling
                    //Urges escalate as mechanite reserves begin to exceed "normal" capacity.
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
                    plaguelust.fallPerDay = 0;
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
