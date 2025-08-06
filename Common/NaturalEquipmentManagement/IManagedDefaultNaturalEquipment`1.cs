using System;
using System.Collections.Generic;

using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World
{
    public interface IManagedDefaultNaturalEquipment<T> 
        : IManagedDefaultNaturalEquipment
        where T 
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        public abstract NaturalEquipmentManager NaturalEquipmentManager { get; }

        public abstract int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract List<ModNaturalEquipment<T>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<T>> Filter = null);

        public abstract ModNaturalEquipment<T> UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract List<ModNaturalEquipment<T>> UpdateNaturalEquipmentMods(List<ModNaturalEquipment<T>> NaturalEquipmentMods, int Level = 1);
    }
}