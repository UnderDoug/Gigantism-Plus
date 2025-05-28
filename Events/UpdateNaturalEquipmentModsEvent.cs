using XRL;
using XRL.World;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Utils;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class UpdateNaturalEquipmentModsEvent : ModPooledEvent<UpdateNaturalEquipmentModsEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(UpdateNaturalEquipmentModsEvent));

    public new static readonly int CascadeLevel = CASCADE_NONE; // CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON;

    public GameObject Creature;

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public virtual string GetRegisteredEventID()
    {
        return $"{nameof(UpdateNaturalEquipmentModsEvent)}";
    }

    public override void Reset()
    {
        base.Reset();
        Creature = null;
    }

    public static void Send(GameObject Creature)
    {
        Debug.Entry(4,
        $"! {nameof(UpdateNaturalEquipmentModsEvent)}."
        + $"{nameof(Send)}"
        + $"(GameObject Object: {Creature?.DebugName ?? NULL})",
        Indent: 0, Toggle: doDebug);

        UpdateNaturalEquipmentModsEvent E = FromPool();

        bool validActor = Creature != null;

        bool wantsMin = validActor && Creature.WantEvent(ID, E.GetCascadeLevel());
        bool wantsStr = validActor && Creature.HasRegisteredEvent(E.GetRegisteredEventID());

        bool anyWants = wantsMin || wantsStr;

        bool proceed = validActor;
        if (anyWants)
        {

            if (proceed && wantsMin)
            {
                E.Creature = Creature;
                proceed = Creature.HandleEvent(E);
            }
            if (proceed && wantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Creature), Creature);
                proceed = Creature.FireEvent(@event);
            }
        }
    }
}