using HNPS_GigantismPlus;
using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

[GameEvent(Cascade = CASCADE_NONE, Cache = Cache.Pool)]
public class GetPrioritisedNaturalEquipmentModsEvent : ModPooledEvent<GetPrioritisedNaturalEquipmentModsEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(GetPrioritisedNaturalEquipmentModsEvent));

    public new static readonly int CascadeLevel = CASCADE_NONE; // CASCADE_EQUIPMENT + CASCADE_SLOTS + CASCADE_EXCEPT_THROWN_WEAPON;

    public static readonly string RegisteredEventID = nameof(GetPrioritisedNaturalEquipmentModsEvent);

    public GameObject Creature;

    public GameObject Equipment;

    public BodyPart TargetBodyPart;

    public SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods;

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
        Creature = null;
        Equipment = null;
        TargetBodyPart = null;
        NaturalEquipmentMods = null;
    }

    public SortedDictionary<int, ModNaturalEquipmentBase> AddNaturalEquipmentMods(List<ModNaturalEquipmentBase> NaturalEquipmentMods)
    {
        foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
        {
            AddNaturalEquipmentMod(naturalEquipmentMod);
        }
        return this.NaturalEquipmentMods;
    }
    public SortedDictionary<int, ModNaturalEquipmentBase> AddNaturalEquipmentMod(ModNaturalEquipmentBase NaturalEquipmentMod)
    {
        int indent = Debug.LastIndent;
        Debug.Entry(4,
            $"@ {nameof(GetPrioritisedNaturalEquipmentModsEvent)}."
            + $"{nameof(AddNaturalEquipmentMod)}"
            + $"(NaturalWeaponMod: {NaturalEquipmentMod.Name})",
            Indent: indent, Toggle: doDebug);

        NaturalEquipmentMods ??= new();
        if (NaturalEquipmentMods.ContainsKey(NaturalEquipmentMod.ModPriority))
        {
            Debug.Warn(2,
                $"{nameof(NaturalEquipmentManager)}",
                $"{nameof(AddNaturalEquipmentMod)}()",
                $"[{NaturalEquipmentMod.ModPriority}]" +
                $"{NaturalEquipmentMods[NaturalEquipmentMod.ModPriority]} " +
                $"in {nameof(NaturalEquipmentMods)} overwritten: Same ModPriority",
                Indent: indent + 1);
        }
        ModNaturalEquipmentBase naturalEquipmentModCopy = NaturalEquipmentMod.DeepCopy(Equipment) as ModNaturalEquipmentBase;
        NaturalEquipmentMods[NaturalEquipmentMod.ModPriority] = naturalEquipmentModCopy;
        Debug.Entry(4, $"NaturalEquipmentMods:", Indent: indent + 1, Toggle: doDebug);
        foreach ((int priority, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
        {
            Debug.CheckYeh(4, $"{priority}::{naturalEquipmentMod.Name}:{naturalEquipmentMod.GetColoredAdjective()}", Indent: indent + 2, Toggle: doDebug);
        }
        Debug.Entry(4,
            $"x {nameof(GetPrioritisedNaturalEquipmentModsEvent)}."
            + $"{nameof(AddNaturalEquipmentMod)}"
            + $"(NaturalWeaponMod: {NaturalEquipmentMod.Name}) @//",
            Indent: indent, Toggle: doDebug);

        Debug.LastIndent = indent;

        return NaturalEquipmentMods;
    }
    public static SortedDictionary<int, ModNaturalEquipmentBase> GetFor(GameObject Creature, GameObject Equipment, BodyPart TargetBodyPart)
    {
        Debug.Entry(4,
        $"! {nameof(GetPrioritisedNaturalEquipmentModsEvent)}."
        + $"{nameof(GetFor)}"
        + $"(Creature: {Creature?.DebugName ?? NULL},"
        + $" Equipment: {Equipment?.DebugName ?? NULL}"
        + $" TargetLimb: {TargetBodyPart?.DebugName() ?? NULL})",
        Indent: 0, Toggle: doDebug);

        GetPrioritisedNaturalEquipmentModsEvent E = FromPool();

        E.NaturalEquipmentMods = new();
        E.Creature = Creature;
        E.Equipment = Equipment;
        E.TargetBodyPart = TargetBodyPart;

        bool haveCreature = Creature != null;

        bool wantsMin = haveCreature && Creature.WantEvent(ID, CascadeLevel);
        bool wantsStr = haveCreature && Creature.HasRegisteredEvent(RegisteredEventID);

        bool anyWants = wantsMin || wantsStr;

        bool proceed = anyWants;
        if (proceed)
        {
            if (proceed && wantsMin)
            {
                proceed = Creature.HandleEvent(E);
            }
            if (proceed && wantsStr)
            {
                Event @event = Event.New(E.GetRegisteredEventID());
                @event.SetParameter(nameof(Creature), E.Creature);
                @event.SetParameter(nameof(Equipment), E.Equipment);
                @event.SetParameter(nameof(TargetBodyPart), E.TargetBodyPart);
                @event.SetParameter(nameof(NaturalEquipmentMods), E.NaturalEquipmentMods);
                proceed = Creature.FireEvent(@event);
                E.NaturalEquipmentMods = @event.GetParameter(nameof(NaturalEquipmentMods)) as SortedDictionary<int, ModNaturalEquipmentBase>;
            }
        }
        SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = new(E.NaturalEquipmentMods);
        E.Reset();
        return naturalEquipmentMods;
    }
}