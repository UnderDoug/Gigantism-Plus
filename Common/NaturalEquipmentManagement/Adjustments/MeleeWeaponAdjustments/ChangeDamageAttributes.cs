using System;
using System.Collections.Generic;
using System.Text;

using XRL.Language;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
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

        public override bool Check(GameObject Subject)
        {
            return Value != null 
                && Subject.GetPart<MeleeWeapon>().Attributes != Value 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Attributes = Value;
            }
            return IsApplied();
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject)
        {
            if (Value != null)
            {
                string attributes = Value;
                if (attributes.Contains(' '))
                {
                    List<string> attributeList = new(attributes.Split(" "));
                    attributes = Grammar.MakeAndList(attributeList);
                }
                attributes ??= AttributeName;
                Effect = $"{attributes} type damage";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }
    }
}
