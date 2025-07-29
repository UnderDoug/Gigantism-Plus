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

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject = null)
        {
            if (Value != null)
            {
                string skillName = Skills.GetGenericSkill(Value)?.GetWeaponCriticalDescription();
                Effect ??= $"as a {skillName}";
                return new(Verb, Effect);
            }
            return GetWeaponDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && Value != null && Subject.GetPart<MeleeWeapon>().Skill != Value)
            {
                Subject.GetPart<MeleeWeapon>().Skill = Value;
                return true;
            }
            return false;
        }
    }
}
