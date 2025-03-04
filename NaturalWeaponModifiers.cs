using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.IDefaultNaturalWeaponManager;

namespace XRL.World
{
    public interface IDefaultNaturalWeaponManager
    {
        public class INaturalWeapon : IPart
        {
            public int DamageDieCount { get; set; }
            public int DamageDieSize { get; set; }
            public int DamageBonus { get; set; }
            public int HitBonus { get; set; }

            public int ModPriority;
            private int AdjectivePriority => ModPriority;
            private int NounPriority => -ModPriority;

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

            public int GetDamageDieCount()
            {
                return DamageDieCount;
            }
            public int GetDamageDieSize()
            {
                return DamageDieSize;
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
                return AdjectiveColor;
            }
            public string GetColoredAdjective()
            {
                return "{{" + GetAdjectiveColor() + "|" + GetAdjective() + "}}";
            }
        }

        public abstract string GetNaturalWeaponMod();

        public abstract bool CalculateNaturalWeaponDamageDieCount(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieSize(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageBonus(int Level = 1);

        public abstract bool CalculateNaturalWeaponHitBonus(int Level = 1);
    }
}

namespace XRL.World.Parts.Mutation
{
    public class BaseManagedDefaultEquipmentMutation : BaseDefaultEquipmentMutation, IDefaultNaturalWeaponManager
    {

        public INaturalWeapon NaturalWeapon = new()
        {
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

            ModPriority = 0,
            Noun = "fist",
            Tile = "Creatures/natural-weapon-fist.bmp",
        };

        public virtual string GetNaturalWeaponMod()
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon";
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1)
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponDamageDieCount(NewLevel);
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            CalculateNaturalWeaponDamageBonus(NewLevel);
            CalculateNaturalWeaponHitBonus(NewLevel);
            return base.ChangeLevel(NewLevel);
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

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            AssigningMutation = Wielder.GetPart<T>();
            Debug.Entry(4, AssigningMutation.GetMutationClass(), Indent: 12);
            Level = AssigningMutation.Level;
            NaturalWeapon = AssigningMutation.NaturalWeapon;
            Debug.Entry(4, NaturalWeapon.GetAdjective(), Indent: 12);
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
            // example: mutation calculates die count should be 6d
            //          this deducts 1, adding 5 to the existing 1
            return Math.Max(0, NaturalWeapon.GetDamageDieCount() - 1);
        }
        public virtual int GetDamageDieSize()
        {
            // base damage die size is 2
            // example: mutation calculates die size should be d5
            //          this deducts 2, adding 3 to the existing 2
            return Math.Max(0, NaturalWeapon.GetDamageDieSize() - 2);
        }

        public virtual int GetDamageBonus()
        {
            // base damage bonus is 0
            return NaturalWeapon.GetDamageBonus();
        }

        public virtual int GetHitBonus()
        {
            // base hit bonus is 0
            return NaturalWeapon.GetHitBonus();
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public virtual void ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon)
        {
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-2 into 1d2+0
            if (!Object.HasNaturalWeaponMods()) weapon.AdjustDamage(1);

            weapon.BaseDamage.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() > 0) weapon.HitBonus = GetHitBonus();
        }

        public virtual int ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon, int NounPriority)
        {
            Render render = Object.Render;
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();
            if (Object.GetIntProperty("CurrentNaturalWeaponBasePriority") > NounPriority)
            {
                Object.SetIntProperty("CurrentNaturalWeaponBasePriority", NounPriority);
                weapon.Skill = NaturalWeapon.Skill;
                render.DisplayName = NaturalWeapon.Noun;
                render.Tile = NaturalWeapon.Tile;
                render.ColorString = (render.ColorString == NaturalWeapon.RenderColorString) ? NaturalWeapon.SecondRenderColorString : NaturalWeapon.RenderColorString;
                render.DetailColor = (render.DetailColor == NaturalWeapon.RenderDetailColor) ? NaturalWeapon.SecondRenderDetailColor : NaturalWeapon.RenderDetailColor;
            }
            return NounPriority;
        }

        public override void ApplyModification(GameObject Object)
        {
            Render render = Object.Render;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation.GetMutationClass(), false);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetShortDescriptionEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public virtual string GetInstanceDescription()
        {
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            return $"{descriptionName}: This weapon has been constructed by modifications.";
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

        public override void ApplyModification(GameObject Object)
        {
            /* Old code for applying the weapon mod.
             * 
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
            */

            ApplyGenericChanges(Object, NaturalWeapon);

            ApplyPriorityChanges(Object, NaturalWeapon, NaturalWeapon.GetNounPriority());
            
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
                E.AddAdjective(NaturalWeapon.GetColoredAdjective(), NaturalWeapon.GetAdjectivePriority());
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
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

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalWeaponBase<GigantismPlus>

    [Serializable]
    public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
    {
        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            ApplyGenericChanges(Object, NaturalWeapon);

            ApplyPriorityChanges(Object, NaturalWeapon, NaturalWeapon.GetNounPriority());

            base.ApplyModification(Object);
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
                E.AddAdjective(NaturalWeapon.GetColoredAdjective(), NaturalWeapon.GetAdjectivePriority());
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

        public override string GetInstanceDescription()
        {
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            string description = $"{descriptionName}: This weapon ";
            if (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"has its die size increased by {NaturalWeapon.GetDamageDieSize()}, and ";
            }
            string bonusDamageIncrease = "";
            if (NaturalWeapon.GetDamageBonus() != 0) bonusDamageIncrease = $"({NaturalWeapon.GetDamageBonus()}) ";
            description += $"gets half its wielder's Strength Modifier {bonusDamageIncrease}added as Bonus Damage.";
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>

    [Serializable]
    public class ModNaturalWeaponBase<T,I> : ModNaturalWeaponBase<BaseManagedDefaultEquipmentMutation> where T : BaseDefaultEquipmentMutation, new() where I : IDefaultNaturalWeaponManager
    {
        public ModNaturalWeaponBase()
        {
        }

        public ModNaturalWeaponBase(int Tier)
            : base(Tier)
        {
        }
    }

    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<ManagedBurrowingClaws, IDefaultNaturalWeaponManager>
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