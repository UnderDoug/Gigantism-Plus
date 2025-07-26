using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeAttributes : AddAttributes
    {
        public ChangeAttributes()
            : base()
        {
            Attributes = null;
        }

        public ChangeAttributes(string Attributes = null)
            : this()
        {
            this.Attributes = Attributes;
        }
        public ChangeAttributes(ChangeAttributes Source)
            : base(Source)
        {
        }
        public ChangeAttributes(MeleeWeapon Source)
            : this(Source.Attributes)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Attributes = Attributes;
                return true;
            }
            return false;
        }
    }
}
