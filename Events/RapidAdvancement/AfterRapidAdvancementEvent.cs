using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
public class AfterRapidAdvancementEvent : ModPooledEvent<AfterRapidAdvancementEvent>
{
    public new static readonly int CascadeLevel = CASCADE_ALL;

    public GameObject Actor;
    public int Amount;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{typeof(AfterRapidAdvancementEvent).Name}";
    }

    public override void Reset()
    {
        base.Reset();
        Actor = null;
        Amount = 0;
    }

    public static void Send(int Amount, GameObject Actor)
    {
        Debug.Entry(4, 
            $"{typeof(AfterRapidAdvancementEvent).Name}." +
            $"{nameof(Send)}(int Amount: {Amount}, GameObject Actor: {Actor?.DebugName})",
            Indent: 0);

        AfterRapidAdvancementEvent E = FromPool();

        bool flag = true;
        if (Actor.WantEvent(ID, E.GetCascadeLevel()))
        {
            E.Amount = Amount;
            E.Actor = Actor;
            flag = Actor.HandleEvent(E);
        }
        if (flag && Actor.HasRegisteredEvent(E.GetRegisteredEventID()))
        {
            Event @event = Event.New(E.GetRegisteredEventID());
            @event.SetParameter(nameof(Amount), Amount);
            @event.SetParameter(nameof(Actor), Actor);
            Actor.FireEvent(@event);
        }
    }
}