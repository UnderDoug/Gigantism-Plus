using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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
        public ModBurrowingNaturalWeaponUnmanaged(ModBurrowingNaturalWeapon Conversion)
            : base(Conversion)
        {
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            AssigningPart ??= !Wielder.Is(null) ? new(Wielder.GetPart<BurrowingClaws>()) : null;
            if (AssigningPart == null)
            {
                Debug.Entry(2, 
                    $"WARN",
                    $"{typeof(ModBurrowingNaturalWeaponUnmanaged).Name}.{nameof(BeingAppliedBy)} (" + 
                    $"GameObject obj: {obj.ID}:{obj.ShortDisplayNameStripped}, " +
                    $"GameObject who: {who.ID}:{who.ShortDisplayNameStripped}) - " + 
                    $"Failed to assign converted {typeof(BurrowingClaws).Name} as AssigningPart",
                    Indent: 0);
                return false; 
            }
            return base.BeingAppliedBy(obj, who);
        }
    } //!-- public class ModBurrowingNaturalWeaponUnmanaged : ModBurrowingNaturalWeapon
}