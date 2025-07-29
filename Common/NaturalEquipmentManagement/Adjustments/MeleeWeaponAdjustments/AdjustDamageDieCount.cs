using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustDamageDieCount : MeleeWeaponCumulativeAdjustment
    {
        public AdjustDamageDieCount()
            : base("damage die")
        {
            Verb = "gain";
        }
        public AdjustDamageDieCount(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustDamageDieCount(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustDamageDieCount(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "damage die";
            Verb = "gain";
        }
        public AdjustDamageDieCount(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }

        public override DescriptionElement GetWeaponDescriptionElement(GameObject Subject = null)
        {
            if (AffectedParameter != null && Amount > 0)
            {
                string amount = ((int)Amount).Signed();
                Effect = $"{amount} additional {AffectedParameter}";
                return new(Verb, Effect);
            }
            return base.GetWeaponDescriptionElement(Subject);
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && Amount > 0)
            {
                Subject.GetPart<MeleeWeapon>().AdjustDamageDieCount((int)Amount);
                return true;
            }
            return false;
        }
    }
}
