using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class PartHasAdjustment<T, TAdjustment> : PartIsNaturalModification<T>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
        where TAdjustment
        : IAdjustment
    {
        public PartHasAdjustment()
            : base()
        {
        }
        public PartHasAdjustment(PartIsModification<ModNaturalEquipment<T>> Source)
            : base(Source)
        {
        }

        public virtual TAdjustment GetAdjustment(IPart IPart)
        {
            if (!base.Check(IPart))
            {
                return null;
            }
            if (IPart is ModNaturalEquipment<T> naturalEquipmentMod)
            {
                Adjustments adjustments = naturalEquipmentMod.Adjustments;
                if (!adjustments.IsNullOrEmpty())
                {
                    foreach (IAdjustment adjustment in adjustments)
                    {
                        if (adjustment is TAdjustment targetAdjustment)
                        {
                            return targetAdjustment;
                        }
                    }
                }
            }
            return null;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(TAdjustment).Name,
            };
        }

        public override bool Check(IPart IPart)
        {
            return GetAdjustment(IPart) != null;
        }
    }
}
