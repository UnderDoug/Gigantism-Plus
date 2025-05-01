using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class VaultedEvent : ModPooledEvent<VaultedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_NONE;

    public GameObject Vaulter;
    public Cell OriginCell;
    public GameObject Vaultee;
    public Cell DestinationCell;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(VaultedEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Vaulter = null;
        OriginCell = null;
        Vaultee = null;
        DestinationCell = null;
    }

    public static void Send(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell DestinationCell)
    {
        Debug.Entry(4, 
            $"{typeof(VaultedEvent).Name}." + 
            $"{nameof(Send)}(Vaulter: {Vaulter?.DebugName}, OriginCell: [{OriginCell?.Location}], Vaulter: {Vaultee?.DebugName}, DestinationCell: [{DestinationCell?.Location}])", 
            Indent: 0);

        VaultedEvent E = FromPool();

        bool VaulterWantsMin = Vaulter.WantEvent(ID, E.GetCascadeLevel());
        bool VaulteeWantsMin = Vaultee.WantEvent(ID, E.GetCascadeLevel());
        bool VaulterWantsStr = Vaulter.HasRegisteredEvent(E.GetRegisteredEventID());
        bool VaulteeWantsStr = Vaultee.HasRegisteredEvent(E.GetRegisteredEventID());
        bool AnyWantsMin = VaulterWantsMin || VaulteeWantsMin;
        bool AnyWantsStr = VaulterWantsStr || VaulteeWantsStr;
        bool AnyWants = AnyWantsMin || AnyWantsStr;

        if (AnyWants)
        {
            if (AnyWantsMin)
            {
                E.Vaulter = Vaulter;
                E.OriginCell = OriginCell;
                E.Vaultee = Vaultee;
                E.DestinationCell = DestinationCell;
                if (VaulterWantsMin) Vaulter.HandleEvent(E);
                if (VaulteeWantsMin) Vaultee.HandleEvent(E);
            }
            if (AnyWantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(OriginCell), OriginCell);
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(DestinationCell), DestinationCell);
                if (VaulterWantsMin) Vaulter.FireEvent(@event);
                if (VaulteeWantsMin) Vaultee.FireEvent(@event); 
            }
        }
        E.Reset();
    }
}