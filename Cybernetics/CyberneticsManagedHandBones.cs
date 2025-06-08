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

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsManagedHandBones : BaseManagedDefaultEquipmentCybernetic<CyberneticsManagedHandBones>
    {
        private static bool doDebug => getClassDoDebug(nameof(CyberneticsManagedHandBones));
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
        public string Material = "carbide";
        public string BonesAdjectiveColor = "b";
        public string BonesTile = "Items/sw_carbidehand.bmp";
        public string BonesTileColorString = "&K";
        public string BonesTileDetailColor = "b";
        public string BonesSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
        public string BonesBlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
        public string BonesAddParts;
        public string BonesAddProps;
        public string BonesEquipmentFrameColors;
        public int BonesDamageDieCount = 1;
        public int BonesDamageDieSize = 1;
        public int BonesDamageBonus = 0;
        public int BonesHitBonus = 0;
        public int BonesPenBonus = 0;

        public int JumpDistanceBonus = 0;
        public double StunningForceLevelFactor = 0.5;

        public CyberneticsManagedHandBones()
        {
        }
        public static ModChromeBonedNaturalWeapon NewChromeBonedNaturalWeaponMod(CyberneticsManagedHandBones assigningPart)
        {
            ModChromeBonedNaturalWeapon chromeBonedNaturalWeapon = new()
            {
                AssigningPart = assigningPart,
                BodyPartType = "Hand",

                ModPriority = 490,
                DescriptionPriority = 490,

                Adjective = "boned",
                AdjectiveColor = assigningPart.BonesAdjectiveColor,
                AdjectiveColorFallback = assigningPart.BonesAdjectiveColor,

                Adjustments = new(),

                AddedParts = new(),

                AddedIntProps = new(),
                AddedStringProps = new(),
            };
            chromeBonedNaturalWeapon.AddAdjustment(RENDER, "Tile", assigningPart.BonesTile);
            chromeBonedNaturalWeapon.AddAdjustment(RENDER, "ColorString", assigningPart.BonesTileColorString);
            chromeBonedNaturalWeapon.AddAdjustment(RENDER, "TileColor", assigningPart.BonesTileColorString);
            chromeBonedNaturalWeapon.AddAdjustment(RENDER, "DetailColor", assigningPart.BonesTileDetailColor, true);

            assigningPart.ProcessNaturalEquipmentAddedParts(chromeBonedNaturalWeapon, assigningPart.BonesAddParts);
            assigningPart.ProcessNaturalEquipmentAddedProps(chromeBonedNaturalWeapon, assigningPart.BonesAddProps);

            if (!assigningPart.BonesSwingSound.IsNullOrEmpty())
            {
                chromeBonedNaturalWeapon.AddedStringProps["SwingSound"] = assigningPart.BonesSwingSound;
            }
            if (!assigningPart.BonesBlockedSound.IsNullOrEmpty())
            {
                chromeBonedNaturalWeapon.AddedStringProps["BlockedSound"] = assigningPart.BonesBlockedSound;
            }
            if (!assigningPart.BonesEquipmentFrameColors.IsNullOrEmpty())
            {
                chromeBonedNaturalWeapon.AddedStringProps["EquipmentFrameColors"] = assigningPart.BonesEquipmentFrameColors;
            }

            return chromeBonedNaturalWeapon;
        }
        public override ModNaturalEquipment<CyberneticsManagedHandBones> NewNaturalEquipmentMod(CyberneticsManagedHandBones NewAssigner = null)
        {
            return NewChromeBonedNaturalWeaponMod(NewAssigner ?? this);
        }

        public string GetMaterialAdjective(int Colorfulness = 0)
        {
            string materialColor = NaturalEquipmentMod.AdjectiveColor;
            string material = Material;
            return $"{material.OptionalColor(materialColor, materialColor, Colorfulness)}";
        }
        public string GetBonedAdjective(int Colorfulness = 0)
        {
            string bonesColor = "r";
            string adjective = NaturalEquipmentMod.Adjective;
            return $"{adjective.OptionalColor(bonesColor, bonesColor, Colorfulness)}";
        }
        public virtual string GetNaturalEquipmentColoredAdjective(int Colorfulness = 0)
        {
            string output = $"{GetMaterialAdjective(Colorfulness)}-{GetBonedAdjective(Colorfulness)}";
            return output.Color("y");
        }
        public override int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<CyberneticsManagedHandBones> NaturalEquipmentMod, int Level = 1)
        {
            int damageDieCount = BonesDamageDieCount;
            if (Implantee.HasPart<GigantismPlus>())
            {
                damageDieCount--;
            }
            return damageDieCount;
        }
        public override int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<CyberneticsManagedHandBones> NaturalEquipmentMod, int Level = 1)
        {
            return BonesDamageDieSize;
        }
        public override int GetNaturalWeaponDamageBonus(ModNaturalEquipment<CyberneticsManagedHandBones> NaturalEquipmentMod, int Level = 1)
        {
            return BonesDamageBonus;
        }
        public override int GetNaturalWeaponHitBonus(ModNaturalEquipment<CyberneticsManagedHandBones> NaturalEquipmentMod, int Level = 1)
        {
            return BonesHitBonus;
        }
        public override int GetNaturalWeaponPenBonus(ModNaturalEquipment<CyberneticsManagedHandBones> NaturalEquipmentMod, int Level = 1)
        {
            return BonesPenBonus;
        }

        public override void OnImplanted(GameObject Implantee, GameObject Implant)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(2,
                $"* {nameof(CyberneticsManagedHandBones)}."
                + $"{nameof(OnImplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

            base.OnImplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsManagedHandBones)}."
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
                $"* {nameof(CyberneticsManagedHandBones)}."
                + $"{nameof(OnUnimplanted)}("
                + $" {nameof(Implantee)}: {Implantee?.DebugName ?? NULL}"
                + $" {nameof(Implant)}: {Implant?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

            // Implantee.CheckEquipmentSlots();

            base.OnUnimplanted(Implantee, Implant);

            Debug.Entry(2,
                $"x {nameof(CyberneticsManagedHandBones)}."
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
                || ID == AfterObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object == ImplantObject)
            {
                if (E.Object.TryGetPart(out Description description))
                {
                    string material = Material.Color(BonesAdjectiveColor);
                    description._Short = description._Short.Replace("*material*", material);
                }
                if (E.Object.TryGetPart(out CyberneticsFistReplacement cyberneticsFistReplacement))
                {
                    E.Object.RemovePart(cyberneticsFistReplacement);
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
            CyberneticsManagedHandBones handBones = base.DeepCopy(Parent, MapInv) as CyberneticsManagedHandBones;
            
            return handBones;
        }
    }
}
