using System;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class WeaponElongator : IScribedPart
    {
        public GameObject Wielder = null;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == StatChangeEvent.ID
                || ID == UnequippedEvent.ID
                || ID == EquippedEvent.ID;
        }

        private int appliedElongatedBonusCap = 0;

        public void ApplyElongatedBonusCap(MeleeWeapon Weapon, GameObject Wielder)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"ApplyElongatedBonusCap()", Indent: 3);
            Debug.Entry(4, "* if (Wielder.TryGetPart(out ElongatedPaws elongatedPaws))", Indent: 3);
            if (Wielder.TryGetPart(out ElongatedPaws elongatedPaws))
            {
                Debug.Entry(4, "+ elongatedPaws not null", Indent: 4);
                UnapplyElongatedBonusCap(Weapon);
                appliedElongatedBonusCap = elongatedPaws.ElongatedBonusDamage;
                Weapon.AdjustBonusCap(appliedElongatedBonusCap);
                Debug.Entry(4, $"New appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 4);
            }
            Debug.Entry(4, "x WeaponWatcher", $"ApplyElongatedBonusCap() >//", Indent: 3);
        }

        public void UnapplyElongatedBonusCap(MeleeWeapon Weapon)
        {
            Debug.Entry(4, $"@ WeaponWatcher", "UnapplyElongatedBonusCap()", Indent: 4);
            Debug.Entry(4, $"Old appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 4);
            Weapon.AdjustBonusCap(-appliedElongatedBonusCap);
            appliedElongatedBonusCap = 0;
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(EquippedEvent E)", Indent: 2);
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                Wielder = E.Actor;
                Debug.Entry(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                ApplyElongatedBonusCap(item.GetPart<MeleeWeapon>(), E.Actor);
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(EquippedEvent E) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(UnequippedEvent E)", Indent: 2);
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                Debug.Entry(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                UnapplyElongatedBonusCap(item.GetPart<MeleeWeapon>());
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(UnequippedEvent E) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(StatChangeEvent E)", Indent: 2);
            Debug.Entry(4, $"E.Name: \"{E.Name}\" | E.Object: \"{E.Object.ShortDisplayNameStripped}\"", Indent: 2);
            if (Wielder != null) Debug.Entry(4, $"Wielder: \"{Wielder.ShortDisplayNameStripped}\"", Indent: 2);
            if (E.Name == "Strength" && E.Object == Wielder)
            {
                Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(StatChangeEvent {E.Name})", Indent: 2);
                ApplyElongatedBonusCap(ParentObject.GetPart<MeleeWeapon>(), E.Object);
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(StatChangeEvent {E.Name}) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class WeaponElongator : IPart

} //!-- namespace XRL.World.Parts
