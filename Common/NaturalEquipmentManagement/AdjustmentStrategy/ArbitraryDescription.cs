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

        public DescriptionElement WeaponDescription;
        public DescriptionElement GeneralDescription;

        public ArbitraryDescription()
            : base()
        {
            Prioritize = false;
            DescriptionOrder = 0;
            WeaponDescription = DescriptionElement.Empty;
            GeneralDescription = DescriptionElement.Empty;
        }
        public ArbitraryDescription(DescriptionElement WeaponDescription, DescriptionElement GeneralDescription)
            : this()
        {
            this.WeaponDescription = WeaponDescription;
            this.GeneralDescription = GeneralDescription;
        }
        public ArbitraryDescription(int DescriptionOrder, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription)
            : this(WeaponDescription, GeneralDescription)
        {
            this.DescriptionOrder = DescriptionOrder;
            if (WeaponDescription != DescriptionElement.Empty)
            {
                WeaponDescription.Priority = DescriptionOrder;
            }
            if (GeneralDescription != DescriptionElement.Empty)
            {
                GeneralDescription.Priority = DescriptionOrder;
            }
        }
        public ArbitraryDescription(Type Source, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription)
            : this(WeaponDescription, GeneralDescription)
        {
            this.Source = Source;
        }
        public ArbitraryDescription(Type Source, int Priority, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription)
            : this(Source, WeaponDescription, GeneralDescription)
        {
            this.Priority = Priority;
        }
        public ArbitraryDescription(Type Source, int Priority, int DescriptionOrder, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription)
            : this(Source, Priority, WeaponDescription, GeneralDescription)
        {
            this.DescriptionOrder = DescriptionOrder;
        }
        public ArbitraryDescription(ArbitraryDescription SourceAdjustment)
            : base(SourceAdjustment)
        {
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
            WeaponDescription = SourceAdjustment.WeaponDescription;
            GeneralDescription = SourceAdjustment.GeneralDescription;
        }
        public ArbitraryDescription(DescriptionElement WeaponDescription, DescriptionElement GeneralDescription, ArbitraryDescription SourceAdjustment)
            : base(SourceAdjustment)
        {
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
            this.WeaponDescription = SourceAdjustment.WeaponDescription;
            this.GeneralDescription = SourceAdjustment.GeneralDescription;
        }
        public ArbitraryDescription(Type Source, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription, ArbitraryDescription SourceAdjustment)
            : this(WeaponDescription, GeneralDescription, SourceAdjustment)
        {
            this.Source = Source;
            DescriptionOrder = SourceAdjustment.DescriptionOrder;
        }
        public ArbitraryDescription(Type Source, int Priority, DescriptionElement WeaponDescription, DescriptionElement GeneralDescription, ArbitraryDescription SourceAdjustment)
            : this(Source, WeaponDescription, GeneralDescription, SourceAdjustment)
        {
            this.Priority = Priority;
        }

        public override List<string> AddToString()
        {
            List<string> output = new(base.AddToString());
            string weapon = null;
            if (WeaponDescription != DescriptionElement.Empty)
            {
                weapon = WeaponDescription;
            }
            string general = null;
            if (GeneralDescription != DescriptionElement.Empty)
            {
                general = GeneralDescription;
            }
            if (!weapon.IsNullOrEmpty())
            {
                output.Add(general != null ? $"{nameof(WeaponDescription)} {weapon}" : weapon);
            }
            if (!general.IsNullOrEmpty())
            {
                output.Add(weapon != null ? $"{nameof(GeneralDescription)} {general}" : general);
            }
            return output;
        }

        public override bool Check(GameObject Subject)
        {
            return (WeaponDescription != DescriptionElement.Empty || GeneralDescription != DescriptionElement.Empty) && base.Check(Subject);
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            if (WeaponDescription != DescriptionElement.Empty)
            {
                return WeaponDescription;
            }
            return base.GetGeneralDescriptionElement(Subject);
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject)
        {
            if (GeneralDescription != DescriptionElement.Empty)
            {
                return GeneralDescription;
            }
            return base.GetGeneralDescriptionElement(Subject);
        }
    }
}
