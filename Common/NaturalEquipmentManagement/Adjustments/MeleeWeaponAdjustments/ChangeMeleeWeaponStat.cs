using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeMeleeWeaponStat : MeleeWeaponAdjustment
    {
        public ChangeMeleeWeaponStat()
            : base()
        {
        }

        public ChangeMeleeWeaponStat(string Skill = null)
            : this()
        {
            Value = Skill;
        }
        public ChangeMeleeWeaponStat(ChangeMeleeWeaponStat Source)
            : base(Source)
        {
        }
        public ChangeMeleeWeaponStat(MeleeWeapon Source)
            : this(Source.Stat)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Stat = Value;
                return true;
            }
            return false;
        }
    }
}
