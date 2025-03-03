using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;
using XRL.Rules;
using XRL.World.Parts.Mutation;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.Mutation.BaseManagedDefaultEquipmentMutation;

namespace XRL.World
{
    public interface IDefaultNaturalWeaponManager
    {
        public abstract class NaturalWeapon
        {
            public int DamageDieCount;
            public int DamageDieSize;
            public int DamageBonus;
            public int HitBonus;

            public int Priority;
            public string Adjective;
            public string AdjectiveColor;
            public string Noun;

            public string Skill;
            public string Stat;
            public string Tile;
            public string RenderColorString;
            public string RenderDetailColor;
            public string SecondRenderColorString;
            public string SecondRenderDetailColor;

            public abstract int GetDamageDieCount(int Level = 1);
            public abstract int GetDamageDieSize(int Level = 1);
            public abstract int GetDamageBonus(int Level = 1);
            public abstract int GetHitBonus(int Level = 1);

            public abstract int GetPriority();
            public abstract int GetAdjectivePriority();
            public abstract int GetNounPriority();

            public abstract string GetNoun();
            public abstract string GetAdjective();
            public abstract string GetAdjectiveColor();
            public abstract string GetColoredAdjective();
        }

        public virtual int GetNaturalWeaponDamageDieCount(int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponDamageDieSize(int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponDamageBonus(int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponPriority()
        {
            return 0;
        }

        public virtual string GetNaturalWeaponAdjective()
        {
            return "";
        }

        public virtual string GetNaturalWeaponAdjectiveColor()
        {
            return "";
        }

        public virtual string GetNaturalWeaponColoredAdjective()
        {
            return "";
        }
    }
}

namespace XRL.World.Parts.Mutation
{
    public class BaseManagedDefaultEquipmentMutation : BaseDefaultEquipmentMutation, IDefaultNaturalWeaponManager
    {
        public class INaturalWeapon : IDefaultNaturalWeaponManager.NaturalWeapon
        {
            public INaturalWeapon()
            {
                DamageDieCount = 1;
                DamageDieSize = 2;
                DamageBonus = 0;
                HitBonus = 0;

                Priority = 0;
                Adjective = "natural";
                AdjectiveColor = "Y";
                Noun = "fist";

                Skill = "Cudgel";
                Stat = "Strength";
                Tile = "Creatures/natural-weapon-fist.bmp";
                RenderColorString = "&w";
                RenderDetailColor = "W";
                SecondRenderColorString = "&w";
                SecondRenderDetailColor = "W";
            }

            public override int GetDamageDieCount(int Level = 1)
            {
                return DamageDieCount;
            }

            public override int GetDamageDieSize(int Level = 1)
            {
                return DamageDieSize;
            }

            public override int GetDamageBonus(int Level = 1)
            {
                return DamageBonus;
            }

            public override int GetHitBonus(int Level = 1)
            {
                return HitBonus;
            }

            public override int GetPriority()
            {
                return Priority;
            }

            public override int GetAdjectivePriority()
            {
                return GetPriority();
            }

            public override int GetNounPriority()
            {
                return -GetPriority();
            }

            public override string GetNoun()
            {
                return Noun;
            }

            public override string GetAdjective()
            {
                return Adjective;
            }

            public override string GetAdjectiveColor()
            {
                return AdjectiveColor;
            }

            public override string GetColoredAdjective()
            {
                return "{{" + GetAdjectiveColor() + "|" + GetAdjective() + "}}";
            }
        }

        public INaturalWeapon NaturalWeapon = new();

        public virtual int GetNaturalWeaponDamageDieCount(int Level = 1)
        {
            return NaturalWeapon.GetDamageDieCount(Level);
        }

        public virtual int GetNaturalWeaponDamageDieSize(int Level = 1)
        {
            return NaturalWeapon.GetDamageDieSize(Level);
        }

        public virtual int GetNaturalWeaponDamageBonus(int Level = 1)
        {
            return NaturalWeapon.GetDamageBonus(Level);
        }

        public virtual int GetNaturalWeaponHitBonus(int Level = 1)
        {
            return NaturalWeapon.GetHitBonus(Level);
        }

        public virtual int GetNaturalWeaponPriority()
        {
            return NaturalWeapon.GetPriority();
        }

        public virtual string GetNaturalWeaponAdjective()
        {
            return NaturalWeapon.GetAdjective();
        }

        public virtual string GetNaturalWeaponAdjectiveColor()
        {
            return NaturalWeapon.GetAdjectiveColor();
        }

        public virtual string GetNaturalWeaponColoredAdjective()
        {
            return NaturalWeapon.GetColoredAdjective();
        }
    }
}

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : IMeleeModification where T : BaseManagedDefaultEquipmentMutation, new()
    {
        public ModNaturalWeaponBase()
        {
        }

        public ModNaturalWeaponBase(int Tier)
            : base(Tier)
        {
        }

        private static string modificationDisplayName;
        public override string GetModificationDisplayName()
        {
            return modificationDisplayName;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            AssigningMutation = Wielder.GetPart<T>();
            Level = AssigningMutation.Level;
            NaturalWeapon = AssigningMutation.NaturalWeapon;
            return true;
        }

        public override int GetModificationSlotUsage()
        {
            return 0;
        }


        private GameObject wielder = null;
        public GameObject Wielder { get => wielder; set => wielder = value; }

        private T assigningMutation = null;
        public T AssigningMutation { get => assigningMutation; set => assigningMutation = value; }

        private int level = 1;
        public int Level { get => level; set => level = value; }

        private INaturalWeapon naturalWeapon;
        public INaturalWeapon NaturalWeapon { get => naturalWeapon; set => naturalWeapon = value; }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalWeaponBase<T> modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalWeaponBase<T>;
            modNaturalWeaponBase.AssigningMutation = null;
            modNaturalWeaponBase.Wielder = null;
            modNaturalWeaponBase.NaturalWeapon = null;
            return modNaturalWeaponBase;
        }

        public virtual int GetDamageDieCount()
        {
            // base damage die count is 1 
            if (Wielder == null || AssigningMutation == null) return AssigningMutation.NaturalWeapon.GetDamageDieCount(1) - 1;
            return AssigningMutation.NaturalWeapon.GetDamageDieCount(Level) - 1;
        }
        public virtual int GetDamageDieSize()
        {
            // base damage die size is 2 
            if (Wielder == null || AssigningMutation == null) return AssigningMutation.NaturalWeapon.GetDamageDieSize(1) - 2;
            return AssigningMutation.NaturalWeapon.GetDamageDieSize(Level) - 2;
        }

        public virtual int GetDamageBonus()
        {
            // base damage bonus is 0 
            if (Wielder == null || AssigningMutation == null) return AssigningMutation.NaturalWeapon.GetDamageBonus(1);
            return AssigningMutation.NaturalWeapon.GetDamageBonus(Level);
        }

        public virtual int GetHitBonus()
        {
            // base hit bonus is 0 
            if (Wielder == null || AssigningMutation == null) return AssigningMutation.NaturalWeapon.GetHitBonus(1);
            return AssigningMutation.NaturalWeapon.GetHitBonus(Level);
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            bool hasMods = !Object.GetPartsDescendedFrom<ModNaturalWeaponBase<BaseManagedDefaultEquipmentMutation>>().Any();

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-2 into 1d2+0
            if (!hasMods) weapon.AdjustDamage(1);

            DieRoll baseDamage = new(weapon.BaseDamage);
            Debug.Entry(4, "baseDamage.ToString()", $"{baseDamage}", Indent: 4);
            Debug.Entry(4, "baseDamage.LeftValue Before", $"{baseDamage.LeftValue}", Indent: 4);

            Debug.Entry(4, "* if (baseDamage.LeftValue == 0)", Indent: 4);
            if (baseDamage.LeftValue == 0)
            {
                Debug.Entry(4, "+ baseDamage.LeftValue is 0", Indent: 5);

                DieRoll baseDamageLeftDieRoll = baseDamage.Left;

                Debug.Entry(4, "baseDamageLeftDieRoll.ToString()", $"{baseDamageLeftDieRoll}", Indent: 5);
                Debug.Entry(4, "baseDamageLeftDieRoll.LeftValue Before", $"{baseDamageLeftDieRoll.LeftValue}", Indent: 5);

                baseDamageLeftDieRoll.LeftValue += GetDamageDieCount();

                Debug.Entry(4, "baseDamageLeftDieRoll.LeftValue After", $"{baseDamageLeftDieRoll.LeftValue}", Indent: 5);
                baseDamage = new(4, baseDamageLeftDieRoll, baseDamage.RightValue);
            }
            else
            {
                Debug.Entry(4, "- baseDamage.LeftValue not 0", Indent: 5);
                baseDamage.LeftValue += GetDamageDieCount();
                Debug.Entry(4, "baseDamage.LeftValue After", $"{baseDamage.LeftValue}", Indent: 4);
            }
            Debug.Entry(4, "x if (baseDamage.LeftValue == 0) ?//", Indent: 4);
            weapon.BaseDamage = baseDamage.ToString();

            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            Debug.Entry(4, "baseDamage", $"{baseDamage}", Indent: 5);
            weapon.HitBonus = GetHitBonus();

            Render render = Object.Render;
            if (Object.GetIntProperty("CurrentNaturalWeaponBasePriority") > NaturalWeapon.GetAdjectivePriority())
            {
                Object.SetIntProperty("CurrentNaturalWeaponBasePriority", NaturalWeapon.GetNounPriority());
                weapon.Skill = NaturalWeapon.Skill;
                render.DisplayName = NaturalWeapon.Noun;
                render.Tile = NaturalWeapon.Tile;
                render.ColorString = (render.ColorString == NaturalWeapon.RenderColorString) ? NaturalWeapon.SecondRenderColorString : NaturalWeapon.RenderColorString;
                render.DetailColor = (render.DetailColor == NaturalWeapon.RenderDetailColor) ? NaturalWeapon.SecondRenderDetailColor : NaturalWeapon.RenderDetailColor;
            }
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation.GetMutationClass(), false);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetCleaveAmountEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetCleaveAmountEvent E)
        {
            bool isReady = IsReady(UseCharge: true,
                                   IgnoreCharge: false,
                                   IgnoreLiquid: false,
                                   IgnoreBootSequence: false,
                                   IgnoreBreakage: false,
                                   IgnoreRust: false, IgnoreEMP: false,
                                   IgnoreRealityStabilization: false,
                                   IgnoreSubject: false,
                                   IgnoreLocallyDefinedFailure: false, 1, null,
                                   UseChargeIfUnpowered: false, 0L, null);

            if (IsObjectActivePartSubject(E.Object) && isReady)
            {
                E.Amount += 1 + GetDamageBonus();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetModificationDisplayName(), NaturalWeapon.GetAdjectivePriority());
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public static string GetDescription()
        {
            string descriptionName = Grammar.MakeTitleCase(modificationDisplayName);
            return $"{descriptionName}: " + "This weapon's damage die count and damage bonus is determined by its wielder's size.";
        }

        public virtual string GetInstanceDescription()
        {
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string descriptionName = Grammar.MakeTitleCase(GetModificationDisplayName());

            return $"{descriptionName}: ";
        }

    } //!-- public class ModNaturalWeaponBase : IMeleeModification

    [Serializable]
    public class ModGiganticNaturalWeapon : ModNaturalWeaponBase<GigantismPlus>
    {
        public ModGiganticNaturalWeapon()
        {
        }

        public ModGiganticNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        private static string modificationDisplayName = "{{gigantic|gigantic}}";
        public override string GetModificationDisplayName()
        {
            return modificationDisplayName;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            return base.BeingAppliedBy(obj,who);
        }

        public override int GetModificationSlotUsage()
        {
            return base.GetModificationSlotUsage();
        }
        

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            return base.DeepCopy(Parent, MapInv);
        }

        public override int GetDamageDieCount()
        {
            return base.GetDamageDieCount();
        }

        public override int GetDamageBonus()
        {
            return base.GetDamageBonus();
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            return base.ModificationApplicable(Object);
        }

        public override void ApplyModification(GameObject Object)
        {
            Object.SetIntProperty("ModGiganticNoShortDescription", 1);

            base.ApplyModification(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetCleaveAmountEvent.ID
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetCleaveAmountEvent E)
        {
            bool isReady = IsReady(UseCharge: true,
                                   IgnoreCharge: false,
                                   IgnoreLiquid: false,
                                   IgnoreBootSequence: false,
                                   IgnoreBreakage: false,
                                   IgnoreRust: false, IgnoreEMP: false,
                                   IgnoreRealityStabilization: false,
                                   IgnoreSubject: false,
                                   IgnoreLocallyDefinedFailure: false, 1, null,
                                   UseChargeIfUnpowered: false, 0L, null);

            if (IsObjectActivePartSubject(E.Object) && isReady)
            {
                E.Amount += 1 + GetDamageBonus();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetModificationDisplayName(), NaturalWeapon.GetAdjectivePriority());
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public static string GetDescription()
        {
            string descriptionName = Grammar.MakeTitleCase(modificationDisplayName);
            return $"{descriptionName}: " + "This weapon's damage die count and damage bonus is determined by its wielder's size.";
        }

        public override string GetInstanceDescription()
        {
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string descriptionName = Grammar.MakeTitleCase(GetModificationDisplayName());
            int damageBonus = 3 + GetDamageBonus();
            List<List<string>> list = new();
            string text = "weapon";
            if (part != null && ParentObject.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                list.Add(new List<string> { "have", $"{damageBonus.Signed()} damage" });
                if (part.Skill == "Cudgel")
                {
                    list.Add(new List<string>
            {
                null,
                "twice as effective when you Slam with " + ParentObject.them
            });
                }
                else if (part.Skill == "Axe")
                {
                    list.Add(new List<string> { "cleave", $"for {(-damageBonus).Signed()} AV" });
                }
            }
            if (ParentObject.HasPart<DiggingTool>() || ParentObject.HasPart<Drill>())
            {
                list.Add(new List<string> { "dig", "twice as fast" });
            }
            if (list.Count == 0)
            {
                return $"{descriptionName}: " + "This weapon's damage die count is determined by its wielder's size.";
            }
            List<string> list2 = new();
            foreach (List<string> item in list)
            {
                list2.Add(GetProcessedItem(item, second: false, list, ParentObject));
            }
            return $"{descriptionName}: " + (ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text)) + " " + Grammar.MakeAndList(list2) + ". Its damage die count is determined by its wielder's size.";
        }

        // Ripped wholesale from ModGigantic.
        private static string GetProcessedItem(List<string> item, bool second, List<List<string>> items, GameObject obj)
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

    } //!-- public class ModGiganticNaturalWeapon : IMeleeModification

    [Serializable]
    public class ModElongatedNaturalWeapon : IMeleeModification
    {
        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        private static readonly string modificationDisplayName = "{{giant|elongated}}";
        public override string GetModificationDisplayName()
        {
            return modificationDisplayName;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            ElongatedPaws = Wielder.GetPart<ElongatedPaws>();
            Level = ElongatedPaws.Level;
            return true;
        }

        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        private GameObject wielder = null;

        public GameObject Wielder { get => wielder; set => wielder = value; }

        private ElongatedPaws elongatedPaws = null;

        public ElongatedPaws ElongatedPaws { get => elongatedPaws; set => elongatedPaws = value; }

        private int level = 1;
        public int Level { get => level; set => level = value; }

        private const int naturalWeaponPriority = 20;

        public static int NaturalWeaponPriority => naturalWeaponPriority;

        public static string WeaponSkill = "ShortBlades";
        public static string WeaponDisplayName = "paw";
        public static string WeaponTile = "ElongatedPaw_Alt.png";
        public static string WeaponColorString = "&x";
        public static string WeaponDetailColor = "z";
        public static string SecondWeaponColorString = "&X";
        public static string SecondWeaponDetailColor = "Z";
        
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModElongatedNaturalWeapon modElongatedNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModElongatedNaturalWeapon;
            modElongatedNaturalWeapon.ElongatedPaws = null;
            modElongatedNaturalWeapon.Wielder = null;
            return modElongatedNaturalWeapon;
        }

        public int GetDieSizeIncrease()
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.GetDieSizeIncrease", Indent: 4);
            if (Wielder == null && ElongatedPaws == null) return 2;
            Debug.Entry(4, $"Wielder not null, ElongatedPaws not null", Indent: 5);
            return ElongatedPaws.ElongatedDieSizeBonus;
        }

        public int GetBonusDamageIncrease()
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.GetBonusDamageIncrease", Indent: 4);
            if (Wielder == null && ElongatedPaws == null) return 2;
            Debug.Entry(4, $"Wielder not null, ElongatedPaws not null", Indent: 5);
            return ElongatedPaws.ElongatedBonusDamage;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped})", Indent: 4);
            
            if (Wielder != null && Wielder.HasPart<ElongatedPaws>())
            {
                Debug.Entry(4, $"Wielder not null, Wielder has ElongatedPaws", Indent: 5);
                ElongatedPaws = Wielder.GetPart<ElongatedPaws>();
            }
            else
            {
                Debug.Entry(4, $"Wielder null or Wielder lacks ElongatedPaws", Indent: 5);
            }

            Object.RemovePart<WeaponElongator>();

            bool hasMods = !Object.GetPartsDescendedFrom<IMeleeModification>().Any();

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            if (!hasMods) weapon.AdjustDamage(1);
            weapon.AdjustDamageDieSize(GetDieSizeIncrease());
            weapon.AdjustDamage(GetBonusDamageIncrease());

            Render render = Object.Render;
            if (Object.GetIntProperty("CurrentNaturalWeaponBasePriority") > -naturalWeaponPriority)
            {
                Object.SetIntProperty("CurrentNaturalWeaponBasePriority", -naturalWeaponPriority);
                weapon.Skill = WeaponSkill;
                render.DisplayName = WeaponDisplayName;
                render.Tile = WeaponTile;
                render.ColorString = (render.ColorString == WeaponColorString) ? SecondWeaponColorString : WeaponColorString;
                render.DetailColor = (render.DetailColor == WeaponDetailColor) ? SecondWeaponDetailColor : WeaponDetailColor;
            }
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

            Object.SetStringProperty("TemporaryDefaultBehavior", "ElongatedPaws", false);
            Debug.Entry(4, $"x ModElongatedNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped}) ]//", Indent: 4);
        }


        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetModificationDisplayName(), naturalWeaponPriority);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("StatChange_Strength");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "StatChange_Strength")
            {
                Debug.Entry(4, "BOI, we got that StatChange_Strength Event listened to!");
            }
            return base.FireEvent(E);
        }

        public static string GetDescription()
        {
            return $"Elongated: This natural weapon has its die size increased by up to 2 based on other present mutations, and gets half your Strength Modifier in Bonus Damage.";
        }

        public string GetInstanceDescription()
        {

            string description = "{{giant|Elongated}}: This natural weapon ";
            if (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"has its die size increased by {GetDieSizeIncrease()}, and ";
            }
            string bonusDamageIncrease = "";
            if (GetBonusDamageIncrease() != 0) bonusDamageIncrease = $"({GetBonusDamageIncrease()}) ";
            description += $"gets half its wielder's Strength Modifier {bonusDamageIncrease}added as Bonus Damage.";
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : IMeleeModification

    [Serializable]
    public class ModBurrowingNaturalWeapon : IMeleeModification
    {
        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        private static readonly string modificationDisplayName = "{{W|burrowing}}";
        public override string GetModificationDisplayName()
        {
            return modificationDisplayName;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            BurrowingClaws = Wielder.GetPart<BurrowingClaws>();
            Level = BurrowingClaws.Level;
            return true;
        }

        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        private GameObject wielder = null;
        public GameObject Wielder { get => wielder; set => wielder = value; }

        private BurrowingClaws burrowingClaws = null;
        public BurrowingClaws BurrowingClaws { get => burrowingClaws; set => burrowingClaws = value; }

        private int level = 1;
        public int Level { get => level; set => level = value; }

        private const int naturalWeaponPriority = 30;
        public static int NaturalWeaponPriority => naturalWeaponPriority;

        public static string WeaponSkill = "ShortBlades";
        public static string WeaponDisplayName = "claw";
        public static string WeaponTile = "Creatures/natural-weapon-claw.bmp";
        public static string WeaponColorString = "&w";
        public static string WeaponDetailColor = "W";
        public static string SecondWeaponColorString = "&w";
        public static string SecondWeaponDetailColor = "W";

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModBurrowingNaturalWeapon modBurrowingNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModBurrowingNaturalWeapon;
            modBurrowingNaturalWeapon.BurrowingClaws = null;
            modBurrowingNaturalWeapon.Wielder = null;
            return modBurrowingNaturalWeapon;
        }

        public int GetDieSizeIncrease()
        {
            Debug.Entry(4, $"@ ModBurrowingNaturalWeapon.GetDieSizeIncrease", Indent: 4);
            if (Wielder == null && BurrowingClaws == null) return 2;
            Debug.Entry(4, $"Wielder not null, BurrowingClaws not null", Indent: 5);

            bool HasGigantism = Wielder.HasPart<GigantismPlus>();
            if (HasGigantism) return 1;
            DieRoll baseDamage = new(BurrowingClaws.GetClawsDamage(BurrowingClaws.Level));
            return baseDamage.RightValue - 2;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ ModBurrowingNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped})", Indent: 4);

            bool hasMods = !Object.GetPartsDescendedFrom<IMeleeModification>().Any();

            Object.RequirePart<DiggingTool>();
            Object.RequirePart<BurrowingClawsProperties>();

            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level);
            burrowingClawsProperties.WallBonusPercentage = BurrowingClaws.GetWallBonusPercentage(Level);

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            if (!hasMods) weapon.AdjustDamage(1);

            weapon.AdjustDamageDieSize(GetDieSizeIncrease());

            Render render = Object.Render;
            if (Object.GetIntProperty("CurrentNaturalWeaponBasePriority") > -naturalWeaponPriority)
            {
                Object.SetIntProperty("CurrentNaturalWeaponBasePriority", -naturalWeaponPriority);
                weapon.Skill = WeaponSkill;
                render.DisplayName = WeaponDisplayName;
                render.Tile = WeaponTile;
                render.ColorString = (render.ColorString == WeaponColorString) ? SecondWeaponColorString : WeaponColorString;
                render.DetailColor = (render.DetailColor == WeaponDetailColor) ? SecondWeaponDetailColor : WeaponDetailColor;
            }
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

            Object.SetStringProperty("TemporaryDefaultBehavior", "BurrowingClaws", false);
            Debug.Entry(4, $"x ModBurrowingNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped}) ]//", Indent: 4);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetModificationDisplayName(), naturalWeaponPriority);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public static string GetDescription()
        {
            string displayNameStrippedTitleCase = Grammar.MakeTitleCase(ColorUtility.StripFormatting(modificationDisplayName));
            string dieSizeIncrease = "up to +10";
            string wallBonusPenetration = "3x mutation level";
            string wallHitsRequired = "not more than 4";

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(displayNameStrippedTitleCase).Append(": ")
                .Append("This natural weapon gets ")
                .Append(dieSizeIncrease).Append(" to its die size").Append(", ")
                .Append(wallBonusPenetration).Append(" penetration vs. walls").Append(", and ")
                .Append("destroys walls after ").Append(wallHitsRequired).Append(" penetrating hits.");
            return Event.FinalizeString(stringBuilder);
        }

        public string GetInstanceDescription()
        {
            string displayNameStrippedTitleCase = Grammar.MakeTitleCase(GetModificationDisplayName());
            int dieSizeIncrease = GetDieSizeIncrease();
            string wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level).Signed();
            int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(Level, Wielder);

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(displayNameStrippedTitleCase).Append(": ")
                .Append("This natural weapon gets ");
            
            if (dieSizeIncrease > 0)
            {
                stringBuilder.Append(dieSizeIncrease.Signed()).Append(" to its die size").Append(wallHitsRequired > 0 ? ", " : " and ");
            }

            stringBuilder.Append(wallBonusPenetration).Append(" penetration vs. walls").Append(dieSizeIncrease > 0 ? ", " : " ")
                .Append(wallHitsRequired > 0 ? "and " : ".");
            
            if (wallHitsRequired > 0)
            {
                stringBuilder.Append("destroys walls after ").Append(wallHitsRequired).Append(" penetrating hits.");
            }
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModBurrowingNaturalWeapon : IMeleeModification

    [Serializable]
    public class ModCrystallineNaturalWeapon : IMeleeModification
    {
        public ModCrystallineNaturalWeapon()
        {
        }

        public ModCrystallineNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        private static readonly string modificationDisplayName = "{{crystallized|crystalline}}";
        public override string GetModificationDisplayName()
        {
            return modificationDisplayName;
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            Crystallinity = Wielder.GetPart<Crystallinity>();
            Level = Crystallinity.Level;
            return true;
        }

        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        private GameObject wielder = null;
        public GameObject Wielder { get => wielder; set => wielder = value; }

        private Crystallinity crystallinity = null;
        public Crystallinity Crystallinity { get => crystallinity; set => crystallinity = value; }

        private int level = 1;
        public int Level { get => level; set => level = value; }

        private const int naturalWeaponPriority = 40;
        public static int NaturalWeaponPriority => naturalWeaponPriority;

        public static string WeaponSkill = "ShortBlades";
        public static string WeaponDisplayName = "point";
        public static string WeaponTile = "Creatures/natural-weapon-claw.bmp";
        public static string WeaponColorString = "&b";
        public static string WeaponDetailColor = "B";
        public static string SecondWeaponColorString = "&B";
        public static string SecondWeaponDetailColor = "m";

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModCrystallineNaturalWeapon modCrystallineNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModCrystallineNaturalWeapon;
            modCrystallineNaturalWeapon.Crystallinity = null;
            modCrystallineNaturalWeapon.Wielder = null;
            return modCrystallineNaturalWeapon;
        }

        public int GetDieSizeIncrease()
        {
            Debug.Entry(4, $"@ ModCrystallineNaturalWeapon.GetDieSizeIncrease", Indent: 4);
            return 1;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ ModCrystallineNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped})", Indent: 4);

            bool hasMods = !Object.GetPartsDescendedFrom<IMeleeModification>().Any();

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            if (!hasMods) weapon.AdjustDamage(1);

            weapon.AdjustDamageDieSize(GetDieSizeIncrease());

            Render render = Object.Render;
            if (Object.GetIntProperty("CurrentNaturalWeaponBasePriority") > -naturalWeaponPriority)
            {
                Object.SetIntProperty("CurrentNaturalWeaponBasePriority", -naturalWeaponPriority);
                weapon.Skill = WeaponSkill;
                render.DisplayName = WeaponDisplayName;
                render.Tile = WeaponTile;
                render.ColorString = (render.ColorString == WeaponColorString) ? SecondWeaponColorString : WeaponColorString;
                render.DetailColor = (render.DetailColor == WeaponDetailColor) ? SecondWeaponDetailColor : WeaponDetailColor;
            }
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

            Object.SetStringProperty("TemporaryDefaultBehavior", "Crystallinity", false);
            Debug.Entry(4, $"x ModBurrowingNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped}) ]//", Indent: 4);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetModificationDisplayName(), naturalWeaponPriority);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public static string GetDescription()
        {
            string displayNameStrippedTitleCase = Grammar.MakeTitleCase(ColorUtility.StripFormatting(modificationDisplayName));
            string dieSizeIncrease = "+1";

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(displayNameStrippedTitleCase).Append(": ")
                .Append("This natural weapon gets ").Append(dieSizeIncrease).Append(" to its die size.");
            return Event.FinalizeString(stringBuilder);
        }

        public string GetInstanceDescription()
        {
            string displayNameStrippedTitleCase = Grammar.MakeTitleCase(GetModificationDisplayName());
            int dieSizeIncrease = GetDieSizeIncrease();

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(displayNameStrippedTitleCase).Append(": ")
                .Append("This natural weapon gets ").Append(dieSizeIncrease.Signed()).Append(" to its die size.");
            return Event.FinalizeString(stringBuilder);
        }
    }
}