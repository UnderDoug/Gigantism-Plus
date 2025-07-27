using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeSkill : MeleeWeaponAdjustment
    {
        public ChangeSkill()
            : base()
        {
        }

        public ChangeSkill(string Skill = null)
            : this()
        {
            Value = Skill;
        }
        public ChangeSkill(ChangeSkill Source)
            : base(Source)
        {
        }
        public ChangeSkill(MeleeWeapon Source)
            : this(Source.Skill)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Skill = Value;
                return true;
            }
            return false;
        }
    }
}
