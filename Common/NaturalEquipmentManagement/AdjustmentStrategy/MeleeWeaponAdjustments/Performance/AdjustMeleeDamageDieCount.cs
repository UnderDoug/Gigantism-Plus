using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AdjustMeleeDamageDieCount : MeleeWeaponCumulativeAdjustment
    {
        public AdjustMeleeDamageDieCount()
            : base("damage die")
        {
            Verb = "gain";
        }
        public AdjustMeleeDamageDieCount(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustMeleeDamageDieCount(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustMeleeDamageDieCount(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage die";
            Verb = "gain";
        }
        public AdjustMeleeDamageDieCount(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override bool Check(GameObject Subject)
        {
            return Amount > 0 
                && base.Check(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieCount((int)Amount);
            }
            return IsApplied();
        }

        public override DescriptionElement GetPrimaryDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && Amount > 0)
            {
                string amount = ((int)Amount).Signed();
                Effect = $"{amount} additional {AffectedParameter}";
                return new(DescriptionElement.ORDER_ADJUST_EXTREMELY_EARLY + 1, Verb, Effect);
            }
            return base.GetPrimaryDescriptionElement(Subject);
        }
    }
}
