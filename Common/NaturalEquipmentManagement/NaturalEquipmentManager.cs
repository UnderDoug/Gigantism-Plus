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
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "S",    // Serialisation
                nameof(BodyPartsUpdatedEvent),
                nameof(AfterBodyPartsUpdatedEvent),
                nameof(BeforeUpdateBodyPartsEvent),
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public List<NaturalEquipmentOperator> NaturalEquipmentOperators => GetNaturalEquipmentOperators(ParentObject, this);

        public bool WantsToManage => ParentObject != null && ParentObject.IsCreature;

        public NaturalEquipmentManager()
        {
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
            base.Remove();
        }

        public void SyncOperators()
        {
            foreach (NaturalEquipmentOperator naturalEquipmentOperator in NaturalEquipmentOperators)
            {
                naturalEquipmentOperator.Manager = this;
            }
        }

        public static List<NaturalEquipmentOperator> GetNaturalEquipmentOperators(GameObject Creature, NaturalEquipmentManager Manager)
        {
            GetNaturalEquipmentOperatorsEvent getNaturalEquipmentOperatorsEvent = GetNaturalEquipmentOperatorsEvent.FromPool();
            getNaturalEquipmentOperatorsEvent.Manager = Manager;
            getNaturalEquipmentOperatorsEvent.Creature = Creature;
            getNaturalEquipmentOperatorsEvent.Operators = new();

            getNaturalEquipmentOperatorsEvent.GetForCreature();

            // List<NaturalEquipmentOperator> naturalEquipmentOperators = GetNaturalEquipmentOperatorsEvent.GetForCreature(Creature, Manager);

            List<BodyPart> bodyParts = Creature?.Body?.GetParts();
            if (!bodyParts.IsNullOrEmpty())
            {
                foreach (BodyPart bodyPart in bodyParts)
                {
                    getNaturalEquipmentOperatorsEvent.Equipment = bodyPart.DefaultBehavior;
                    if (getNaturalEquipmentOperatorsEvent.Equipment != null && getNaturalEquipmentOperatorsEvent.Equipment.IsNaturalEquipment())
                    {
                        getNaturalEquipmentOperatorsEvent.GetForEquipment();
                    }
                    getNaturalEquipmentOperatorsEvent.Equipment = bodyPart.Equipped;
                    if (getNaturalEquipmentOperatorsEvent.Equipment != null && getNaturalEquipmentOperatorsEvent.Equipment.IsNaturalEquipment())
                    {
                        getNaturalEquipmentOperatorsEvent.GetForEquipment();
                    }
                }
            }
            List<NaturalEquipmentOperator> naturalEquipmentOperators = getNaturalEquipmentOperatorsEvent.Operators;
            getNaturalEquipmentOperatorsEvent.Reset();

            return naturalEquipmentOperators;
        }

        public IEnumerable<IManagedDefaultNaturalEquipment> GetManagedNaturalEquipmentCompatibleParts()
        {
            List<IManagedDefaultNaturalEquipment> managedDefaultNaturalEquipmentParts = ParentObject?.GetPartsDescendedFrom<IManagedDefaultNaturalEquipment>();
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
            EquipperEquippedEvent.ID,
            BodyPartsUpdatedEvent.ID,
            AfterBodyPartsUpdatedEvent.ID,
        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (WantsToManage && WantEvents.Contains(ID));
        }
        public override bool HandleEvent(EquipperEquippedEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentManager)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(EquipperEquippedEvent)} E)",
                Indent: 0, Toggle: getDoDebug());

            if (E.Actor == ParentObject)
            {
                Debug.Entry(4,
                    $"{nameof(E.Actor)}: {E?.Actor?.DebugName ?? NULL}",
                    Indent: 1, Toggle: getDoDebug());

                if (E.Item.IsNaturalEquipment() && E.Item.TryGetPart(out NaturalEquipmentOperator naturalEquipmentOperator))
                {
                    naturalEquipmentOperator.Manager = this;
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
                + $"{nameof(BeforeUpdateBodyPartsEvent)} E)",
                Indent: 0, Toggle: doDebug);

            if (E.Creature != null && E.Creature == ParentObject)
            {
                Debug.Entry(4,
                    $"Creature: {E?.Creature?.DebugName ?? NULL}",
                    Indent: 1, Toggle: doDebug);

                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
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
                + $"{nameof(BeforeUpdateBodyPartsEvent)}"
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

                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
                    {
                        BodyPart parentLimb = naturalEquipmentOperator?.ParentLimb;
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
                List<NaturalEquipmentOperator> naturalEquipmentOperators = NaturalEquipmentOperators;
                if (!naturalEquipmentOperators.IsNullOrEmpty())
                {
                    foreach (NaturalEquipmentOperator naturalEquipmentOperator in naturalEquipmentOperators)
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

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentManager naturalEquipmentManager = base.DeepCopy(Parent, MapInv) as NaturalEquipmentManager;
            return naturalEquipmentManager;
        }

        public override void FinalizeCopyEarly(GameObject Source, bool CopyEffects, bool CopyID, Func<GameObject, GameObject> MapInv)
        {
            base.FinalizeCopyEarly(Source, CopyEffects, CopyID, MapInv);
            SyncOperators();
        }
    }
}
