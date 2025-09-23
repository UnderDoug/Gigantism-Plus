using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Base = true, Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public abstract class IVaultedEvent<T> : ModPooledEvent<T>
        where T : IVaultedEvent<T>, new()
{
    private static bool doDebug => getClassDoDebug("IVaultedEvent");

    public new static readonly int CascadeLevel = CASCADE_NONE;
    public static readonly string RegisteredEventID = typeof(T).Name;

    public GameObject Vaulter;
    public Cell OriginCell;
    public Cell OverCell;
    public Cell DestinationCell;
    public string Message;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return RegisteredEventID;
    }

    public override void Reset()
    {
        base.Reset();
        Vaulter = null;
        OriginCell = null;
        OverCell = null;
        DestinationCell = null;
        Message = null;
    }

    public static T FromPool(GameObject Vaulter, Cell OriginCell, Cell OverCell, Cell DestinationCell, string Message = null)
    {
        T E = FromPool();
        if (Vaulter != null && OriginCell != null && OverCell != null && DestinationCell != null)
        {
            E.Vaulter = Vaulter;
            E.OriginCell = OriginCell;
            E.OverCell = OverCell;
            E.DestinationCell = DestinationCell;
            E.Message = Message;
            return E;
        }
        E.Reset();
        return E;
    }
    public static void Send(GameObject Vaulter, Cell OriginCell, Cell OverCell, Cell DestinationCell)
    {
        Debug.Entry(4,
            $"{typeof(T).Name}." +
            $"{nameof(Send)}" +
            $"({nameof(Vaulter)}: {Vaulter?.DebugName}," +
            $" {nameof(OriginCell)}: [{OriginCell?.Location}]," +
            $" {nameof(OverCell)}: [{OverCell?.Location}]," +
            $" {nameof(DestinationCell)}: [{DestinationCell?.Location}])",
            Indent: 0, Toggle: doDebug);

        T E = FromPool(Vaulter, OriginCell, OverCell, DestinationCell);

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

    public static bool CheckFor(GameObject Vaulter, Cell OriginCell, Cell OverCell, Cell DestinationCell, out string Message)
    {
        Debug.Entry(4,
            $"{typeof(T).Name}." +
            $"{nameof(CheckFor)}(" +
            $"{nameof(Vaulter)}: {Vaulter?.DebugName}, " +
            $"{nameof(OriginCell)}: [{OriginCell?.Location}], " +
            $"{nameof(OverCell)}: [{OverCell?.Location}], " +
            $"{nameof(DestinationCell)}: [{DestinationCell?.Location}])",
            Indent: 0, Toggle: doDebug);

        T E = FromPool(Vaulter, OriginCell, OverCell, DestinationCell);

        bool VaulterWantsMin = Vaulter.WantEvent(ID, E.GetCascadeLevel());
        bool OverCellWantsMin = OverCell.WantEvent(ID, E.GetCascadeLevel());
        bool DestinationCellWantsMin = DestinationCell.WantEvent(ID, E.GetCascadeLevel());
        bool AnyWantsMin = VaulterWantsMin || OverCellWantsMin || DestinationCellWantsMin;

        bool VaulterWantsStr = Vaulter.HasRegisteredEvent(E.GetRegisteredEventID());
        bool OverCellWantsStr = OverCell.HasObjectWithRegisteredEvent(E.GetRegisteredEventID());
        bool DestinationCellWantsStr = DestinationCell.HasObjectWithRegisteredEvent(E.GetRegisteredEventID());
        bool AnyWantsStr = VaulterWantsStr || OverCellWantsStr || DestinationCellWantsStr;

        bool AnyWants = AnyWantsMin || AnyWantsStr;

        E.Message = string.Empty;
        Message = E.Message;

        bool check = true;
        if (AnyWants)
        {
            if (check && AnyWantsMin)
            {
                E.Vaulter = Vaulter;
                E.OriginCell = OriginCell;
                E.OverCell = OverCell;
                E.DestinationCell = DestinationCell;
                if (check && VaulterWantsMin) check = Vaulter.HandleEvent(E);
                if (check && OverCellWantsMin) check = OverCell.HandleEvent(E);
                if (check && DestinationCellWantsMin) check = DestinationCell.HandleEvent(E);
                if (!check)
                {
                    Message = E.Message;
                }
            }
            if (check && AnyWantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Vaulter), Vaulter);
                @event.SetParameter(nameof(OriginCell), OriginCell);
                @event.SetParameter(nameof(OverCell), OverCell);
                @event.SetParameter(nameof(DestinationCell), DestinationCell);
                @event.SetParameter(nameof(Message), E.Message);
                if (check && VaulterWantsMin) check = Vaulter.FireEvent(@event);
                if (check && OverCellWantsMin) check = OverCell.FireEvent(@event);
                if (check && DestinationCellWantsMin) check = DestinationCell.FireEvent(@event);
                if (!check)
                {
                    Message = @event.GetStringParameter(nameof(Message));
                }
            }
        }
        E.Reset();
        return check;
    }
}