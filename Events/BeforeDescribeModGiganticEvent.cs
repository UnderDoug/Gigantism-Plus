using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

[GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
public class BeforeDescribeModGiganticEvent : ModPooledEvent<BeforeDescribeModGiganticEvent>
{
    public new static readonly int CascadeLevel = CASCADE_ALL;

    public GameObject Object;

    public ModGigantic ModPart;

    public string ObjectNoun;

    public List<List<string>> WeaponDescriptions;

    public List<List<string>> GeneralDescriptions;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public override bool Dispatch(IEventHandler Handler)
    {
        return Handler.HandleEvent(this);
    }

    public override void Reset()
    {
        base.Reset();
        Object = null;
        ObjectNoun = null;
        ModPart = null;
        WeaponDescriptions = null;
        GeneralDescriptions = null;
    }

    public static void Send(GameObject Object, ModGigantic ModPart, string ObjectNoun, List<List<string>> WeaponDescriptions, List<List<string>> GeneralDescriptions)
    {
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("BeforeDescribeModGiganticEvent"))
        {
            Event @event = Event.New("BeforeDescribeModGiganticEvent");
            @event.SetParameter("Object", Object);
            @event.SetParameter("ModPart", ModPart);
            @event.SetParameter("ObjectNoun", ObjectNoun);
            @event.SetParameter("WeaponDescriptions", WeaponDescriptions);
            @event.SetParameter("GeneralDescriptions", GeneralDescriptions);
            flag = Object.FireEvent(@event);
        }
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            BeforeDescribeModGiganticEvent beforeDescribeModGiganticEvent = FromPool();
            beforeDescribeModGiganticEvent.Object = Object;
            beforeDescribeModGiganticEvent.ModPart = ModPart;
            beforeDescribeModGiganticEvent.ObjectNoun = ObjectNoun;
            beforeDescribeModGiganticEvent.WeaponDescriptions = WeaponDescriptions;
            beforeDescribeModGiganticEvent.GeneralDescriptions = GeneralDescriptions;
            Object.HandleEvent(beforeDescribeModGiganticEvent);
        }
    }
}