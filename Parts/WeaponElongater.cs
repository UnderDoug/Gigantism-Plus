using System;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class WeaponElongator : IScribedPart
    {
        private GameObject Wielder = null;

        private ElongatedPaws ElongatedPaws = null;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == StatChangeEvent.ID
                || ID == UnequippedEvent.ID
                || ID == EquippedEvent.ID;
        }

        private int appliedElongatedBonusCap = 0;

        public void ApplyElongatedBonusCap(MeleeWeapon Weapon, ElongatedPaws elongatedPaws = null)
        {
            elongatedPaws ??= ElongatedPaws;
            Debug.Entry(4, $"* {nameof(ApplyElongatedBonusCap)}(MeleeWeapon Weapon)", Indent: 3);
            UnapplyElongatedBonusCap(Weapon);
            appliedElongatedBonusCap = elongatedPaws.ElongatedBonusDamage;
            Weapon.AdjustBonusCap(appliedElongatedBonusCap);
            Debug.LoopItem(4, $"New appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 4);
            Debug.Entry(4, $"x {nameof(ApplyElongatedBonusCap)}(MeleeWeapon Weapon) *//", Indent: 3);
        }

        public void UnapplyElongatedBonusCap(MeleeWeapon Weapon)
        {
            Debug.Entry(4, $"* {nameof(UnapplyElongatedBonusCap)}(MeleeWeapon Weapon)", Indent: 4);
            Debug.LoopItem(4, $"Old appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 5);
            Weapon.AdjustBonusCap(-appliedElongatedBonusCap);
            appliedElongatedBonusCap = 0;
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                Wielder = E.Actor;
                if (Wielder.TryGetPart(out ElongatedPaws elongatedPaws))
                {
                    Debug.Entry(4, $"@ {nameof(WeaponElongator)}.{nameof(HandleEvent)}({nameof(EquippedEvent)} E)", Indent: 2);
                    ElongatedPaws = elongatedPaws;
                    Debug.LoopItem(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                    ApplyElongatedBonusCap(item.GetPart<MeleeWeapon>());
                    Debug.Entry(4, $"x {nameof(WeaponElongator)}", $"{nameof(HandleEvent)}({nameof(EquippedEvent)} E) @//", Indent: 2);
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {
            GameObject item = E.Item;
            if (item == ParentObject && Wielder != null && ElongatedPaws != null)
            {
                Debug.Entry(4, $"@ {nameof(WeaponElongator)}.{nameof(HandleEvent)}({nameof(UnequippedEvent)} E)", Indent: 2);
                Debug.LoopItem(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                UnapplyElongatedBonusCap(item.GetPart<MeleeWeapon>());
                Debug.Entry(4, $"x {nameof(WeaponElongator)}", $"{nameof(HandleEvent)}({nameof(UnequippedEvent)} E) @//", Indent: 2);

                ElongatedPaws = null;
                Wielder = null;
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Object == Wielder && ElongatedPaws != null && E.Name == "Strength")
            {
                Debug.Entry(4, $"@ {nameof(WeaponElongator)}.{nameof(HandleEvent)}({nameof(StatChangeEvent)} E: {E.Name})", Indent: 2);
                Debug.LoopItem(4, $" E.Object: \"{E.Object.ShortDisplayNameStripped}\"", Indent: 2);
                ApplyElongatedBonusCap(ParentObject.GetPart<MeleeWeapon>());
                Debug.Entry(4, $"x {nameof(WeaponElongator)}", $"{nameof(HandleEvent)}({nameof(StatChangeEvent)} E) @//", Indent: 2);
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class WeaponElongator : IPart

} //!-- namespace XRL.World.Parts
