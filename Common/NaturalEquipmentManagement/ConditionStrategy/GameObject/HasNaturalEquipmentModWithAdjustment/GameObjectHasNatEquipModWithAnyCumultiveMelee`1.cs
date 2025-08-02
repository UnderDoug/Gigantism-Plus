using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNatEquipModWithAnyCumultiveMelee<T> : AnyConditions<GameObject>
        where T : ModNaturalEquipmentBase, new()
    {
        public static new bool IsReadOnly => true;

        public GameObjectHasNatEquipModWithAnyCumultiveMelee()
            : base()
        {
            Clear();
            EnsureCapacity(5);
            Add(new GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, AdjustDamageDieCount>());
            Add(new GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, AdjustDamageDieSize>());
            Add(new GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, AdjustDamageBonus>());
            Add(new GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, AdjustHitBonus>());
            Add(new GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, AdjustPenBonus>());
        }
    }
}
