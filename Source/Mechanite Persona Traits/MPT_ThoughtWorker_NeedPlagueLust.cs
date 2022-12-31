using RimWorld;
using System.Linq;
using Verse;

namespace MPT_MechaniteHelper
{
    public class ThoughtWorker_NeedPlagueLust : ThoughtWorker_WeaponTrait
    {
        // Derived from Rimworld Source Code, ThoughtWorker_NeedFood
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            //Temporary - This targets ANY need a pawn has that's Need_Chemical not just my custom "Plaguelust"
            //If you have any idea how I can specify the EXACT need to check for that would be awesome.
            //Can randomly cause errors despite the worker not existing in any pawn during my testing
            //Only occured to pawns with a Need_Chemical 
            var PlagueLust = p.needs.AllNeeds.Where(need => need is Need_Chemical).RandomElementWithFallback();
            if (PlagueLust == null)
            {
                return ThoughtState.Inactive;
            }
            else if (PlagueLust.CurLevelPercentage >= 0.85f)
            {
                //+ 20 Mood for keeping Need "Plaguelust" at or above 85%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(0);
            }
            else if (PlagueLust.CurLevelPercentage < 0.85f && PlagueLust.CurLevelPercentage >= 0.65f)
            {
                //+ 12 Mood for keeping Need "Plaguelust" at or above 65% but lower than 85%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(1);
            }
            else if (PlagueLust.CurLevelPercentage < 0.65f && PlagueLust.CurLevelPercentage >= 0.45f)
            {
                //+ 4 Mood for keeping Need "Plaguelust" at or above 45% but lower than 65%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(2);
            }
            else if (PlagueLust.CurLevelPercentage < 0.45f && PlagueLust.CurLevelPercentage >= 0.25f)
            {
                //- 8 Mood for keeping Need "Plaguelust" below 45% but no lower than 25%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(3);
            }
            else if (PlagueLust.CurLevelPercentage < 0.25f && PlagueLust.CurLevelPercentage >= 0.10f)
            {
                //- 24 Mood for keeping Need "Plaguelust" below 45% but no lower than 25%
                return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(4);
            }
                //- 36 Mood for keeping Need "Plaguelust" below 25%
            else return p.equipment.bondedWeapon.TryGetComp<CompBladelinkWeapon>().TraitsListForReading.Exists(trait => trait.bondedThought == def)
                && p.HitPoints > 0.001
                    ? true
                    : ThoughtState.ActiveAtStage(5);
        }
    }
}