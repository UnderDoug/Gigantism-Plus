using System;
using System.Collections.Generic;
using System.Text;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AddPartMetal : AddPart<Inorganic>
    {
        public bool DoDescription;

        private static DescriptionElement DefaultDescription => new(
            Priority: DescriptionElement.ORDER_ADJUST_VERY_LATE,
            Verb: "",
            Effect: $"made of {Grammar.MakeLowerCase(nameof(Metal))}");

        public AddPartMetal()
            : base()
        {
            DoDescription = true;
            SecondaryDescriptionElement = DefaultDescription;
        }
        public AddPartMetal(bool DoDescription)
            : this()
        {
            this.DoDescription = DoDescription;
        }
        public AddPartMetal(AddPartAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            DoDescription = true;
            SecondaryDescriptionElement = DefaultDescription;
        }
        public AddPartMetal(bool DoDescription, AddPartAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.DoDescription = DoDescription;
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject)
        {
            return DoDescription ? base.GetSecondaryDescriptionElement(Subject) : DescriptionElement.Empty;
        }
    }
}
