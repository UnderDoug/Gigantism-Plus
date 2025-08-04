using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public static class NaturalEquipmentConditions
    {
        public static AnyConditions<GameObject> IsOrganicFist => new()
        {
            new GameObjectBlueprintIs("DefaultFist"),
            new NotCondition<GameObject>(new GameObjectHasAnyParts(new Type[] { typeof(Inorganic), typeof(Metal) })),
        };

        public static AnyConditions<GameObject> IsCybernetic => new()
        {
            IsAugmented,
            IsChromeBoned,
        };

        public static GameObjectHasNaturalEquipmentMod<GigantismPlus> IsGigantic => new();
        public static GameObjectHasNaturalEquipmentMod<ElongatedPaws> IsElongated => new();
        public static GameObjectHasNaturalEquipmentMod<UD_ManagedBurrowingClaws> IsBurrowing => new();
        public static GameObjectHasNaturalEquipmentMod<UD_ManagedCrystallinity> IsCrystalline => new();
        public static GameObjectHasNaturalEquipmentMod<CyberneticsGiganticExoframe> IsAugmented => new();
        public static GameObjectHasNaturalEquipmentMod<CyberneticsManagedHandBones> IsChromeBoned => new();

        public static GameObjectWielderHasPart<GigantismPlus> WielderHasGigantismPlus => new();
        public static GameObjectWielderHasPart<ElongatedPaws> WielderHasElongatedPaws => new();
        public static GameObjectWielderHasPart<UD_ManagedBurrowingClaws> WielderHasBurrowingClaws => new();
        public static GameObjectWielderHasPart<UD_ManagedCrystallinity> WielderHasCrystallinity => new();

        public static GameObjectHasNatEquipModWithAnyCumultiveMelee<T> ThisModAdjustsMeleeCumulatively<T>()
            where T : IPart , IManagedDefaultNaturalEquipment<T> , new() => new();
    }
}
