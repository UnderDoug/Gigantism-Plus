using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Secrets;
using static HNPS_GigantismPlus.Options;
using static XRL.World.Parts.NaturalEquipmentManager;
using UnityEngine.Tilemaps;

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
            NaturalEquipmentMod = new()
            {
                AssigningPart = this,
                BodyPartType = "Hand",

                ModPriority = -10,
                DescriptionPriority = -10,

                Adjective = "augmented",
                AdjectiveColor = "gigantic",
                AdjectiveColorFallback = "w",

                Adjustments = new(),
            };
            NaturalEquipmentMod.AddAdjustment(RENDER, "DisplayName", "fist", true);

        }

        public string GetShortAugmentAdjective(bool Pretty = true)
        {
            return Pretty ? NaturalEquipmentMod.Adjective.OptionalColor(NaturalEquipmentMod.AdjectiveColor, NaturalEquipmentMod.AdjectiveColorFallback, Colorfulness) : NaturalEquipmentMod.Adjective;
        }
        public string GetAugmentAdjective(bool Pretty = true)
        {
            return Pretty ? GetNaturalWeaponColoredAdjective() : $"EF-" + GetShortAugmentAdjective(Pretty);
        }
        public virtual string GetNaturalWeaponColoredAdjective()
        {
            string output = $"E{"F".Color("c")}-";
            output += NaturalEquipmentMod.Adjective.OptionalColor(NaturalEquipmentMod.AdjectiveColor, NaturalEquipmentMod.AdjectiveColorFallback, Colorfulness);
            return output.Color("Y");
        }

        public override void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            Debug.Entry(2, $"* OnImplanted({Implantee.ShortDisplayName}, {Implant.ShortDisplayName})");

            // Mapping Augment properties to NaturalEquipmentMod ones.
            NaturalEquipmentMod.AdjectiveColor = AugmentAdjectiveColor;

            NaturalEquipmentMod.AddAdjustment(RENDER, "Tile", AugmentTile);
            NaturalEquipmentMod.AddAdjustment(RENDER, "ColorString", AugmentTileColorString);
            NaturalEquipmentMod.AddAdjustment(RENDER, "DetailColor", AugmentTileDetailColor);

            ProcessNaturalEquipmentAddedParts(NaturalEquipmentMod, AugmentAddParts);
            ProcessNaturalEquipmentAddedProps(NaturalEquipmentMod, AugmentAddProps);
            NaturalEquipmentMod.AddedStringProps.Add("SwingSound", AugmentSwingSound);
            NaturalEquipmentMod.AddedStringProps.Add("BlockedSound", AugmentBlockedSound);
            NaturalEquipmentMod.AddedStringProps.Add("EquipmentFrameColors", AugmentEquipmentFrameColors);

            Become(Implantee, Model, Implant);

            OnBodyPartsUpdated(Implantee.Body);

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
            exoframe.NaturalEquipmentMod = new(NaturalEquipmentMod, exoframe);
            return exoframe;
        }
    }
}
