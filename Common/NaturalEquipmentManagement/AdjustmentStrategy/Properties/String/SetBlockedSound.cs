using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class SetBlockedSound : SetStringProperty
    {
        public SetBlockedSound()
            : base()
        {
            PropertyName = "BlockedSound";
        }
        public SetBlockedSound(string Value)
            : this()
        {
            this.Value = Value;
        }
        public SetBlockedSound(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            PropertyName = "BlockedSound";
            Value = SourceAdjustment.Value;
        }
        public SetBlockedSound(string Value, SetStringProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            PropertyName = "BlockedSound";
            this.Value = Value;
        }
    }
}
