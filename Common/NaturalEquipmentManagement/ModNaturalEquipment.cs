using HNPS_GigantismPlus;
using Sheeter;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
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
            };
            List<object> dontList = new()
            {
                "AM",    // Apply Mod
                "APP",   // Apply Part & Prop
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
                Debug.LoopItem(4, 
                    $"{GetType().Name}.{nameof(HandleEvent)}({nameof(BeforeDescribeModificationEvent<ModNaturalEquipment<T>>)} E)", 
                    Indent: indent + 1, Toggle: true);

                if (!Adjustments.IsNullOrEmpty() && !Adjustments.GetApplied().IsNullOrEmpty())
                {
                    Debug.CheckYeh(4, $"Have Adjustments", Indent: indent + 2, Toggle: true);
                    foreach (IAdjustment adjustment in Adjustments.GetApplied())
                    {
                        Debug.Entry(4, $"{adjustment.ToString(true)}", Indent: indent + 3, Toggle: true);
                        if (adjustment.TryGetDescriptionElements(ParentObject, out List<DescriptionElement> weaponElements, out List<DescriptionElement> generalElements))
                        {
                            if (!weaponElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(weaponElements)}", Indent: indent + 4, Toggle: true);
                                foreach (DescriptionElement element in weaponElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: true);
                                }
                                E.PrimaryDescriptions.AddRange(weaponElements);
                            }
                            if (!generalElements.IsNullOrEmpty())
                            {
                                Debug.LoopItem(4, $"{nameof(generalElements)}", Indent: indent + 4, Toggle: true);
                                foreach (DescriptionElement element in generalElements)
                                {
                                    Debug.LoopItem(4, $"{element}", Indent: indent + 5, Toggle: true);
                                }
                                E.SecondaryDescriptions.AddRange(generalElements);
                            }
                        }
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