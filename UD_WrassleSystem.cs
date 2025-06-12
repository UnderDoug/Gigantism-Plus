using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using XRL.World;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL
{
    [Serializable]
    [HasGameBasedStaticCache]
    public class UD_WrassleSystem : IScribedSystem
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_WrassleSystem));
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

        public static UD_WrassleSystem System;

        [GameBasedCacheInit]
        public static void WrassleSystemInit()
        {
            System = The.Game?.RequireSystem(InitializeSystem);
        }
        public static UD_WrassleSystem InitializeSystem()
        {
            return new();
        }
        public static Dictionary<string, List<string>> GetColorBag(string Primary = null, string Secondary = null)
        {
            Dictionary<string, List<string>> colorBag = new()
            {
                { "Bright", new() { "W", "Y", "R", "G", "B", "C", "M", } },
                { "Dull", new() {"K", "y", "r", "g", "b", "c", "m", } },
            };
            Primary = Primary.Replace("&", "").Replace("^", "");
            Secondary = Secondary.Replace("&", "").Replace("^", "");
            if (!Primary.IsNullOrEmpty() && Secondary == Primary) 
            {
                Debug.Warn(4,
                    nameof(UD_WrassleSystem),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) is the same as supplied Secondary ({Secondary})");
                return colorBag;
            }
            if (Primary.Length > 1 || Secondary.Length > 1)
            {
                Debug.Warn(4,
                    nameof(UD_WrassleSystem),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) is longer than expected");
                return colorBag;
            }
            if (!colorBag.Contains(Primary) || !colorBag.Contains(Secondary))
            {
                Debug.Warn(4,
                    nameof(UD_WrassleSystem),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) do not exist in {nameof(colorBag)}");
                return colorBag;
            }
            return colorBag;
        }
        public static bool GetWrassleColorPair(Guid WrassleID, out string PrimaryColor, out string SecondaryColor)
        {
            PrimaryColor = null;
            SecondaryColor = null;
            if (WrassleID != Guid.Empty)
            {
                return false;
            }

            Dictionary<string, List<string>> colorBag = GetColorBag();

            PrimaryColor = colorBag.DrawSeededElement(WrassleID);
            // SecondaryColor
            return true;
        }

        public override void Register(XRLGame Game, IEventRegistrar Registrar)
        {
            Registrar.Register(AwardingXPEvent.ID, EventOrder.EXTREMELY_EARLY);
        }

        public override bool HandleEvent(AwardingXPEvent E)
        {
            return base.HandleEvent(E);
        }

        public override void Write(SerializationWriter Writer)
        {
            base.Write(Writer);

            // do writing
        }
        public override void Read(SerializationReader Reader)
        {
            base.Read(Reader);

            // do reading
        }
    }
}
