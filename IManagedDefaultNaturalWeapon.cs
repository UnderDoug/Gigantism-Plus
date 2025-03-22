using System;
using System.Collections.Generic;
using XRL.World.Parts;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon<T> where T : IPart, IManagedDefaultNaturalWeapon<T>, new ()
    {
        public Dictionary<string, NaturalWeaponSubpart<T>> NaturalWeaponSubparts { get; set; }
        public NaturalWeaponSubpart<T> NaturalWeaponSubpart { get; set; }

        public virtual NaturalWeaponSubpart<T> GetNaturalWeaponSubpart(
            string Type = "",
            GameObject Object = null,
            BodyPart BodyPart = null)
        {
            if (Type != "")
            {
                if (Type == NaturalWeaponSubpart?.Type)
                    return NaturalWeaponSubpart;
                if (NaturalWeaponSubparts.ContainsKey(Type))
                    return NaturalWeaponSubparts[Type];
            }
            if (Object?.Equipped?.Body != null)
            {
                foreach (BodyPart part in Object.Equipped.Body.LoopParts())
                {
                    if (Object.IsDefaultEquipmentOf(part) || (part.Equipped == Object && Object.HasPart<NaturalEquipment>()))
                    {
                        Type = part.Type;
                        if (Type == NaturalWeaponSubpart?.Type)
                            return NaturalWeaponSubpart;
                        if (NaturalWeaponSubparts.ContainsKey(Type))
                            return NaturalWeaponSubparts[Type];
                    }
                }
            }
            if (BodyPart != null)
            {
                Type = BodyPart.Type;
                if (Type == NaturalWeaponSubpart?.Type)
                    return NaturalWeaponSubpart;
                if (NaturalWeaponSubparts.ContainsKey(Type))
                    return NaturalWeaponSubparts[Type];
            }
            return null;
        }

        public abstract string GetNaturalWeaponModName(NaturalWeaponSubpart<T> Subpart, bool Managed = true);
        public abstract ModNaturalWeaponBase<T> GetNaturalWeaponMod(NaturalWeaponSubpart<T> Subpart, bool Managed = true);

        public abstract bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<T> Subpart, string Parts);

        public abstract bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<T> Subpart, string Props);

        public abstract int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart<T> NaturalWeaponSubpart);

        public abstract Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart);

        public abstract Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart);

        public abstract bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<T> Subpart, int Level = 1);

        public abstract bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false);
        public abstract bool UnprocessNaturalWeaponSubparts(Body body);

        // These should allow a base cybernetics part to be wrappered into having natural weapon modifiers included
        public abstract void OnDecorateDefaultEquipment(Body body);
        public abstract void OnRegenerateDefaultEquipment(Body body);
    }
}