using System;
using HarmonyLib;
using MorePersonaTraits.Patches;
using MechanitePersonaTraits.OnHitWorkerClasses;
using MorePersonaTraits.WorkerClasses.OnHitWorkerClasses;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.Patches
{
    //PatchWeaponTraitDefSpecialDisplayStats made by Arquebus. Modified by ImJustJoshin.
    [HarmonyPatch(typeof(PatchWeaponTraitDefSpecialDisplayStats), methodName: "ReportText")]
    static class ReportTextPatch
    {
        private static bool Prefix(OnHitWorker worker, ref TaggedString __result)
        {
            //Newly added Switch case to detect ApplyMechanite.
            //If not then default to original Arquebus code.
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    __result = "MPT_MechaniteOnHitDesc".Translate(
                    worker.ProcChance.ToStringPercent(),
                    "MPT_ApplyMechaniteDesc".Translate("Mechanite plague"),
                    worker.TargetSelf ? "MPT_TargetSelf".Translate() : "MPT_TargetTarget".Translate(),
                    Math.Abs(onHit.minInfectionSeverity) > 0f ? "MPT_MinSeverityDesc".Translate((onHit.minInfectionSeverity * 100).ToString()) : TaggedString.Empty,
                    Math.Abs(onHit.maxInfectionSeverity) > 0f ? "MPT_MaxSeverityDesc".Translate(onHit.maxInfectionSeverity.ToStringPercent()) : TaggedString.Empty,
                    onHit.extraSpawns > 0 ? "\n\n" + "MPT_ExtraSpawnDesc".Translate(onHit.extraSpawns.ToString()) : TaggedString.Empty
                    );
                    return false;
                default:
                    return true;
            }
        }
    }
    [HarmonyPatch(typeof(PatchWeaponTraitDefSpecialDisplayStats), methodName: "workerEffect")]
    static class workerEffectPatch
    {
        private static bool Prefix(OnHitWorker worker, ref string __result)
        {
            //Added one new case for ApplyMechanite
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    __result = "MPT_ApplyMechaniteDesc".Translate("Mechanite plague");
                    return false;
                default:
                    return true;
            }
        }
    }
    [HarmonyPatch(typeof(PatchWeaponTraitDefSpecialDisplayStats), methodName: "workerLabel")]
    static class workerLabelPatch
    {
        private static bool Prefix(OnHitWorker worker, ref string __result)
        {
            //Added one new case for ApplyMechanite
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    __result = "MPT_HediffDef".Translate();
                    return false;
                default:
                    return true;
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class MechaniteUIPatch
    {
        static MechaniteUIPatch()
        {
            //Fires Second
            Log.Message("Static Mechanite Persona Traits Mod Class Loaded");

            var harmony = new Harmony("rimworld.mod.ImJustJoshin.MechanitePersonaTraits");
            harmony.PatchAll();
        }
    }
}