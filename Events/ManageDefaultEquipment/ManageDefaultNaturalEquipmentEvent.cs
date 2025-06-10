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
    [GameEvent(Cascade = CASCADE_EQUIPMENT, Cache = Cache.Pool)]
    public class ManageDefaultNaturalEquipmentEvent : IManageDefaultNaturalEquipmentEvent<ManageDefaultNaturalEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(ManageDefaultNaturalEquipmentEvent));

        public ManageDefaultNaturalEquipmentEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
    }
}