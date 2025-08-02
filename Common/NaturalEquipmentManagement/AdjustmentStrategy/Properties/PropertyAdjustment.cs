using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class PropertyAdjustment : IAdjustment
    {
        public string PropertyName;

        public PropertyAdjustment()
            : base()
        {
            PropertyName = null;
        }
        public PropertyAdjustment(string PropertyName)
            : this()
        {
            this.PropertyName = PropertyName;
        }
        public PropertyAdjustment(PropertyAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            PropertyName = SourceAdjustment.PropertyName;
        }

        public override bool Check(GameObject Subject)
        {
            return !PropertyName.IsNullOrEmpty()
                && base.Check(Subject);
        }
    }
}
