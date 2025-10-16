using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class IPartIsNaturalModification<T> : IPartIsModification<ModNaturalEquipment<T>>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        public IPartIsNaturalModification()
            : base()
        {
        }
        public IPartIsNaturalModification(IPartIsModification<ModNaturalEquipment<T>> Source)
            : base(Source)
        {
        }

        public override bool Check(IPart IPart)
        {
            return base.Check(IPart);
        }
    }
}
