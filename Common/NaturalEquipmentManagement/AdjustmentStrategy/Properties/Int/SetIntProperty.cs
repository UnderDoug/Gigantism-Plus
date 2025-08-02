using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class SetIntProperty : PropertyAdjustment
    {
        public SetIntProperty()
            : base()
        {
        }
        public SetIntProperty(string PropertyName)
            : base(PropertyName)
        {
        }
        public SetIntProperty(string PropertyName, int Amount)
            : this(PropertyName)
        {
            this.Amount = Amount;
        }
        public SetIntProperty(SetIntProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            Amount = SourceAdjustment.Amount;
        }
        public SetIntProperty(int Amount, SetIntProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public SetIntProperty(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            if (int.TryParse(SourceAdjustment.Value, out int amount))
            {
                Amount = amount;
            }
        }

        public override bool Check(GameObject Subject)
        {
            return Amount != null
                && base.Check(Subject)
                && Subject.GetIntProperty(PropertyName) != Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.SetIntProperty(PropertyName, (int)Amount, true);
            }
            return IsApplied();
        }
    }
}
