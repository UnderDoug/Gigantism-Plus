using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class ChangeMeleeWeaponStat : MeleeWeaponAdjustment
    {
        public ChangeMeleeWeaponStat()
            : base()
        {
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
        }
        public ChangeMeleeWeaponStat(MeleeWeapon Source)
            : this(Source.Stat)
        {
        }

        public override void Configure()
        {
            base.Configure();
            Verb = "get";
        }

        public override bool Check(GameObject Subject)
        {
            return !Value.IsNullOrEmpty() 
                && base.Check(Subject) 
                && Subject.GetPart<MeleeWeapon>().Stat != Value;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().Stat = Value;
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (!Value.IsNullOrEmpty())
            {
                Effect = $"bonus penetration from {Value}";
                return new(Verb, Effect);
            }
            return GetPrimaryDescriptionElement(Subject);
        }
    }
}
