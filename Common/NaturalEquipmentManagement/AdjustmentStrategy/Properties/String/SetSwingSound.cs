using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class SetSwingSound : SetStringProperty
    {
        public SetSwingSound()
            : base()
        {
            PropertyName = "SwingSound";
        }
        public SetSwingSound(string Value)
            : this()
        {
            this.Value = Value;
        }
        public SetSwingSound(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            PropertyName = "SwingSound";
            Value = SourceAdjustment.Value;
        }
        public SetSwingSound(string Value, SetStringProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            PropertyName = "SwingSound";
            this.Value = Value;
        }
    }
}
