using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustDamageDieSize : MeleeWeaponPerformanceAdjustment
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
        public AdjustDamageDieSize(MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage die size";
            Verb = "gain";
        }
        public AdjustDamageDieSize(int Amount, MeleeWeaponPerformanceAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieSize((int)Amount);
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && !Amount.IsNullOrZero())
            {
                string amount = ((int)Amount).Signed();
                Effect = $"{amount} {AffectedParameter}";
                return new(DescriptionElement.ORDER_ADJUST_EXTREMELY_EARLY + 2, Verb, Effect);
            }
            return base.GetPrimaryDescriptionElement(Subject);
        }
    }
}
