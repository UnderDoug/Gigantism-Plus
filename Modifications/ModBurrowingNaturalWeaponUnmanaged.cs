using System;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeaponUnmanaged : ModBurrowingNaturalWeapon
    {
        public ModBurrowingNaturalWeaponUnmanaged()
        {
        }

        public ModBurrowingNaturalWeaponUnmanaged(int Tier)
            : base(Tier)
        {
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder ??= who;
            AssigningMutation ??= Wielder.GetPart<BurrowingClaws>().ConvertToManaged();
            NaturalWeapon ??= AssigningMutation.GetNaturalWeapon();
            Level = AssigningMutation.Level;
            AssigningMutation.CalculateNaturalWeaponDamageDieCount(Level);
            AssigningMutation.CalculateNaturalWeaponDamageDieSize(Level);
            AssigningMutation.CalculateNaturalWeaponDamageBonus(Level);
            AssigningMutation.CalculateNaturalWeaponHitBonus(Level);
            return base.BeingAppliedBy(obj, who);
        }

    } //!-- public class ModBurrowingNaturalWeaponUnmanaged : ModBurrowingNaturalWeapon
}