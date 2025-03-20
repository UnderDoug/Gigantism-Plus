using System;
using System.Collections.Generic;
using XRL.World.Parts;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XRL.World.Anatomy;

namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon<T> where T : IPart, IManagedDefaultNaturalWeapon<T>, new ()
    {
        public abstract NaturalWeaponSubpart<T> GetNaturalWeaponSubpart(
            string Type = "", 
            GameObject RequestingObject = null,
            BodyPart BodyPart = null);
        public abstract string GetNaturalWeaponModName(NaturalWeaponSubpart<T> Subpart, bool Managed = true);
        public abstract ModNaturalWeaponBase<T> GetNaturalWeaponMod(NaturalWeaponSubpart<T> Subpart);

        public abstract bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<T> Subpart, string Parts);

        public abstract bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<T> Subpart, string Props);

        public abstract int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        // These should allow a base cybernetics part to be wrappered into having natural weapon modifiers included
        public abstract void OnDecorateDefaultEquipment(Body body);
        public abstract void OnRegenerateDefaultEquipment(Body body);
    }
}