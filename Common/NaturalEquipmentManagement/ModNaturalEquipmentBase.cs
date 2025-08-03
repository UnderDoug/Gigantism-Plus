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
            Debug.LoopItem(4, $"Adding: {Adjustment}", Indent: indent + 1, Toggle: true);
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

        public virtual ModNaturalEquipmentBase AddNounAdjustment(int Priority, ICondition<GameObject> Condition = null)
        {
            string noun = GetNoun();
            if (noun != null)
            {
                return AddAdjustment(new ChangeRenderDisplayName(noun), Priority, false, Condition);
            }
            return this;
        }
        public virtual ModNaturalEquipmentBase AddNounAdjustment(bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddNounAdjustment(modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddSkillAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeMeleeWeaponSkill(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddSkillAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddSkillAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddMeleeStatAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeMeleeWeaponStat(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddMeleeStatAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddMeleeStatAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddTileAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return
            AddAdjustment(new ChangeTile(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddTileAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddTileAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddColorStringAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeColorString(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddColorStringAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddColorStringAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddTileColorAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeTileColor(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddTileColorAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddTileColorAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDetailColorAdjustment(string Value, int Priority, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new ChangeDetailColor(Value), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddDetailColorAdjustment(string Value, bool FlipPriority = false, ICondition<GameObject> Condition = null)
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddDetailColorAdjustment(Value, modPriority, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDamageDieCountAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustDamageDieCount(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDamageDieSizeAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustDamageDieSize(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDamageBonusAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustDamageBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddHitBonusAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustHitBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddDamageAttributesAdjustment(string Attributes, string AtrtibuteName, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AddDamageAttribute(Attributes, AtrtibuteName), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddPenBonusAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustPenBonus(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddArmorAVAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustArmorAV(Amount), 0, false, Condition);
        }

        public virtual ModNaturalEquipmentBase AddArmorDVAdjustment(int Amount, ICondition<GameObject> Condition = null)
        {
            return AddAdjustment(new AdjustArmorDV(Amount), 0, false, Condition);
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

        public virtual ModNaturalEquipmentBase AddAddPartAdjustment<P>(int Priority, ICondition<GameObject> Condition = null)
            where P : IPart, new()
        {
            return AddAdjustment(new AddPart<P>(), Priority, false, Condition);
        }
        public virtual ModNaturalEquipmentBase AddAddPartAdjustment<P>(bool FlipPriority = false, ICondition<GameObject> Condition = null)
            where P : IPart, new()
        {
            int modPriority = FlipPriority ? -ModPriority : ModPriority;
            return AddAddPartAdjustment<P>(modPriority, Condition);
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