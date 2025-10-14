using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using System.Reflection;

namespace BoneMod
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new("rimworld.Sihv.bonemod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            harmony.Patch(
                original: AccessTools.Method(type: typeof(Corpse), name: nameof(Corpse.SpecialDisplayStats)), 
                prefix: null,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(SpecialDisplayStats_PostFix)));

            harmony.Patch(
                original: AccessTools.Method(type: typeof(Pawn), name: nameof(Pawn.ButcherProducts)), 
                prefix: null,
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(ButcherProducts_PostFix)));

            //Harmony.DEBUG = true;
        }

        static void SpecialDisplayStats_PostFix(Corpse __instance, ref IEnumerable<StatDrawEntry> __result)
        {
            // Create a modifiable list
            List<StatDrawEntry> NewList = __result.ToList();

            // copy vanilla entries into the new list

            // custom code to modify list contents
            StatDef BoneAmount = DefDatabase<StatDef>.GetNamed("BoneAmount");
            float pawnBoneCount = __instance.InnerPawn.GetStatValue(BoneAmount) * BoneModSettings.boneFactor;
            NewList.Add(new StatDrawEntry(BoneAmount.category, BoneAmount, pawnBoneCount, StatRequest.For(__instance.InnerPawn)));

            // convert list to IEnumerable to match the caller's expectations
            IEnumerable<StatDrawEntry> output = NewList;

            // make caller use the list
            __result = output;
        }

        static void ButcherProducts_PostFix(Pawn __instance, ref IEnumerable<Thing> __result, float efficiency)
        {
            int boneCount = GenMath.RoundRandom(__instance.GetStatValue(DefDatabase<StatDef>.GetNamed("BoneAmount")) * BoneModSettings.boneFactor * efficiency);
            int meatCountCheck = GenMath.RoundRandom(__instance.GetStatValue(DefDatabase<StatDef>.GetNamed("MeatAmount")));
            if (boneCount <= 0) return;
            List<Thing> NewList = __result.ToList();
            if (meatCountCheck > 1)
            {
                Thing bones = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("BoneItem"));
                bones.stackCount = boneCount;
                NewList.Add(bones);
            }
                
            IEnumerable<Thing> output = NewList;
            __result = output;
        }
    }
}
