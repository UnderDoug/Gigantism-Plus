using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class BaseManagedDefaultEquipmentMutation : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon
    {
        [Serializable]
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {
            public override string GetColoredAdjective()
            {
                return GetAdjective().OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
            }
        }

        public INaturalWeapon NaturalWeapon = new()
        {
            Level = 1,
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

            ModPriority = 0,
            AdjectiveColor = "y",
            AdjectiveColorFallback = "Y",
            ColorString = "&K",
            DetailColor = "y",
            SecondColorString = "&y",
            SecondDetailColor = "Y"
        };

        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        public virtual string GetNaturalWeaponModName(bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
        }
        public virtual ModNaturalWeaponBase<T> GetNaturalWeaponMod<T>()
            where T : IPart, IManagedDefaultNaturalWeapon, new()
        {
            return GetNaturalWeaponModName().ConvertToNaturalWeaponModification<T>();
        }

        public virtual bool CalculateNaturalWeaponLevel(int Level = 1)
        {
            NaturalWeapon.Level = Level;
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1)
        {
            NaturalWeapon.DamageDieCount = GetNaturalWeaponDamageDieCount(Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            NaturalWeapon.DamageDieSize = GetNaturalWeaponDamageDieSize(Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1)
        {
            NaturalWeapon.DamageBonus = GetNaturalWeaponDamageBonus(Level);
            return true;
        }

        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1)
        {
            NaturalWeapon.HitBonus = GetNaturalWeaponHitBonus(Level);
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(string Parts)
        {
            if (Parts == null) return false;
            NaturalWeapon.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeapon.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeapon.AddedStringProps = StringProps;
                NaturalWeapon.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(int Level = 1)
        {
            return NaturalWeapon.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(int Level = 1)
        {
            return NaturalWeapon.DamageDieSize;
        }

        public virtual int GetNaturalWeaponDamageBonus(int Level = 1)
        {
            return NaturalWeapon.DamageBonus;
        }

        public virtual int GetNaturalWeaponHitBonus(int Level = 1)
        {
            return NaturalWeapon.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts()
        {
            return NaturalWeapon.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps()
        {
            return NaturalWeapon.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps()
        {
            return NaturalWeapon.AddedIntProps;
        }

        public virtual string GetNaturalWeaponEquipmentFrameColors()
        {
            return NaturalWeapon.EquipmentFrameColors;
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentMutation mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation;
            mutation.NaturalWeapon = new()
            {
                Level = 1,
                DamageDieCount = 1,
                DamageDieSize = 2,
                DamageBonus = 0,
                HitBonus = 0,

                ModPriority = 0,
                AdjectiveColor = "y",
                AdjectiveColorFallback = "Y",
                ColorString = "&K",
                DetailColor = "y",
                SecondColorString = "&y",
                SecondDetailColor = "Y"
            };
            return mutation;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponLevel(NewLevel);
            CalculateNaturalWeaponDamageDieCount(NewLevel);
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            CalculateNaturalWeaponDamageBonus(NewLevel);
            CalculateNaturalWeaponHitBonus(NewLevel);
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
    }
}
