using System;
using System.Collections.Generic;
using XRL;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [HasModSensitiveStaticCache]
    public static class Options
    {
        // Per the wiki, code is taken 1:1
        private static string GetOption(string ID, string Default = "")
        {
            return XRL.UI.Options.GetOption(ID, Default: Default);
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

        public static bool SelectGiganticTinkering => GetOption("Option_GigantismPlus_SelectGiganticTinkering").EqualsNoCase("Yes");
        public static bool SelectGiganticDerarification => GetOption("Option_GigantismPlus_SelectGiganticDerarification").EqualsNoCase("Yes");

        // NPC equipment options
        public static bool EnableGiganticNPCGear => GetOption("Option_GigantismPlus_EnableGiganticNPCGear").EqualsNoCase("Yes");
        public static bool EnableGiganticNPCGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticNPCGear_Grenades").EqualsNoCase("Yes");

        public static bool EnableManagedVanillaMutations => GetOption("Option_GigantismPlus_ManagedVanilla").EqualsNoCase("Yes");

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

        // OnClick Handlers
        public static bool OnOptionManagedVanilla()
        {
            Debug.Entry(4, $"@ {nameof(Options)}.{nameof(OnOptionManagedVanilla)}", Indent: 0);
            ManagedVanillaMutation();
            Debug.Entry(4, $"x {nameof(Options)}.{nameof(OnOptionManagedVanilla)} @//", Indent: 0);
            return true;
        }

    } //!-- public static class Options
}
