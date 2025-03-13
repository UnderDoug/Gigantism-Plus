﻿using System;
using XRL.World.Parts.Mutation;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.IManagedDefaultNaturalWeapon;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : IMeleeModification
        where T : IPart, IManagedDefaultNaturalWeapon, new()
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
            Wielder ??= who;
            AssigningPart ??= Wielder.GetPart<T>();
            NaturalWeapon ??= AssigningPart.GetNaturalWeapon();
            Level = NaturalWeapon.GetLevel();
            return base.BeingAppliedBy(obj, who);
        }

        private GameObject _wielder = null;
        public GameObject Wielder { get => _wielder; set => _wielder = value; }

        private T _assigningPart = null;
        public T AssigningPart { get => _assigningPart; set => _assigningPart = value; }

        private int _level = 1;
        public int Level { get => _level; set => _level = value; }

        private INaturalWeapon _naturalWeapon;
        public INaturalWeapon NaturalWeapon { get => _naturalWeapon; set => _naturalWeapon = value; }

        public const string CURRENT_PRIORITY = "CurrentNaturalWeaponBasePriority";

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalWeaponBase<T> modNaturalWeaponBase = base.DeepCopy(Parent, MapInv) as ModNaturalWeaponBase<T>;
            modNaturalWeaponBase.AssigningPart = null;
            modNaturalWeaponBase.Wielder = null;
            modNaturalWeaponBase.NaturalWeapon = null;
            return modNaturalWeaponBase;
        }

        public virtual int GetDamageDieCount()
        {
            return Math.Max(0, NaturalWeapon.GetDamageDieCount());
        }
        public virtual int GetDamageDieSize()
        {
            return Math.Max(0, NaturalWeapon.GetDamageDieSize());
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

        public virtual void ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon, string InstanceDescription)
        {
            Debug.Entry(4, $"* {nameof(ApplyGenericChanges)}(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}; Level: {Level}", Indent: 5);

            Object.RequirePart<NaturalWeaponDescriber>();
            Debug.Entry(4, "? if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))", Indent: 5);
            if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))
            {
                Debug.Entry(4, "+ NaturalWeaponDescriber is Present", Indent: 6);
                if (!Object.HasNaturalWeaponMods())
                {
                    Debug.Entry(4, "No NaturalWeaponMods", "Resetting Short Description", Indent: 6);
                    NaturalWeaponDescriber.ResetShortDescription();
                }
                else
                {
                    Debug.Entry(4, "Have NaturalWeaponMods", "Continuting to accumulate descripiptions", Indent: 6);
                }
                NaturalWeaponDescriber.AddShortDescriptionEntry(NaturalWeapon.GetPriority(), InstanceDescription);
            }
            else
            {
                Debug.Entry(4, "- NaturalWeaponDescriber not Present", Indent: 6);
            }
            Debug.Entry(4, $"x if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber)) ?//", Indent: 5);

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-2 into 1d2+0
            if (!Object.HasNaturalWeaponMods()) weapon.AdjustDamage(1);

            string[] vomitCats = new string[] { "Damage" };
            weapon.Vomit(4, "Generic, Before", vomitCats, Indent: 4);

            Debug.Entry(4, $"NaturalWeapon Adjustments", Indent: 4);
            Debug.LoopItem(4, $"DamageDieCount", $"{GetDamageDieCount().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageDieSize", $"{GetDamageDieSize().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetDamageBonus", $"{GetDamageBonus().Signed()}", Indent: 5);
            Debug.LoopItem(4, $"GetHitBonus", $"{GetHitBonus().Signed()}", Indent: 5);

            weapon.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();
            weapon.Vomit(4, "Generic, After", vomitCats, Indent: 4);
            Debug.Entry(4, $"* {nameof(ApplyGenericChanges)}(GameObject Object, INaturalWeapon NaturalWeapon) *//", Indent: 4);
        }

        public virtual int ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon, int NounPriority)
        {
            Debug.Entry(4, $"* {nameof(ApplyPriorityChanges)}(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}", Indent: 5);

            Render render = Object.Render;
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            string[] vomitCats = new string[] { "Combat", "Render" };

            int CurrentPriority = Object.GetIntProperty(CURRENT_PRIORITY);

            Debug.Entry(4, $"? if (CurrentPriority > NounPriority)", Indent: 5);
            if (CurrentPriority > NounPriority)
            {
                Debug.Entry(4, $"+ CurrentPriority ({CurrentPriority}) > NounPriority ({NounPriority})", Indent: 6);

                Object.SetIntProperty(CURRENT_PRIORITY, NounPriority);
                
                weapon.Vomit(4, "Priority, Before", vomitCats, Indent: 6);

                Debug.Entry(4, $"NaturalWeapon Attribute", Indent: 6);
                Debug.LoopItem(4, $"NaturalWeapon.Skill", $"{NaturalWeapon.Skill}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.Noun", $"{NaturalWeapon.Noun}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.Tile", $"{NaturalWeapon.Tile}", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.ColorString", $"{NaturalWeapon.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SecondColorString", $"{NaturalWeapon.SecondColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.DetailColor", $"{NaturalWeapon.ColorString} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SecondDetailColor", $"{NaturalWeapon.SecondDetailColor} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.SwingSound", $"{NaturalWeapon.SwingSound} ", Indent: 7);
                Debug.LoopItem(4, $"NaturalWeapon.BlockedSound", $"{NaturalWeapon.BlockedSound} ", Indent: 7);

                weapon.Skill = NaturalWeapon.Skill ?? weapon.Skill;
                render.DisplayName = NaturalWeapon.Noun ?? render.DisplayName;
                render.Tile = NaturalWeapon.Tile ?? render.Tile;

                render.ColorString = 
                    (render.ColorString == NaturalWeapon.ColorString) 
                    ? NaturalWeapon.SecondColorString 
                    : NaturalWeapon.ColorString;

                render.DetailColor = 
                    (render.DetailColor == NaturalWeapon.DetailColor) 
                    ? NaturalWeapon.SecondDetailColor 
                    : NaturalWeapon.DetailColor;

                Object.SetSwingSound(NaturalWeapon.SwingSound);
                Object.SetBlockedSound(NaturalWeapon.BlockedSound);

                weapon.Vomit(4, "Priority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4, $"- CurrentPriority ({CurrentPriority}) <= NounPriority ({NounPriority})", Indent: 6);
                weapon.Vomit(4, "Priority, Unchanged", vomitCats, Indent: 6);
            }
            Debug.Entry(4, $"x {nameof(ApplyPriorityChanges)}(GameObject Object, INaturalWeapon NaturalWeapon) *//", Indent: 4);
            return NounPriority;
        }

        public override void ApplyModification(GameObject Object)
        {
            Render render = Object.Render;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);
            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningPart.Name, false);
            Object.ModIntProperty("ModNaturalWeaponCount", 1);
            base.ApplyModification(Object);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }

        public virtual string GetInstanceDescription()
        {
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            return $"{descriptionName}: This weapon has been constructed by modifications.";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class ModNaturalWeaponBase : IMeleeModification
}