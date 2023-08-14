using MorePersonaTraits.Utils;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.WorkerClasses.Item
{
    public class CompTargetEffect_BladelinkMechaAnnul : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            var compBladelink = target.TryGetComp<CompBladelinkWeapon>();
            var existingTraits = FieldRefUtils.TraitsFieldRef.Invoke(compBladelink);

            WeaponTraitDef[] mechaniteTraits = new WeaponTraitDef[]
            {
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_MechaniteSelf"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_Unique_MechaniteInfester")
            };

            if (compBladelink == null)
            {
                return;
            }

            //triple check to see if mechanites aren't on the persona weapon.
            //if they aren't send a message.
            //I might change this later to add the Mechan't trait which prevents mechanites...
            //from invading the persona weapon if you do use a guardian effector...
            //on a persona with no mechanites on it.

            if (!existingTraits.Contains(mechaniteTraits[0]) && !existingTraits.Contains(mechaniteTraits[1]) && !existingTraits.Contains(mechaniteTraits[2]))
            {
                Messages.Message("MPT_MechaniteWeaponTraitMissing".Translate(target.LabelShort), target, MessageTypeDefOf.NeutralEvent);
                return;
            }

            compBladelink.TempUnbond();

            if (existingTraits.NullOrEmpty())
            {
                WeaponTraitUtils.InitializeTraits(compBladelink);
            }

            //foreach loop to check every element in the mechaniteTraits array above.
            //if an element is found, purge the heretic infesting our persona.
            //oh and I guess tell the player about what was removed.
            //if element isn't found, move to the next element.
            foreach (WeaponTraitDef traitToRemove in mechaniteTraits)
            {
                if (existingTraits.Contains(traitToRemove))
                {
                    existingTraits.Remove(traitToRemove);
                    Messages.Message("MPT_WeaponTraitLost".Translate(target.LabelShort, traitToRemove.LabelCap), target, MessageTypeDefOf.NeutralEvent);
                    if (existingTraits.NullOrEmpty()) WeaponTraitUtils.InitializeTraits(compBladelink);
                }
            }

            compBladelink.RegainBond();

        }
    }
}
