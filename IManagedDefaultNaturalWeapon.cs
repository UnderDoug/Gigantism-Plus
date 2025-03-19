using System;
using System.Collections.Generic;
using XRL.World.Parts;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon 
    {
        
        public abstract string GetNaturalWeaponModName(NaturalWeaponSubpart NaturalWeaponSubpart, bool Managed = true);
        public abstract ModNaturalWeaponBase<TPart> GetNaturalWeaponMod<TPart>(NaturalWeaponSubpart NaturalWeaponSubpart)
            where TPart : IPart, IManagedDefaultNaturalWeapon, new();
        public abstract bool CalculateNaturalWeaponLevel(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieCount(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieSize(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract bool CalculateNaturalWeaponHitBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart NaturalWeaponSubpart, string Parts);

        public abstract bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart NaturalWeaponSubpart, string Props);

        public abstract int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1);

        public abstract List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart NaturalWeaponSubpart);

        public abstract Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart NaturalWeaponSubpart);

        public abstract Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart NaturalWeaponSubpart);

        public abstract string GetNaturalWeaponEquipmentFrameColors(NaturalWeaponSubpart NaturalWeaponSubpart);

        // These should allow a base cybernetics part to be wrappered into having natural weapon modifiers included
        public abstract void OnDecorateDefaultEquipment(Body body);
        public abstract void OnRegenerateDefaultEquipment(Body body);
    }
}