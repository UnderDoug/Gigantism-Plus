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
            Prioritize = false;
            DescriptionOrder = 0;
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
            if (PrimaryDescription != DescriptionElement.Empty)
            {
                PrimaryDescription.Priority = DescriptionOrder;
            }
            if (SecondaryDescription != DescriptionElement.Empty)
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

        public override List<string> AddToString()
        {
            List<string> output = new(base.AddToString());
            string weapon = null;
            if (PrimaryDescription != DescriptionElement.Empty)
            {
                weapon = PrimaryDescription;
            }
            string general = null;
            if (SecondaryDescription != DescriptionElement.Empty)
            {
                general = SecondaryDescription;
            }
            if (!weapon.IsNullOrEmpty())
            {
                output.Add(general != null ? $"{nameof(PrimaryDescription)} {weapon}" : weapon);
            }
            if (!general.IsNullOrEmpty())
            {
                output.Add(weapon != null ? $"{nameof(SecondaryDescription)} {general}" : general);
            }
            return output;
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && (PrimaryDescription != DescriptionElement.Empty || SecondaryDescription != DescriptionElement.Empty);
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject)
        {
            if (PrimaryDescription != DescriptionElement.Empty)
            {
                return PrimaryDescription;
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }

        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject)
        {
            if (SecondaryDescription != DescriptionElement.Empty)
            {
                return SecondaryDescription;
            }
            return base.GetSecondaryDescriptionElement(Subject);
        }
    }
}
