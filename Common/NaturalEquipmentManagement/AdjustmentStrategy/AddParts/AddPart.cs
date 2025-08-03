using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AddPart : AddPartAdjustment
    {
        public string Part;
        public string Namespace;
        public AddPart()
            : base()
        {
            Part = null;
            Namespace = null;
        }
        public AddPart(string Part)
            : this()
        {
            this.Part = Part;
        }
        public AddPart(string Part, string Namespace)
            : this(Part)
        {
            this.Namespace = Namespace;
        }
        public AddPart(List<DescriptionElement> PrimaryDescriptionElements, List<DescriptionElement> SecondaryDescriptionElements)
            : base(PrimaryDescriptionElements ?? new(), SecondaryDescriptionElements ?? new())
        {
            Part = null;
            Namespace = null;
        }
        public AddPart(DescriptionElement PrimaryDescriptionElement, DescriptionElement SecondaryDescriptionElement)
            : base(PrimaryDescriptionElement, SecondaryDescriptionElement)
        {
            Part = null;
            Namespace = null;
        }
        public AddPart(string Part, string Namespace, List<DescriptionElement> PrimaryDescriptionElements = null, List<DescriptionElement> SecondaryDescriptionElements = null)
            : base(PrimaryDescriptionElements ?? new(), SecondaryDescriptionElements ?? new())
        {
            this.Part = Part;
            this.Namespace = Namespace;
        }
        public AddPart(string Part, string Namespace, DescriptionElement PrimaryDescriptionElement, DescriptionElement SecondaryDescriptionElement)
            : base(PrimaryDescriptionElement, SecondaryDescriptionElement)
        {
            this.Part = Part;
            this.Namespace = Namespace;
        }
        public AddPart(AddPartAdjustment SourceAdjustment)
            : base(SourceAdjustment.PrimaryDescriptionElements ?? new(), SourceAdjustment.SecondaryDescriptionElements ?? new())
        {
            Part = null;
            Namespace = null;
        }
        public AddPart(string Part, AddPartAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            this.Part = Part;
        }
        public AddPart(string Part, string Namespace, AddPartAdjustment SourceAdjustment)
            : this(Part, SourceAdjustment)
        {
            this.Namespace = Namespace;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                Part
            };
        }

        public override bool Check(GameObject Subject)
        {
            return base.Check(Subject)
                && !Part.IsNullOrEmpty()
                && (new GamePartBlueprint(Part) is not null || new GamePartBlueprint(Namespace, Part) is not null)
                && !Subject.HasPart(Part);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.RequirePart(Part, Namespace);
            }
            return IsApplied();
        }
    }
}
