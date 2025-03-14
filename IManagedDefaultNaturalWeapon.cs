using System.Collections.Generic;
using XRL.World.Parts;

namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon
    {
        public class INaturalWeapon : IScribedPart
        {
            public int Level { get; set; }
            public int DamageDieCount { get; set; }
            public int DamageDieSize { get; set; }
            public int DamageBonus { get; set; }
            public int HitBonus { get; set; }

            public int ModPriority;
            private int AdjectivePriority => ModPriority;
            private int NounPriority => -ModPriority;

            public string Adjective;
            public string AdjectiveColor;
            public string AdjectiveColorFallback;
            public string Noun;

            public string Skill;
            public string Stat;
            public string Tile;
            public string ColorString;
            public string DetailColor;
            public string SecondColorString;
            public string SecondDetailColor;
            public string SwingSound;
            public string BlockedSound;

            public List<string> AddedParts;
            public Dictionary<string, string> AddedStringProps;
            public Dictionary<string, int> AddedIntProps;

            public string EquipmentFrameColors;

            public int GetLevel()
            {
                return Level;
            }
            public int GetDamageDieCount()
            {
                // base damage die count is 1
                // example: mutation calculates die count should be 6d
                //          this deducts 1, adding 5 to the existing 1
                return DamageDieCount - 1;
            }
            public int GetDamageDieSize()
            {
                // base damage die size is 2
                // example: mutation calculates die size should be d5
                //          this deducts 2, adding 3 to the existing 2
                return DamageDieSize - 2;
            }
            public int GetDamageBonus()
            {
                return DamageBonus;
            }
            public int GetHitBonus()
            {
                return HitBonus;
            }

            public int GetPriority()
            {
                return ModPriority;
            }

            public int GetAdjectivePriority()
            {
                return AdjectivePriority;
            }

            public int GetNounPriority()
            {
                return NounPriority;
            }

            public string GetNoun()
            {
                return Noun;
            }
            public string GetAdjective()
            {
                return Adjective;
            }
            public string GetAdjectiveColor()
            {
                return AdjectiveColor ?? "Y";
            }
            public string GetAdjectiveColorFallback()
            {
                return AdjectiveColorFallback ?? "y";
            }
            public virtual string GetColoredAdjective()
            {
                string colorText = "{{" + GetAdjectiveColor() + "|" + GetAdjective() + "}}";
                return "{{" + GetAdjectiveColorFallback() + "|" + colorText + "}}";
            }
            public string GetSwingSound()
            {
                return SwingSound;
            }
            public string GetBlockedSound()
            {
                return BlockedSound;
            }
            public List<string> GetAddedParts()
            {
                return AddedParts;
            }
            public Dictionary<string, string> GetAddedStringProps()
            {
                return AddedStringProps;
            }
            public Dictionary<string, int> GetAddedIntProps()
            {
                return AddedIntProps;
            }
            public string GetEquipmentFrameColors()
            {
                return EquipmentFrameColors;
            }
        }

        public abstract INaturalWeapon GetNaturalWeapon();

        public abstract string GetNaturalWeaponMod(bool Managed = true);
        public abstract bool CalculateNaturalWeaponLevel(int Level = 1);

        public virtual string GetNaturalWeaponColoredAdjective()
        {
            return GetNaturalWeapon().GetColoredAdjective();
        }

        public abstract bool CalculateNaturalWeaponDamageDieCount(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieSize(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageBonus(int Level = 1);

        public abstract bool CalculateNaturalWeaponHitBonus(int Level = 1);

        public abstract bool ProcessNaturalWeaponAddedParts(string Parts);

        public abstract bool ProcessNaturalWeaponAddedProps(string Props);

        public abstract int GetNaturalWeaponDamageDieCount(int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(int Level = 1);

        public abstract List<string> GetNaturalWeaponAddedParts();

        public abstract Dictionary<string, string> GetNaturalWeaponAddedStringProps();

        public abstract Dictionary<string, int> GetNaturalWeaponAddedIntProps();

        public abstract string GetNaturalWeaponEquipmentFrameColors();

        // These should allow a base cybernetics part to be wrappered into having natural weapon modifiers included
        public abstract void OnDecorateDefaultEquipment(Body body);
        public abstract void OnRegenerateDefaultEquipment(Body body);
    }
}