﻿using System;
using System.Collections.Generic;

using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_EQUIPMENT | CASCADE_SLOTS, Cache = Cache.Pool)]
    public class AfterManageDefaultNaturalEquipmentEvent : IManageDefaultNaturalEquipmentEvent<AfterManageDefaultNaturalEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(AfterManageDefaultNaturalEquipmentEvent));

        public AfterManageDefaultNaturalEquipmentEvent()
        {
        }
    }
}
