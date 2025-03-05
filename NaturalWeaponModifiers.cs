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
using static XRL.World.IManagedDefaultNaturalWeapon;

namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon
    {
        public abstract class INaturalWeapon : IPart
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

        public abstract INaturalWeapon GetNaturalWeapon();

        public abstract string GetNaturalWeaponMod();

        public abstract bool CalculateNaturalWeaponDamageDieCount(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieSize(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageBonus(int Level = 1);

        public abstract bool CalculateNaturalWeaponHitBonus(int Level = 1);
    }
}

namespace XRL.World.Parts.Mutation
{
    public class BaseManagedDefaultEquipmentMutation : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon 
    {
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {

        }
        
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

        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

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
    public abstract class ModNaturalWeaponBase : IMeleeModification
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
        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }
        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            return base.BeingAppliedBy(obj, who);
        }

        private GameObject wielder = null;
        public GameObject Wielder { get => wielder; set => wielder = value; }

        public override void ApplyModification(GameObject Object)
        {
            Render render = Object.Render;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);

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
    }

    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : ModNaturalWeaponBase
        where T : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon, new()
    {
        public ModNaturalWeaponBase()
        {
        }

        public ModNaturalWeaponBase(int Tier)
            : base(Tier)
        {
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            AssigningMutation = Wielder.GetPart<T>();
            Level = AssigningMutation.Level;
            NaturalWeapon = AssigningMutation.GetNaturalWeapon();
            return base.BeingAppliedBy(obj, who);
        }

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
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetDamageDieCount()", $"{NaturalWeapon.GetDamageDieCount()-1}", Indent: 6);
            // base damage die count is 1
            // example: mutation calculates die count should be 6d
            //          this deducts 1, adding 5 to the existing 1
            return Math.Max(0, NaturalWeapon.GetDamageDieCount() - 1);
        }
        public virtual int GetDamageDieSize()
        {
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetDamageDieSize()", $"{NaturalWeapon.GetDamageDieSize()-2}", Indent: 6);
            // base damage die size is 2
            // example: mutation calculates die size should be d5
            //          this deducts 2, adding 3 to the existing 2
            return Math.Max(0, NaturalWeapon.GetDamageDieSize() - 2);
        }

        public virtual int GetDamageBonus()
        {
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetDamageBonus()", $"{NaturalWeapon.GetDamageBonus()}", Indent: 6);
            // base damage bonus is 0
            return NaturalWeapon.GetDamageBonus();
        }

        public virtual int GetHitBonus()
        {
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetHitBonus()", $"{NaturalWeapon.GetHitBonus()}", Indent: 6);
            // base hit bonus is 0
            return NaturalWeapon.GetHitBonus();
        }

        public virtual void ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon)
        {
            Debug.Entry(4, $"@ ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>", "ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 5);
            Debug.Entry(4, $"{AssigningMutation.GetMutationClass()} Mutation Level: {Level}", Indent: 6);
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-2 into 1d2+0

            if (Object.GetNaturalWeaponModsCount() <= 1) weapon.AdjustDamage(1);

            string[] cats = new string[] { "Damage", "Combat", "Render" };
            weapon.Trace(4, "Before", cats, Indent: 5);
            weapon.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();
            weapon.Trace(4, "After", cats, Indent: 5);
            Debug.Entry(4, $"x ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>", "ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 5);
        }

        public virtual int ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon, int NounPriority)
        {
            Debug.Entry(4, $"@ ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>", "ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon, int NounPriority)", Indent: 5);
            Debug.Entry(4, $"{AssigningMutation.GetMutationClass()} Mutation Level: {Level}", Indent: 6);
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
            Debug.Entry(4, $"x ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>", "ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon, int NounPriority)", Indent: 5);
            return NounPriority;
        }

        public override void ApplyModification(GameObject Object)
        {
            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation.GetMutationClass(), false);

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
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            return base.HandleEvent(E);
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
            string descriptionName = ParentObject.GetNaturalWeaponModsCount() <= 1 ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            int damageBonus = 3 + GetDamageBonus();
            List<List<string>> list = new();
            string text = "weapon";
            if (part != null && ParentObject.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                list.Add(new List<string> { "have", $"{damageBonus.Signed()} damage" });
                if (GetDamageDieCount() != 0)
                {
                    list.Add(new List<string> { "have", $"{GetHitBonus().Signed()} to {ParentObject.theirs} damage die count" });
                }
                if (GetHitBonus() != 0)
                {
                    list.Add(new List<string> { "have", $"a {GetHitBonus().Signed()} {(GetHitBonus() >= 0 ? "bonus" : "penalty")} to hit" });
                }
                if (part.Skill == "Cudgel")
                {
                    list.Add(new List<string> { null, "twice as effective when you Slam with " + ParentObject.them });
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
                return $"{descriptionName}: " + $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} has {ParentObject.theirs} damage die count determined by {ParentObject.theirs} wielder's size.";
            }
            List<string> list2 = new();
            foreach (List<string> item in list)
            {
                list2.Add(GetProcessedItem(item, second: false, list, ParentObject));
            }
            return $"{descriptionName}: " + (ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text)) + " " + Grammar.MakeAndList(list2) + ".";
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

        public override string GetInstanceDescription()
        {
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string text = "weapon";
            string descriptionName = ParentObject.GetNaturalWeaponModsCount() <= 1 ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            string description = $"{descriptionName}: ";
            description += $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} ";
            if (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"gets {GetDamageDieSize().Signed()} to {ParentObject.theirs} die size, and ";
            }
            string bonusDamageIncrease = "";
            if (GetDamageBonus() != 0) bonusDamageIncrease = $"({GetDamageBonus()}) ";
            description += $"gets half {ParentObject.theirs} wielder's Strength Modifier {bonusDamageIncrease}added as bonus damage.";
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>

    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
    {
        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModBurrowingNaturalWeapon modBurrowingNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModBurrowingNaturalWeapon;
            modBurrowingNaturalWeapon.AssigningMutation = null;
            modBurrowingNaturalWeapon.Wielder = null;
            modBurrowingNaturalWeapon.NaturalWeapon = null;
            return modBurrowingNaturalWeapon;
        }

        public override void ApplyModification(GameObject Object)
        {
            ApplyGenericChanges(Object, NaturalWeapon);

            ApplyPriorityChanges(Object, NaturalWeapon, NaturalWeapon.GetNounPriority());

            Object.RequirePart<DiggingTool>();
            Object.RequirePart<BurrowingClawsProperties>();

            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level);
            burrowingClawsProperties.WallBonusPercentage = BurrowingClaws.GetWallBonusPercentage(Level);

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

        public new string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = ParentObject.GetNaturalWeaponModsCount() <= 1 ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            string wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level).Signed();
            int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(Level, Wielder);

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append($"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} gets ");
            
            if (dieSizeIncrease > 0)
            {
                stringBuilder.Append(dieSizeIncrease.Signed()).Append($" to {ParentObject.theirs} die size").Append(wallHitsRequired > 0 ? ", " : " and ");
            }
            stringBuilder.Append(wallBonusPenetration).Append(" penetration vs. walls").Append(dieSizeIncrease > 0 ? ", " : " ")
                .Append(wallHitsRequired > 0 ? "and " : ".");
            
            if (wallHitsRequired > 0)
            {
                stringBuilder.Append("destroys walls after ").Append(wallHitsRequired).Append(" penetrating hits.");
            }
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>

    [Serializable]
    public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
    {
        public ModCrystallineNaturalWeapon()
        {
        }

        public ModCrystallineNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModCrystallineNaturalWeapon modCrystallineNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModCrystallineNaturalWeapon;
            modCrystallineNaturalWeapon.AssigningMutation = null;
            modCrystallineNaturalWeapon.Wielder = null;
            modCrystallineNaturalWeapon.NaturalWeapon = null;
            return modCrystallineNaturalWeapon;
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

        public new string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = ParentObject.GetNaturalWeaponModsCount() <= 1 ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append($"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} gets ").Append(dieSizeIncrease.Signed()).Append($" to {ParentObject.theirs} die size.");
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}