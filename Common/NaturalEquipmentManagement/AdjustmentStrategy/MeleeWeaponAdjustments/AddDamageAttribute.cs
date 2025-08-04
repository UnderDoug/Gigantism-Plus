using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
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

        public override bool Check(GameObject Subject)
        {
            return !Value.IsNullOrEmpty() 
                && base.Check(Subject) 
                && (Subject.GetPart<MeleeWeapon>().Attributes.IsNullOrEmpty() || !Subject.GetPart<MeleeWeapon>().Attributes.Contains(Value));
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                MeleeWeapon meleeWeapon = Subject.GetPart<MeleeWeapon>();
                meleeWeapon.Attributes ??= "";
                if (!meleeWeapon.Attributes.IsNullOrEmpty())
                {
                    meleeWeapon.Attributes += " ";
                }
                meleeWeapon.Attributes += Value;
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject)
        {
            if (Value != null)
            {
                Effect = $"{AttributeName} to {Subject.poss("damage")} types";
                return new(DescriptionElement.ORDER_ADJUST_SLIGHTLY_LATE, Verb, Effect);
            }
            return base.GetPrimaryDescriptionElement(Subject);
        }
    }
}
