using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class SetEquipmentFrameColors : SetStringProperty
    {
        public bool OverrideExisting;

        private string FrameColors => !Value.IsNullOrEmpty() && Value.Length > 4 ? Value[..3] : Value;

        public SetEquipmentFrameColors()
            : base()
        {
            PropertyName = "EquipmentFrameColors";
            OverrideExisting = true;
        }
        public SetEquipmentFrameColors(string Value, bool OverrideExisting = true)
            : this()
        {
            this.Value = Value;
            this.OverrideExisting = OverrideExisting;
        }
        public SetEquipmentFrameColors(SetStringProperty SourceAdjustment)
            : base(SourceAdjustment)
        {
            PropertyName = "EquipmentFrameColors";
            OverrideExisting = true;

            Value = SourceAdjustment.Value;
        }
        public SetEquipmentFrameColors(string Value, SetStringProperty SourceAdjustment)
            : this(SourceAdjustment)
        {
            PropertyName = "EquipmentFrameColors";
            OverrideExisting = true;

            this.Value = Value;
        }
        public SetEquipmentFrameColors(string Value, bool OverrideExisting, SetStringProperty SourceAdjustment)
            : this(Value, SourceAdjustment)
        {
            this.OverrideExisting = OverrideExisting;
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && !FrameColors.IsNullOrEmpty()
                && FrameColors.Length > 3
                && (OverrideExisting || Subject.GetStringProperty(PropertyName, "").IsNullOrEmpty())
                && Subject.GetStringProperty(PropertyName) != FrameColors;
        }

        public override List<string> AddToString()
        {
            Value = FrameColors;
            return base.AddToString();
        }

        public override bool Apply(GameObject Subject)
        {
            Value = FrameColors;
            return base.Apply(Subject);
        }
    }
}
