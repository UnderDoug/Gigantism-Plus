using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AddDamageAttribute : MeleeWeaponAdjustment
    {
        public string AttributeName;

        public AddDamageAttribute()
            : base()
        {
            AttributeName = null;
        }

        public AddDamageAttribute(string Attributes, string AttributeName = null)
            : this()
        {
            Value = Attributes;
            this.AttributeName = AttributeName;
        }
        public AddDamageAttribute(AddDamageAttribute SourceAdjustment)
            : base(SourceAdjustment)
        {
        }
        public AddDamageAttribute(MeleeWeapon Source, string AttributeName = null)
            : this(Source.Attributes, AttributeName)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Prioritize = false;
            Verb = "add";
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            if (Value != null)
            {
                Effect = $"{AttributeName} to {Subject.poss("damage")} types";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && Value != null)
            {
                MeleeWeapon meleeWeapon = Subject.GetPart<MeleeWeapon>();
                string attributes = meleeWeapon.Attributes;
                if (!attributes.IsNullOrEmpty() && attributes.Contains(Value))
                {
                    return false;
                }
                else if (!attributes.IsNullOrEmpty())
                {
                    attributes += " ";
                }
                attributes += Value;
                meleeWeapon.Attributes = attributes;
                return true;
            }
            return false;
        }
    }
}
