using RimWorld;
using MP_MechanitePlague;
using MorePersonaTraits.WorkerClasses.OnHitWorkerClasses;
using Verse;

namespace MechanitePersonaTraits.OnHitWorkerClasses
{
    //ApplyHediff from More Persona Traits by Arquebus. Modified by ImJustJoshin.
    public class OnHitWorker_ApplyMechanite : OnHitWorker
    {
        //Didn't want this in the XML as it could confuse people as to
        //how the Mechanite Plague is applied to a pawn.
        //public HediffDef HediffDef = null;

        //New Varibles for my purposes >:)
        public int MechaniteLevel = 0;
        public float minInfectionSeverity = 0f;
        public float maxInfectionSeverity = 0f;
        public int extraSpawns = 0;

        public override void OnHitEffect(Thing hitThing, Thing originThing)
        {
            //Checks target and Mod Settings to Deterime:
            //if target is humanlike
            //if target is an insect AND player allowed insects to spawn bursters in Mechanite Plague Settings
            //if target is an animal AND player allowed animals to spawn bursters in Mechanite Plague Settings
            //Should it fail to find any of these, PurgeMechanites will not fire.
            //Target will still receive the Mechanite Plague, it just won't soothe those urges...
            //Also there is a check done by More Persona Traits to deterime if both hitThing and originThing is alive AND Biological
            //So dead pawns or humanlike non-biological pawns don't count from that check alone.

            bool targetAndSettingsCheck = (hitThing as Pawn).RaceProps.Humanlike || 
                ((hitThing as Pawn).RaceProps.Insect && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowInsectSpawns) ||
                ((hitThing as Pawn).RaceProps.Animal && LoadedModManager.GetMod<MechPlague>().GetSettings<MechPlagueSettings>().allowAnimalSpawns);

            ApplyOnHitEffect(hitThing, originThing, ApplyMechanites);
            if (targetAndSettingsCheck)
            {
                PurgeMechanites(originThing as Pawn, MechaniteLevel);
            }
        }

        void ApplyMechanites(Thing infectedThing)
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
                //As the Advanced version, also applies some damage to the pawn.
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
            //Reduce Mechanite Capacity and increase need Plaguelust if origin pawn has Internal Mechanite Gestation

            NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechaniteFactory");
            Hediff mechaniteCapacity = pawn?.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("MPT_MechaniteCapacity"));

            float mechaniteRecovery = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaTraitsSettings>().mechaniteRecovery;
            float plaguelustRecovery = LoadedModManager.GetMod<MechanitePersonaTraits>().GetSettings<MechanitePersonaTraitsSettings>().plaguelustRecovery;

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