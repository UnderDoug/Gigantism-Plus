using System;
using XRL.World.Parts.Mutation;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.IManagedDefaultNaturalWeapon;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class ModNaturalWeaponBase<T> : IMeleeModification
        where T : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon, new()
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
            Wielder = who;
            AssigningMutation = Wielder.GetPart<T>();
            Level = AssigningMutation.Level;
            NaturalWeapon = AssigningMutation.GetNaturalWeapon();
            return base.BeingAppliedBy(obj, who);
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
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetDamageDieCount()", $"{NaturalWeapon.GetDamageDieCount()}", Indent: 6);
            return Math.Max(0, NaturalWeapon.GetDamageDieCount());
        }
        public virtual int GetDamageDieSize()
        {
            Debug.Entry(4, $"ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>; Level: {Level}", Indent: 5);
            Debug.Entry(4, "NaturalWeapon.GetDamageDieSize()", $"{NaturalWeapon.GetDamageDieSize()}", Indent: 6);
            return Math.Max(0, NaturalWeapon.GetDamageDieSize());
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

        public virtual void ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon, string InstanceDescription)
        {
            Debug.Entry(4, $"@ ModNaturalWeaponBase<{AssigningMutation.GetMutationClass()}>", "ApplyGenericChanges(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 5);
            Debug.Entry(4, $"{AssigningMutation.GetMutationClass()} Mutation Level: {Level}", Indent: 6);

            Object.RequirePart<NaturalWeaponDescriber>();
            Debug.Entry(4, "* if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))", Indent: 6);
            if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber))
            {
                Debug.Entry(4, "+ NaturalWeaponDescriber is Present", Indent: 7);
                if (!Object.HasNaturalWeaponMods())
                {
                    Debug.Entry(4, "No NaturalWeaponMods", "Resetting Short Description", Indent: 7);
                    NaturalWeaponDescriber.ResetShortDescription();
                }
                else
                {
                    Debug.Entry(4, "Have NaturalWeaponMods", "Continuting to accumulate descripiptions", Indent: 7);
                }
                NaturalWeaponDescriber.AddShortDescriptionEntry(NaturalWeapon.GetPriority(), InstanceDescription);
            }
            else
            {
                Debug.Entry(4, " NaturalWeaponDescriber not Present", Indent: 7);
            }
            Debug.Entry(4, "x if (Object.TryGetPart(out NaturalWeaponDescriber NaturalWeaponDescriber)) ?//", Indent: 6);

            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            // if no other mods, bump the damage penalty of -1 off.
            // 1d2-2 into 1d2+0
            if (!Object.HasNaturalWeaponMods()) weapon.AdjustDamage(1);

            string[] traceCats = new string[] { "Damage", "Combat", "Render" };
            weapon.Trace(4, "Before", traceCats, Indent: 5);
            weapon.AdjustDieCount(GetDamageDieCount());
            weapon.AdjustDamageDieSize(GetDamageDieSize());
            weapon.AdjustDamage(GetDamageBonus());
            if (GetHitBonus() != 0) weapon.HitBonus += GetHitBonus();
            weapon.Trace(4, "After", traceCats, Indent: 5);
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
            Render render = Object.Render;
            if (TryGetTilePath(BuildCustomTilePath(ParentObject.DisplayNameOnly), out string tilePath)) render.Tile = tilePath;

            Object.SetIntProperty("ShowAsPhysicalFeature", 1);
            Object.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation.GetMutationClass(), false);
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