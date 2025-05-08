using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class AutoActTryToMoveEvent : ModPooledEvent<AutoActTryToMoveEvent>
{
    private static bool doDebug => false;

    public new static readonly int CascadeLevel = CASCADE_NONE;

    public GameObject Actor;
    public Cell FromCell;
    public GameObject LastDoor;
    public Cell ToCell;
    public string Direction;
    public bool AllowDigging;
    public bool OpenDoors;
    public bool Peaceful;
    public bool PostMoveHostileCheck;
    public bool PostMoveSidebarCheck;
    public bool? Result;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(AutoActTryToMoveEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Actor = null;
        FromCell = null;
        LastDoor = null;
        ToCell = null;
    }

    public static bool GetFor(out bool? Result, GameObject Actor, Cell FromCell, ref GameObject LastDoor, Cell ToCell = null, string Direction = null, bool AllowDigging = true, bool OpenDoors = true, bool Peaceful = true, bool PostMoveHostileCheck = true, bool PostMoveSidebarCheck = true)
    {
        Debug.Entry(4, 
            $"{typeof(AutoActTryToMoveEvent).Name}." + 
            $"{nameof(GetFor)}()", 
            Indent: 0, Toggle: doDebug);

        AutoActTryToMoveEvent E = FromPool();

        bool ActorWantsMin = Actor.WantEvent(ID, E.GetCascadeLevel());
        bool ToCellWantsMin = ToCell != null && ToCell.WantEvent(ID, E.GetCascadeLevel());
        bool ActorWantsStr = Actor.HasRegisteredEvent(E.GetRegisteredEventID());
        bool ToCellWantsStr = ToCell != null && ToCell.HasObjectWithRegisteredEvent(E.GetRegisteredEventID());
        bool AnyWantsMin = ActorWantsMin || ToCellWantsMin;
        bool AnyWantsStr = ActorWantsStr || ToCellWantsStr;
        bool AnyWants = AnyWantsMin || AnyWantsStr;

        Result = null;
        E.Result = null;
        bool Continue = Result == null;
        if (Continue && AnyWants)
        {
            if (Continue && AnyWantsMin)
            {
                E.Actor = Actor;
                E.FromCell = FromCell;
                E.LastDoor = LastDoor;
                E.ToCell = ToCell;
                E.Direction = Direction;
                E.AllowDigging = AllowDigging;
                E.OpenDoors = OpenDoors;
                E.Peaceful = Peaceful;
                E.PostMoveHostileCheck = PostMoveHostileCheck;
                E.PostMoveSidebarCheck = PostMoveSidebarCheck;
                E.Result = Result;
                if (Continue && ActorWantsMin)
                {
                    Actor.HandleEvent(E);
                    Direction = E.Direction;
                    AllowDigging = E.AllowDigging;
                    OpenDoors = E.OpenDoors;
                    Peaceful = E.Peaceful;
                    PostMoveHostileCheck = E.PostMoveHostileCheck;
                    PostMoveSidebarCheck = E.PostMoveSidebarCheck;
                    Result = E.Result;
                    Continue = Result == null;
                }
                if (Continue && ToCellWantsMin)
                {
                    ToCell?.HandleEvent(E);
                    Direction = E.Direction;
                    AllowDigging = E.AllowDigging;
                    OpenDoors = E.OpenDoors;
                    Peaceful = E.Peaceful;
                    PostMoveHostileCheck = E.PostMoveHostileCheck;
                    PostMoveSidebarCheck = E.PostMoveSidebarCheck;
                    Result = E.Result;
                    Continue = E.Result == null;
                }
            }
            if (Continue && AnyWantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Actor), Actor);
                @event.SetParameter(nameof(FromCell), FromCell);
                @event.SetParameter(nameof(LastDoor), LastDoor);
                @event.SetParameter(nameof(ToCell), ToCell);
                @event.SetParameter(nameof(Direction), Direction);
                @event.SetParameter(nameof(AllowDigging), AllowDigging);
                @event.SetParameter(nameof(OpenDoors), OpenDoors);
                @event.SetParameter(nameof(Peaceful), Peaceful);
                @event.SetParameter(nameof(PostMoveHostileCheck), PostMoveHostileCheck);
                @event.SetParameter(nameof(PostMoveSidebarCheck), PostMoveSidebarCheck);
                @event.SetParameter(nameof(Result), Result);
                if (Continue && ActorWantsStr)
                {
                    Actor.FireEvent(@event);
                    Direction = @event.GetParameter<string>(nameof(Direction));
                    AllowDigging = @event.GetParameter<bool>(nameof(AllowDigging));
                    OpenDoors = @event.GetParameter<bool>(nameof(OpenDoors));
                    Peaceful = @event.GetParameter<bool>(nameof(Peaceful));
                    PostMoveHostileCheck = @event.GetParameter<bool>(nameof(PostMoveHostileCheck));
                    PostMoveSidebarCheck = @event.GetParameter<bool>(nameof(PostMoveSidebarCheck));
                    Result = @event.GetParameter<bool?>(nameof(Result));
                    Continue = Result == null;
                }
                if (Continue && ToCellWantsStr)
                {
                    ToCell?.FireEvent(@event);
                    Direction = @event.GetParameter<string>(nameof(Direction));
                    AllowDigging = @event.GetParameter<bool>(nameof(AllowDigging));
                    OpenDoors = @event.GetParameter<bool>(nameof(OpenDoors));
                    Peaceful = @event.GetParameter<bool>(nameof(Peaceful));
                    PostMoveHostileCheck = @event.GetParameter<bool>(nameof(PostMoveHostileCheck));
                    PostMoveSidebarCheck = @event.GetParameter<bool>(nameof(PostMoveSidebarCheck));
                    Result = @event.GetParameter<bool?>(nameof(Result));
                    Continue = Result == null;
                }
            }
        }
        E.Reset();

        return Continue;
    }
} //!-- public class AutoActTryToMoveEvent : ModPooleddEvent<AutoActTryToMoveEvent>