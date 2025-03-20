using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentMutation<T> : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon<T> 
        where T : BaseManagedDefaultEquipmentMutation<T>, new()
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalWeaponSubpart for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalWeaponSubpart.Type).
        public Dictionary<string, NaturalWeaponSubpart<T>> NaturalWeaponSubparts = new();
        public NaturalWeaponSubpart<T> NaturalWeaponSubpart { get; set; }

        public BaseManagedDefaultEquipmentMutation()
        {
        }

        // Takes an existing NaturalWeaponSubparts Dictionary
        public BaseManagedDefaultEquipmentMutation(Dictionary<string, NaturalWeaponSubpart<T>> naturalWeaponSubparts, T NewParent)
            : this()
        {
            Dictionary<string, NaturalWeaponSubpart<T>> NewNaturalWeaponSubparts = new();
            foreach ((string Part, NaturalWeaponSubpart<T> Subpart) in naturalWeaponSubparts)
            {
                NaturalWeaponSubpart<T> subpart = new(Subpart, NewParent);
                NewNaturalWeaponSubparts.Add(Part, subpart);
            }
            NaturalWeaponSubparts = NewNaturalWeaponSubparts;
        }

        public BaseManagedDefaultEquipmentMutation(NaturalWeaponSubpart<T> naturalWeaponSubpart, T NewParent)
            : this()
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public BaseManagedDefaultEquipmentMutation(Dictionary<string, NaturalWeaponSubpart<T>> naturalWeaponSubparts, NaturalWeaponSubpart<T> naturalWeaponSubpart, T NewParent)
            : this(naturalWeaponSubparts, NewParent)
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public virtual NaturalWeaponSubpart<T> GetNaturalWeaponSubpart(
            string Type = "",
            GameObject Object = null,
            BodyPart BodyPart = null)
        {
            if (Type == "") goto CheckObject;

            if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
            if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];

            CheckObject:
            if (Object == null) goto CheckBodyPart;
            foreach (BodyPart part in Object?.Equipped.Body.LoopParts())
            {
                if (Object.IsDefaultEquipmentOf(part) || (part.Equipped == Object && Object.HasPart<NaturalEquipment>()))
                {
                    Type = part.Type;
                    if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
                    if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];
                }
            }

            CheckBodyPart:
            if (BodyPart == null) return null;
            Type = BodyPart.Type;
            if (Type == NaturalWeaponSubpart.Type) return NaturalWeaponSubpart;
            if (NaturalWeaponSubparts[Type] != null) return NaturalWeaponSubparts[Type];

            return null;
        }
        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart<T> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<T> GetNaturalWeaponMod(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            ModNaturalWeaponBase<T> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod();
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = (T)this;
            return NaturalWeaponMod;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<T> NaturalWeaponSubpart, string Parts)
        {
            if (Parts == null) return false;
            NaturalWeaponSubpart.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeaponSubpart.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieSize;
        }

        public virtual int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageBonus;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<T> Subpart, int Level)
        {
            Subpart.Level = Level;
            Subpart.DamageDieCount = GetNaturalWeaponDamageDieCount(Subpart, Level);
            Subpart.DamageDieSize = GetNaturalWeaponDamageDieSize(Subpart, Level);
            Subpart.DamageBonus = GetNaturalWeaponDamageBonus(Subpart, Level);
            Subpart.HitBonus = GetNaturalWeaponHitBonus(Subpart, Level);
            return true;
        }
        public override bool ChangeLevel(int NewLevel)
        {
            foreach ((_, NaturalWeaponSubpart<T> Subpart) in NaturalWeaponSubparts)
            {
                UpdateNaturalWeaponSubpart(Subpart, NewLevel);
            }
            if (NaturalWeaponSubpart != null) UpdateNaturalWeaponSubpart(NaturalWeaponSubpart, NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false)
        {
            if (body == null) goto Skip;
            List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
            foreach (BodyPart part in partsList)
            {
                ModNaturalWeaponBase<T> modNaturalWeapon = null;
                if (NaturalWeaponSubpart != null && part.Type == NaturalWeaponSubpart.Type && NaturalWeaponSubpart.IsCosmeticOnly() == CosmeticOnly)
                {
                    modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                }
                else if (NaturalWeaponSubparts[part.Type] != null && NaturalWeaponSubparts[part.Type].IsCosmeticOnly() == CosmeticOnly)
                {
                    modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                }

                if (modNaturalWeapon == null) continue;

                if (part.DefaultBehavior != null)
                {
                    part.DefaultBehavior.ApplyModification(modNaturalWeapon, Actor: ParentObject);
                }
                else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                {
                    part.Equipped.ApplyModification(modNaturalWeapon, Actor: ParentObject);
                }
            }
            Skip:
            return true;
        }
        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (body == null) goto Skip;
            ProcessNaturalWeaponSubparts(body, CosmeticOnly: false);
            Skip:
            base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            if (body == null) goto Skip;
            ProcessNaturalWeaponSubparts(body, CosmeticOnly: true);
            Skip:
            base.OnDecorateDefaultEquipment(body);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentMutation<T> mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation<T>;
            mutation.NaturalWeaponSubparts = new();
            foreach ((string type, NaturalWeaponSubpart<T> subpart) in NaturalWeaponSubparts)
            {
                mutation.NaturalWeaponSubparts.Add(type, new(subpart, (T)mutation));
            }
            mutation.NaturalWeaponSubpart = new(NaturalWeaponSubpart, (T)mutation);
            return mutation;
        }
    }
}
