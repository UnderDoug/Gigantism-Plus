using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeDamageAttributes : AddDamageAttribute
    {
        public ChangeDamageAttributes()
            : base()
        {
            Prioritize = true;
            Verb = "deal";
        }
        public ChangeDamageAttributes(string Attributes, string AttributeName = null)
            : this()
        {
            Value = Attributes;
            this.AttributeName = AttributeName;
        }
        public ChangeDamageAttributes(ChangeDamageAttributes SourceAdjustment)
            : base(SourceAdjustment)
        {
            Prioritize = true;
            Verb = "deal";
        }
        public ChangeDamageAttributes(MeleeWeapon Source, string AttributeName = null)
            : this(Source.Attributes, AttributeName)
        {
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            if (Value != null)
            {
                Effect = $"{AttributeName} type damage";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && Value != null && Subject.GetPart<MeleeWeapon>().Attributes != Value)
            {
                Subject.GetPart<MeleeWeapon>().Attributes = Value;
                return true;
            }
            return false;
        }
    }
}
