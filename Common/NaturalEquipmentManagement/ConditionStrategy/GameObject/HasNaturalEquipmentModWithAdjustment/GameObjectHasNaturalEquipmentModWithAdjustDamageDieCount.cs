using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentModWithAdjustDamageDieCount<T> : GameObjectHasNaturalEquipmentModWithAdjustment<T, AdjustDamageDieCount>
        where T : ModNaturalEquipmentBase, new()
    {
        public GameObjectHasNaturalEquipmentModWithAdjustDamageDieCount()
            : base()
        {
        }
        public GameObjectHasNaturalEquipmentModWithAdjustDamageDieCount(GameObjectHasNaturalEquipmentModWithAdjustDamageDieCount<T> Source)
            : base(Source)
        {
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                && GetAdjustment(GameObject).Check(GameObject);
        }
    }
}
