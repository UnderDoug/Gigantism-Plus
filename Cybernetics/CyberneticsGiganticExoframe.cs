using System;
using System.Collections.Generic;
using System.Linq;

using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static XRL.World.Parts.NaturalEquipmentManager;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.SecretGiganticExoframe;

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
        public string Model = "Alpha";
        public string Material = "carbide";
        public string AugmentAdjectiveColor = "b";
        public string AugmentTile = "NaturalWeapons/EF-AugmentedGiganticManipulator.png";
        public string AugmentTileColorString = "&c";
        public string AugmentTileDetailColor = "b";
        public string AugmentSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
        public string AugmentBlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
        public string AugmentAddParts;
        public string AugmentAddProps;
        public string AugmentEquipmentFrameColors;

        public int JumpDistanceBonus = 0;
        public double StunningForceLevelFactor = 0.5;

        public CyberneticsGiganticExoframe()
        {
        }
        public static ModAugmentedNaturalWeapon NewAugmentedManipulatorMod(CyberneticsGiganticExoframe assigningPart)
        {
            ModAugmentedNaturalWeapon augmentedManipulator = new()
            {
                AssigningPart = assigningPart,
                BodyPartType = "Hand",

                ModPriority = -500,
                DescriptionPriority = -500,

                ForceNoun = true,
                Noun = "manipulator",

                Adjective = "augmented",
                AdjectiveColor = assigningPart.AugmentAdjectiveColor,
                AdjectiveColorFallback = "c",

                Adjustments = new(),

                AddedParts = new(),

                AddedIntProps = new(),
                AddedStringProps = new(),
            };
            augmentedManipulator.AddNounAdjustment();

            augmentedManipulator.AddTileAdjustment(assigningPart.AugmentTile);
            augmentedManipulator.AddColorStringAdjustment(assigningPart.AugmentTileColorString, true);
            augmentedManipulator.AddTileColorAdjustment(assigningPart.AugmentTileColorString, true);
            augmentedManipulator.AddDetailColorAdjustment(assigningPart.AugmentTileDetailColor);

            assigningPart.ProcessNaturalEquipmentAddedParts(augmentedManipulator, assigningPart.AugmentAddParts);
            assigningPart.ProcessNaturalEquipmentAddedProps(augmentedManipulator, assigningPart.AugmentAddProps);

            if (!assigningPart.AugmentSwingSound.IsNullOrEmpty())
            {
                augmentedManipulator.AddedStringProps["SwingSound"] = assigningPart.AugmentSwingSound;
            }
            if (!assigningPart.AugmentBlockedSound.IsNullOrEmpty())
            {
                augmentedManipulator.AddedStringProps["BlockedSound"] = assigningPart.AugmentBlockedSound;
            }
            if (!assigningPart.AugmentEquipmentFrameColors.IsNullOrEmpty())
            {
                augmentedManipulator.AddedStringProps["EquipmentFrameColors"] = assigningPart.AugmentEquipmentFrameColors;
            }

            return augmentedManipulator;
        }
        public override ModNaturalEquipment<CyberneticsGiganticExoframe> GetNaturalEquipmentMod(Predicate<ModNaturalEquipment<CyberneticsGiganticExoframe>> Filter = null, CyberneticsGiganticExoframe NewAssigner = null)
        {
            ModNaturalEquipment<CyberneticsGiganticExoframe> naturalEquipmentMod = NewAugmentedManipulatorMod(NewAssigner ?? this);
            return Filter == null || Filter(naturalEquipmentMod) ? naturalEquipmentMod : base.GetNaturalEquipmentMod(Filter, NewAssigner);
        }

        public string GetAugmentAdjective(int Colorfulness = 0)
        {
            string augmentedColor = NaturalEquipmentMod.AdjectiveColor;
            string augmentedColorFallback = NaturalEquipmentMod.AdjectiveColorFallback;
            string adjective = NaturalEquipmentMod.Adjective;
            return adjective.OptionalColor(augmentedColor, augmentedColorFallback, Colorfulness);
        }
        public string GetAugmentPrefix(int Colorfulness = 0)
        {
            string prefixColor = "c";
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

            Become(Implantee, Model, Implant);

            base.OnImplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnImplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        } //!--- public override void OnImplanted(GameObject Object)

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

            Unbecome(Implantee, Model, ImplantObject);

            base.OnUnimplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsGiganticExoframe)}."
                + $"{nameof(OnUnimplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        } //!--- public override void OnUnimplanted(GameObject Object)

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
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == AfterObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you install this cybernetic despite being a disparate size to you.
            if (E.Object == ImplantObject && E.Object.HasPart<CyberneticsBaseItem>())
            {
                if (!E.Actor.IsGiganticCreature && E.Object.IsGiganticEquipment)
                {
                    E.Decreases++;
                }
                else if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment)
                {
                    E.Increases++;
                }
                E.CanBeTooSmall = false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object == ImplantObject)
            {
                if (E.Object.TryGetPart(out Description description))
                {
                    string material = Material.Color(AugmentAdjectiveColor);
                    description._Short = description._Short.Replace("*material*", material);
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
