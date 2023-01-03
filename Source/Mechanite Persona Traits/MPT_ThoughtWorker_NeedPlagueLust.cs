using RimWorld;
using Verse;

namespace MPT_MechaniteHelper
{
    // Derived from Rimworld Source Code, ThoughtWorker_NeedFood
    public class ThoughtWorker_NeedPlagueLust : ThoughtWorker_WeaponTrait
    {
        //Define NeedDef for Plaguelust
        public NeedDef plaguelust = DefDatabase<NeedDef>.GetNamed("MPT_Need_MechanitePlagueLich");

        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            //Define Need that checks if a pawn has the defined NeedDef
            //Then check a list of conditions
            Need hasPlaguelust = p.needs.TryGetNeed(plaguelust);
            if (hasPlaguelust == null)
            {
                return ThoughtState.Inactive;
            }
            else if (hasPlaguelust.CurLevelPercentage >= 0.85f)
            {
                //+ 20 Mood for keeping Need "Plaguelust" at or above 85%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(0);
            }
            else if (hasPlaguelust.CurLevelPercentage < 0.85f && hasPlaguelust.CurLevelPercentage >= 0.65f)
            {
                //+ 12 Mood for keeping Need "Plaguelust" at or above 65% but lower than 85%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(1);
            }
            else if (hasPlaguelust.CurLevelPercentage < 0.65f && hasPlaguelust.CurLevelPercentage >= 0.45f)
            {
                //+ 4 Mood for keeping Need "Plaguelust" at or above 45% but lower than 65%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(2);
            }
            else if (hasPlaguelust.CurLevelPercentage < 0.45f && hasPlaguelust.CurLevelPercentage >= 0.25f)
            {
                //- 8 Mood for keeping Need "Plaguelust" below 45% but no lower than 25%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(3);
            }
            else if (hasPlaguelust.CurLevelPercentage < 0.25f && hasPlaguelust.CurLevelPercentage >= 0.10f)
            {
                //- 24 Mood for keeping Need "Plaguelust" below 25% but no lower than 10%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(4);
            }
                //- 36 Mood for keeping Need "Plaguelust" below 10% - PREPARE FOR OVERLOAD BABY!
            else return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(5);
        }
    }
}