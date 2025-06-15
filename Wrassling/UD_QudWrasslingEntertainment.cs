using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;
using XRL.World;
using XRL.World.Capabilities;
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

        [NonSerialized]
        public Dictionary<Guid, Dictionary<int, IEnumerable<string>>> WrassleColorSequenceCache;

        public UD_QudWrasslingEntertainment()
        {
            WrassleColorSequenceCache = new();
        }

        public IEnumerable<string> CacheWrassleColorSequence(Guid WrassleID, int Length, IEnumerable<string> Sequence)
        {
            WrassleColorSequenceCache ??= new();
            if (WrassleID == Guid.Empty || Length < 1 || Sequence.IsNullOrEmpty())
            {
                return null;
            }
            WrassleColorSequenceCache.TryAdd(WrassleID, new() { { Length, Sequence } });
            return WrassleColorSequenceCache[WrassleID][Length];
        }
        public IEnumerable<string> GetCachedWrassleColorSequence(Guid WrassleID, int Length)
        {
            WrassleColorSequenceCache ??= new();
            if (WrassleID == Guid.Empty || Length < 1)
            {
                return null;
            }
            if (WrassleColorSequenceCache.IsNullOrEmpty() 
                || WrassleColorSequenceCache[WrassleID].IsNullOrEmpty() 
                || WrassleColorSequenceCache[WrassleID][Length].IsNullOrEmpty())
            {
                CacheWrassleColorSequence(WrassleID, Length, UD_QWE.GetWrassleColorSequence(WrassleID, Length));
            }
            return WrassleColorSequenceCache[WrassleID][Length];
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
