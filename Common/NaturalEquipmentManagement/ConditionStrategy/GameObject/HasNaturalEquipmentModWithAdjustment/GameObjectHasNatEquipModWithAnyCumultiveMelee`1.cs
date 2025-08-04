using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNatEquipModWithAnyCumultiveMelee<T> : GameObjectHasNaturalEquipmentMod<T>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject, out ModNaturalEquipment<T> naturalEquipmentMod) 
                && (new AnyConditions<IPart>()
                { 
                    new PartHasAdjustment<T, AdjustDamageDieCount>(),
                    new PartHasAdjustment<T, AdjustDamageDieSize>(),
                    new PartHasAdjustment<T, AdjustDamageBonus>(),
                    new PartHasAdjustment<T, AdjustHitBonus>(),
                    new PartHasAdjustment<T, AdjustPenBonus>(),
                })
                .Check(naturalEquipmentMod);
        }
    }
}
