using System;
using System.Collections.Generic;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Secrets;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsGiganticExoframe : BaseManagedDefaultEquipmentCybernetic
    {
        // XML Set Properties.
        public string Model = "Alpha";
        public string AugmentAdjectiveColor = "b";
        public string AugmentTile = "NaturalWeapons/GiganticManipulator.png";
        public string AugmentTileColorString = "&c";
        public string AugmentTileDetailColor = "b";
        public string AugmentedSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
        public string AugmentedBlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
        
        public int JumpDistanceBonus = 0;
        public double StunningForceLevelFactor = 0.5;

        public new class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {
            public override string GetColoredAdjective()
            {
                string output = $"E{"F".Color("c")}-";
                output += GetAdjective().OptionalColor(GetAdjectiveColor(), GetAdjectiveColorFallback(), Colorfulness);
                return  output.Color("Y");
            }
        }

        public CyberneticsGiganticExoframe()
        {
            NaturalWeapon = new()
            {
                DamageDieCount = 1, // Default, equal to +0
                DamageDieSize = 2,  // Default, equal to +0
                ModPriority = -10,  // Lower = more priority. Cosmetic, so highest priority;
                Adjective = "augmented",
                AdjectiveColor = "b",
                Noun = null,        // this should stop it from being overridden
                Skill = null,       // this should stop it from being overridden
                Stat = null,        // this should stop it from being overridden
                Tile = "NaturalWeapons/GiganticManipulator.png",
                ColorString = "&c",
                DetailColor = "b",
                SecondColorString = "&c",
                SecondDetailColor = "b",
                SwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing",
                BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked"
            };
        }
        public string GetShortAugmentAdjective(bool Pretty = true)
        {
            return ("augmented").MaybeColor(AugmentAdjectiveColor, Pretty);
        }
        public string GetAugmentAdjective(bool Pretty = true)
        {
            return ($"E{ ("F").MaybeColor("c", Pretty) }-{GetShortAugmentAdjective(Pretty)}").MaybeColor("Y", Pretty);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            CyberneticsGiganticExoframe exoframe = base.DeepCopy(Parent, MapInv) as CyberneticsGiganticExoframe;
            exoframe.Implantee = null;
            exoframe.ImplantObject = null;
            return exoframe;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == CanEnterInteriorEvent.ID;
        }

        public override void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Debug.Entry(2, $"* OnImplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName})");

            // Mapping Augment properties to NaturalWeapon ones.
            NaturalWeapon.AdjectiveColor = AugmentAdjectiveColor;
            NaturalWeapon.Tile = AugmentTile;
            NaturalWeapon.ColorString = AugmentTileColorString;
            NaturalWeapon.DetailColor = AugmentTileDetailColor;
            NaturalWeapon.SecondColorString = NaturalWeapon.ColorString;
            NaturalWeapon.SecondDetailColor = NaturalWeapon.DetailColor;
            NaturalWeapon.SwingSound = AugmentedSwingSound;
            NaturalWeapon.BlockedSound = AugmentedBlockedSound;

            Become(Implantee, Model, Implant);

            Debug.Entry(2, $"x OnImplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName}) *//");
        } //!--- public override void OnImplanted(GameObject Object)

        public override void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            Debug.Entry(3, $"* OnUnimplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName})");

            Implantee.CheckAffectedEquipmentSlots();
            
            Unbecome(Implantee, Model, ImplantObject);
            Debug.Entry(3, $"x OnUnimplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName}) *//");
        } //!--- public override void OnUnimplanted(GameObject Object)

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you install this cybernetic despite being a disparate size to you.
            if (E.Object.HasPart<CyberneticsBaseItem>())
            {
                if (!E.Actor.IsGiganticCreature && E.Object.IsGiganticEquipment)
                    E.Decreases++;
                else if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment)
                    E.Increases++;

                E.CanBeTooSmall = false;
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        // These prevent the cybernetic in question from being disassembled.
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("CanBeDisassembled");
            base.Register(Object, Registrar);
        }
        public void CanBeDisassembled()
        {
            Event CanBeDisassembled = Event.New("CanBeDisassembled");
            ParentObject.FireEvent(CanBeDisassembled);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "CanBeDisassembled")
            {
                return false;
            }

            return base.FireEvent(E);
        }
    }
}
