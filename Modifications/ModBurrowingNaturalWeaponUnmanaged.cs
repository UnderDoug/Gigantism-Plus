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
            AssigningPart ??= Wielder?.GetPart<BurrowingClaws>()?.ConvertToManaged();
            if (AssigningPart == null) return false;
            return base.BeingAppliedBy(obj, who);
        }
    } //!-- public class ModBurrowingNaturalWeaponUnmanaged : ModBurrowingNaturalWeapon
}