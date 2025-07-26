using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeStat : MeleeWeaponAdjustment
    {
        public string Stat;

        public ChangeStat()
            : base()
        {
            Stat = null;
        }

        public ChangeStat(string Skill = null)
            : this()
        {
            this.Stat = Skill;
        }
        public ChangeStat(ChangeStat Source)
            : base(Source)
        {
        }
        public ChangeStat(MeleeWeapon Source)
            : this(Source.Stat)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Stat = Stat;
                return true;
            }
            return false;
        }
    }
}
