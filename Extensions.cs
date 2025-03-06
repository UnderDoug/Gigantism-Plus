using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

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
    }
}
