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
            Verb = "get";
        }
        public ChangeMeleeWeaponStat(string Skill)
            : this()
        {
            Value = Skill;
        }
        public ChangeMeleeWeaponStat(Type Source, string Skill)
            : this(Skill)
        {
            this.Source = Source;
        }
        public ChangeMeleeWeaponStat(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            Verb = "get";
        }
        public ChangeMeleeWeaponStat(MeleeWeapon Source)
            : this(Source.Stat)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Effect = $"bonus penetration from {Value}";
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            return new(Verb, Effect);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && Subject.GetPart<MeleeWeapon>().Stat != Value)
            {
                Subject.GetPart<MeleeWeapon>().Stat = Value;
                return true;
            }
            return false;
        }
    }
}
