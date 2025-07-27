using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AddAttributes : MeleeWeaponAdjustment
    {
        public AddAttributes()
            : base()
        {
        }

        public AddAttributes(string Attributes = null)
            : this()
        {
            Value = Attributes;
        }
        public AddAttributes(AddAttributes Source)
            : base(Source)
        {
        }
        public AddAttributes(MeleeWeapon Source)
            : this(Source.Attributes)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                MeleeWeapon meleeWeapon = Subject.GetPart<MeleeWeapon>();
                string attributes = meleeWeapon.Attributes;
                if (!attributes.IsNullOrEmpty())
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
