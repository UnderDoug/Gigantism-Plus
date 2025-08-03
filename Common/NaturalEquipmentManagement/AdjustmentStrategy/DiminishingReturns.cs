using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class DiminishingReturns : ArbitraryDescription
    {
        public string Affected;

        public DiminishingReturns()
            : base()
        {
            Prioritize = false;
            Affected = null;
            DescriptionOrder = DescriptionElement.ORDER_ADJUST_VERY_LATE;
        }
        public DiminishingReturns(string Affected)
            : this()
        {
            this.Affected = Affected;
        }
        public DiminishingReturns(int DescriptionOrder, string Affected)
            : this()
        {
            this.Affected = Affected;
            this.DescriptionOrder = DescriptionOrder;
        }
        public DiminishingReturns(Type Source, string Affected)
            : this(Affected)
        {
            this.Source = Source;
        }
        public DiminishingReturns(Type Source, int Priority, string Affected)
            : this(Source, Affected)
        {
            this.Priority = Priority;
        }
        public DiminishingReturns(Type Source, int Priority, int DescriptionOrder, string Affected)
            : this(Source, Priority, Affected)
        {
            this.DescriptionOrder = DescriptionOrder;
        }
        public DiminishingReturns(DiminishingReturns SourceAdjustment)
            : base(SourceAdjustment)
        {
            Prioritize = false;
            DescriptionOrder = DescriptionElement.ORDER_ADJUST_VERY_LATE;
        }
        public DiminishingReturns(string Affected, DiminishingReturns SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Affected = Affected;
        }
        public DiminishingReturns(Type Source, string Affected, DiminishingReturns SourceAdjustment)
            : this(Affected, SourceAdjustment)
        {
            this.Source = Source;
        }
        public DiminishingReturns(Type Source, int Priority, string Affected, DiminishingReturns SourceAdjustment)
            : this(Source, Affected, SourceAdjustment)
        {
            this.Priority = Priority;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                Affected.Quote(),
            };
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject) && !GetSecondaryDescriptionElement(Subject).IsEmpty();
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject)
        {
            if (!Affected.IsNullOrEmpty())
            {
                Effect = $"suffering diminishing returns on {Affected}";
                SecondaryDescription = new(DescriptionElement.ORDER_ADJUST_VERY_LATE, null, Effect);
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }
    }
}
