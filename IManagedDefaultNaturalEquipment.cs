using System;
using System.Collections.Generic;
using XRL.World.Parts;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

namespace XRL.World
{
    public interface IManagedDefaultNaturalEquipment<T> where T : IPart, IManagedDefaultNaturalEquipment<T>, new ()
    {
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }

        public abstract bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod, string Parts);

        public abstract bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<T> NaturalEquipmentMod, string Props);

        public abstract int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract bool UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract bool ProcessNaturalEquipment(Body body);

        public virtual bool WantEvent(int ID, int cascade)
        {
            return ID == BodyPartsUpdatedEvent.ID;
        }

        public abstract bool HandEvent(BodyPartsUpdatedEvent E);
        public abstract void OnBodyPartsUpdated(Body body);

    }
}