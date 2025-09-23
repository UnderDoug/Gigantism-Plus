using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class SetStringProperty : PropertyAdjustment
    {
        public SetStringProperty()
            : base()
        {
        }
        public SetStringProperty(string PropertyName)
            : base(PropertyName)
        {
        }
        public SetStringProperty(string PropertyName, string Value)
            : this(PropertyName)
        {
            this.Value = Value;
        }
        public SetStringProperty(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            Value = SourceAdjustment.Value;
        }
        public SetStringProperty(string Value, SetStringProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Value = Value;
        }
        public SetStringProperty(SetIntProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            Value = $"{SourceAdjustment.Amount}";
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && Subject.GetStringProperty(PropertyName) != Value;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.SetStringProperty(PropertyName, Value, true);
            }
            return IsApplied();
        }
    }
}
