using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class PartIsNaturalModification<T> : PartIsModification<ModNaturalEquipment<T>>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        public PartIsNaturalModification()
            : base()
        {
        }
        public PartIsNaturalModification(PartIsModification<ModNaturalEquipment<T>> Source)
            : base(Source)
        {
        }

        public override bool Check(IPart IPart)
        {
            return base.Check(IPart);
        }
    }
}
