using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class AfterVaultedEvent : IVaultedEvent<AfterVaultedEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(AfterVaultedEvent));

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public override string GetRegisteredEventID()
    {
        return RegisteredEventID;
    }

    public AfterVaultedEvent()
    {

    }
}