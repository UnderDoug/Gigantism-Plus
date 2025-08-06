using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class PartIsModification<T> : ICondition<IPart>
        where T : IModification
    {
        public PartIsModification()
            : base()
        {
        }
        public PartIsModification(PartIsModification<T> Source)
            : base(Source)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(T).ToStringWithGenerics()
            };
        }

        public override bool Check(IPart IPart)
        {
            return base.Check(IPart)
                && IPart.InheritsFrom<T>();
        }
    }
}
