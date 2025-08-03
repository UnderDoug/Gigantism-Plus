using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AddPart<T> : AddPartAdjustment
        where T : IPart, new()
    {
        public AddPart()
            : base()
        {
        }
        public AddPart(List<DescriptionElement> PrimaryDescriptionElements, List<DescriptionElement> SecondaryDescriptionElements)
            : base(PrimaryDescriptionElements ?? new(), SecondaryDescriptionElements ?? new())
        {
        }
        public AddPart(DescriptionElement PrimaryDescriptionElement, DescriptionElement SecondaryDescriptionElement)
            : base(PrimaryDescriptionElement, SecondaryDescriptionElement)
        {
        }
        public AddPart(AddPartAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(T).Name
            };
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && !Subject.HasPart<T>();
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.AddPart<T>();
            }
            return IsApplied();
        }
    }
}
