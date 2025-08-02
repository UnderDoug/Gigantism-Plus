using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

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
            new NotCondition<GameObject>(new GameObjectHasAnyParts(new Type[]
            {
                typeof(Inorganic),
                typeof(Metal),
            }))
        };

        public static GameObjectHasNatEquipModWithAnyCumultiveMelee<T> ThisModAdjustsMeleeCumulatively<T>()
            where T : ModNaturalEquipmentBase, new()
        {
            return new();
        }
    }
}
