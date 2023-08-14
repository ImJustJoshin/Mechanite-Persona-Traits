using System;
using System.Collections.Generic;
using System.Linq;
using MorePersonaTraits.Utils;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.WorkerClasses.Item
{
    public class CompTargetEffect_BladelinkMechaExalt : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            var compBladelink = target.TryGetComp<CompBladelinkWeapon>();
            var existingTraits = target.TryGetComp<CompBladelinkWeapon>().TraitsListForReading;

            //Zero-Based Array - 0, 1, 2 for the three elements below.
            WeaponTraitDef[] mechaniteTraits = new WeaponTraitDef[]
            {
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_MechaniteSelf"),
                DefDatabase<WeaponTraitDef>.GetNamed("MPT_Bonded_ImmuneMechanite")
            };

            //Two string array's containing all the exclusion tags for the first two elements in the array above
            //Used to prevent incompatible traits from being applied to the persona weapon.
            string[] firstTraitsExclusionTagArray = new string[DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite").exclusionTags.Count()];
            DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite").exclusionTags.ToArray().CopyTo(firstTraitsExclusionTagArray, 0);

            string[] secondTraitsExclusionTagArray = new string[DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_Mechanite").exclusionTags.Count()];
            DefDatabase<WeaponTraitDef>.GetNamed("MPT_OnHit_MechaniteSelf").exclusionTags.ToArray().CopyTo(secondTraitsExclusionTagArray, 0);

            //RNG BABY! Randomly determines if randomTraitToAdd is Mechanite Infector or Mechanite Injector
            Random rng = new Random();
            int rngIndex = rng.Next(0, mechaniteTraits.Length - 1);
            var randomTraitToAdd = mechaniteTraits[rngIndex];

            if (compBladelink == null)
            {
                return;
            }

            if (existingTraits == null)
            {
                existingTraits = new List<WeaponTraitDef>();
            }

            //if statement to check if the persona has both the traits the Mechanite Effector can give.
            //if you already have both. you just wasted a Mechanite Effector. RIP.
            if (existingTraits.Contains(mechaniteTraits[0]) && existingTraits.Contains(mechaniteTraits[1]))
            {
                Messages.Message("MPT_MechaniteWeaponTraitFail".Translate(target.LabelShort, mechaniteTraits[0].LabelCap, 
                    mechaniteTraits[1].LabelCap), target, MessageTypeDefOf.NeutralEvent);
                return;
            }

            //if statement to check if the persona has the trait: Mechan't.
            //if Mechan't exists on the persona, it prevents & wastes the Mechanite Effector. RIP.
            if (existingTraits.Contains(mechaniteTraits[2]))
            {
                Messages.Message("MPT_MechaniteWeaponTraitResist".Translate(target.LabelShort, mechaniteTraits[2].LabelCap), target, MessageTypeDefOf.NegativeEvent);
                return;
            }

            //Huge loops. If exclusionTags held more data this would kill performance.
            //if statement checks what the randomTraitToAdd is and if it matches...
            //it starts a foreach loop for each element (exclusion tags) as incompatibleTrait
            //which starts a for loop which checks if the first trait on the persona equal to incompatibleTrait
            //if it doesn't it then moves on to the second trait to check if THATS equal incompatibleTrait
            //THEN once all the traits are checked against the first incompatibleTrait...
            //the foreach loop moves on to the SECOND incompatibleTrait and repeats everything....
            //until every exclusion tag is checked against every trait on the persona weapon... 
            //to make sure no trait on the persona weapon is incompatible to randomTraitToAdd
            //which is either Mechanite Infector or Mechanite Injector.
            //if at any point a trait is found incompatible... it then sets randomTraitToAdd...
            //to be Mechanite Injector and takes that WHOLE process again.
            //If you can't get a positive trait then its my honor to give you a negative trait :)

            if (randomTraitToAdd == mechaniteTraits[0])
            {            
                foreach(string incompatibleTrait in firstTraitsExclusionTagArray)
                {
                    for (int i = 0; i < existingTraits.Count(); i++)
                    {
                         if (existingTraits[i] == DefDatabase<WeaponTraitDef>.GetNamed(incompatibleTrait))
                         {
                             Messages.Message("MPT_MechaniteTraitIncompatOne".Translate(target.LabelShort, existingTraits[i].LabelCap,
                                 randomTraitToAdd.LabelCap), target, MessageTypeDefOf.NeutralEvent);
                            randomTraitToAdd = mechaniteTraits[1];
                            break;
                         }
                    }
                    if (randomTraitToAdd == mechaniteTraits[1]) { break; }
                }
            }

            if (randomTraitToAdd == mechaniteTraits[1])
            {
                foreach (string incompatibleTrait in secondTraitsExclusionTagArray)
                {
                    for (int i = 0; i < existingTraits.Count(); i++)
                    {
                        if (existingTraits[i] == DefDatabase<WeaponTraitDef>.GetNamed(incompatibleTrait))
                        {
                            Messages.Message("MPT_MechaniteTraitIncompatTwo".Translate(target.LabelShort, existingTraits[i].LabelCap,
                                randomTraitToAdd.LabelCap), target, MessageTypeDefOf.NeutralEvent);
                            return;
                        }
                    }
                }
            }

            compBladelink.TempUnbond();

            if (!existingTraits.Contains(randomTraitToAdd))
            {
                existingTraits.Add(randomTraitToAdd);
                Messages.Message("MPT_WeaponTraitGained".Translate(target.LabelShort, randomTraitToAdd.LabelCap), target, MessageTypeDefOf.NeutralEvent);
                compBladelink.RegainBond();
                return;
            }

            if (existingTraits.Contains(randomTraitToAdd))
            {
                var otherTraitToAdd = mechaniteTraits[Math.Abs(rngIndex - 1)];
                existingTraits.Add(otherTraitToAdd);
                Messages.Message("MPT_WeaponTraitGained".Translate(target.LabelShort, otherTraitToAdd.LabelCap), target, MessageTypeDefOf.NeutralEvent);
                compBladelink.RegainBond();
            }
            else
            {
                Log.Error("[Mechanite Persona Traits]: Cannot add anymore traits to this weapon. This should have been caught by the targeting class. Please let the mod author know.");
            }
        }
    }
}
