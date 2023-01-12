using RimWorld;
using MP_MechanitePlague;
using MorePersonaTraits.WorkerClasses.OnHitWorkerClasses;
using Verse;

namespace MechanitePersonaTraits.OnHitWorkerClasses
{
    //Modified ApplyHediff from More Persona Traits by Arquebus
    public class OnHitWorker_ApplyMechanite : OnHitWorker
    {
        //public HediffDef HediffDef = DefDatabase<HediffDef>.GetNamed("MP_MechanitePlague");

        //Additional variables added by me.
        public int MechaniteLevel = 0;
        public float minInfectionSeverity = 0f;
        public float maxInfectionSeverity = 0f;
        public int extraSpawns = 0;

        public override void OnHitEffect(Thing hitThing, Thing originThing)
        {
            ApplyOnHitEffect(hitThing, originThing, ApplyMechanites);
            PurgeMechanites(originThing as Pawn, MechaniteLevel);
        }

        private void ApplyMechanites(Thing infectedThing)
        {
            //Mechanite Infector - Basic
            if (MechaniteLevel == 1)
            {
                //Infect using XML defined variables. That's it.
                PlagueMethodHolder.InfectPawn(infectedThing as Pawn, Faction.OfPlayer, minInfectionSeverity, maxInfectionSeverity, extraSpawns);
            }
            
            //Mechanite Infester - Advanced
            else if (MechaniteLevel == 2)
            {
                //Apply extra damage then if wounded, infect using XML defined variables.
                //Might add something more here. If at Overload, purge more mechanites...
                //into your enemies for an even greater infection potential!
                DamageWorker.DamageResult damageResult = (infectedThing as Pawn).TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("MPT_Damage_MechaniteInfestation", true), 3f, 0.5f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
                if (damageResult.wounded)
                {
                    PlagueMethodHolder.InfectPawn(infectedThing as Pawn, Faction.OfPlayer, minInfectionSeverity, maxInfectionSeverity, extraSpawns);
                }
            }

            //Nothing should go here unless player shenanigans in which case...
            else
            {
                //Player Shenanigans: Prevented :)
                Log.Error("[Mechanite Persona Traits]: Persona weapon MechaniteLevel used on " + infectedThing + " is out of bounds. Expected range 1-2.");
                return;
            }
        }

        private static void PurgeMechanites(Pawn pawn, int level)
        {
            //Reduce Mechanite Capacity and increase Plaguelust if origin pawn is a Mechanite Plague Lich
            //With this being a WeaponTrait onHitWorker now instead of two hediffClasses that I made liches
            //are now truly seperate from each other and you may now have an infinite amount of liches!
            //Question: Why would you do that though? You monster.

            NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");
            Hediff mechaniteCapacity = pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            float mechaniteRecovery = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().mechaniteRecovery;
            float plaguelustRecovery = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaSettings>().plaguelustRecovery;

            //Player Shenanigans could set Mechanite Infector or give any other trait ApplyMechanite and set it Level 2
            //Check prevents script from doing anything unless passed.
            //GOTTA have Mechanite Capacity AND use a trait with Level 2 Mechanites
            if (mechaniteCapacity != null && level == 2)
            {
                //PURGE THE MECHANITES
                mechaniteCapacity.Severity -= mechaniteRecovery;
                var plaguelustNeed = pawn.needs.TryGetNeed(plaguelust);
                if (plaguelust == null) return;
                //THEN save the damned soul from infecting others... at least for a time.
                plaguelustNeed.CurLevel += plaguelustNeed.MaxLevel * plaguelustRecovery;
            }
            else return;
        }
    }
}