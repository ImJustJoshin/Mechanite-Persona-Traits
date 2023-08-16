using System.Collections.Generic;
using System.Linq;
using MorePersonaTraits.Utils;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.WorkerClasses.Item
{
    public class CompTargetEffect_BladelinkMechacorruption : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            var compBladelink = target.TryGetComp<CompBladelinkWeapon>();
            var existingTraits = target.TryGetComp<CompBladelinkWeapon>().TraitsListForReading;

            //Zero-Based Array - 0, 1, 2, 3 for the four elements below
            var mechaniteTraits = new WeaponTraitDef[] 
            {
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_Unique_MechaniteInfester"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_MechaniteSelf"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_Bonded_ImmuneMechanite")
            };

            //A string list to hold all the names of traits that will be devoured by Mechanite Infester
            //(Those that would be incompatible with it)
            List<string> traitsDevoured = new List<string>(existingTraits.Count());

            //A string array that contains all the exclusionTags for Mechanite Infester
            string[] firstTraitsExclusionTagArray = new string[DefDatabase<WeaponTraitDef>.GetNamed("MPT_Unique_MechaniteInfester").exclusionTags.Count()];
            DefDatabase<WeaponTraitDef>.GetNamed("MPT_Unique_MechaniteInfester").exclusionTags.ToArray().CopyTo(firstTraitsExclusionTagArray, 0);

            if (compBladelink == null)
            {
                return;
            }

            if (existingTraits == null)
            {
                existingTraits = new List<WeaponTraitDef>();
            }

            //If Mechanite Infester exists,
            //Scold the player for attempting to completely destroy their own
            //persona weapon to quite the grizzly death at the hands of mechanites...
            //Also the Mechanite Effector is wasted. RIP.
            if (existingTraits.Contains(mechaniteTraits[0]))
            {
                Messages.Message("MPT_MechaniteInfestationExists".Translate(target.LabelShort), target, MessageTypeDefOf.NegativeEvent);
                return;
            }

            //If Mechan't exists,
            //Prevent the action entirely thus wasting the Mechanite Effector. RIP.
            if (existingTraits.Contains(mechaniteTraits[3]))
            {
                Messages.Message("MPT_MechaniteWeaponTraitResist".Translate(target.LabelShort, mechaniteTraits[3].LabelCap), target, MessageTypeDefOf.NegativeEvent);
                return;
            }

            //Had a lot of problems with traitsDevoured.Count so
            //I made this int here to keep track of how many
            //traits were destroyed so they could be displayed later.
            int destroyedTraitsCount = 0;

            //Huge loop. Just like before. Go read it at 
            //CompTargetEffect_BladelinkMechaExalt.cs
            foreach (string incompatibleTrait in firstTraitsExclusionTagArray)
            {
                for (int i = 0; i < existingTraits.Count(); i++)
                {
                    if (existingTraits[i] == DefDatabase<WeaponTraitDef>.GetNamed(incompatibleTrait))
                    {
                        destroyedTraitsCount++;
                        traitsDevoured.Add(existingTraits[i].LabelCap);
                        existingTraits.Remove(DefDatabase<WeaponTraitDef>.GetNamed(incompatibleTrait));
                    }
                }
            }

            //The number of traits a persona could have is dependent on
            //whatever persona weapon the player managed to get their
            //hands on plus their own possibly unique settings.
            //Due to that the list setup prior uses existingTraits.Count for size
            //However, I must now remove and trim the list if any elements in it 
            //are null or blank ("") for correct output.
            traitsDevoured.RemoveAll(s => string.IsNullOrWhiteSpace(s));
            traitsDevoured.TrimExcess();

            //Baby Loop. Just outputs everything in list
            //into a single string to output into a message later
            string traitsDevouredResult = null; int t = 1;
            foreach (string trait in traitsDevoured)
            {
                if (t < destroyedTraitsCount)
                {
                    traitsDevouredResult += trait + ", ";
                    t++;
                }
                else
                {
                    traitsDevouredResult += trait;
                }
            }

            //The actual trait getting added.
            //Notably both negative events.
            //You reap what you sow.
            compBladelink.TempUnbond();

            if (!existingTraits.Contains(mechaniteTraits[0]))
            {
                existingTraits.Add(mechaniteTraits[0]);
                if (destroyedTraitsCount != 0)
                {
                    Messages.Message("MPT_MechaniteWeaponTraitDevoured".Translate(destroyedTraitsCount) + traitsDevouredResult, target, MessageTypeDefOf.NegativeEvent, true);
                }
                Messages.Message("MPT_WeaponTraitGained".Translate(target.LabelShort, mechaniteTraits[0].LabelCap), target, MessageTypeDefOf.NegativeEvent);
            }

            else
            {
                Log.Error("[Mechanite Persona Traits]: Cannot add anymore traits to this weapon. This should have been caught by the targeting class. Please let the mod author know.");
            }

            compBladelink.RegainBond();

        }
    }
}
