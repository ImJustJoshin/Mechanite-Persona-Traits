using System;
using HarmonyLib;
using MorePersonaTraits.Patches;
using MechanitePersonaTraits.OnHitWorkerClasses;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.Patches
{
    [StaticConstructorOnStartup]
    public static class MechaniteUIPatch
    {
        static MechaniteUIPatch()
        {
            //Fires Second
            Log.Message("Static Mechanite Persona Traits Mod Class Loaded");

            Harmony harmony = new Harmony("rimworld.mod.ImJustJoshin.MechanitePersonaTraits");
            harmony.PatchAll();
        }
    }
    
    [HarmonyPatch]
    public static class MechaniteCompBladelinkWeaponPatches
    {
        [HarmonyPatch(typeof(CompBladelinkWeapon), nameof(PatchWeaponTraitDefSpecialDisplayStats))]
        [HarmonyPostfix]
        public static void Mechanite_Postfix()
        {

        }
    }
}
