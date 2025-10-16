using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentModWithAdjustDamageDieCount<T> : GameObjectHasNaturalEquipmentModWithAdjustment<T, AdjustMeleeDamageDieCount>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
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
            if (GameObject == null)
            {
                return base.Check(GameObject);
            }
            return GetAdjustment(GameObject).Check(GameObject);
        }
    }
}
