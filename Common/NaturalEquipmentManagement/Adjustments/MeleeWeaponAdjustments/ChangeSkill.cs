using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeSkill : MeleeWeaponAdjustment
    {
        public string Skill;

        public ChangeSkill()
            : base()
        {
            Skill = null;
        }

        public ChangeSkill(string Skill = null)
            : this()
        {
            this.Skill = Skill;
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
                Subject.GetPart<MeleeWeapon>().Skill = Skill;
                return true;
            }
            return false;
        }
    }
}
