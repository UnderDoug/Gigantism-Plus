using System;
using System.Collections.Generic;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalWeaponSubpart : IScribedPart
    {
        public string Type;
        public int Level;
        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;

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

        public NaturalWeaponSubpart()
        {
            Level = 1;
            DamageDieCount = 1;
            DamageDieSize = 2;
            DamageBonus = 0;
            HitBonus = 0;

            ModPriority = 0;
            ColorString = "&K";
            DetailColor = "y";
            SecondColorString = "&y";
            SecondDetailColor = "Y";
        }
        public NaturalWeaponSubpart(NaturalWeaponSubpart Source)
        {
            Type = Source.Type;
            Level = Source.Level;
            DamageDieCount = Source.DamageDieCount;
            DamageDieSize = Source.DamageDieSize;
            DamageBonus = Source.DamageBonus;
            HitBonus = Source.HitBonus;

            ModPriority = Source.ModPriority;

            Adjective = Source.Adjective;
            AdjectiveColor = Source.AdjectiveColor;
            AdjectiveColorFallback = Source.AdjectiveColorFallback;
            Noun = Source.Noun;

            Skill = Source.Skill;
            Stat = Source.Stat;
            Tile = Source.Tile;
            ColorString = Source.ColorString;
            DetailColor = Source.DetailColor;
            SecondColorString = Source.SecondColorString;
            SecondDetailColor = Source.SecondDetailColor;
            SwingSound = Source.SwingSound;
            BlockedSound = Source.BlockedSound;

            AddedParts = Source.AddedParts;
            AddedStringProps = Source.AddedStringProps;
            AddedIntProps = Source.AddedIntProps;

            EquipmentFrameColors = Source.EquipmentFrameColors;
        }

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
            if (Adjective.IsNullOrEmpty()) return null;
            return GetAdjective().OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
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
}
