using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentModWithAdjustment<T, TAdjustment>
        : GameObjectHasNaturalEquipmentMod<T>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
        where TAdjustment
        : IAdjustment
    {
        public GameObjectHasNaturalEquipmentModWithAdjustment()
            : base()
        {
        }
        public GameObjectHasNaturalEquipmentModWithAdjustment(GameObjectHasNaturalEquipmentModWithAdjustment<T, TAdjustment> Source)
            : base(Source)
        {
        }

        public virtual TAdjustment GetAdjustment(GameObject GameObject)
        {
            if (!base.Check(GameObject))
            {
                return null;
            }

            Adjustments adjustments = GetNaturalEquipmentMod(GameObject).Adjustments;
            if (!adjustments.IsNullOrEmpty())
            {
                foreach (IAdjustment adjustment in adjustments)
                {
                    if (adjustment is TAdjustment targetAdjustment)
                    {
                        return targetAdjustment;
                    }
                }
            }
            return null;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(TAdjustment).Name,
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return GetAdjustment(GameObject) != null;
        }
    }
}
