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
                    new PartHasAdjustment<T, AdjustMeleeDamageDieCount>(),
                    new PartHasAdjustment<T, AdjustMeleeDamageDieSize>(),
                    new PartHasAdjustment<T, AdjustMeleeDamageBonus>(),
                    new PartHasAdjustment<T, AdjustMeleeHitBonus>(),
                    new PartHasAdjustment<T, AdjustMeleePenBonus>(),
                })
                .Check(naturalEquipmentMod);
        }
    }
}
