﻿using System;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    public class BaseManagedDefaultEquipmentCybernetic : IScribedPart, IManagedDefaultNaturalWeapon
    {
        [NonSerialized]
        private GameObject _User;
        public GameObject Implantee
        {
            get
            {
                return _User ??= ParentObject.Implantee;
            }
            set
            {
                _User = value;
            }
        }

        public GameObject ImplantObject;

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentCybernetic cybernetic = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentCybernetic;
            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;
            return cybernetic;
        }

        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {
            public override string GetColoredAdjective()
            {
                return GetAdjective().OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
            }
        }

        public INaturalWeapon NaturalWeapon = new()
        {
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

            ModPriority = 0,
            AdjectiveColor = "y",
            AdjectiveColorFallback = "Y",
            Noun = "fist",
            Tile = "Creatures/natural-weapon-fist.bmp",
            ColorString = "&K",
            DetailColor = "y",
            SecondColorString = "&y",
            SecondDetailColor = "Y",
            SwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing",
            BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked"
        };

        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        public virtual string GetNaturalWeaponMod(bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
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

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
                || ID == PooledEvent<RegenerateDefaultEquipmentEvent>.ID
                || ID == PooledEvent<DecorateDefaultEquipmentEvent>.ID;
        }

        public virtual void OnImplanted(GameObject Implantee, GameObject Implant)
        {
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
        } //!--- public override void OnUnimplanted(GameObject Object)

        public override bool HandleEvent(ImplantedEvent E)
        {
            Implantee = E.Implantee;
            ImplantObject = E.Item;
            OnImplanted(Implantee, ImplantObject);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnimplantedEvent E)
        {
            OnUnimplanted(E.Implantee, E.Item);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(RegenerateDefaultEquipmentEvent E)
        {
            OnRegenerateDefaultEquipment(Implantee.Body);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(DecorateDefaultEquipmentEvent E)
        {
            OnDecorateDefaultEquipment(Implantee.Body);
            return base.HandleEvent(E);
        }

        public virtual void OnRegenerateDefaultEquipment(Body body)
        {
        }
        public virtual void OnDecorateDefaultEquipment(Body body)
        {
        }

    }
}
