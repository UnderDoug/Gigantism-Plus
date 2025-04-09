using System;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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
        public ModCrystallineNaturalWeaponUnmanaged(ModCrystallineNaturalWeapon Conversion)
            : base(Conversion)
        {
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            AssigningPart ??= !Wielder.Is(null) ? new(Wielder.GetPart<Crystallinity>()) : null;
            if (AssigningPart == null)
            {
                Debug.Entry(2,
                    $"WARN",
                    $"{typeof(ModCrystallineNaturalWeaponUnmanaged).Name}.{nameof(BeingAppliedBy)} (" +
                    $"GameObject obj: {obj.ID}:{obj.ShortDisplayNameStripped}, " +
                    $"GameObject who: {who.ID}:{who.ShortDisplayNameStripped}) - " +
                    $"Failed to assign converted {typeof(Crystallinity).Name} as AssigningPart",
                    Indent: 0);
                return false;
            }
            return base.BeingAppliedBy(obj, who);
        }

    } //!-- public class ModCrystallineNaturalWeaponUnmanaged : ModCrystallineNaturalWeapon
}