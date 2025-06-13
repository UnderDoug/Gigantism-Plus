using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;
using XRL.World;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL
{
    [Serializable]
    [HasGameBasedStaticCache]
    public class UD_QudWrasslingEntertainment : IScribedSystem
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_QudWrasslingEntertainment));
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

        public static UD_QudWrasslingEntertainment System;

        [GameBasedCacheInit]
        public static void WrassleSystemInit()
        {
            System = The.Game?.RequireSystem(InitializeSystem);
        }
        public static UD_QudWrasslingEntertainment InitializeSystem()
        {
            return new();
        }

        public override void Register(XRLGame Game, IEventRegistrar Registrar)
        {
            // Example 
            Registrar.Register(AwardingXPEvent.ID, EventOrder.EXTREMELY_EARLY);
        }

        public override bool HandleEvent(AwardingXPEvent E)
        {
            // Example 
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
