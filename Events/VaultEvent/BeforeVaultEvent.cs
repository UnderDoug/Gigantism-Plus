using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class BeforeVaultEvent : ModPooledEvent<BeforeVaultEvent>
{
    public new static readonly int CascadeLevel = CASCADE_NONE;

    public GameObject Vaulter;
    public Cell OriginCell;
    public GameObject Vaultee;
    public Cell DestinationCell;
    public string Message;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(BeforeVaultEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Vaulter = null;
        OriginCell = null;
        Vaultee = null;
        DestinationCell = null;
    }

    public static bool CheckFor(GameObject Vaulter, Cell OriginCell, GameObject Vaultee, Cell DestinationCell, out string Message)
    {
        Debug.Entry(4, 
            $"{typeof(BeforeVaultEvent).Name}." + 
            $"{nameof(CheckFor)}(Vaulter: {Vaulter?.DebugName}, OriginCell: [{OriginCell?.Location}], Vaulter: {Vaultee?.DebugName}, DestinationCell: [{DestinationCell?.Location}])", 
            Indent: 0);

        BeforeVaultEvent E = FromPool();

        bool VaulterWantsMin = Vaulter.WantEvent(ID, E.GetCascadeLevel());
        bool VaulteeWantsMin = Vaultee.WantEvent(ID, E.GetCascadeLevel());
        bool VaulterWantsStr = Vaulter.HasRegisteredEvent(E.GetRegisteredEventID());
        bool VaulteeWantsStr = Vaultee.HasRegisteredEvent(E.GetRegisteredEventID());
        bool AnyWantsMin = VaulterWantsMin || VaulteeWantsMin;
        bool AnyWantsStr = VaulterWantsStr || VaulteeWantsStr;
        bool AnyWants = AnyWantsMin || AnyWantsStr;

        E.Message = string.Empty;
        Message = E.Message;

        bool flag = true;
        if (AnyWants)
        {
            if (flag && AnyWantsMin)
            {
                E.Vaulter = Vaulter;
                E.OriginCell = OriginCell;
                E.Vaultee = Vaultee;
                E.DestinationCell = DestinationCell;
                if (flag && VaulterWantsMin) flag = Vaulter.HandleEvent(E);
                if (flag && VaulteeWantsMin) flag = Vaultee.HandleEvent(E);
                if (!flag)
                {
                    Message = E.Message;
                }
            }
            if (flag && AnyWantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(OriginCell), OriginCell);
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(DestinationCell), DestinationCell);
                @event.SetParameter(nameof(Message), E.Message);
                if (flag && VaulterWantsMin) flag = Vaulter.FireEvent(@event);
                if (flag && VaulteeWantsMin) flag = Vaultee.FireEvent(@event); 
                if (!flag)
                {
                    Message = @event.GetStringParameter(nameof(Message));
                }
            }
        }
        E.Reset();
        return flag;
    }
}