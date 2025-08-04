using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ArbitraryDescription : IAdjustment
    {
        public int DescriptionOrder;

        public DescriptionElement PrimaryDescription;
        public DescriptionElement SecondaryDescription;

        public ArbitraryDescription()
            : base()
        {
            PrimaryDescription = DescriptionElement.Empty;
            SecondaryDescription = DescriptionElement.Empty;
        }
        public ArbitraryDescription(DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription)
            : this()
        {
            this.PrimaryDescription = PrimaryDescription;
            this.SecondaryDescription = SecondaryDescription;
        }
        public ArbitraryDescription(int DescriptionOrder, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription)
            : this(PrimaryDescription, SecondaryDescription)
        {
            this.DescriptionOrder = DescriptionOrder;
            if (!PrimaryDescription.IsEmpty())
            {
                PrimaryDescription.Priority = DescriptionOrder;
            }
            if (!SecondaryDescription.IsEmpty())
            {
                SecondaryDescription.Priority = DescriptionOrder;
            }
        }
        public ArbitraryDescription(Type Source, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription)
            : this(PrimaryDescription, SecondaryDescription)
        {
            this.Source = Source;
        }
        public ArbitraryDescription(Type Source, int Priority, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription)
            : this(Source, PrimaryDescription, SecondaryDescription)
        {
            this.Priority = Priority;
        }
        public ArbitraryDescription(Type Source, int Priority, int DescriptionOrder, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription)
            : this(Source, Priority, PrimaryDescription, SecondaryDescription)
        {
            this.DescriptionOrder = DescriptionOrder;
        }
        public ArbitraryDescription(ArbitraryDescription SourceAdjustment)
            : base(SourceAdjustment)
        {
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
            PrimaryDescription = SourceAdjustment.PrimaryDescription;
            SecondaryDescription = SourceAdjustment.SecondaryDescription;
        }
        public ArbitraryDescription(DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription, ArbitraryDescription SourceAdjustment)
            : base(SourceAdjustment)
        {
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
            this.PrimaryDescription = PrimaryDescription;
            this.SecondaryDescription = SecondaryDescription;
        }
        public ArbitraryDescription(Type Source, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription, ArbitraryDescription SourceAdjustment)
            : this(PrimaryDescription, SecondaryDescription, SourceAdjustment)
        {
            this.Source = Source;
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
        }
        public ArbitraryDescription(Type Source, int Priority, DescriptionElement PrimaryDescription, DescriptionElement SecondaryDescription, ArbitraryDescription SourceAdjustment)
            : this(Source, PrimaryDescription, SecondaryDescription, SourceAdjustment)
        {
            this.Priority = Priority;
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
            DescriptionOrder = 0;
        }

        public override List<string> AddToString()
        {
            List<string> output = new(base.AddToString());
            string primary = null;
            if (!PrimaryDescription.IsEmpty())
            {
                primary = PrimaryDescription;
            }
            string secondary = null;
            if (!SecondaryDescription.IsEmpty())
            {
                secondary = SecondaryDescription;
            }
            if (!primary.IsNullOrEmpty())
            {
                output.Add(secondary != null ? $"{nameof(PrimaryDescription)} {primary}" : primary);
            }
            if (!secondary.IsNullOrEmpty())
            {
                output.Add(primary != null ? $"{nameof(SecondaryDescription)} {secondary}" : secondary);
            }
            return output;
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && (!PrimaryDescription.IsEmpty() || !SecondaryDescription.IsEmpty());
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject)
        {
            if (!PrimaryDescription.IsEmpty())
            {
                return PrimaryDescription;
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject)
        {
            if (!SecondaryDescription.IsEmpty())
            {
                return SecondaryDescription;
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }
    }
}
