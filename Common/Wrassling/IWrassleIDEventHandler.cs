using System;
using System.Collections.Generic;

using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World
{
    public interface IWrassleIDEventHandler
        : IModEventHandler<GetWrassleIDEvent>
        , IModEventHandler<AddWrassleIDEvent>
        , IModEventHandler<UpdateWrassleIDEvent>
        , IModEventHandler<WrassleIDUpdatedEvent>
        , IModEventHandler<SyncWrassleIDEvent>
    {

    }
}
