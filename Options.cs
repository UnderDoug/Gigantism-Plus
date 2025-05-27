using System;
using System.Collections.Generic;

using XRL;
using XRL.World.Parts;
using XRL.World.Parts.Skill;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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

        public static bool doDebug = true;
        public static Dictionary<string, bool> classDoDebug = new()
        {
            { nameof(NaturalEquipmentManager), true },
            { nameof(ModNaturalEquipmentBase), true },
            { "ModNaturalEquipment", true },
            { nameof(GigantismPlus), true },
            { nameof(Tactics_Vault), true },
            { nameof(Vaultable), true },
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

        // Checkbox settings
        public static bool EnableGiganticStartingGear => GetOption("Option_GigantismPlus_EnableGiganticStartingGear").EqualsNoCase("Yes");
        public static bool EnableGiganticStartingGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticStartingGear_Grenades").EqualsNoCase("Yes");
        public static bool EnableGigantismRapidAdvance => GetOption("Option_GigantismPlus_EnableGigantismRapidAdvance").EqualsNoCase("Yes");

        public static bool SelectGiganticTinkering => GetOption("Option_GigantismPlus_EnableGiganticTinkering").EqualsNoCase("Yes");
        public static bool SelectGiganticDerarification => GetOption("Option_GigantismPlus_EnableGiganticDerarification").EqualsNoCase("Yes");

        // NPC equipment options
        public static bool EnableGiganticNPCGear => GetOption("Option_GigantismPlus_EnableGiganticNPCGear").EqualsNoCase("Yes");
        public static bool EnableGiganticNPCGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticNPCGear_Grenades").EqualsNoCase("Yes");

        public static bool EnableManagedVanillaMutationsCurrent => true; // GetOption("Option_GigantismPlus_ManagedVanilla").EqualsNoCase("Yes");
        public static bool? EnableManagedVanillaMutations = null;

        // Debug Settings
        public static int DebugVerbosity
        {
            get
            {
                return Convert.ToInt32(GetOption("Option_GigantismPlus_DebugVerbosity"));
            }
            private set
            {
                DebugVerbosity = value;
            }
        }

        public static bool DebugIncludeInMessage
        {
            get
            {
                return GetOption("Option_GigantismPlus_DebugIncludeInMessage").EqualsNoCase("Yes");
            }
            private set
            {
                DebugIncludeInMessage = value;
            }
        }

        public static bool DebugVaultDescriptions
        {
            get
            {
                return GetOption("Option_GigantismPlus_DebugIncludeVaultDebugDescriptions").EqualsNoCase("Yes");
            }
            private set
            {
                DebugVaultDescriptions = value;
            }
        }

        // OnClick Handlers
        public static bool OnOptionManagedVanilla()
        {
            Debug.Entry(4, $"@ {nameof(Options)}.{nameof(OnOptionManagedVanilla)}", Indent: 0);
            ManagedVanillaMutationOptionHandler();
            Debug.Entry(4, $"x {nameof(Options)}.{nameof(OnOptionManagedVanilla)} @//", Indent: 0);
            return true;
        }

    } //!-- public static class Options
}
