using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class DiminishingReturns : IAdjustment
    {
        public string Affected;

        public DiminishingReturns()
            : base()
        {
            Prioritize = false;
            Affected = null;
        }
        public DiminishingReturns(string Affected)
            : this()
        {
            this.Affected = Affected;
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
        public DiminishingReturns(DiminishingReturns SourceAdjustment)
            : base(SourceAdjustment)
        {
            Prioritize = false;
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

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject)
        {
            if (!Affected.IsNullOrEmpty())
            {
                Effect = $"suffering diminishing returns on {Affected}";
                return new(null, Effect);
            }
            return base.GetGeneralDescriptionElement(Subject);
        }
    }
}
