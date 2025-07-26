using System;
using System.Collections.Generic;
using System.Text;

namespace XRL.World
{
    [GameEvent(Cache = Cache.Pool)]
    public class DamageDieCountAdjustedEvent : ModPooledEvent<DamageDieCountAdjustedEvent>
    {
        public GameObject Object;

        public IPart Part;

        public int Amount;

        public override bool Dispatch(IEventHandler Handler)
        {
            return Handler.HandleEvent(this);
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            Part = null;
            Amount = 0;
        }

        public static void Send(GameObject Object, IPart Part, int Amount)
        {
            bool proceed = true;
            if (proceed && GameObject.Validate(ref Object) && Object.HasRegisteredEvent("DamageDieCountAdjustedEvent"))
            {
                Event @event = Event.New("DamageDieCountAdjustedEvent");
                @event.SetParameter("Object", Object);
                @event.SetParameter("Part", Part);
                @event.SetParameter("Amount", Amount);
                proceed = Object.FireEvent(@event);
            }
            if (proceed && GameObject.Validate(ref Object) && Object.WantEvent(ID, CascadeLevel))
            {
                DamageDieCountAdjustedEvent E = FromPool();
                E.Object = Object;
                E.Part = Part;
                E.Amount = Amount;
                proceed = Object.HandleEvent(E);
            }
        }
    }
}
