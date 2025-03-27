using XRL;
using XRL.World;
using XRL.World.Parts;

[GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
public class AfterModGiganticAppliedEvent : ModPooledEvent<AfterModGiganticAppliedEvent>
{
    public new static readonly int CascadeLevel = CASCADE_ALL;

    public GameObject Object;

    public ModGigantic Modification;

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
        Modification = null;
    }

    public static void Send(GameObject Object, ModGigantic Modification)
    {
        bool flag = true;
        if (flag && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
        {
            AfterModGiganticAppliedEvent E = FromPool();
            E.Object = Object;
            E.Modification = Modification;
            flag = The.Game.HandleEvent(E) || Object.HandleEvent(E);
        }
        if (flag && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("AfterModGiganticAppliedEvent"))
        {
            Event @event = Event.New("AfterModGiganticAppliedEvent");
            @event.SetParameter("Object", Object);
            @event.SetParameter("Modification", Modification);
            Object.FireEvent(@event);
        }
    }
}