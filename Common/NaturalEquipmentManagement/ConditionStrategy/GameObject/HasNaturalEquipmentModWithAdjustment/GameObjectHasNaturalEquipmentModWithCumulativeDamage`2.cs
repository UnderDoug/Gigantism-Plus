using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, TAdjustment>
        : GameObjectHasNaturalEquipmentModWithAdjustment<T, TAdjustment>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
        where TAdjustment
        : MeleeWeaponCumulativeAdjustment
        , new()
    {
        public GameObjectHasNaturalEquipmentModWithCumulativeDamage()
            : base()
        {
        }
        public GameObjectHasNaturalEquipmentModWithCumulativeDamage(GameObjectHasNaturalEquipmentModWithCumulativeDamage<T, TAdjustment> Source)
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
