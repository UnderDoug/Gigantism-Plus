using System;
using XRL.World;
using XRL.World.Anatomy;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class BodyPartVariantIs : ICondition<BodyPart>
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
        public BodyPartVariantIs(GameObjectBlueprintIs Source)
            : this(Source?.Variant)
        {
        }

        public override bool Check(BodyPart BodyPart)
        {
            return Variant.IsNullOrEmpty()
                || BodyPart == null
                || BodyPart?.VariantType == Variant;
        }
    }
}
