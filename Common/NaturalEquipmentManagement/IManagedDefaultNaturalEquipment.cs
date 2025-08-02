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

        public virtual List<ModNaturalEquipment<T>> NaturalEquipmentMods => GetNaturalEquipmentMods();

        public abstract bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod, string Parts);

        public abstract bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<T> NaturalEquipmentMod, string Props);

        public abstract int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod = null, int Level = 1);

        public abstract List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<T> NaturalEquipmentMod);

        public abstract List<ModNaturalEquipment<T>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<T>> Filter = null, NaturalEquipmentManager NewManager = null);

        public abstract ModNaturalEquipment<T> UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1);

        public abstract List<ModNaturalEquipment<T>> UpdateNaturalEquipmentMods(List<ModNaturalEquipment<T>> NaturalEquipmentMods, int Level = 1);
    }

    public interface IManagedDefaultNaturalEquipment 
        : IModEventHandler<GetNaturalEquipmentOperatorsEvent>
        , IModEventHandler<BeforeUpdateBodyPartsEvent>
        , IModEventHandler<BodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
        , IModEventHandler<GetPrioritisedNaturalEquipmentModsEvent>
        , IModEventHandler<BeforeManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<ManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<AfterManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<BeforeRapidAdvancementEvent>
        , IModEventHandler<AfterRapidAdvancementEvent>
    {
        public int Level { get; set; }

        public abstract void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Operator, BodyPart TargetBodyPart);
    }
}