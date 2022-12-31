using RimWorld;
using MP_MechanitePlague;
using System.Linq;
using Verse;

namespace MPT_MechaniteHelper
{
    //The Big One!
    class Hediff_MechaniteCapacity : HediffWithComps
    {
        static bool targetPlagued = false;
        public static bool IsInfected()
        {
            //Once true, final result will bestow relief to those
            //who are infested with mechanites... for a time...
            return targetPlagued = true;
        }
        public void PlagueLustTick()
        {
            //Check if pawn has Mechanite Capacity...
            Hediff mechaniteCapacity = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));
            //Temporary - One pawn with this Hediff defines how fast Plaguelust need falls. Two pawns with this hediff destroys all of reality.
            //Find the Database entry for my custom NeedDef
            var plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");
            if (mechaniteCapacity != null)
            {
                if (mechaniteCapacity.Severity >= 0.90f)
                {
                    //Overflowing rises to Bursting :(
                    //Plaguelust if its not 0 by now... it will be.
                    plaguelust.fallPerDay = 3;
                }
                else if (mechaniteCapacity.Severity >= 0.75f && mechaniteCapacity.Severity < 0.90f)
                {
                    //Swelling rising to Overflowing
                    //Urges begin to spiral out of control as mechanite reserves greatly exceed "normal" capacity.
                    plaguelust.fallPerDay = 0.95f;
                }
                else if (mechaniteCapacity.Severity >= 0.50f && mechaniteCapacity.Severity < 0.75f)
                {
                    //Normal rising to Swelling
                    //Urges escalate as mechanite reserves begin to exceed "normal" capacity.
                    plaguelust.fallPerDay = 0.65f;
                }
                else if (mechaniteCapacity.Severity >= 0.25f && mechaniteCapacity.Severity < 0.50f)
                {
                    //Diminished rising to Normal
                    //Urges return to normal as mechanite reserves return to "normal".
                    plaguelust.fallPerDay = 0.35f;
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
        //To show how high/low it is :)
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
            base.Tick();
            //Check if pawn is a Mechanite Plague Lich...
            Hediff plagueLich = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_Bonded_MechanitePlagueLich"));
            //Check if pawn has Mechanite Capacity - Ticks once until new infection is found
            Hediff mechaniteCapacity = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));
            if (plagueLich != null && targetPlagued)
            {
                //If Mechanite Lich then purge some Mechanites
                mechaniteCapacity.Severity -= 0.018f;
                var plaguelust = pawn.needs.AllNeeds.Where(need => need is Need_Chemical).RandomElementWithFallback();
                var plaguelustRecovery = 0.045f;
                if (plaguelust == null) return;
                //THEN save the damned soul from infecting others... at least for a time.
                plaguelust.CurLevel += plaguelust.MaxLevel * plaguelustRecovery;
            }
            //Call PlagueLust then return bool to false so code doesn't loop
            PlagueLustTick();
            targetPlagued = false;
        }
    }

    //Basic Mechanite Plague Trait: Allows any persona weapon to inflict the mechanite plague.
    class Hediff_MechaniteUnion : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            //Check if pawn has Union Hediff to replace with Mechanite Plague
            Hediff unionOnPawn = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteUnion"));
            //Internal check against Non-humanlike targets incase of player shenanigans
            if (unionOnPawn != null && pawn.RaceProps.Humanlike)
            {
                PlagueMethodHolder.InfectPawn(pawn, Faction.OfPlayer, 0.09f, 0.12f, 0);
            }
            //Remove itself for a job well done :)
            unionOnPawn.Severity = 0.00f;
        }
    }

    //Advanced Mechanite Plague Trait: Damages AND infects the target pawn with the mechanite plague...
    //However that pawn might have gotten off lucky...
    class Hediff_MechaniteInfester : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            //Check if pawn has Infester Hediff to replace with Mechanite Plague
            Hediff infestedPawn = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteInfester"));
            //Internal check against Non-humanlike targets incase of player shenanigans
            if (infestedPawn != null && pawn.RaceProps.Humanlike)
            {
                //If pawn does have MPT_MechaniteInfester, run code stolen from Irecreeper (With Permission :)
                DamageWorker.DamageResult damageResult = pawn.TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("MPT_Damage_MechaniteInfestation", true), 3f, 0.5f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
                if (damageResult.wounded)
                {
                    PlagueMethodHolder.InfectPawn(pawn, Faction.OfPlayer, 0.13f, 0.16f, 1);
                    Hediff_MechaniteCapacity.IsInfected();
                }
            }
            //Remove itself... only to be added later when the urge to infect returns...
            infestedPawn.Severity = 0.00f;
        }
    }
}