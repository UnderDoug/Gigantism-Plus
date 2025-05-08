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
    public Cell OverCell;
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
        OverCell = null;
        DestinationCell = null;
    }

    public static void Send(GameObject Vaulter, Cell OriginCell, Cell OverCell, Cell DestinationCell)
    {
        Debug.Entry(4, 
            $"{typeof(VaultedEvent).Name}." + 
            $"{nameof(Send)}" + 
            $"(Vaulter: {Vaulter?.DebugName}," + 
            $" OriginCell: [{OriginCell?.Location}]," + 
            $" OverCell: [{OverCell?.Location}]," + 
            $" DestinationCell: [{DestinationCell?.Location}])", 
            Indent: 0);

        VaultedEvent E = FromPool();

        bool VaulterWantsMin = Vaulter.WantEvent(ID, E.GetCascadeLevel());
        bool OverCellWantsMin = OverCell.WantEvent(ID, E.GetCascadeLevel());
        bool DestinationCellWantsMin = DestinationCell.WantEvent(ID, E.GetCascadeLevel());
        bool AnyWantsMin = VaulterWantsMin || OverCellWantsMin || DestinationCellWantsMin;

        bool VaulterWantsStr = Vaulter.HasRegisteredEvent(E.GetRegisteredEventID());
        bool OverCellWantsStr = OverCell.HasObjectWithRegisteredEvent(E.GetRegisteredEventID());
        bool DestinationCellWantsStr = DestinationCell.HasObjectWithRegisteredEvent(E.GetRegisteredEventID());
        bool AnyWantsStr = VaulterWantsStr || OverCellWantsStr || DestinationCellWantsStr;

        bool AnyWants = AnyWantsMin || AnyWantsStr;

        if (AnyWants)
        {
            if (AnyWantsMin)
            {
                E.Vaulter = Vaulter;
                E.OriginCell = OriginCell;
                E.OverCell = OverCell;
                E.DestinationCell = DestinationCell;
                if (VaulterWantsMin) Vaulter.HandleEvent(E);
                if (OverCellWantsMin) OverCell.HandleEvent(E);
                if (DestinationCellWantsMin) DestinationCell.HandleEvent(E);
            }
            if (AnyWantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(OriginCell), OriginCell);
                @event.SetParameter(nameof(OverCell), OverCell);
                @event.SetParameter(nameof(DestinationCell), DestinationCell);
                if (VaulterWantsMin) Vaulter.FireEvent(@event);
                if (OverCellWantsMin) OverCell.FireEvent(@event);
                if (DestinationCellWantsMin) DestinationCell.FireEvent(@event);
            }
        }
        E.Reset();
    }
}