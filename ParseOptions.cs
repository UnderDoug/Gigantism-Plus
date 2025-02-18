using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;

namespace Mods.GigantismPlus
{
    public static class Options
    {
        // Per the wiki, code is taken 1:1
        private static string GetOption(string ID, string Default = "")
        {
            return XRL.UI.Options.GetOption(ID, Default: Default);
        }

        // Checkbox settings
        public static bool EnableGiganticStartingGear => GetOption("Option_GigantismPlus_EnableGiganticStartingGear").EqualsNoCase("Yes");
        public static bool EnableGiganticStartingGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticStartingGear_Grenades").EqualsNoCase("Yes");
        public static bool EnableGigantismRapidAdvance => GetOption("Option_GigantismPlus_EnableGigantismRapidAdvance").EqualsNoCase("Yes");

        public static bool SelectGiganticTinkering => GetOption("Option_GigantismPlus_SelectGiganticTinkering").EqualsNoCase("Yes");
        public static bool SelectGiganticDerarification => GetOption("Option_GigantismPlus_SelectGiganticDerarification").EqualsNoCase("Yes");

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
    } //!--- public static class Options
}