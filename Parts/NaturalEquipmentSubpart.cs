using System;
using System.Collections.Generic;
using HNPS_GigantismPlus;
using Sheeter;
using XRL.Language;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentSubpart<T> : IScribedPart
        where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
    {
        public const string DAMAGE_CATEGORY_NAME = "Damage";
        public const string COMBAT_CATEGORY_NAME = "Combat";
        public const string GRAMMAR_CATEGORY_NAME = "Grammar";
        public const string RENDER_CATEGORY_NAME = "Render";

        public T ParentPart;

        public string Type;
        public bool CosmeticOnly;
        public bool Managed = true;
        public string ModName;
        public int Level;

        public int ModPriority;

        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;

        public int? CombatPriority;
        public string Skill;
        public string Stat;

        public int? SkillPriority;
        public int? StatPriority;

        public int? GrammarPriority;
        public string Adjective;
        public string AdjectiveColor;
        public string AdjectiveColorFallback;
        public string Noun;

        public int? AdjectivePriority;
        public int? NounPriority;

        public int? RenderPriority;
        public string Tile;
        public string ColorString;
        public string DetailColor;
        public string SecondColorString;
        public string SecondDetailColor;
        public string SwingSound;
        public string BlockedSound;
        public string EquipmentFrameColors;

        public int? TilePriority;
        public int? ColorStringPriority;
        public int? DetailColorPriority;
        public int? SecondColorStringPriority;
        public int? SecondDetailColorPriority;
        public int? SwingSoundPriority;
        public int? BlockedSoundPriority;
        public int? EquipmentFrameColorsPriority;

        public List<string> AddedParts;
        public Dictionary<string, string> AddedStringProps;
        public Dictionary<string, int> AddedIntProps;

        public static Dictionary<string, string> PropertyCategories = new()
        {
            // Damage
            { nameof(DamageDieCount), DAMAGE_CATEGORY_NAME },
            { nameof(DamageDieSize), DAMAGE_CATEGORY_NAME },
            { nameof(DamageBonus), DAMAGE_CATEGORY_NAME },
            { nameof(HitBonus), DAMAGE_CATEGORY_NAME },

            // Combat
            { nameof(Skill), COMBAT_CATEGORY_NAME },
            { nameof(Stat), COMBAT_CATEGORY_NAME },

            //Grammar
            { nameof(Adjective), GRAMMAR_CATEGORY_NAME },
            { nameof(Noun), GRAMMAR_CATEGORY_NAME },
            
            // Render
            { nameof(Tile), RENDER_CATEGORY_NAME },
            { nameof(ColorString), RENDER_CATEGORY_NAME },
            { nameof(DetailColor), RENDER_CATEGORY_NAME },
            { nameof(SecondColorString), RENDER_CATEGORY_NAME },
            { nameof(SecondDetailColor), RENDER_CATEGORY_NAME },
            { nameof(SwingSound), RENDER_CATEGORY_NAME },
            { nameof(BlockedSound), RENDER_CATEGORY_NAME },
            { nameof(EquipmentFrameColors), RENDER_CATEGORY_NAME },
        };

        public Dictionary<string, int?> CategoryPriorities = new();
        public Dictionary<string, int?> PropertyPriorities = new();

        public NaturalEquipmentSubpart()
        {
            CosmeticOnly = false;
            Level = 1;
        }
        public NaturalEquipmentSubpart(NaturalEquipmentSubpart<T> Source)
            : this()
        {
            Type = Source.Type;
            CosmeticOnly = Source.CosmeticOnly;
            Managed = Source.Managed;
            ModName = Source.ModName;
            Level = Source.Level;

            ModPriority = Source.ModPriority;

            DamageDieCount = Source.DamageDieCount;
            DamageDieSize = Source.DamageDieSize;
            DamageBonus = Source.DamageBonus;
            HitBonus = Source.HitBonus;

            CombatPriority = Source.CombatPriority;
            Skill = Source.Skill;
            Stat = Source.Stat;

            SkillPriority = Source.SkillPriority;
            StatPriority = Source.StatPriority;

            GrammarPriority = Source.GrammarPriority;
            Adjective = Source.Adjective;
            AdjectiveColor = Source.AdjectiveColor;
            AdjectiveColorFallback = Source.AdjectiveColorFallback;
            Noun = Source.Noun;

            AdjectivePriority = Source.AdjectivePriority;
            NounPriority = Source.NounPriority;

            RenderPriority = Source.RenderPriority;
            Tile = Source.Tile;
            ColorString = Source.ColorString;
            DetailColor = Source.DetailColor;
            SecondColorString = Source.SecondColorString;
            SecondDetailColor = Source.SecondDetailColor;
            SwingSound = Source.SwingSound;
            BlockedSound = Source.BlockedSound;
            EquipmentFrameColors = Source.EquipmentFrameColors;

            TilePriority = Source.TilePriority;
            ColorStringPriority = Source.ColorStringPriority;
            DetailColorPriority = Source.DetailColorPriority;
            SecondColorStringPriority = Source.SecondColorStringPriority;
            SecondDetailColorPriority = Source.SecondDetailColorPriority;
            SwingSoundPriority = Source.SwingSoundPriority;
            BlockedSoundPriority = Source.BlockedSoundPriority;
            EquipmentFrameColorsPriority = Source.EquipmentFrameColorsPriority;

            AddedParts = new List<string>(Source.AddedParts);
            AddedStringProps = new Dictionary<string, string>(Source.AddedStringProps);
            AddedIntProps = new Dictionary<string, int>(Source.AddedIntProps);

            CategoryPriorities = new Dictionary<string, int?>(CategoryPriorities);
            PropertyPriorities = new Dictionary<string, int?>(PropertyPriorities);
        }
        public NaturalEquipmentSubpart(T NewParent)
            : this()
        {
            ParentPart = NewParent;
        }
        public NaturalEquipmentSubpart(NaturalEquipmentSubpart<T> Source, T NewParent)
            : this(Source)
        {
            ParentPart = NewParent;
        }

        public virtual bool IsCosmeticOnly()
        {
            NaturalEquipmentSubpart<T> @default = new();
            bool SameBonusAsDefault = (DamageDieCount == @default.DamageDieCount
                                    && DamageDieSize == @default.DamageDieSize
                                    && DamageBonus == @default.DamageBonus
                                    && HitBonus == @default.HitBonus
                                    && Skill == @default.Skill
                                    && Stat == @default.Stat);
            return SameBonusAsDefault || CosmeticOnly;
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
            if (ModName != null && ModName != "") return ModName;
            string unmanaged = string.Empty;
            if (!this.Managed && !Managed) unmanaged = "Unmanaged";
            return "Mod" + Grammar.MakeTitleCase(Adjective) + "NaturalWeapon" + unmanaged;
        }
        public virtual ModNaturalEquipment<T> GetNaturalWeaponMod(bool Managed = true)
        {
            ModNaturalEquipment<T> NaturalWeaponMod = GetNaturalWeaponModName(Managed).ConvertToNaturalWeaponModification<T>();
            NaturalWeaponMod.NaturalWeaponSubpart = this;
            NaturalWeaponMod.AssigningPart = ParentPart;
            return GetNaturalWeaponModName().ConvertToNaturalWeaponModification<T>();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentSubpart<T> naturalWeaponSubpart = base.DeepCopy(Parent, MapInv) as NaturalEquipmentSubpart<T>;
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

        public virtual void InitializeCategoryPriorities()
        {
            if (!CategoryPriorities.IsNullOrEmpty()) return;
            CategoryPriorities[COMBAT_CATEGORY_NAME] = CombatPriority;
            CategoryPriorities[GRAMMAR_CATEGORY_NAME] = GrammarPriority;
            CategoryPriorities[RENDER_CATEGORY_NAME] = RenderPriority;
        }
        public virtual void InitializePropertyPriorities()
        {
            if (!PropertyPriorities.IsNullOrEmpty()) return;

            PropertyPriorities[nameof(Skill)] = SkillPriority;
            PropertyPriorities[nameof(Stat)] = StatPriority;

            PropertyPriorities[nameof(Adjective)] = AdjectivePriority;
            PropertyPriorities[nameof(Noun)] = NounPriority;

            PropertyPriorities[nameof(Tile)] = TilePriority;
            PropertyPriorities[nameof(ColorString)] = ColorStringPriority;
            PropertyPriorities[nameof(DetailColor)] = DetailColorPriority;
            PropertyPriorities[nameof(SecondColorString)] = SecondColorStringPriority;
            PropertyPriorities[nameof(SecondDetailColor)] = SecondDetailColorPriority;
            PropertyPriorities[nameof(SwingSound)] = SwingSoundPriority;
            PropertyPriorities[nameof(BlockedSound)] = BlockedSoundPriority;
            PropertyPriorities[nameof(EquipmentFrameColors)] = EquipmentFrameColorsPriority;
        }

        public virtual int? GetCategoryPriority(string Category)
        {
            InitializeCategoryPriorities();

            // category has priority? return it.
            if (CategoryPriorities[Category] != null )
                return CategoryPriorities[Category];
            // otherwise...

            return ModPriority;
        }

        public virtual int? GetPropertyPriority(string Property)
        {
            InitializePropertyPriorities();

            // property has priority? return it.
            if (PropertyPriorities[Property] != null) 
                return PropertyPriorities[Property];
            // otherwise...
            return GetCategoryPriority(PropertyCategories[Property]);;
        }
    }
}
