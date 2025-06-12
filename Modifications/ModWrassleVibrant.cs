using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModWrassleVibrant : IModification
    {
        private static bool doDebug => getClassDoDebug(nameof(ModWrassleVibrant));
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



        public ModWrassleVibrant()
        {
        }

    } //!-- public class ModWrassleVibrant : IModification
}