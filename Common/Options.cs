using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;

using HNPS_GigantismPlus.Harmony;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;

namespace HNPS_GigantismPlus
{
    [HasModSensitiveStaticCache]
    [HasOptionFlagUpdate(Prefix = "Option_GigantismPlus_")]
    public static class Options
    {
        public static bool doDebug = true;

        public static Dictionary<string, bool> classDoDebug = new()
        {
            // General
            { nameof(NaturalEquipmentOperator), true },
            { nameof(ModNaturalEquipmentBase), true },
            { "ModNaturalEquipment", true },
            { nameof(ModGiganticNaturalWeapon), true },
            { nameof(ModClosedGiganticNaturalWeapon), true },
            { nameof(ModElongatedNaturalWeapon), true },
            { nameof(ModBurrowingNaturalWeapon), true },
            { nameof(ModCrystallineNaturalWeapon), true },
            { nameof(ModAugmentedNaturalWeapon), true },
            { nameof(IManagedDefaultNaturalEquipment), true },
            { "BaseManagedDefaultEquipmentMutation", true },
            { "BaseManagedDefaultEquipmentCybernetic", true },
            { nameof(GigantismPlus), true },
            { nameof(ElongatedPaws), true },
            { nameof(UD_ManagedBurrowingClaws), true },
            { nameof(UD_ManagedCrystallinity), true },
            { nameof(UD_HornsPlus), true },
            { nameof(UD_QuillsPlus), true },
            { nameof(CyberneticsGiganticExoframe), true },
            { nameof(StewBelly), true },
            { nameof(Tactics_Vault), true },
            { nameof(Vaultable), true },
            { nameof(Gigantified), true },
            { nameof(InventoryGigantifier), true },
            { nameof(SecretGiantWhoCooksBuilderExtension), true },
            { nameof(GiantAbodePopulator), true },
            { nameof(WeaponElongator), true },

            //Conditions
            { "ICondition", true && doConditionsDebug },
            { "IConditions", true && doConditionsDebug },
            { "NotCondition", true && doConditionsDebug },
            { "OnlyOneCondition", true && doConditionsDebug },
            { "AnyConditions", true && doConditionsDebug },
            { "AllConditions", true && doConditionsDebug },
            { "NotAnyConditions", true && doConditionsDebug },
            { "NotAllConditions", true && doConditionsDebug },

            // Adjustments
            { nameof(IAdjustment), true },
            { nameof(Adjustments), true },

            // Events
            { "IBodyPartsUpdatedEvent", false },
            { nameof(BeforeUpdateBodyPartsEvent), false },
            { nameof(BodyPartsUpdatedEvent), false },
            { nameof(AfterBodyPartsUpdatedEvent), false },

            { nameof(GetPrioritisedNaturalEquipmentModsEvent), false },

            { "IManageDefaultNaturalEquipmentEvent", false },
            { nameof(GetNaturalEquipmentOperatorsEvent), false },
            { nameof(BeforeManageDefaultNaturalEquipmentEvent), false },
            { nameof(ManageDefaultNaturalEquipmentEvent), false },
            { nameof(AfterManageDefaultNaturalEquipmentEvent), false },

            { nameof(BeforeModGiganticAppliedEvent), false },
            { nameof(AfterModGiganticAppliedEvent), false },
            { nameof(BeforeRapidAdvancementEvent), false },
            { nameof(AfterRapidAdvancementEvent), false },

            { "IVaultedEvent", false },
            { nameof(BeforeVaultEvent), false },
            { nameof(VaultedEvent), false },

            { nameof(CrayonsGetColorsEvent), false },

            { "IDescribeModificationEvent", false },
            { nameof(DescriptionElement), false },
            { nameof(BeforeDescribeModificationEvent<ModGigantic>), false },
            { nameof(DescribeModificationEvent<ModGigantic>), false },

            { "IWrassleIDEvent", true },
            { nameof(AddWrassleIDEvent), true },
            { nameof(GetWrassleIDEvent), true },
            { nameof(UpdateWrassleIDEvent), true },
            { nameof(WrassleIDUpdatedEvent), true },
            { nameof(SyncWrassleIDEvent), true },

            // Handlers
            { nameof(CrayonsGetColorHandler), false },
            { nameof(BeforeModGiganticAppliedHandler), false },
            { nameof(AfterModGiganticAppliedHandler), false },
            { nameof(BeforeDescribeModGiganticHandler), false },
            { nameof(DescribeModGiganticHandler), false },

            // Harmony Patches
            { nameof(Body_Patches), false },
            { nameof(BodyPart_Patches), false },
            { nameof(Brain_Patches), false },
            { nameof(Carapace_Patches), false },
            { nameof(ColorUtility_Patches), false },
            { nameof(Crayons_Patches), false },
            { nameof(CookingDomainSpecial_UnitCrystalTransform_Patches), false },
            { nameof(Crystallinity_Patches), false },
            { nameof(CyberneticsTerminal2_Patches), false },
            { nameof(GameObject_Patches), false },
            { nameof(GameObjectFactory_Patches), false },
            { nameof(GigantismPlus_ControlledWeight_GameObject_Patches), false },
            { nameof(GigantismPlus_ControlledCarryCap_GetMaxCarriedWeightEvent_Patches), false },
            { nameof(PseudoGiganticCreature_RegenerateDefaultEquipment_Patches), false },
            { nameof(Horns_Patches), false },
            { nameof(Leveler_Patches), false },
            { nameof(MeleeWeapon_Patches), false },
            { nameof(ModGigantic_Patches), false },
            { nameof(MutationBGoneWishHandler_Patches), true },
            { nameof(Physics_Patches), false },
            { nameof(Skulk_Tonic_Patches), true },
            { nameof(Tinkering_Disassemble_Patches), false },

            // Widgets
            { nameof(RandomDebris), false },
            { nameof(RandomTree), false },
            { nameof(WallOrDebris), false },
            { nameof(WallOrNot), false },

            // Wrassle
            { nameof(UD_QWE), true },
            { nameof(UD_QudWrasslingEntertainment), true },
            { nameof(WrassleGiantHero), true },
            { nameof(IWrassle), true },
            { nameof(IWrasslePart), true },
            { nameof(IWrassleModification), true },
            { nameof(Wrassler), true },
            { nameof(WrassleGear), true },
            { nameof(ModWrassleVibrant), true },
        };

        public static bool doConditionsDebug = true;

        public static bool getDoDebug(object what = null, List<object> DoList = null, List<object> DontList = null, bool? DoDebug = null)
        {
            DoList = new();
            DontList = new();

            if (what != null && !DoList.IsNullOrEmpty() && DoList.Contains(what))
            {
                return true;
            }

            if (what != null && !DontList.IsNullOrEmpty() && DontList.Contains(what))
            {
                return false;
            }

            return DoDebug ?? doDebug;
        }

        public static bool getClassDoDebug(string Class)
        {
            if (classDoDebug.ContainsKey(Class))
            {
                return classDoDebug[Class];
            }
            return doDebug;
        }

        // Debug Settings
        [OptionFlag] public static int DebugVerbosity;
        [OptionFlag] public static bool DebugIncludeInMessage;
        [OptionFlag] public static bool StewBellyDebugDescriptions;
        [OptionFlag] public static bool WrassleIDDebugDescriptions;
        [OptionFlag] public static bool DebugVaultDescriptions;

        // General Settings
        [OptionFlag] public static int Colorfulness;

        // Starting Gear Settings
        [OptionFlag] public static bool EnableGiganticStartingGear;
        [OptionFlag] public static bool EnableGiganticStartingGear_Grenades;

        // Mutation Interactions Settings
        [OptionFlag] public static bool EnableGigantismRapidAdvance;

        // Tinkering Settings
        [OptionFlag] public static bool EnableGiganticTinkering;
        [OptionFlag] public static bool EnableGiganticDerarification;

        // NPC Equipment Settings
        [OptionFlag] public static bool EnableGiganticNPCGear;
        [OptionFlag] public static bool EnableGiganticNPCGear_Grenades;

        // Wrassle Player Settings
        [OptionFlag] public static bool EnableWrasslePlayerStart;
        [OptionFlag] public static int SlideWrasslePlayerStart;

        // Advanced Settings
        [OptionFlag] public static bool EnablePrereleaseContent;
    }
}
