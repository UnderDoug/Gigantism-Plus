using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;

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
            new NotCondition<GameObject>(new GameObjectHasAnyParts(new string[]
            {
                "Inorganic",
                "Metal",
            }))
        };
    }
}
