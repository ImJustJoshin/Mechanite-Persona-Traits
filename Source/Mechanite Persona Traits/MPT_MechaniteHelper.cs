using RimWorld;
using MP_MechanitePlague;
using Verse;

namespace MPT_MechaniteHelper
{
    //The Big One!
    class Hediff_MechaniteCapacity : HediffWithComps
    {
        static bool targetPlagued = false;
        public NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");

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

            //Gotta be honest. I have no idea how this doesn't conflict if multiple pawns have this hediffClass
            //on them. I truly believed that changing the GLOBAL setting for Plaguelust's fallPerDay on a per
            //pawn basis would lead to one pawn setting the fallPerDay too high or too low depending on
            //their Mechanite Capacity such as one pawn at Bursting making another pawn at Depleted
            //drop the plaguelust super fast but nope. In-game this doesn't happen at all.
            //No clue why. My best guess is that on the pawn's tick it then changes to the
            //correct value for themselves and the next pawn does the same.
            //Because this... works? Unless I am told otherwise I'm just going to leave it.
            if (mechaniteCapacity != null)
            {
                if (mechaniteCapacity.Severity >= 0.90f)
                {
                    //Overflowing rises to Bursting :(
                    //Plaguelust if its not 0 by now... it soon will be.
                    plaguelust.fallPerDay = 6.5f;
                }
                else if (mechaniteCapacity.Severity >= 0.75f && mechaniteCapacity.Severity < 0.90f)
                {
                    //Swelling rising to Overflowing
                    //Urges begin to spiral out of control as mechanite reserves greatly exceed "normal" capacity.
                    plaguelust.fallPerDay = 2.5f;
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
            base.Tick();
            //Check if pawn is a Mechanite Plague Lich...
            //Then check if pawn has Mechanite Capacity...
            Hediff plagueLich = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_Bonded_MechanitePlagueLich"));
            Hediff mechaniteCapacity = pawn.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            //If they do and Hediff_MechaniteCapacity.IsInfected() was called do this...
            //To Do: Currently this setup allows all Mechanite Plague Lich to recover
            //the moment one Lich attacks and infects successfully. Fix this.
            if (plagueLich != null && targetPlagued)
            {
                //PURGE THEM MECHANITES
                mechaniteCapacity.Severity -= 0.018f;
                var plaguelustLevel = pawn.needs.TryGetNeed(plaguelust);
                //To Do: Expose this variable for player modification (Configs)
                var plaguelustRecovery = 0.095f;
                if (plaguelust == null) return;
                //THEN save the damned soul from infecting others... at least for a time.
                plaguelustLevel.CurLevel += plaguelustLevel.MaxLevel * plaguelustRecovery;
                //End loop so code doesn't go BRR
                targetPlagued = false;
            }
            //Call PlagueLustTick - Gotta update that plaguelust fallPerDay for the pawn.
            PlagueLustTick();
        }
    }

    //Basic Mechanite Plague Trait: Allows any persona weapon to inflict the mechanite plague. Simple.
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