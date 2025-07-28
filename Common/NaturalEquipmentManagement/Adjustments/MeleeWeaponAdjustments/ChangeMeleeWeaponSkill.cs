using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class ChangeMeleeWeaponSkill : MeleeWeaponAdjustment
    {
        public ChangeMeleeWeaponSkill()
            : base()
        {
        }

        public ChangeMeleeWeaponSkill(string Skill = null)
            : this()
        {
            Value = Skill;
        }
        public ChangeMeleeWeaponSkill(ChangeMeleeWeaponSkill Source)
            : base(Source)
        {
        }
        public ChangeMeleeWeaponSkill(MeleeWeapon Source)
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
