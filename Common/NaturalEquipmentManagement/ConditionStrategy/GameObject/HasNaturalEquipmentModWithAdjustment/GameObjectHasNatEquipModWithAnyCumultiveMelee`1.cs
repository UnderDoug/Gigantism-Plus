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
                && new ConditionsAny<IPart>()
                {
                    new IPartHasAdjustment<T, AdjustMeleeDamageDieCount>(),
                    new IPartHasAdjustment<T, AdjustMeleeDamageDieSize>(),
                    new IPartHasAdjustment<T, AdjustMeleeDamageBonus>(),
                    new IPartHasAdjustment<T, AdjustMeleeHitBonus>(),
                    new IPartHasAdjustment<T, AdjustMeleePenBonus>(),
                }
                .Check(naturalEquipmentMod);
        }
    }
}
