using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class BaseManagedDefaultEquipmentMutation<T> : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon where T : BaseManagedDefaultEquipmentMutation<T>, new()
    {
        public Dictionary<string, NaturalWeaponSubpart> NaturalWeaponSubparts = new();
        public NaturalWeaponSubpart NaturalWeaponSubpart;
        public string NaturalWeaponBodyPart;

        public BaseManagedDefaultEquipmentMutation()
        {
        }

        public BaseManagedDefaultEquipmentMutation(Dictionary<string, NaturalWeaponSubpart> naturalWeaponSubparts)
        {
            Dictionary<string, NaturalWeaponSubpart> NewNaturalWeaponSubparts = new();
            foreach ((string Part, NaturalWeaponSubpart Subpart) in naturalWeaponSubparts)
            {
                NewNaturalWeaponSubparts.Add(Part, Subpart);
            }
            NaturalWeaponSubparts = NewNaturalWeaponSubparts;
        }

        public BaseManagedDefaultEquipmentMutation(Dictionary<string, NaturalWeaponSubpart> naturalWeaponSubparts, NaturalWeaponSubpart naturalWeaponSubpart)
        {
            Dictionary<string, NaturalWeaponSubpart> NewNaturalWeaponSubparts = new();
            foreach ((string Part, NaturalWeaponSubpart Subpart) in naturalWeaponSubparts)
            {
                NewNaturalWeaponSubparts.Add(Part, Subpart);
            }
            NaturalWeaponSubparts = NewNaturalWeaponSubparts;
            NaturalWeaponSubpart = new(naturalWeaponSubpart);
        }

        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart NaturalWeaponSubpart, bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeaponSubpart.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
        }
        public virtual ModNaturalWeaponBase<TPart> GetNaturalWeaponMod<TPart>(NaturalWeaponSubpart NaturalWeaponSubpart)
            where TPart : IPart, IManagedDefaultNaturalWeapon, new()
        {
            return GetNaturalWeaponModName(NaturalWeaponSubpart).ConvertToNaturalWeaponModification<TPart>();
        }

        public virtual bool CalculateNaturalWeaponLevel(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            ModNaturalWeaponBase<T> NaturalWeaponMod = GetNaturalWeaponMod<T>(NaturalWeaponSubpart);
            NaturalWeaponSubpart.Level = Level;
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            NaturalWeaponSubpart.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart, Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieSize(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            NaturalWeaponSubpart.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart, Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            NaturalWeaponSubpart.DamageBonus = GetNaturalWeaponDamageBonus(NaturalWeaponSubpart, Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponHitBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            NaturalWeaponSubpart.HitBonus = GetNaturalWeaponHitBonus(NaturalWeaponSubpart, Level);
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart NaturalWeaponSubpart, string Parts)
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

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart NaturalWeaponSubpart, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieSize;
        }

        public virtual int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageBonus;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual string GetNaturalWeaponEquipmentFrameColors(NaturalWeaponSubpart NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.EquipmentFrameColors;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            foreach ((string Part, NaturalWeaponSubpart Subpart) in NaturalWeaponSubparts)
            {
                CalculateNaturalWeaponLevel(Subpart, NewLevel);
                CalculateNaturalWeaponDamageDieCount(Subpart, NewLevel);
                CalculateNaturalWeaponDamageDieSize(Subpart, NewLevel);
                CalculateNaturalWeaponDamageBonus(Subpart, NewLevel);
                CalculateNaturalWeaponHitBonus(Subpart, NewLevel);
            }
            return base.ChangeLevel(NewLevel);
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
            mutation.NaturalWeaponSubparts = new(NaturalWeaponSubparts);
            mutation.NaturalWeaponSubpart = null;
            return mutation;
        }
    }
}
