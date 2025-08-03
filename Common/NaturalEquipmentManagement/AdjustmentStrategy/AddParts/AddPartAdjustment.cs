using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public abstract class AddPartAdjustment : IAdjustment
    {
        public List<DescriptionElement> PrimaryDescriptionElements;
        public List<DescriptionElement> SecondaryDescriptionElements;

        public DescriptionElement PrimaryDescriptionElement;
        public DescriptionElement SecondaryDescriptionElement;

        public AddPartAdjustment()
            : base()
        {
            PrimaryDescriptionElements = new();
            SecondaryDescriptionElements = new();
            PrimaryDescriptionElement = DescriptionElement.Empty;
            SecondaryDescriptionElement = DescriptionElement.Empty;
        }
        public AddPartAdjustment(List<DescriptionElement> PrimaryDescriptionElements, List<DescriptionElement> SecondaryDescriptionElements)
            : this()
        {
            this.PrimaryDescriptionElements = PrimaryDescriptionElements ?? new();
            this.SecondaryDescriptionElements = SecondaryDescriptionElements ?? new();
        }
        public AddPartAdjustment(DescriptionElement PrimaryDescriptionElement, DescriptionElement SecondaryDescriptionElement)
            : this()
        {
            this.PrimaryDescriptionElement = PrimaryDescriptionElement;
            this.SecondaryDescriptionElement = SecondaryDescriptionElement;
        }
        public AddPartAdjustment(AddPartAdjustment SourceAdjustment)
            : this(SourceAdjustment.PrimaryDescriptionElements, SourceAdjustment.SecondaryDescriptionElements)
        {
            PrimaryDescriptionElement = SourceAdjustment.PrimaryDescriptionElement;
            SecondaryDescriptionElement = SourceAdjustment.SecondaryDescriptionElement;
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject)
        {
            return !PrimaryDescriptionElement.IsEmpty() ? PrimaryDescriptionElement : base.GetPrimaryDescriptionElement(Subject);
        }
        public override DescriptionElement GetSecondaryDescriptionElement(GameObject Subject)
        {
            return !SecondaryDescriptionElement.IsEmpty() ? SecondaryDescriptionElement : base.GetSecondaryDescriptionElement(Subject);
        }

        public override List<DescriptionElement> GetPrimaryDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = base.GetPrimaryDescriptionElements(Subject);
            if (!PrimaryDescriptionElements.IsNullOrEmpty())
            {
                descriptionElements.AddRange(PrimaryDescriptionElements);
            }
            return descriptionElements;
        }
        public override List<DescriptionElement> GetSecondaryDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = base.GetSecondaryDescriptionElements(Subject);
            if (!SecondaryDescriptionElements.IsNullOrEmpty())
            {
                descriptionElements.AddRange(SecondaryDescriptionElements);
            }
            return descriptionElements;
        }
    }
}
