using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ChangeMeleeWeaponSkill : MeleeWeaponAdjustment
    {
        public ChangeMeleeWeaponSkill()
            : base()
        {
            Verb = "function";
        }
        public ChangeMeleeWeaponSkill(string Skill)
            : this()
        {
            Value = Skill;
        }
        public ChangeMeleeWeaponSkill(Type Source, string Skill)
            : this(Skill)
        {
            this.Source = Source;
        }
        public ChangeMeleeWeaponSkill(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            Verb = "function";
        }
        public ChangeMeleeWeaponSkill(MeleeWeapon Source)
            : this(Source.Stat)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return !Value.IsNullOrEmpty() 
                && base.Check(Subject) 
                && Subject.GetPart<MeleeWeapon>().Skill != Value;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Skill = Value;
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (!Value.IsNullOrEmpty())
            {
                string skillName = Skills.GetGenericSkill(Value)?.DisplayName;
                Effect ??= $"as a {skillName}";
                return new(DescriptionElement.ORDER_ADJUST_SLIGHTLY_LATE, Verb, Effect);
            }
            return GetPrimaryDescriptionElement(Subject);
        }
    }
}
