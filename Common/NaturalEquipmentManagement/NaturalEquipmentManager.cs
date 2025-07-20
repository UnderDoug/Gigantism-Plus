using HNPS_GigantismPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.World.Parts.ModNaturalEquipmentBase;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentManager
        : IScribedPart
        , IModEventHandler<GetNaturalEquipmentOperatorsEvent>
        , IModEventHandler<BeforeUpdateBodyPartsEvent>
        , IModEventHandler<BodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(NaturalEquipmentManager));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "OC",   // ObjectCreation
                nameof(BodyPartsUpdatedEvent),
                nameof(AfterBodyPartsUpdatedEvent),
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "S"     // Serialisation
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public List<NaturalEquipmentOperator> NaturalEquipmentOperators;

        public bool WantsToManage => ParentObject != null && ParentObject.IsCreature;

        public NaturalEquipmentManager()
        {
            NaturalEquipmentOperators = new();
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
        }
        public override void Remove()
        {
            ClearNaturalEquipmentOperators();
            base.Remove();
        }

        public void ClearNaturalEquipmentOperators()
        {
            NaturalEquipmentOperators ??= new();
            if (!NaturalEquipmentOperators.IsNullOrEmpty())
            {
                foreach (NaturalEquipmentOperator naturalEquipmentOperator in  NaturalEquipmentOperators)
                {
                    RemoveOperator(naturalEquipmentOperator);
                }
                NaturalEquipmentOperators = new();
            }
        }
        public bool AddOperator(NaturalEquipmentOperator NaturalEquipmentOperator)
        {
            NaturalEquipmentOperators ??= new();
            NaturalEquipmentOperators.TryAdd(NaturalEquipmentOperator);
            NaturalEquipmentOperator.Manager = this;
            return NaturalEquipmentOperators.Contains(NaturalEquipmentOperator) && NaturalEquipmentOperator.Manager == this;
        }
        public bool RemoveOperator(NaturalEquipmentOperator NaturalEquipmentOperator)
        {
            NaturalEquipmentOperators ??= new();
            NaturalEquipmentOperators.Remove(NaturalEquipmentOperator);
            NaturalEquipmentOperator.Manager = null;
            return !NaturalEquipmentOperators.Contains(NaturalEquipmentOperator) && NaturalEquipmentOperator.Manager != this;
        }

        public bool CollectNaturalEquipmentOperators(GameObject Creature = null)
        {
            Creature ??= ParentObject;
            NaturalEquipmentOperators = new();
            List<BodyPart> bodyParts = Creature?.Body?.GetParts();

            GetNaturalEquipmentOperatorsEvent.GetForCreature(Creature, this);
            if (!bodyParts.IsNullOrEmpty())
            {
                foreach (BodyPart bodyPart in bodyParts)
                {
                    GameObject equipment = bodyPart.DefaultBehavior;
                    if (equipment != null && equipment.IsNaturalEquipment())
                    {
                        GetNaturalEquipmentOperatorsEvent.GetForEquipment(equipment, this);
                    }
                    equipment = bodyPart.Equipped;
                    if (equipment != null && equipment.IsNaturalEquipment())
                    {
                        GetNaturalEquipmentOperatorsEvent.GetForEquipment(equipment, this);
                    }
                }
            }
            return !NaturalEquipmentOperators.IsNullOrEmpty();
        }

        public IEnumerable<IManagedDefaultNaturalEquipment> GetManagedNaturalEquipmentCompatibleParts()
        {
            List<IManagedDefaultNaturalEquipment> managedDefaultNaturalEquipmentParts = ParentObject.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>();
            if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
            {
                foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentPart in ParentObject.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>())
                {
                    yield return managedDefaultNaturalEquipmentPart;
                }
            }
            List<GameObject> installedCyberneticsList = Event.NewGameObjectList(ParentObject?.Body?.GetInstalledCybernetics());
            if (!installedCyberneticsList.IsNullOrEmpty())
            {
                foreach (GameObject installedCybernetic in installedCyberneticsList)
                {
                    managedDefaultNaturalEquipmentParts = installedCybernetic.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>();
                    if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
                    {
                        foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentCybernetic in managedDefaultNaturalEquipmentParts)
                        {
                            yield return managedDefaultNaturalEquipmentCybernetic;
                        }
                    }
                }
            }
            yield break;
        }

        public T GetManagedNaturalEquipmentCompatiblePart<T>()
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            List<IManagedDefaultNaturalEquipment> managedDefaultNaturalEquipmentParts = new(GetManagedNaturalEquipmentCompatibleParts());
            if (!managedDefaultNaturalEquipmentParts.IsNullOrEmpty())
            {
                foreach (IManagedDefaultNaturalEquipment managedDefaultNaturalEquipmentPart in managedDefaultNaturalEquipmentParts)
                {
                    if (managedDefaultNaturalEquipmentPart is T managedNaturalEquipmentCompatiblePart)
                    {
                        return managedNaturalEquipmentCompatiblePart;
                    }
                }
            }
            return null;
        }

        public static List<string> WantStringEvents = new()
        {
        };
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            if (!WantStringEvents.IsNullOrEmpty())
            {
                foreach (string EventID in WantStringEvents)
                {
                    Registrar.Register(EventID);
                }
            }
            base.Register(Object, Registrar);
        }
        public static List<int> WantEvents = new()
        {
            EquippedEvent.ID,
            BodyPartsUpdatedEvent.ID,
            AfterBodyPartsUpdatedEvent.ID,
        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (WantsToManage && WantEvents.Contains(ID));
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(EquippedEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Actor == ParentObject)
            {
                Debug.Entry(4,
                    $"{nameof(E.Actor)}: {E?.Actor?.DebugName ?? NULL}",
                    Indent: 1, Toggle: getDoDebug());

                if (E.Item.IsNaturalEquipment() && E.Item.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator))
                {
                    AddOperator(naturalEquipmentOperator);
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(EquippedEvent)}"
                + $" E.Creature: {E.Actor?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: getDoDebug());

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeUpdateBodyPartsEvent E)
        {
            bool doDebug = getDoDebug(nameof(BeforeUpdateBodyPartsEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)} E)",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                Debug.Entry(4,
                    $"Creature: {E?.Creature?.DebugName ?? NULL}",
                    Indent: 1, Toggle: doDebug);

                if (CollectNaturalEquipmentOperators(E.Creature))
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in NaturalEquipmentOperators)
                    {
                        BodyPart parentLimb = naturalEquipmentOperator.ParentLimb;
                        Debug.LoopItem(4,
                            $"Limb: [{parentLimb?.ID}:{parentLimb?.Type}] {parentLimb?.Description ?? NULL}",
                            Indent: 2, Toggle: doDebug);

                        naturalEquipmentOperator.ClearShortDescriptionCache();
                        naturalEquipmentOperator.HasManaged = false;
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            bool doDebug = getDoDebug(nameof(BodyPartsUpdatedEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)} E)",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                Debug.Entry(4,
                    $"Creature: {E?.Creature?.DebugName ?? NULL}",
                    Indent: 1, Toggle: doDebug);

                if (CollectNaturalEquipmentOperators(E.Creature))
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in NaturalEquipmentOperators)
                    {
                        BodyPart parentLimb = naturalEquipmentOperator.ParentLimb;
                        Debug.LoopItem(4,
                            $"Limb: [{parentLimb?.ID}:{parentLimb?.Type}] {parentLimb?.Description ?? NULL}",
                            Indent: 2, Toggle: doDebug);

                        naturalEquipmentOperator.ClearShortDescriptionCache();
                        naturalEquipmentOperator.HasManaged = false;
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            bool doDebug = getDoDebug(nameof(AfterBodyPartsUpdatedEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AfterBodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL})",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                NaturalEquipmentOperators = new();

                if (CollectNaturalEquipmentOperators(E.Creature))
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in NaturalEquipmentOperators)
                    {
                        GameObject naturalEquipment = naturalEquipmentOperator.ParentObject;
                        BodyPart equipmentLimb = naturalEquipmentOperator.ParentLimb;

                        if (BeforeManageDefaultNaturalEquipmentEvent.CheckFor(
                            Equipment: naturalEquipment, 
                            Creature: ParentObject, 
                            BodyPart: equipmentLimb, 
                            Operator: naturalEquipmentOperator))
                        {
                            ManageDefaultNaturalEquipmentEvent.Send(
                                Equipment: naturalEquipment, 
                                Creature: ParentObject, 
                                BodyPart: equipmentLimb, 
                                Operator: naturalEquipmentOperator).Reset();

                            AfterManageDefaultNaturalEquipmentEvent.Send(
                                Equipment: naturalEquipment, 
                                Creature: ParentObject, 
                                BodyPart: equipmentLimb, 
                                Operator: naturalEquipmentOperator).Reset();
                        }
                    }
                }
            }

            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AfterBodyPartsUpdatedEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: 0, Toggle: doDebug);

            return base.HandleEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(NaturalEquipmentOperators);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            NaturalEquipmentOperators = Reader.ReadList<NaturalEquipmentOperator>() ?? new();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalEquipmentManager = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            naturalEquipmentManager.NaturalEquipmentOperators = new();
            return naturalEquipmentManager;
        }

    } //!-- public class NaturalEquipmentOperator 
      //: IScribedPart
      //, IModEventHandler<BeforeBodyPartsUpdatedEvent>
      //, IModEventHandler<AfterBodyPartsUpdatedEvent>
}
