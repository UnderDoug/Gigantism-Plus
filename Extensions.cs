using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus
{
    public static class Extensions
    {
        // checks if part is managed externally
        public static bool IsExternallyManagedLimb(this BodyPart part)  // Renamed method
        {
            if (part?.Manager == null) return false;

            // Check for HelpingHands or AgolgotChord managed parts
            if (part.Manager.EndsWith("::HelpingHands") || part.Manager.EndsWith("::AgolgotChord"))
                return true;

            // Check for Nephal claws (Agolgot parts that don't use the manager)
            if (!string.IsNullOrEmpty(part.DefaultBehaviorBlueprint) &&
                part.DefaultBehaviorBlueprint.StartsWith("Nephal_Claw"))
                return true;

            return false;
        } //!-- public static bool IsExternallyManagedLimb(BodyPart part)

        public static DieRoll AdjustDieCount(this DieRoll DieRoll, int Amount)
        {
            Debug.Entry(4, $"@ AdjustDieCount(this DieRoll DieRoll: {DieRoll.ToString()}, int Amount: {Amount})", Indent: 6);
            if (DieRoll == null)
            {
                Debug.Entry(4, "AdjustDieCount", "DieRoll null", Indent: 7);
                return null;
            }
            int type = DieRoll.Type;
            if (DieRoll.LeftValue > 0)
            {
                Debug.Entry(4, "AdjustDieCount", "DieRoll.LeftValue > 0", Indent: 7);
                DieRoll.LeftValue += Amount;
                Debug.Entry(4, "DieRoll.LeftValue", $"{DieRoll.LeftValue}", Indent: 8);
                Debug.Entry(4, "Collapse ^^<<", Indent: 8);
                return DieRoll;
            }
            else
            {
                Debug.Entry(4, "AdjustDieCount", "Recursing >>VV", Indent: 7);
                if (DieRoll.RightValue > 0) return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.RightValue);
                return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.Right);
            }
        }
        public static string AdjustDieCount(this string DieRoll, int Amount)
        {
            DieRoll dieRoll = new(DieRoll);
            return dieRoll.AdjustDieCount(Amount).ToString();
        }
        public static bool AdjustDieCount(this MeleeWeapon MeleeWeapon, int Amount)
        {
            MeleeWeapon.BaseDamage = MeleeWeapon.BaseDamage.AdjustDieCount(Amount);
            return true;
        }

        public static int GetNaturalWeaponModsCount(this GameObject GO)
        {
            Debug.Entry(4, $"GetNaturalWeaponModsCount(this GameObject {GO.DebugName})", $"{GO.GetIntProperty("ModNaturalWeaponCount")}", Indent: 5);
            return GO.GetIntProperty("ModNaturalWeaponCount");
        }
        public static bool HasNaturalWeaponMods(this GameObject GO)
        {
            Debug.Entry(4, $"HasNaturalWeaponMods(this GameObject {GO.DebugName})", $"{GO.GetNaturalWeaponModsCount() > 0}", Indent: 4);
            return GO.GetNaturalWeaponModsCount() > 0;
        }

        public static string BonusOrPenalty(this int Int) 
        {
            return Int >= 0 ? "bonus" : "penalty";
        }

        public static string BonusOrPenalty(this string SignedInt)
        {
            if (int.TryParse(SignedInt, out int Int))
                return Int >= 0 ? "bonus" : "penalty";
            throw new ArgumentException($"int.TryParse(SignedInt) failed to parse \"{SignedInt}\". SignedInt must be capable of conversion to int.");
        }

        public static StringBuilder AppendGigantic(this StringBuilder sb, string value)
        {
            sb.AppendColored("gigantic", value);
            return sb;
        }

        public static string Color(this string Text, string Color)
        {
            return "{{" + Color + "|" + Text + "}}";
        }
        public static string MaybeColor(this string Text, string Color, bool Pretty = true)
        {
            if (Pretty) return Text.Color(Color);
            return Text;
        }

        // ripped from the CyberneticPropertyModifier part, converted into extension.
        // Props must equal "string:int;string:int;string:int" where
        // string   is an IntProperty
        // int      is the value
        // ;        delimits each pair.
        // Example: "ChargeRangeModifier:2;JumpRangeModifier:1"
        public static Dictionary<string, int> ParseProps(this string Props)
        {
            Dictionary<string, int> dictionary = new();
            string[] array = Props.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(array2[0], Convert.ToInt32(array2[1]));
            }
            return dictionary;
        }

        // as above, but for int:int progressions (good for single value level progressions).
        // Progression must equal "int:int;int:int;int:int" where
        // int      is the progression "interval"
        // int      is the value being progression
        // ;        delimits each pair.
        // Example: "1:2;3:3;6:4;9:5" starts at 2, and increases 1 every 3rd "interval"
        public static Dictionary<int, int> ParseIntProgInt(this string Progression)
        {
            Dictionary<int, int> dictionary = new();
            string[] array = Progression.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]));
            }
            return dictionary;
        }

        // as above, but for int:DieRoll progressions (good for level-based damage progressions).
        // Progression must equal "int:(string)DieRoll;int:(string)DieRoll;int:(string)DieRoll" where
        // int              is the progression "interval"
        // (string)DieRoll  is string formatted DieRoll being progression
        // ;                delimits each pair.
        // Example: "1:1d2;3:1d3;6:1d4;9:1d5" starts at 1d2, and increases d1 every 3rd "interval"
        public static Dictionary<int, DieRoll> ParseIntProgDieRoll(this string Progression)
        {
            Dictionary<int, DieRoll> dictionary = new();
            string[] array = Progression.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                DieRoll dieRoll = new DieRoll(array2[1]);
                dictionary.Add(Convert.ToInt32(array2[0]), dieRoll);
            }
            return dictionary;
        }

        public static UD_ManagedBurrowingClaws ConvertToManaged(this BurrowingClaws burrowingClaws)
        {
            UD_ManagedBurrowingClaws managedBurrowingClaws = new()
            {
                Level = burrowingClaws.Level
            };

            return managedBurrowingClaws;
        }
        public static UD_ManagedCrystallinity ConvertToManaged(this Crystallinity crystallinity)
        {
            UD_ManagedCrystallinity managedCrystallinity = new()
            {
                Level = crystallinity.Level,
                RefractAdded = crystallinity.RefractAdded
            };

            return managedCrystallinity;
        }
    }
}
