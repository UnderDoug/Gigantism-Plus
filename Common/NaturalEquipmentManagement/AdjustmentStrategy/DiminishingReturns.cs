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
        public bool Constructed;

        public string Affected;

        public DiminishingReturns()
            : base()
        {
            Affected = null;
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
            Constructed = SourceAdjustment.Constructed;
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
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

        public override void Configure()
        {
            base.Configure();
            Effect = $"suffering diminishing returns on ";
            Prioritize = false;
            DescriptionOrder = DescriptionElement.ORDER_ADJUST_VERY_LATE * 2;
            Constructed = false;
        }

        public bool SetSecondaryDescription()
        {
            if (!Constructed && !Affected.IsNullOrEmpty() && !Effect.IsNullOrEmpty())
            {
                Effect += Affected;
                SecondaryDescription = new(DescriptionOrder, null, Effect);
                Constructed = !SecondaryDescription.IsEmpty();
            }
            return Constructed;
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
            return SetSecondaryDescription() && base.Check(Subject);
        }
    }
}
