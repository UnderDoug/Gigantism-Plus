using System;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModCrystallineNaturalWeaponUnmanaged : ModCrystallineNaturalWeapon
    {
        public ModCrystallineNaturalWeaponUnmanaged()
        {
        }

        public ModCrystallineNaturalWeaponUnmanaged(int Tier)
            : base(Tier)
        {
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder ??= who;
            AssigningPart ??= Wielder.GetPart<Crystallinity>().ConvertToManaged();
            NaturalWeapon ??= AssigningPart.GetNaturalWeapon();
            Level = AssigningPart.Level;
            AssigningPart.CalculateNaturalWeaponDamageDieCount(Level);
            AssigningPart.CalculateNaturalWeaponDamageDieSize(Level);
            AssigningPart.CalculateNaturalWeaponDamageBonus(Level);
            AssigningPart.CalculateNaturalWeaponHitBonus(Level);
            return base.BeingAppliedBy(obj, who);
        }

    } //!-- public class ModCrystallineNaturalWeaponUnmanaged : ModCrystallineNaturalWeapon
}