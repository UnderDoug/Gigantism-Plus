using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ModIntProperty : SetIntProperty
    {
        public ModIntProperty()
            : base()
        {
        }
        public ModIntProperty(string PropertyName)
            : base(PropertyName)
        {
        }
        public ModIntProperty(string PropertyName, int Amount)
            : this(PropertyName)
        {
            this.Amount = Amount;
        }
        public ModIntProperty(ModIntProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            Amount = SourceAdjustment.Amount;
        }
        public ModIntProperty(int Amount, ModIntProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public ModIntProperty(SetIntProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            Amount = SourceAdjustment.Amount;
        }
        public ModIntProperty(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            if (int.TryParse(SourceAdjustment.Value, out int amount))
            {
                Amount = amount;
            }
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject) && !Amount.IsNullOrZero() && Subject.GetIntProperty(PropertyName) != Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.ModIntProperty(PropertyName, (int)Amount, true);
            }
            return IsApplied();
        }
    }
}
