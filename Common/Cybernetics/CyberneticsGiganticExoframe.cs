using System;
using System.Collections.Generic;
using System.Linq;

using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static XRL.World.Parts.NaturalEquipmentOperator;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.SecretGiganticExoframe;
using static HNPS_GigantismPlus.NaturalEquipmentConditions;

using static XRL.World.Parts.ModAugmentedNaturalWeapon;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsGiganticExoframe : BaseManagedDefaultEquipmentCybernetic<CyberneticsGiganticExoframe>
    {
        private static bool doDebug => getClassDoDebug(nameof(CyberneticsGiganticExoframe));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        // XML Set Fields.
        public string Model;
        public string Material;
        public int AV;
        public int DV;
        public string AugmentAdjectiveColor;
        public string AugmentTile;
        public string AugmentTileColorString;
        public string AugmentTileDetailColor;
        public string AugmentSwingSound;
        public string AugmentBlockedSound;
        public string AugmentAddParts;
        public string AugmentAddProps;
        public string AugmentEquipmentFrameColors;

        public int JumpDistanceBonus;
        public double StunningForceLevelFactor;

        public CyberneticsGiganticExoframe()
        {
            Model = "Alpha";
            Material = "carbide";
            AV = 2;
            DV = -5;
            AugmentAdjectiveColor = "b";
            AugmentTile = "NaturalWeapons/EF-AugmentedGiganticManipulator.png";
            AugmentTileColorString = "&c";
            AugmentTileDetailColor = "b";
            AugmentSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
            AugmentBlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
            AugmentAddParts = null;
            AugmentAddProps = null;
            AugmentEquipmentFrameColors = null;

            JumpDistanceBonus = 0;
            StunningForceLevelFactor = 0.5;
        }
        public static ModAugmentedNaturalWeapon NewAugmentedManipulatorMod(NaturalEquipmentManager NewManager)
        {
            CyberneticsGiganticExoframe giganticExoframe = NewManager?.GetManagedNaturalEquipmentCompatiblePart<CyberneticsGiganticExoframe>();
            if (giganticExoframe == null)
            {
                Debug.Warn(2,
                    nameof(CyberneticsGiganticExoframe),
                    nameof(NewAugmentedManipulatorMod),
                    $"Couldn't get instance of {nameof(giganticExoframe)} from " +
                    $"{nameof(NaturalEquipmentManager.GetManagedNaturalEquipmentCompatiblePart)}",
                    Indent: 0);
                return null;
            }
            ModAugmentedNaturalWeapon augmentedManipulator = new(NewManager)
            {
                AdjectiveColor = giganticExoframe.AugmentAdjectiveColor,
            };
            augmentedManipulator.AdjustNoun()

                .AdjustTile(giganticExoframe.AugmentTile)

                .AdjustColorString(giganticExoframe.AugmentTileColorString, true)
                .AdjustTileColor(giganticExoframe.AugmentTileColorString, true)
                .AdjustDetailColor(giganticExoframe.AugmentTileDetailColor);

            if (!giganticExoframe.AugmentSwingSound.IsNullOrEmpty())
            {
                augmentedManipulator.SetSwingSound(giganticExoframe.AugmentSwingSound, true);
            }
            if (!giganticExoframe.AugmentBlockedSound.IsNullOrEmpty())
            {
                augmentedManipulator.SetBlockedSound(giganticExoframe.AugmentBlockedSound, true);
            }
            if (!giganticExoframe.AugmentEquipmentFrameColors.IsNullOrEmpty())
            {
                augmentedManipulator.SetEquipmentFrameColors(giganticExoframe.AugmentEquipmentFrameColors, true, true);
            }
            if (!giganticExoframe.AugmentAddParts.IsNullOrEmpty())
            {
                foreach (string addedPart in giganticExoframe.AugmentAddParts.CachedCommaExpansion())
                {
                    augmentedManipulator.AddPart(addedPart);
                }
            }
            if (!giganticExoframe.AugmentAddProps.IsNullOrEmpty()
                && giganticExoframe.AugmentAddProps.ParseProps(out Dictionary<string, string> stringProps, out Dictionary<string, int> intProps))
            {
                if (!stringProps.IsNullOrEmpty())
                {
                    foreach ((string label, string prop) in stringProps)
                    {
                        augmentedManipulator.SetStringProperty(label, prop, true);
                    }
                }
                if (!intProps.IsNullOrEmpty())
                {
                    foreach ((string label, int prop) in intProps)
                    {
                        augmentedManipulator.SetIntProperty(label, prop, true);
                    }
                }
            }
            return augmentedManipulator;
        }

        public string GetAugmentAdjective(int Colorfulness = 0)
        {
            ModAugmentedNaturalWeapon naturalEquipmentMod = NewAugmentedManipulatorMod(NaturalEquipmentManager);
            string augmentedColor = naturalEquipmentMod?.AdjectiveColor ?? AugmentAdjectiveColor;
            string augmentedColorFallback = naturalEquipmentMod?.AdjectiveColorFallback ?? ADJ_COL_FALLBACK;
            string adjective = naturalEquipmentMod?.Adjective ?? ADJ;
            return adjective.OptionalColor(augmentedColor, augmentedColorFallback, Colorfulness);
        }
        public string GetAugmentPrefix(int Colorfulness = 0)
        {
            string prefixColor = ADJ_COL_FALLBACK;
            string prefix = "E" + "F".OptionalColor(prefixColor, prefixColor, Colorfulness);
            return prefix;
        }
        public virtual string GetNaturalEquipmentColoredAdjective(int Colorfulness = 0)
        {
            string output = $"{GetAugmentPrefix(Colorfulness)}-{GetAugmentAdjective(Colorfulness)}";
            return output.Color("y");
        }

        public override void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(2,
                $"* {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnImplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

            Becoome(Implantee, Model, Implant);

            Armor armor = ParentObject.RequirePart<Armor>();
            if (armor != null)
            {
                armor.WornOn = "Body";
                armor.AV = AV;
                armor.DV = DV;
            }

            base.OnImplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnImplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        }

        public override void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(2,
                $"* {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnUnimplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

            // Implantee.CheckEquipmentSlots();

            Unbecoome(Implantee, Model, ParentObject);

            ParentObject.RemovePart<Armor>();

            base.OnUnimplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnUnimplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetSlotsRequiredEvent.ID
                || ID == AfterObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you install this cybernetic despite being a disparate size to you.
            if (E.Object == ParentObject && E.Object.HasPart<CyberneticsBaseItem>())
            {
                if (!E.Actor.IsGiganticCreature && E.Object.IsGiganticEquipment)
                {
                    E.Decreases++;
                }
                else
                if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment)
                {
                    E.Increases++;
                }
                E.CanBeTooSmall = false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object == ParentObject)
            {
                if (E.Object.TryGetPart(out Description description))
                {
                    description._Short = description._Short.Replace("*material*", Material);
                }
            }
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            return base.FireEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            CyberneticsGiganticExoframe exoframe = base.DeepCopy(Parent, MapInv) as CyberneticsGiganticExoframe;

            return exoframe;
        }
    }
}
