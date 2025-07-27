using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static XRL.World.Parts.ModNaturalEquipmentBase;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class EarlyAfterApplyAdjustmentEvent : IAdjustmentEvent<EarlyAfterApplyAdjustmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(EarlyAfterApplyAdjustmentEvent));

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
        public override string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }
    }
}