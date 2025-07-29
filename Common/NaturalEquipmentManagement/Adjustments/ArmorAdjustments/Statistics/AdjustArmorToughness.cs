using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustArmorToughness : AdjustArmorStatistic
    {
        public AdjustArmorToughness()
            : base(nameof(Armor.Toughness))
        {
        }
        public AdjustArmorToughness(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustArmorToughness(AdjustArmorStatistic SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = nameof(Armor.Toughness);
        }
        public AdjustArmorToughness(int Amount, AdjustArmorStatistic SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustArmorToughness(Armor Source)
            : this(Source.Toughness)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {GetType().Name}.{nameof(Apply)}()", Indent: indent + 1, Toggle: true);

            if (base.Apply(Subject))
            {
                Debug.CheckYeh(4, $"Doing Apply", Indent: indent + 2, Toggle: true);
                Subject.GetPart<Armor>().Toughness += (int)Amount;
                // return false;
            }
            else
            {
                Debug.CheckNah(4, $"Skipping Apply", Indent: indent + 2, Toggle: true);
            }
            Debug.Entry(4, $"x {GetType().Name}.{nameof(Apply)}() *//", Indent: indent + 1, Toggle: true);
            Debug.LastIndent = indent;
            return IsApplied();
        }
    }
}
