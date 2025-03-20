using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using HNPS_GigantismPlus;
using XRL.Language;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalWeaponSubpart<T> : IScribedPart
        where T : IPart, IManagedDefaultNaturalWeapon<T>, new()
    {
        public T ParentPart;

        public string Type;
        public bool CosmeticOnly;
        public bool Managed = true;

        public int Level;
        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;

        public int ModPriority;
        public int AdjectivePriority => ModPriority;
        public int NounPriority => -ModPriority;

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
            CosmeticOnly = false;
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
        public NaturalWeaponSubpart(NaturalWeaponSubpart<T> Source)
            : this()
        {
            Type = Source.Type;
            CosmeticOnly = Source.CosmeticOnly;
            Managed = Source.Managed;

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
        public NaturalWeaponSubpart(T NewParent)
            : this()
        {
            ParentPart = NewParent;
        }
        public NaturalWeaponSubpart(NaturalWeaponSubpart<T> Source, T NewParent)
            : this(Source)
        {
            ParentPart = NewParent;
        }

        public virtual bool IsCosmeticOnly()
        {
            NaturalWeaponSubpart<T> @default = new();
            bool SameBonusAsDefault = (DamageDieCount == @default.DamageDieCount 
                                    && DamageDieSize == @default.DamageDieSize 
                                    && DamageBonus == @default.DamageBonus 
                                    && HitBonus == @default.HitBonus);
            return SameBonusAsDefault || CosmeticOnly;
        }
        public virtual int GetDamageDieCount()
        {
            // base damage die count is 1
            // example: mutation calculates die count should be 6d
            //          this deducts 1, adding 5 to the existing 1
            return DamageDieCount - 1;
        }
        public virtual int GetDamageDieSize()
        {
            // base damage die size is 2
            // example: mutation calculates die size should be d5
            //          this deducts 2, adding 3 to the existing 2
            return DamageDieSize - 2;
        }
        public virtual int GetDamageBonus()
        {
            return DamageBonus;
        }
        public int GetHitBonus()
        {
            return HitBonus;
        }

        public virtual string GetAdjectiveColor()
        {
            return AdjectiveColor ?? "Y";
        }
        public virtual string GetAdjectiveColorFallback()
        {
            return AdjectiveColorFallback ?? "y";
        }
        public virtual string GetColoredAdjective()
        {
            if (Adjective.IsNullOrEmpty()) return null;
            return Adjective.OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
        }

        public virtual string GetNaturalWeaponModName(bool Managed = true)
        {
            string unmanaged = string.Empty;
            if (!this.Managed && !Managed) unmanaged = "Unmanaged";
            return "Mod" + Grammar.MakeTitleCase(Adjective) + "NaturalWeapon" + unmanaged;
        }
        public virtual ModNaturalWeaponBase<T> GetNaturalWeaponMod(bool Managed = true)
        {
            ModNaturalWeaponBase<T> NaturalWeaponMod = GetNaturalWeaponModName(Managed).ConvertToNaturalWeaponModification<T>();
            NaturalWeaponMod.NaturalWeaponSubpart = this;
            NaturalWeaponMod.AssigningPart = ParentPart;
            return GetNaturalWeaponModName().ConvertToNaturalWeaponModification<T>();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalWeaponSubpart<T> naturalWeaponSubpart = base.DeepCopy(Parent, MapInv) as NaturalWeaponSubpart<T>;
            return naturalWeaponSubpart;
        }

        public virtual bool ProcessAddedParts(string Parts)
        {
            if (Parts == null) return false;
            AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessAddedProps(string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                AddedStringProps = StringProps;
                AddedIntProps = IntProps;
            }
            return true;
        }
    }
}
