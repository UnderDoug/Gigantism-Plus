using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class IPartIsModification<T> : Condition<IPart>
        where T : IModification
    {
        public IPartIsModification()
            : base()
        {
        }
        public IPartIsModification(IPartIsModification<T> Source)
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
            if (IPart == null)
            {
                base.Check(IPart);
            }
            return IPart.InheritsFrom<T>();
        }
    }
}
