using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModNaturalEquipment<T> 
        : ModNaturalEquipmentBase
        , IDescribeModificationHandler<ModNaturalEquipment<T>>
        // , IModEventHandler<BeforeDescribeModificationEvent<ModNaturalEquipment<T>>>
        // , IModEventHandler<DescribeModificationEvent<ModNaturalEquipment<T>>>
        where T 
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("ModNaturalEquipment");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                nameof(DescriptionElement) + ":Final",
            };
            List<object> dontList = new()
            {
                "AM",    // Apply Mod
                "APP",   // Apply Part & Prop
                nameof(DescriptionElement) + ":Collection",
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public T AssigningPart => 
            Manager?.GetManagedNaturalEquipmentCompatiblePart<T>() 
         ?? Operator?.Manager?.GetManagedNaturalEquipmentCompatiblePart<T>();

        public ModNaturalEquipment()
        {
        }
        public ModNaturalEquipment(NaturalEquipmentManager NewManager)
            : base(NewManager)
        {
        }
        public ModNaturalEquipment(ModNaturalEquipment<T> Source)
            : base(Source)
        {
        }
        public ModNaturalEquipment(NaturalEquipmentManager NewManager, ModNaturalEquipment<T> Source)
            : base(NewManager, Source)
        {
        }

        public override ModNaturalEquipmentBase AddAdjustment(IAdjustment Adjustment, int Priority, bool FlipPriority, ICondition<GameObject> Condition = null)
        {
            int indent = Debug.LastIndent;
            Adjustments ??= new();
            Adjustment.Source ??= GetType();
            base.AddAdjustment(Adjustment, Priority, FlipPriority, Condition);
            Debug.LastIndent = indent;
            return this;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Operator.Manager ??= who?.RequirePart<NaturalEquipmentManager>();
            if (AssigningPart == null || AssigningPart.GetType() != typeof(T))
            {
                Debug.Warn(2,
                    $"{typeof(ModNaturalEquipment<T>).Name}<{GetSource()}>",
                    $"{nameof(BeingAppliedBy)}(" +
                    $"obj: {obj?.DebugName ?? NULL}, " +
                    $"who: {who?.DebugName ?? NULL})",
                    $"Failed to assign {typeof(T).Name} as {nameof(AssigningPart)}",
                    Indent: 0);
            }
            return base.BeingAppliedBy(obj, who);
        }
        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, 
                $"@ {Name}[{GetSource()}]."
                + $"{nameof(ApplyModification)}"
                + $"(Object: \"{Object.BaseDisplayName}\")", 
                Indent: 3, Toggle: getDoDebug("AM"));
            
            // Do Code?

            Debug.Entry(4, 
                $"x {Name}[{GetSource()}]."
                + $"{nameof(ApplyModification)}"
                + $"(Object: \"{Object.BaseDisplayName}\")"
                + $" @//", 
                Indent: 3, Toggle: getDoDebug("AM"));
            base.ApplyModification(Object);
        }
        public override string GetSource()
        {
            return typeof(T).Name;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(BeforeDescribeModificationEvent<ModNaturalEquipment<T>>.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(DescribeModificationEvent<ModNaturalEquipment<T>>.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade);
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(GetColoredAdjective(), ModPriority);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeDescribeModificationEvent<ModNaturalEquipment<T>> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                int indent = Debug.LastIndent;
                bool doDebugDuringCollection = getDoDebug(nameof(DescriptionElement) + ":Collection");
                bool doDebugFinal = getDoDebug(nameof(DescriptionElement) + ":Final");
                Debug.LoopItem(4, 
                    $"{GetType().Name}." +
                    $"{nameof(HandleEvent)}(" +
                    $"{nameof(BeforeDescribeModificationEvent<ModNaturalEquipment<T>>)} E)", 
                    Indent: indent + 1, Toggle: true);

                if (!Adjustments.IsNullOrEmpty() && !Adjustments.GetApplied().IsNullOrEmpty())
                {
                    List<DescriptionElement> combinedPrimaryElements = new();
                    List<DescriptionElement> combinedSecondaryElements = new();
                    Debug.CheckYeh(4, $"Have Adjustments", Indent: indent + 2, Toggle: getDoDebug());
                    foreach (IAdjustment adjustment in Adjustments.GetApplied())
                    {
                        Debug.Entry(4, $"{adjustment.ToString(true)}", Indent: indent + 3, Toggle: getDoDebug());
                        if (adjustment.TryGetDescriptionElements(ParentObject, out List<DescriptionElement> primaryElements, out List<DescriptionElement> secondaryElements))
                        {
                            if (!primaryElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(primaryElements)}", Indent: indent + 4, Toggle: doDebugDuringCollection);
                                foreach (DescriptionElement element in primaryElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: doDebugDuringCollection);
                                }
                                combinedPrimaryElements.AddRange(primaryElements);
                                E.PrimaryDescriptions.AddRange(primaryElements);
                            }
                            if (!secondaryElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(secondaryElements)}", Indent: indent + 4, Toggle: doDebugDuringCollection);
                                foreach (DescriptionElement element in secondaryElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: doDebugDuringCollection);
                                }
                                combinedSecondaryElements.AddRange(primaryElements);
                                E.SecondaryDescriptions.AddRange(secondaryElements);
                            }
                        }
                    }
                    Debug.LoopItem(4, $"{GetType().Name} {nameof(E.PrimaryDescriptions)}", Indent: indent + 2, Toggle: doDebugFinal);
                    if (!combinedPrimaryElements.IsNullOrEmpty())
                    {
                        foreach (DescriptionElement element in combinedPrimaryElements)
                        {
                            Debug.LoopItem(4, $"{element.ToString(ParentObject)}", Indent: indent + 3, Toggle: doDebugFinal);
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, $"Empty", Indent: indent + 3, Toggle: doDebugFinal);
                    }

                    Debug.LoopItem(4, $"{GetType().Name} {nameof(E.SecondaryDescriptions)}", Indent: indent + 2, Toggle: doDebugFinal);
                    if (!combinedSecondaryElements.IsNullOrEmpty())
                    {
                        foreach (DescriptionElement element in combinedSecondaryElements)
                        {
                            Debug.LoopItem(4, $"{element.ToString(ParentObject)}", Indent: indent + 3, Toggle: doDebugFinal);
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, $"Empty", Indent: indent + 3, Toggle: doDebugFinal);
                    }
                }

                Debug.LastIndent = indent;
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(DescribeModificationEvent<ModNaturalEquipment<T>> E)
        {
            return base.HandleEvent(E);
        }

        public override string GetAdjective()
        {
            return Adjective ?? typeof(T).Name;
        }
        public override string GetInstanceDescription(GameObject Object = null)
        {
            return DescribeModificationEvent<ModNaturalEquipment<T>>
                .Send(Object, GetColoredAdjective(), Context: NATURAL_EQUIPMENT)
                .Process();
        }
        
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModNaturalEquipment<T> modNaturalEquipment = base.DeepCopy(Parent, MapInv) as ModNaturalEquipment<T>;
            return ClearForCopy(modNaturalEquipment);
        }
        public override IPart DeepCopy(GameObject Parent)
        {
            ModNaturalEquipment<T> naturalEquipmentMod = base.DeepCopy(Parent) as ModNaturalEquipment<T>;
            return ClearForCopy(naturalEquipmentMod);
        }
        public static ModNaturalEquipment<T> ClearForCopy(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod;
        }
    }
}