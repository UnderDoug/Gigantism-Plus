using HNPS_GigantismPlus.Harmony;
using System;
using System.Collections.Generic;
using XRL;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.ZoneBuilders;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.DescribeModGiganticEvent;
using static XRL.World.Parts.ModNaturalEquipmentBase;

namespace HNPS_GigantismPlus
{
    [HasModSensitiveStaticCache]
    public static class Options
    {
        // Per the wiki, code is taken 1:1
        private static string GetOption(string ID, string Default = "")
        {
            return XRL.UI.Options.GetOption(ID, Default);
        }

        private static string Label(string Option = null)
        {
            string Label = "Option_GigantismPlus";
            if (Option == null)
                return Label;
            return $"{Label}_{Option}";
        }
        private static Dictionary<string, string> Directory => new()
        {
            { nameof(DebugVerbosity), Label("DebugVerbosity") },
            { nameof(DebugIncludeInMessage), Label("DebugIncludeInMessage") },
            { nameof(DebugVaultDescriptions), Label("DebugIncludeVaultDebugDescriptions") },
            { nameof(Colorfulness), Label("Colorfulness") },
            { nameof(EnableGiganticStartingGear), Label("EnableGiganticStartingGear") },
            { nameof(EnableGiganticStartingGear_Grenades), Label("EnableGiganticStartingGear_Grenades") },
            { nameof(EnableGigantismRapidAdvance), Label("EnableGigantismRapidAdvance") },
            { nameof(EnableGiganticTinkering), Label("EnableGiganticTinkering") },
            { nameof(EnableGiganticDerarification), Label("EnableGiganticDerarification") },
            { nameof(EnableGiganticNPCGear), Label("EnableGiganticNPCGear") },
            { nameof(EnableGiganticNPCGear_Grenades), Label("EnableGiganticNPCGear_Grenades") },
        };

        private static string GetStringOption(string ID, string Default = "")
        {
            if (Directory.ContainsKey(ID))
            {
                return XRL.UI.Options.GetOption(Directory[ID], Default: Default);
            }
            return Default;
        }
        private static bool GetBoolOption(string ID, bool Default = false)
        {
            return GetStringOption(ID, Default ? "Yes" : "No").EqualsNoCase("Yes");
        }
        private static int GetIntOption(string ID, int Default = 0)
        {
            return int.Parse(GetStringOption(ID, $"{Default}"));
        }

        private static void SetBoolOption(string ID, bool Value)
        {
            if (Directory.ContainsKey(ID))
                XRL.UI.Options.SetOption(Directory[ID], Value);
        }
        private static void SetStringOption(string ID, string Value)
        {
            if (Directory.ContainsKey(ID))
                XRL.UI.Options.SetOption(Directory[ID], Value);
        }
        private static void SetIntOption(string ID, int Value)
        {
            SetStringOption(Directory[ID], $"{Value}");
        }

        public static bool doDebug = true;
        public static Dictionary<string, bool> classDoDebug = new()
        {
            // General
            { nameof(NaturalEquipmentManager), true },
            { nameof(ModNaturalEquipmentBase), true },
            { nameof(PartAdjustment), true },
            { nameof(DescriptionElement), true },
            { nameof(ModGiganticNaturalWeapon), true },
            { nameof(ModClosedGiganticNaturalWeapon), true },
            { nameof(ModElongatedNaturalWeapon), true },
            { nameof(ModBurrowingNaturalWeapon), true },
            { nameof(ModCrystallineNaturalWeapon), true },
            { nameof(ModAugmentedNaturalWeapon), true },
            { "ModNaturalEquipment", true },
            { "BaseManagedDefaultEquipmentMutation", true },
            { "BaseManagedDefaultEquipmentCybernetic", true },
            { nameof(GigantismPlus), true },
            { nameof(ElongatedPaws), true },
            { nameof(UD_ManagedBurrowingClaws), true },
            { nameof(UD_ManagedCrystallinity), true },
            { nameof(UD_HornsPlus), false },
            { nameof(UD_QuillsPlus), false },
            { nameof(CyberneticsGiganticExoframe), false },
            { nameof(StewBelly), false },
            { nameof(Wrassler), true },
            { nameof(WrassleGear), true },
            { nameof(Tactics_Vault), false },
            { nameof(Vaultable), false },
            { nameof(Gigantified), true },
            { nameof(SecretGiantWhoCooksBuilderExtension), true },
            { nameof(GiantAbodePopulator), true },
            { nameof(WrassleGiantHero), true },

            // Events
            { nameof(AfterBodyPartsUpdatedEvent), false },
            { nameof(BeforeBodyPartsUpdatedEvent), false },
            { "IDescribeModGiganticEvent", false },
            { nameof(BeforeDescribeModGiganticEvent), true },
            { nameof(DescribeModGiganticEvent), true },
            { nameof(AfterManageDefaultNaturalEquipmentEvent), false },
            { nameof(BeforeManageDefaultNaturalEquipmentEvent), false },
            { nameof(ManageDefaultNaturalEquipmentEvent), false },
            { nameof(AfterModGiganticAppliedEvent), false },
            { nameof(BeforeModGiganticAppliedEvent), false },
            { nameof(AfterRapidAdvancementEvent), false },
            { nameof(BeforeRapidAdvancementEvent), false },
            { nameof(BeforeVaultEvent), false },
            { nameof(VaultedEvent), false },
            { nameof(CrayonsGetColorsEvent), false },
            { nameof(GetPrioritisedNaturalEquipmentModsEvent), true },

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
            { nameof(MutationBGoneWishHandler_Patches), false },
            { nameof(Physics_Patches), false },
            { nameof(Skulk_Tonic_Patches), true },
            { nameof(Tinkering_Disassemble_Patches), false },

            // Widgets
            { nameof(RandomDebris), false },
            { nameof(RandomTree), false },
            { nameof(WallOrDebris), false },
            { nameof(WallOrNot), false },
        };

        public static bool getClassDoDebug(string Class)
        {
            if (classDoDebug.ContainsKey(Class)) 
                return classDoDebug[Class];

            return doDebug;
        }

        public static int Colorfulness
        {
            get
            {
                return Convert.ToInt32(GetOption("Option_GigantismPlus_Colorfulness"));
            }
            private set 
            {
                Colorfulness = value;
            }
        }

        // Debug Settings
        public static int DebugVerbosity
        {
            get => GetIntOption(nameof(DebugVerbosity), 0);
            set => SetIntOption(nameof(DebugVerbosity), value);
        }
        public static bool DebugIncludeInMessage
        {
            get => GetBoolOption(nameof(DebugIncludeInMessage), false);
            set => SetBoolOption(nameof(DebugIncludeInMessage), value);
        }
        public static bool DebugVaultDescriptions
        {
            get => GetBoolOption(nameof(DebugVaultDescriptions), false);
            set => SetBoolOption(nameof(DebugVaultDescriptions), value);
        }

        // Starting Gear Settings
        public static bool EnableGiganticStartingGear
        {
            get => GetBoolOption(nameof(EnableGiganticStartingGear), false);
            set => SetBoolOption(nameof(EnableGiganticStartingGear), value);
        }
        public static bool EnableGiganticStartingGear_Grenades
        {
            get => GetBoolOption(nameof(EnableGiganticStartingGear_Grenades), false);
            set => SetBoolOption(nameof(EnableGiganticStartingGear_Grenades), value);
        }

        // Mutation Interactions Settings
        public static bool EnableGigantismRapidAdvance
        {
            get => GetBoolOption(nameof(EnableGigantismRapidAdvance), false);
            set => SetBoolOption(nameof(EnableGigantismRapidAdvance), value);
        }

        // Tinkering Settings
        public static bool EnableGiganticTinkering
        {
            get => GetBoolOption(nameof(EnableGiganticTinkering), false);
            set => SetBoolOption(nameof(EnableGiganticTinkering), value);
        }
        public static bool EnableGiganticDerarification
        {
            get => GetBoolOption(nameof(EnableGiganticDerarification), false);
            set => SetBoolOption(nameof(EnableGiganticDerarification), value);
        }

        // NPC Equipment Settings
        public static bool EnableGiganticNPCGear
        {
            get => GetBoolOption(nameof(EnableGiganticNPCGear), false);
            set => SetBoolOption(nameof(EnableGiganticNPCGear), value);
        }
        public static bool EnableGiganticNPCGear_Grenades
        {
            get => GetBoolOption(nameof(EnableGiganticNPCGear_Grenades), false);
            set => SetBoolOption(nameof(EnableGiganticNPCGear_Grenades), value);
        }

        // Advanced Settings
        public static bool EnableManagedVanillaMutationsCurrent => true; // GetOption("Option_GigantismPlus_ManagedVanilla").EqualsNoCase("Yes");
        public static bool? EnableManagedVanillaMutations = null;

        /*
        // OnClick Handlers
        public static bool OnOptionManagedVanilla()
        {
            Debug.Entry(4, $"@ {nameof(Options)}.{nameof(OnOptionManagedVanilla)}", Indent: 0);
            ManagedVanillaMutationOptionHandler();
            Debug.Entry(4, $"x {nameof(Options)}.{nameof(OnOptionManagedVanilla)} @//", Indent: 0);
            return true;
        }
        */

    } //!-- public static class Options
}
