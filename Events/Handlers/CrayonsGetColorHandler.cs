using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public class CrayonsGetColorHandler : IEventHandler, IModEventHandler<CrayonsGetColorsEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(CrayonsGetColorHandler));

        private static readonly CrayonsGetColorHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, CrayonsGetColorsEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<CrayonsGetColorHandler, CrayonsGetColorsEvent>();
        }

        public bool HandleEvent(CrayonsGetColorsEvent E)
        {
            if (!E.BrightColors.Contains("z")) E.BrightColors.Add("z");
            if (!E.BrightColors.Contains("x")) E.BrightColors.Add("x");
            if (!E.DarkColors.Contains("Z")) E.DarkColors.Add("Z");
            if (!E.DarkColors.Contains("X")) E.DarkColors.Add("X");
            return true;
        }
    } //!-- public class CrayonsGetColorHandler : IEventHandler, IModEventHandler<CrayonsGetColorsEvent>
}
