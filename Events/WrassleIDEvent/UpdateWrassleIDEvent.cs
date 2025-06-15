using System.Collections.Generic;
using System;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
    public class UpdateWrassleIDEvent : IWrassleIDEvent<UpdateWrassleIDEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(UpdateWrassleIDEvent));

        public UpdateWrassleIDEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
    }
}