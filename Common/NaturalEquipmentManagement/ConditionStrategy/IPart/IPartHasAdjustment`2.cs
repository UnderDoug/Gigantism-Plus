using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class IPartHasAdjustment<T, TAdjustment> : IPartIsNaturalModification<T>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
        where TAdjustment
        : IAdjustment
    {
        public IPartHasAdjustment()
            : base()
        {
        }
        public IPartHasAdjustment(IPartIsModification<ModNaturalEquipment<T>> Source)
            : base(Source)
        {
        }

        public virtual TAdjustment GetAdjustment(IPart IPart)
        {
            if (IPart == null)
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
                typeof(TAdjustment).ToStringWithGenerics(),
            };
        }

        public override bool Check(IPart IPart)
        {
            if (IPart == null)
            {
                return base.Check(IPart);
            }
            return GetAdjustment(IPart) != null;
        }
    }
}
