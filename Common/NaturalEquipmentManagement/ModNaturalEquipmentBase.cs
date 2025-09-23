using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using XRL.Language;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalEquipmentBase
        : IModification
        , IModEventHandler<BeforeApplyAdjustmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModNaturalEquipmentBase));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {

            };
            List<object> dontList = new()
            {
                nameof(AddAdjustment),
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        private GameObject _wielder = null;
        public GameObject Wielder
        {
            get => _wielder ??= GetWielder();
            set => _wielder = value;
        }

        [NonSerialized]
        public NaturalEquipmentManager Manager;

        private NaturalEquipmentOperator _operator = null;
        public NaturalEquipmentOperator Operator
        {
            get => _operator ??= GetOperator();
            set => _operator = value;
        }

        public Adjustments Adjustments;

        public string BodyPartType;

        public int ModPriority;
        public int DescriptionPriority;

        public int DamageDieCount;
        public int DamageDieSize;
        public int DamageBonus;
        public int HitBonus;
        public int PenBonus;

        public bool ForceNoun;
        public string Noun;

        public string Adjective;
        public string AdjectiveColor;
        public string AdjectiveColorFallback;
        public bool ExludeFromDynamicTile;

        public ModNaturalEquipmentBase()
        {
            Manager = null;

            Adjustments = new();

            ForceNoun = false;

            ExludeFromDynamicTile = false;
        }
        public ModNaturalEquipmentBase(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }
        public ModNaturalEquipmentBase(ModNaturalEquipmentBase Source)
            : this()
        {
            Manager = Source.Manager;

            BodyPartType = Source.BodyPartType;

            Adjustments = new(Adjustments ??= new());

            ModPriority = Source.ModPriority;
            DescriptionPriority = Source.DescriptionPriority;

            DamageDieCount = Source.DamageDieCount;
            DamageDieSize = Source.DamageDieSize;
            DamageBonus = Source.DamageBonus;
            HitBonus = Source.HitBonus;
            PenBonus = Source.PenBonus;

            ForceNoun = Source.ForceNoun;
            Noun = Source.Noun;

            Adjective = Source.Adjective;
            AdjectiveColor = Source.AdjectiveColor;
            AdjectiveColorFallback = Source.AdjectiveColorFallback;
            ExludeFromDynamicTile = Source.ExludeFromDynamicTile;
        }
        public ModNaturalEquipmentBase(NaturalEquipmentManager NewManager, ModNaturalEquipmentBase Source)
            : this(Source)
        {
            Manager = NewManager;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            return Object.HasPart<Physics>()
                && Object.IsNaturalEquipment();
        }

        public GameObject GetWielder()
        {
            return ParentObject?.Wielder();
        }

        public NaturalEquipmentOperator GetOperator()
        {
            return ParentObject?.NaturalEquipmentOperator();
        }

        public virtual ModNaturalEquipmentBase AddAdjustment(IAdjustment Adjustment, int Priority, bool FlipPriority, ICondition<GameObject> Condition = null)
        {
            int indent = Debug.LastIndent;
            Adjustments ??= new();
            int modPriority = FlipPriority ? -Priority : Priority;
            Adjustment.Source ??= GetType();
            Adjustment.Condition ??= Condition;
            Adjustment.Priority = modPriority;
            Debug.LoopItem(4, $"Adding: {Adjustment}", Indent: indent + 1, Toggle: getDoDebug(nameof(AddAdjustment)));
            Adjustments.Add(Adjustment);
            Debug.LastIndent = indent;
            return this;
        }

        public virtual ModNaturalEquipmentBase AddAdjustment(IAdjustment Adjustment, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(Adjustment, Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddAdjustment(IAdjustment Adjustment, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(Adjustment, ModPriority, FlipPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustNoun(int Priority, ICondition<GameObject> Condition = null)
        {
            string noun = GetNoun();
            if (noun != null)
            {
                return AddAdjustment(new ChangeRenderDisplayName(noun), Priority, false, Condition);
            }
            return this;
        }
        public virtual ModNaturalEquipmentBase AdjustNoun(bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustNoun(modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeSkill(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeMeleeWeaponSkill(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustMeleeSkill(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustMeleeSkill(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeStat(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeMeleeWeaponStat(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustMeleeStat(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustMeleeStat(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustTile(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeTile(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustTile(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustTile(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustColorString(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeColorString(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustColorString(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustColorString(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustTileColor(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeTileColor(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustTileColor(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustTileColor(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustDetailColor(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeDetailColor(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AdjustDetailColor(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AdjustDetailColor(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeDamageDieCount(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustMeleeDamageDieCount(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeDamageDieSize(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustMeleeDamageDieSize(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeDamageBonus(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustMeleeDamageBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustMeleeHitBonus(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustMeleeHitBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddMeleeDamageAttributes(string Attributes, string AtrtibuteName, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AddMeleeDamageAttribute(Attributes, AtrtibuteName), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustPenBonus(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustMeleePenBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustArmorAV(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustArmorAV(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustArmorDV(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustArmorDV(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustArmorStatistic(string Statistic, int Amount, ICondition<GameObject> Condition = null)
        {
            AdjustArmorStatistic adjustment = Statistic switch
            {
                "Strength" => new AdjustArmorStrength(Amount),
                "Agility" => new AdjustArmorAgility(Amount),
                "Toughness" => new AdjustArmorToughness(Amount),
                "Intelligence" => new AdjustArmorIntelligence(Amount),
                "Willpower" => new AdjustArmorWillpower(Amount),
                "Ego" => new AdjustArmorEgo(Amount),
                _ => null,
            };
            return AddAdjustment(adjustment, 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AdjustArmorResistance(string Resistance, int Amount, ICondition<GameObject> Condition = null)
        {
            AdjustArmorResistance adjustment = Resistance switch
            {
                "Acid" => new AdjustArmorAcidResist(Amount),
                "AcidResist" => new AdjustArmorAcidResist(Amount),
                "AcidResistance" => new AdjustArmorAcidResist(Amount),
                "Acidic" => new AdjustArmorAcidResist(Amount),
                "AcidicResist" => new AdjustArmorAcidResist(Amount),
                "AcidicResistance" => new AdjustArmorAcidResist(Amount),
                "AR" => new AdjustArmorAcidResist(Amount),

                "Cold" => new AdjustArmorColdResist(Amount),
                "ColdResist" => new AdjustArmorColdResist(Amount),
                "ColdResistance" => new AdjustArmorColdResist(Amount),
                "CR" => new AdjustArmorColdResist(Amount),

                "Elec" => new AdjustArmorElecResist(Amount),
                "ElecResist" => new AdjustArmorElecResist(Amount),
                "ElecResistance" => new AdjustArmorElecResist(Amount),
                "Electric" => new AdjustArmorElecResist(Amount),
                "ElectricResist" => new AdjustArmorElecResist(Amount),
                "ElectricResistance" => new AdjustArmorElecResist(Amount),
                "Electrical" => new AdjustArmorElecResist(Amount),
                "ElectricalResist" => new AdjustArmorElecResist(Amount),
                "ElectrialcResistance" => new AdjustArmorElecResist(Amount),
                "ER" => new AdjustArmorElecResist(Amount),

                "Heat" => new AdjustArmorHeatResist(Amount),
                "HeatResist" => new AdjustArmorHeatResist(Amount),
                "HeatResistance" => new AdjustArmorElecResist(Amount),
                "HR" => new AdjustArmorHeatResist(Amount),

                _ => null,
            };
            return AddAdjustment(adjustment, 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddArmorStatisticAdjustment(string Statistic, int Amount, ICondition<GameObject> Condition = null)
        {
            AdjustArmorStatistic adjustment = Statistic switch
            {
                "Strength" => new AdjustArmorStrength(Amount),
                "Agility" => new AdjustArmorAgility(Amount),
                "Toughness" => new AdjustArmorToughness(Amount),
                "Intelligence" => new AdjustArmorIntelligence(Amount),
                "Willpower" => new AdjustArmorWillpower(Amount),
                "Ego" => new AdjustArmorEgo(Amount),
                _ => null,
            };
            return AddAdjustment(adjustment, 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddPart<P>(int Priority, ICondition<GameObject> Condition = null)
            where P : IPart, new()
        {
            return AddAdjustment(new AddPart<P>(), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddPart<P>(bool FlipPriority = false, ICondition<GameObject> Condition = null)
            where P : IPart, new()
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddPart<P>(modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddPart(string Part, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AddPart(Part), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddPart(string Part, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddPart(Part, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase SetStringProperty(string Label, string Prop, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetStringProperty(Label, Prop), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetStringProperty(string Label, string Prop, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetStringProperty(Label, Prop, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase SetIntProperty(string Label, int Prop, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetIntProperty(Label, Prop), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetIntProperty(string Label, int Prop, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetIntProperty(Label, Prop, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase SetSwingSound(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetSwingSound(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetSwingSound(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetSwingSound(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase SetBlockedSound(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetBlockedSound(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetBlockedSound(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetBlockedSound(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase SetEquipmentFrameColors(string Value, bool Override, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetEquipmentFrameColors(Value, Override), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetEquipmentFrameColors(string Value, bool Override, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetEquipmentFrameColors(Value, Override, modPriority, Condition);
        }
        public virtual ModNaturalEquipmentBase SetEquipmentFrameColors(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new SetEquipmentFrameColors(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase SetEquipmentFrameColors(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return SetEquipmentFrameColors(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddPrimaryDescription(DescriptionElement Value, int Order, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ArbitraryDescription(Order, Value, default), 0, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddPrimaryDescription(DescriptionElement Value, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ArbitraryDescription(Value, default), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddSecondaryDescription(DescriptionElement Value, int Order, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ArbitraryDescription(Order, default, Value), 0, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddSecondaryDescription(DescriptionElement Value, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ArbitraryDescription(default, Value), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDiminishingReturnsDescription(string Affected, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new DiminishingReturns(Affected), 0, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddDiminishingReturnsDescription(string Affected, int Order, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new DiminishingReturns(Order, Affected), 0, false, Condition);
        }

        public virtual int GetDamageDieCount()
        {
            return DamageDieCount;
        }
        public virtual int GetDamageDieSize()
        {
            return DamageDieSize;
        }

        public virtual int GetDamageBonus()
        {
            return DamageBonus;
        }

        public virtual int GetHitBonus()
        {
            return HitBonus;
        }
        public virtual int GetPenBonus()
        {
            return PenBonus;
        }

        public virtual string GetNoun()
        {
            return Noun;
        }
        public virtual string GetAdjective()
        {
            return Adjective ?? "adjective?";
        }
        public virtual string GetColoredAdjective()
        {
            return GetAdjective().OptionalColor(AdjectiveColor, AdjectiveColorFallback, Colorfulness);
        }

        public virtual string GetAdjectiveIndicativeNoun(GameObject Object = null)
        {
            Object ??= ParentObject;

            string adjective = Grammar.MakeTitleCase(GetColoredAdjective());

            string objectNoun = Object?.GetObjectNoun();
            objectNoun = Object != null && Object.IsPlural ? Grammar.Pluralize(objectNoun) : objectNoun;

            return $"{adjective}: {Object?.IndicativeProximal} {objectNoun} ";
        }

        public abstract string GetInstanceDescription(GameObject Object = null);

        public virtual int GetDescriptionPriority()
        {
            return DescriptionPriority;
        }

        public override void ApplyModification(GameObject Object)
        {
            if (Object.HasPartDescendedFrom<ModNaturalEquipmentBase>()
                && !Object.GetPartsDescendedFrom<ModNaturalEquipmentBase>(p => SameModification(p, false)).IsNullOrEmpty())
            {
                return;
            }
            base.ApplyModification(Object);
        }
        public virtual bool SameModification(ModNaturalEquipmentBase m, bool Strict = true, bool? DescriptionOnly = null)
        {
            bool sameBodyPartType = BodyPartType == m.BodyPartType;
            bool sameModPriority = ModPriority == m.ModPriority;
            bool sameDescriptionPriority = DescriptionPriority == m.DescriptionPriority;
            bool sameNoun = Noun == m.Noun;
            bool sameAdjective = Adjective == m.Adjective;
            bool sameSource = GetSource() == m.GetSource();

            bool sameGenerally = sameBodyPartType && sameNoun && sameAdjective && sameSource;
            bool sameForDescription = sameGenerally && sameDescriptionPriority;
            bool sameForMod = sameGenerally && sameModPriority;

            bool sameStrictly = sameForDescription && sameForMod;

            if (!Strict)
            {
                if (DescriptionOnly != null)
                {
                    if ((bool)DescriptionOnly)
                    {
                        return sameForDescription;
                    }
                    else
                    {
                        return sameForMod;
                    }
                }
                return sameGenerally;
            }
            return sameStrictly;
        }
        public abstract string GetSource();

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID
                || ID == BeforeApplyAdjustmentEvent.ID;
        }
        public virtual bool HandleEvent(BeforeApplyAdjustmentEvent E)
        {
            return base.HandleEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent, MapInv) as ModNaturalEquipmentBase;

            if (!Adjustments.IsNullOrEmpty())
            {
                naturalEquipmentMod.Adjustments = new();
                foreach (IAdjustment adjustment in Adjustments)
                {
                    naturalEquipmentMod.Adjustments.Add(adjustment.DeepCopy());
                }
            }
            naturalEquipmentMod.Adjustments ??= new();

            return ClearForCopy(naturalEquipmentMod);
        }
        public override IPart DeepCopy(GameObject Parent)
        {
            ModNaturalEquipmentBase naturalEquipmentMod = base.DeepCopy(Parent) as ModNaturalEquipmentBase;

            naturalEquipmentMod.BodyPartType = BodyPartType;

            naturalEquipmentMod.ModPriority = ModPriority;
            naturalEquipmentMod.DescriptionPriority = DescriptionPriority;

            naturalEquipmentMod.DamageDieCount = DamageDieCount;
            naturalEquipmentMod.DamageDieSize = DamageDieSize;
            naturalEquipmentMod.DamageBonus = DamageBonus;
            naturalEquipmentMod.HitBonus = HitBonus;
            naturalEquipmentMod.PenBonus = PenBonus;

            naturalEquipmentMod.ForceNoun = ForceNoun;
            naturalEquipmentMod.Noun = Noun;

            naturalEquipmentMod.Adjective = Adjective;
            naturalEquipmentMod.AdjectiveColor = AdjectiveColor;
            naturalEquipmentMod.AdjectiveColorFallback = AdjectiveColorFallback;
            naturalEquipmentMod.ExludeFromDynamicTile = ExludeFromDynamicTile;

            return ClearForCopy(naturalEquipmentMod);
        }
        public static ModNaturalEquipmentBase ClearForCopy(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            NaturalEquipmentMod.Wielder = null;
            NaturalEquipmentMod.Manager = null;
            return NaturalEquipmentMod;
        }

        public override void FinalizeCopy(GameObject Source, bool CopyEffects, bool CopyID, Func<GameObject, GameObject> MapInv)
        {
            base.FinalizeCopy(Source, CopyEffects, CopyID, MapInv);
            Manager = Operator?.Manager;
        }

    } //!-- public class ModNaturalEquipmentBase : ModPart
}