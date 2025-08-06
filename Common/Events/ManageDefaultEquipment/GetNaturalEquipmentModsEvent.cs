using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class GetNaturalEquipmentModsEvent : ModPooledEvent<GetNaturalEquipmentModsEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(GetNaturalEquipmentModsEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL; // CASCADE_EQUIPMENT | CASCADE_SLOTS;

        public static readonly string RegisteredEventID = nameof(GetNaturalEquipmentModsEvent);

        public GameObject Creature;

        public GameObject Equipment;

        public BodyPart TargetBodyPart;

        public List<ModNaturalEquipmentBase> NaturalEquipmentMods;

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

        public List<ModNaturalEquipmentBase> AddNaturalEquipmentMod(ModNaturalEquipmentBase NaturalEquipmentMod)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(GetNaturalEquipmentModsEvent)}."
                + $"{nameof(AddNaturalEquipmentMod)}"
                + $"(NaturalEquipmentMod: {NaturalEquipmentMod.Name}[{NaturalEquipmentMod.Adjective}])"
                + $" Creature: {Creature?.DebugName ?? NULL},"
                + $" Equipment: {Equipment?.DebugName ?? NULL}",
                Indent: indent, Toggle: doDebug);

            NaturalEquipmentMods ??= new();
            if (NaturalEquipmentMod != null)
            {
                ModNaturalEquipmentBase naturalEquipmentModCopy = NaturalEquipmentMod.DeepCopy(Equipment) as ModNaturalEquipmentBase;
                NaturalEquipmentMods.Add(naturalEquipmentModCopy);
            }
            else
            {
                Debug.Warn(2,
                    $"{nameof(NaturalEquipmentOperator)}",
                    $"{nameof(AddNaturalEquipmentMod)}()",
                    $"Supplied {nameof(NaturalEquipmentMod)} was null",
                    Indent: indent + 1);
            }
            Debug.Entry(4, $"{nameof(NaturalEquipmentMods)}:", Indent: indent + 1, Toggle: doDebug);
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
                {
                    Debug.CheckYeh(4, $"{naturalEquipmentMod.Name}:{naturalEquipmentMod.GetColoredAdjective()}",
                        Indent: indent + 2, Toggle: doDebug);
                }
            }
            else
            {
                Debug.CheckNah(4, $"Empty List", Indent: indent + 2, Toggle: doDebug);
            }
            Debug.Entry(4,
                $"x {nameof(GetNaturalEquipmentModsEvent)}."
                + $"{nameof(AddNaturalEquipmentMod)}"
                + $"(NaturalEquipmentMod: {NaturalEquipmentMod.Name}) @//",
                Indent: indent, Toggle: doDebug);

            Debug.LastIndent = indent;
            return NaturalEquipmentMods;
        }
        public List<ModNaturalEquipmentBase> AddNaturalEquipmentMod<T>(ModNaturalEquipment<T> NaturalEquipmentMod)
            where T
            : IPart
            , IManagedDefaultNaturalEquipment<T>
            , new()
        {
            return AddNaturalEquipmentMod((ModNaturalEquipmentBase)NaturalEquipmentMod);
        }

        public List<ModNaturalEquipmentBase> AddNaturalEquipmentMods(List<ModNaturalEquipmentBase> NaturalEquipmentMods)
        {
            int indent = Debug.LastIndent;
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipmentBase naturalEquipmentMod in NaturalEquipmentMods)
                {
                    AddNaturalEquipmentMod(naturalEquipmentMod);
                }
            }
            else
            {
                Debug.CheckNah(4, $"Empty List", Indent: indent + 2, Toggle: doDebug);
            }

            Debug.LastIndent = indent;
            return this.NaturalEquipmentMods ?? new();
        }
        public List<ModNaturalEquipmentBase> AddNaturalEquipmentMods<T>(List<ModNaturalEquipment<T>> NaturalEquipmentMods)
            where T
            : IPart
            , IManagedDefaultNaturalEquipment<T>
            , new()
        {
            int indent = Debug.LastIndent;
            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipment<T> naturalEquipmentMod in NaturalEquipmentMods)
                {
                    AddNaturalEquipmentMod(naturalEquipmentMod);
                }
            }
            else
            {
                Debug.CheckNah(4, $"Empty List", Indent: indent + 2, Toggle: doDebug);
            }

            Debug.LastIndent = indent;
            return this.NaturalEquipmentMods ?? new();
        }

        public static List<ModNaturalEquipmentBase> GetFor(GameObject Creature, GameObject Equipment, BodyPart TargetBodyPart)
        {
            Debug.Entry(4,
            $"! {nameof(GetNaturalEquipmentModsEvent)}."
            + $"{nameof(GetFor)}"
            + $"(Creature: {Creature?.DebugName ?? NULL},"
            + $" Equipment: {Equipment?.DebugName ?? NULL}"
            + $" TargetLimb: {TargetBodyPart?.DebugName() ?? NULL})",
            Indent: 0, Toggle: doDebug);

            GetNaturalEquipmentModsEvent E = FromPool();

            E.NaturalEquipmentMods = NaturalEquipmentManager.NewNaturalEquipmentModList();
            E.Creature = Creature;
            E.Equipment = Equipment;
            E.TargetBodyPart = TargetBodyPart;

            bool haveCreature = Creature != null;

            bool wantsMin = haveCreature && E.Creature.WantEvent(ID, CascadeLevel);
            bool wantsStr = haveCreature && E.Creature.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = wantsMin || wantsStr;

            bool proceed = anyWants;
            if (proceed)
            {
                if (proceed && wantsMin)
                {
                    proceed = E.Creature.HandleEvent(E);
                }
                if (proceed && wantsStr)
                {
                    Event @event = Event.New(E.GetRegisteredEventID());
                    @event.SetParameter(nameof(E.Creature), E.Creature);
                    @event.SetParameter(nameof(E.Equipment), E.Equipment);
                    @event.SetParameter(nameof(E.TargetBodyPart), E.TargetBodyPart);
                    @event.SetParameter(nameof(NaturalEquipmentMods), E.NaturalEquipmentMods);
                    proceed = Creature.FireEvent(@event);
                    E.NaturalEquipmentMods = @event.GetParameter(nameof(NaturalEquipmentMods)) as List<ModNaturalEquipmentBase>;
                }
            }
            List<ModNaturalEquipmentBase> naturalEquipmentMods = E.NaturalEquipmentMods;
            E.Reset();
            return naturalEquipmentMods;
        }
    }
}