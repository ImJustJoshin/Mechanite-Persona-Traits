using RimWorld;
using MP_MechanitePlague;
using MorePersonaTraits.WorkerClasses.OnHitWorkerClasses;
using Verse;

namespace MPT_MechaniteHelper
{
    //Modified ApplyHediff from More Persona Traits by Arquebus
    public class OnHitWorker_ApplyMechanite : OnHitWorker
    {
        public HediffDef HediffDef = null;

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

        private void ApplyMechanites(Thing infestedThing)
        {
            //Mechanite Infector - Basic
            if (MechaniteLevel == 1)
            {
                //Infect. That's it.
                PlagueMethodHolder.InfectPawn(infestedThing as Pawn, Faction.OfPlayer, minInfectionSeverity, maxInfectionSeverity, extraSpawns);
            }
            
            //Mechanite Infester - Advanced
            else if (MechaniteLevel == 2)
            {
                //Apply extra damage then if wounded, infect.
                //Might add something more here. If at Overload,
                //purge more mechanites into your enemies for
                //an even greater infection potential!
                DamageWorker.DamageResult damageResult = (infestedThing as Pawn).TakeDamage(new DamageInfo(DefDatabase<DamageDef>.GetNamed("MPT_Damage_MechaniteInfestation", true), 3f, 0.5f, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null, true, true));
                if (damageResult.wounded)
                {
                    PlagueMethodHolder.InfectPawn(infestedThing as Pawn, Faction.OfPlayer, minInfectionSeverity, maxInfectionSeverity, extraSpawns);
                }
            }

            //Nothing should go here unless player shenanigans in which case...
            else
            {
                //Player Shenanigans: Prevented :)
                Log.Error("[MechanitePersonaTraits]: Persona weapon MechaniteLevel used on " + infestedThing + " is out of bounds. Expected range 1-2.");
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
            Hediff plagueLich = pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_Bonded_MechanitePlagueLich"));
            Hediff mechaniteCapacity = pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            if (plagueLich != null && mechaniteCapacity != null && level == 2)
            {
                //PURGE THEM MECHANITES
                mechaniteCapacity.Severity -= 0.048f;
                var plaguelustLevel = pawn.needs.TryGetNeed(plaguelust);
                //To Do: Expose this variable for player modification (Configs)
                var plaguelustRecovery = 0.085f;
                if (plaguelust == null) return;
                //THEN save the damned soul from infecting others... at least for a time.
                plaguelustLevel.CurLevel += plaguelustLevel.MaxLevel * plaguelustRecovery;
            }
            else return;
        }

    }
}