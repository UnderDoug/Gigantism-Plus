using HarmonyLib;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch(typeof(ModGigantic))]
    public static class ModGigantic_Patches
    {
        // adds shader to ModGigantic adjective
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ModGigantic), nameof(ModGigantic.HandleEvent), new Type[] { typeof(GetDisplayNameEvent) })]
        static void GiganticAdjective_HandleEvent_Postfix(GetDisplayNameEvent E)
        {
            // goal display the SizeAdjective gigantic with its associated shader.
            string Adjective = "gigantic";
            int Priority = 30;
            bool IncludeProperName = false;
            if (E.Object.HasProperName && !IncludeProperName) return; // skip for Proper Named items, unless including them.
            if (E.Object.HasTagOrProperty("ModGiganticNoDisplayName")) return; // skip for items that explicitly hide the adjective
            if (E.Object.HasTagOrProperty("ModGiganticNoUnknownDisplayName")) return; // skip for unknown items that explicitly hide the adjective
            if (!E.Understood()) return; // skip items not understood by the player

            if (E.DB.SizeAdjective == Adjective && E.DB.SizeAdjectivePriority == Priority)
            {
                // The base event runs every game tick for equipped range weapons.
                // possibly due to the item being displayed in the UI (bottom right)
                // Any form of output here will completely clog up the logs.

                E.DB.SizeAdjective = Adjective.MaybeColor("gigantic");
            }
        }

        // adds additional effects for different parts when the GameObject they're attched to is modified to be gigantic
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ModGigantic.ApplyModification))]
        static void ApplyModification_AdditionalEffects_Postfix(ModGigantic __instance, GameObject Object)
        {
            LightSource lightSource = Object.GetPart<LightSource>();
            if (lightSource != null)
            {
                lightSource.Radius *= 2;
            }
            Backpack backpack = Object.GetPart<Backpack>();
            if (backpack != null)
            {
                backpack.WeightWhenWorn = (int)(backpack.WeightWhenWorn * 2.5f);
            }
            Armor Armor = Object.GetPart<Armor>();
            if (Armor != null && Armor.CarryBonus > 0)
            {
                Armor.CarryBonus = (int)(Armor.CarryBonus * 1.25f);
            }
        }

        // overwrites the entire GetDescrption method (it's not super to target specific locations throughout) to include the above additions
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ModGigantic.GetDescription))]
        public static bool GetDescription_AdditionalEffects_Prefix(ref int Tier, ref GameObject Object, ref string __Result, ref ModGigantic __Instance)
        {
            if (Object == null)
            {
                // send to default
                return true;
            }

            ModGigantic @this = __Instance;
            string objectNoun = "item";
            List<List<string>> weaponDescriptions = new List<List<string>>();
            List<List<string>> generalDescriptions = new List<List<string>>();
            if (Object.LiquidVolume != null)
            {
                generalDescriptions.Add(new List<string> { "hold", "twice as much liquid" });
            }
            if (Object.HasPart<EnergyCell>())
            {
                generalDescriptions.Add(new List<string> { "have", "twice the energy capacity" });
            }
            if (Object.HasPartDescendedFrom<IGrenade>())
            {
                generalDescriptions.Add(new List<string> { "have", "twice as large a radius of effect" });
            }
            if (Object.HasPart<Tonic>())
            {
                generalDescriptions.Add(new List<string> { "contain", "double the tonic dosage" });
            }
            if (Object.GetIntProperty("Currency") > 0)
            {
                generalDescriptions.Add(new List<string> { null, "much more valuable" });
            }
            bool isDefaultBehavior = Object.EquipAsDefaultBehavior();
            if (!isDefaultBehavior)
            {
                generalDescriptions.Add(new List<string> { null, "much heavier than usual" });
            }

            MeleeWeapon meleeWeapon = Object.GetPart<MeleeWeapon>();
            bool isDefaultBehaviorOrFloating = isDefaultBehavior || Object.IsEntirelyFloating();
            if (meleeWeapon != null && Object.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                objectNoun = "weapon";
                weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
                if (meleeWeapon.Skill == "Cudgel")
                {
                    weaponDescriptions.Add(new List<string> { null, "twice as effective when you Slam with " + Object.them });
                }
                else if (meleeWeapon.Skill == "Axe")
                {
                    weaponDescriptions.Add(new List<string> { "cleave", "for -3 AV" });
                }
                // moved duplicate code to isDefaultBehaviorOrFloatingHandler
            }
            else if (Object.HasPart<MissileWeapon>())
            {
                weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
                // moved duplicate code to isDefaultBehaviorOrFloatingHandler
            }
            else if (Object.HasPart<ThrownWeapon>())
            {
                objectNoun = "weapon";
                if (!Object.HasPartDescendedFrom<IGrenade>())
                {
                    weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
                }
                // moved duplicate code to isDefaultBehaviorOrFloatingHandler
            }

            // begin adjustment
            if (Object.HasPart<Armor>() || Object.HasPart<Shield>())
            {
                // start addition
                if (objectNoun == "item")
                {
                    objectNoun = Object.HasPart<Armor>() ? "armor" : "shield";
                }

                if (Object.GetPart<Armor>().CarryBonus > 0)
                {
                    generalDescriptions.Add(new List<string> { "have", "a quarter more carry capcity" });
                }
                // end addition

            }
            // end adjustment

            // isDefaultBehaviorOrFloatingHandler
            if (!isDefaultBehaviorOrFloating)
            {
                if (Object.UsesSlots == null)
                {
                    generalDescriptions.Add(new List<string> { "", Object.it + " must be wielded " + (Object.UsesTwoSlots ? "four" : "two") + "-handed by non-gigantic creatures" });
                }
                else
                {
                    generalDescriptions.Add(new List<string> { "", "can only be equipped by gigantic creatures" });
                }
            }

            if (Object.HasPart<DiggingTool>() || Object.HasPart<Drill>())
            {
                weaponDescriptions.Add(new List<string> { "dig", "twice as fast" });
            }

            // start addition
            if (Object.HasPart<Backpack>())
            {
                generalDescriptions.Add(new List<string> { "support", "twice and a half as much weight" });
            }
            if (Object.HasPart<LightSource>())
            {
                generalDescriptions.Add(new List<string> { "illuminate", "twice as far" });
            }
            // end addition

            // start addition
            BeforeDescribeModGiganticEvent.Send(Object, @this, objectNoun, generalDescriptions, weaponDescriptions);
            // end addition

            if (weaponDescriptions.Count == 0)
            {
                List<string> processedGeneralDescription = new();
                foreach (List<string> entry in generalDescriptions)
                {
                    processedGeneralDescription.Add(GetProcessedItem(entry, second: false, generalDescriptions, Object));
                }

                // added colour to output
                __Result = "Gigantic".OptionalColor("gigantic", "w", Colorfulness) + ": " + (Object.IsPlural ? ("These " + Grammar.Pluralize(objectNoun)) : ("This " + objectNoun)) + " " + Grammar.MakeAndList(processedGeneralDescription) + ".";
                return false;
            }
            List<string> processedCombinedWeaponDescription = new();
            List<string> processedCombinedGeneralDescription = new();
            foreach (List<string> weaponEnrty in weaponDescriptions)
            {
                processedCombinedWeaponDescription.Add(GetProcessedItem(weaponEnrty, second: false, weaponDescriptions, Object));
            }
            foreach (List<string> generalEntry in generalDescriptions)
            {
                processedCombinedGeneralDescription.Add(GetProcessedItem(generalEntry, second: true, generalDescriptions, Object));
            }

            // added colour to output
            __Result = "Gigantic".OptionalColor("gigantic", "w", Colorfulness) + ": " + (Object.IsPlural ? ("These " + Grammar.Pluralize(objectNoun)) : ("This " + objectNoun)) + " " + Grammar.MakeAndList(processedCombinedWeaponDescription) + ". " + Grammar.MakeAndList(processedCombinedGeneralDescription) + ".";
            return false;
        }
        // the below is included to assist the above. The original method is private.
        public static string GetProcessedItem(List<string> item, bool second, List<List<string>> items, GameObject obj)
        {
            if (item[0] == "")
            {
                if (second && item == items[0])
                {
                    return obj.It + " " + item[1];
                }
                return item[1];
            }
            if (item[0] == null)
            {
                if (second && item == items[0])
                {
                    return obj.Itis + " " + item[1];
                }
                if (item != items[0])
                {
                    bool flag = true;
                    foreach (List<string> item2 in items)
                    {
                        if (item2[0] != null)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return item[1];
                    }
                }
                return obj.GetVerb("are", PrependSpace: false) + " " + item[1];
            }
            if (second && item == items[0])
            {
                return obj.It + obj.GetVerb(item[0]) + " " + item[1];
            }
            return obj.GetVerb(item[0], PrependSpace: false) + " " + item[1];
        }
    }
}
