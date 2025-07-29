using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class AdjustBonusCap : MeleeWeaponCumulativeAdjustment
    {
        public AdjustBonusCap()
            : base("penetration cap")
        {
        }
        public AdjustBonusCap(int Amount)
            : this()
        {
            this.Amount = Amount;
        }
        public AdjustBonusCap(Type Source, int Amount)
            : this(Amount)
        {
            this.Source = Source;
        }
        public AdjustBonusCap(MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : base(SourceAdjustment)
        {
            AffectedParameter = "penetration cap";
        }
        public AdjustBonusCap(int Amount, MeleeWeaponCumulativeAdjustment SourceAdjustment)
            : this(SourceAdjustment)
        {
            this.Amount = Amount;
        }
        public AdjustBonusCap(MeleeWeapon Source)
            : this(Source.MaxStrengthBonus)
        {
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject) && !Amount.IsNullOrZero())
            {
                MeleeWeapon meleeWeapon = Subject.GetPart<MeleeWeapon>();
                if ((meleeWeapon.MaxStrengthBonus < 999 && Amount > 0) || Amount < 0)
                {
                    meleeWeapon.AdjustBonusCap((int)Amount);
                    return true;
                }
            }
            return false;
        }
    }
}
