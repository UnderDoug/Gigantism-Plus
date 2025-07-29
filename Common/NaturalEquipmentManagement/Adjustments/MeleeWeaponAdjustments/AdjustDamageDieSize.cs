using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustDamageDieSize : MeleeWeaponCumulativeAdjustment
    {
        public AdjustDamageDieSize()
            : base("damage die size")
        {
            Verb = "gain";
        }
        public AdjustDamageDieSize(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustDamageDieSize(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustDamageDieSize(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage die size";
            Verb = "gain";
        }
        public AdjustDamageDieSize(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override DescriptionElement GetGeneralDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null)
            {
                string amount = ((int)Amount).Signed();
                Effect = $"{amount} {AffectedParameter}";
            }
            return new(Verb, Effect);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieSize((int)Amount);
                return true;
            }
            return false;
        }
    }
}
