using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AddPartAdjustment<T> : IAdjustment
        where T : IPart, new()
    {
        public List<DescriptionElement> WeaponDescriptionElements;
        public List<DescriptionElement> GeneralDescriptionElements;

        public AddPartAdjustment()
            : base()
        {
            WeaponDescriptionElements = new();
            GeneralDescriptionElements = new();
        }
        public AddPartAdjustment(List<DescriptionElement> WeaponDescriptionElements = null, List<DescriptionElement> GeneralDescriptionElements = null)
            : this()
        {
            this.WeaponDescriptionElements = WeaponDescriptionElements ?? new();
            this.GeneralDescriptionElements = GeneralDescriptionElements ?? new();
        }
        public AddPartAdjustment(AddPartAdjustment<T> SourceAdjustment)
            : this(SourceAdjustment.WeaponDescriptionElements, SourceAdjustment.GeneralDescriptionElements)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
        }

        public override string AddToString()
        {
            return $"{typeof(T).Name}";
        }

        public override bool Check(GameObject Subject)
        {
            return !Subject.HasPart<T>() 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.AddPart<T>();
            }
            return IsApplied();
        }

        public override List<DescriptionElement> GetGeneralDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = base.GetGeneralDescriptionElements(Subject);
            if (!GeneralDescriptionElements.IsNullOrEmpty())
            {
                descriptionElements.AddRange(GeneralDescriptionElements);
            }
            return descriptionElements;
        }
        public override List<DescriptionElement> GetWeaponDescriptionElements(GameObject Subject)
        {
            List<DescriptionElement> descriptionElements = base.GetWeaponDescriptionElements(Subject);
            if (!WeaponDescriptionElements.IsNullOrEmpty())
            {
                descriptionElements.AddRange(WeaponDescriptionElements);
            }
            return descriptionElements;
        }
    }
}
