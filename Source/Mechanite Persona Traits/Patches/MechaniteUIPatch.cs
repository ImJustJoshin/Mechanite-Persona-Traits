using System;
using System.Collections.Generic;
using HarmonyLib;
using MorePersonaTraits.Extensions;
using MechanitePersonaTraits.OnHitWorkerClasses;
using MorePersonaTraits.WorkerClasses.OnHitWorkerClasses;
using RimWorld;
using Verse;

namespace MechanitePersonaTraits.Patches
{
    //PatchWeaponTraitDefSpecialDisplayStats made by Arquebus. Modified by ImJustJoshin.

    [HarmonyPatch(typeof(Def), "SpecialDisplayStats")]
    public static class MechanitePatchWeaponTraitDefSpecialDisplayStats
    {
        [HarmonyPostfix]
        static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> values, Def __instance)
        {
            var statEntries = new List<StatDrawEntry>();
            if (__instance is WeaponTraitDef traitDef)
            {
                if (traitDef.equippedStatOffsets != null)
                {
                    foreach (var equippedOffset in traitDef.equippedStatOffsets)
                    {
                        statEntries.Add(
                            new StatDrawEntry(
                                StatCategoryDefOf.EquippedStatOffsets,
                                equippedOffset.stat.label,
                                equippedOffset.ValueToStringAsOffset,
                                equippedOffset.stat.description,
                                6010
                            )
                        );
                    }
                }

                if (traitDef.GetModExtension<WeaponTraitOnHitExtension>() != null && traitDef.GetModExtension<WeaponTraitOnHitExtension>().OnHitWorkers != null)
                {
                    foreach (var worker in traitDef.GetModExtension<WeaponTraitOnHitExtension>().OnHitWorkers)
                    {
                        statEntries.Add(new StatDrawEntry(
                            MPT_StatCategoryDefOf.MPT_OnHitEffects,
                            workerLabel(worker),
                            ReportText(worker),
                            ReportText(worker),
                            6010
                        ));
                    }
                }

                // if (traitDef.exclusionTags != null)
                // {
                //     statEntries.Add(new StatDrawEntry(
                //         StatCategoryDefOf.Weapon,
                //         "ExclusionTags",
                //         traitDef.exclusionTags.ToCommaList(),
                //         "List of exclusion tags:\n" + traitDef.exclusionTags.ToCommaList(),
                //         6010
                //     ));
                // }

                if (traitDef.bondedHediffs != null)
                {
                    foreach (var hediff in traitDef.bondedHediffs)
                    {
                        statEntries.AddRange(hediff.SpecialDisplayStats(StatRequest.ForEmpty()));
                    }
                }
            }

            statEntries.AddRange(values);
            return statEntries;
        }
        private static TaggedString ReportText(OnHitWorker worker)
        {
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    return "MPT_MechaniteOnHitDesc".Translate(
                    worker.ProcChance.ToStringPercent(),
                    workerEffect(worker),
                    worker.TargetSelf ? "MPT_TargetSelf".Translate() : "MPT_TargetTarget".Translate(),
                    Math.Abs(onHit.minInfectionSeverity) > 0f ? "MPT_MinSeverityDesc".Translate((onHit.minInfectionSeverity * 100).ToString()) : TaggedString.Empty,
                    Math.Abs(onHit.maxInfectionSeverity) > 0f ? "MPT_MaxSeverityDesc".Translate(onHit.maxInfectionSeverity.ToStringPercent()) : TaggedString.Empty,
                    onHit.extraSpawns > 0 ? "MPT_ExtraSpawnDesc".Translate(onHit.extraSpawns.ToString()) : TaggedString.Empty
                    );
                default:
                    return "MPT_OnHitDesc".Translate(
                    worker.ProcChance.ToStringPercent(),
                    workerEffect(worker),
                    worker.TargetSelf ? "MPT_TargetSelf".Translate() : "MPT_TargetTarget".Translate(),
                    Math.Abs(worker.ProcMagnitude) > 0f ? "MPT_MagnitudeDesc".Translate(worker.ProcMagnitude.ToStringPercent()) : TaggedString.Empty
                    );
            }
        }
        private static string workerEffect(OnHitWorker worker)
        {
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    return "MPT_ApplyMechaniteDesc".Translate("Mechanite plague");
                case OnHitWorker_ApplyHediff onHit:
                    return "MPT_ApplyHediffDesc".Translate(onHit.HediffDef.LabelCap);
                case OnHitWorker_ApplyGeneResource onHit:
                    return "MPT_ApplyGeneResourceDesc".Translate(onHit.GeneDef.resourceLabel);
                case OnHitWorker_ApplyNeed onHit:
                    return onHit.ProcMagnitude > 0f ? "MPT_NeedOffsetTypeUpDesc".Translate(onHit.NeedDef.LabelCap) : "MPT_NeedOffsetTypeDownDesc".Translate(onHit.NeedDef.LabelCap);
                case OnHitWorker_ApplyThought onHit:
                    return "MPT_GiveThoughDesc".Translate(onHit.ThoughtDef.Label, onHit.ThoughtDef.stackLimit > 1 ? "MPT_ThoughtStackDesc".Translate(onHit.ThoughtDef.stackLimit) : TaggedString.Empty);
                default:
                    return "";
            }
        }

        private static string workerLabel(OnHitWorker worker)
        {
            switch (worker)
            {
                case OnHitWorker_ApplyMechanite onHit:
                    return "MPT_HediffDef".Translate();
                case OnHitWorker_ApplyHediff onHit:
                    return onHit.HediffDef.LabelCap;
                case OnHitWorker_ApplyGeneResource onHit:
                    return onHit.GeneDef.resourceLabel;
                case OnHitWorker_ApplyNeed onHit:
                    return onHit.NeedDef.LabelCap;
                case OnHitWorker_ApplyThought onHit:
                    return onHit.ThoughtDef.LabelCap;
                default:
                    return "";
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

            Harmony harmony = new Harmony("rimworld.mod.ImJustJoshin.MechanitePersonaTraits");
            harmony.PatchAll();
        }
    }
}



