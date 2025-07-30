using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class AddBurrowingClawsProperties : AddPartAdjustment<BurrowingClawsProperties>
    {
        public int WallBonusPenetration;
        public double WallBonusPercentage;

        public AddBurrowingClawsProperties()
            : base()
        {
            WallBonusPenetration = 0;
            WallBonusPercentage = 0.0;
        }
        public AddBurrowingClawsProperties(int WallBonusPenetration, double WallBonusPercentage)
            : base()
        {
            this.WallBonusPenetration = WallBonusPenetration;
            this.WallBonusPercentage = WallBonusPercentage;
        }
        public AddBurrowingClawsProperties(List<DescriptionElement> WeaponDescriptionElements = null, List<DescriptionElement> GeneralDescriptionElements = null)
            : this()
        {
            this.WeaponDescriptionElements = WeaponDescriptionElements ?? new();
            this.GeneralDescriptionElements = GeneralDescriptionElements ?? new();
        }
        public AddBurrowingClawsProperties(AddBurrowingClawsProperties SourceAdjustment)
            : this(SourceAdjustment.WeaponDescriptionElements, SourceAdjustment.GeneralDescriptionElements)
        {
        }

        public override bool Check(GameObject Subject)
        {
            return WallBonusPenetration > 0 && WallBonusPercentage > 0.0 && base.Check(Subject);
        }

        public override void AfterApply(GameObject Subject)
        {
            if (Subject.GetPart<BurrowingClawsProperties>() is BurrowingClawsProperties burrowingClawsProperties)
            {
                burrowingClawsProperties.WallBonusPenetration = WallBonusPenetration;
                burrowingClawsProperties.WallBonusPercentage = WallBonusPercentage;

                WeaponDescriptionElements ??= new();
                GeneralDescriptionElements ??= new();

                WeaponDescriptionElements.Add(new("get", $"{WallBonusPenetration.Signed()} penetration vs. walls"));
                GeneralDescriptionElements.Add(new("destroy", $"walls after {Drill.GetWallHitsRequired(WallBonusPercentage)} penetrating hits"));
            }
            base.AfterApply(Subject);
        }
    }
}
