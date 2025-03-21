using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Secrets;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsGiganticExoframe : BaseManagedDefaultEquipmentCybernetic<CyberneticsGiganticExoframe>
    {
        // XML Set Properties.
        public string Model = "Alpha";
        public string AugmentAdjectiveColor = "b";
        public string AugmentTile = "NaturalWeapons/GiganticManipulator.png";
        public string AugmentTileColorString = "&c";
        public string AugmentTileDetailColor = "b";
        public string AugmentSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
        public string AugmentBlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
        public string AugmentAddParts;
        public string AugmentAddProps;
        public string AugmentEquipmentFrameColors;

        public int JumpDistanceBonus = 0;
        public double StunningForceLevelFactor = 0.5;

        public bool AugmentAdded = false;

        public CyberneticsGiganticExoframe()
        {
            NaturalWeaponSubpart = new()
            {
                // There'd be more here but it's all assigned upon being implanted.
                ParentPart = this,
                Type = "Hand",
                CosmeticOnly = true,
                Level = 1,
                ModPriority = -10,  // Lower = more priority. Cosmetic, so highest priority;
                Adjective = "augmented",
                AddedParts = new(),
                AddedStringProps = new(),
                AddedIntProps = new(),
            }; 
        }

        public string GetShortAugmentAdjective(bool Pretty = true)
        {
            return Pretty ? NaturalWeaponSubpart.Adjective.OptionalColor(NaturalWeaponSubpart.GetAdjectiveColor(), NaturalWeaponSubpart.GetAdjectiveColorFallback(), Colorfulness) : NaturalWeaponSubpart.Adjective;
        }
        public string GetAugmentAdjective(bool Pretty = true)
        {
            return Pretty ? GetNaturalWeaponColoredAdjective() : $"EF-" + GetShortAugmentAdjective(Pretty);
        }
        public virtual string GetNaturalWeaponColoredAdjective()
        {
            string output = $"E{"F".Color("c")}-";
            output += NaturalWeaponSubpart.Adjective.OptionalColor(NaturalWeaponSubpart.GetAdjectiveColor(), NaturalWeaponSubpart.GetAdjectiveColorFallback(), Colorfulness);
            return output.Color("Y");
        }

        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }

        public override void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Debug.Entry(2, $"* OnImplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName})");

            // Mapping Augment properties to NaturalWeaponSubpart ones.
            NaturalWeaponSubpart.AdjectiveColor = AugmentAdjectiveColor;

            NaturalWeaponSubpart.Tile = AugmentTile;
            NaturalWeaponSubpart.ColorString = AugmentTileColorString;
            NaturalWeaponSubpart.DetailColor = AugmentTileDetailColor;
            NaturalWeaponSubpart.SecondColorString = NaturalWeaponSubpart.ColorString;
            NaturalWeaponSubpart.SecondDetailColor = NaturalWeaponSubpart.DetailColor;
            NaturalWeaponSubpart.SwingSound = AugmentSwingSound;
            NaturalWeaponSubpart.BlockedSound = AugmentBlockedSound;
            NaturalWeaponSubpart.EquipmentFrameColors = AugmentEquipmentFrameColors;

            NaturalWeaponSubpart.ProcessAddedParts(AugmentAddParts);
            NaturalWeaponSubpart.ProcessAddedProps(AugmentAddProps);

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

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID;
        }
        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you install this cybernetic despite being a disparate size to you.
            if (E.Object == ImplantObject && E.Object.HasPart<CyberneticsBaseItem>())
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
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            CyberneticsGiganticExoframe exoframe = base.DeepCopy(Parent, MapInv) as CyberneticsGiganticExoframe;
            exoframe.NaturalWeaponSubpart = new(NaturalWeaponSubpart, exoframe);
            return exoframe;
        }
    }
}
