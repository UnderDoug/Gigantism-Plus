using System;
using XRL.World;
using XRL.World.Anatomy;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class BodyPartVariantIs : Condition<BodyPart>
    {
        public string Variant;

        public BodyPartVariantIs()
            : base()
        {
            Variant = null;
        }
        public BodyPartVariantIs(string Variant = null)
            : this()
        {
            this.Variant = Variant;
        }
        public BodyPartVariantIs(BodyPartVariantIs Source)
            : this(Source?.Variant)
        {
        }

        public override bool Check(BodyPart BodyPart)
        {
            if (BodyPart == null)
            {
                return IfSubjectNull;
            }
            return Variant.IsNullOrEmpty()
                || BodyPart?.VariantType == Variant;
        }
    }
}
