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
            AssigningPart ??= Wielder?.GetPart<Crystallinity>()?.ConvertToManaged();
            if (AssigningPart == null) return false;
            return base.BeingAppliedBy(obj, who);
        }

    } //!-- public class ModCrystallineNaturalWeaponUnmanaged : ModCrystallineNaturalWeapon
}