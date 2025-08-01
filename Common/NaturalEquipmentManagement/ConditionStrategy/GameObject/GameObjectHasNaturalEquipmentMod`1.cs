using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentMod<T> : GameObjectHasPart<T>
        where T : ModNaturalEquipmentBase, new()
    {
        public GameObjectHasNaturalEquipmentMod()
            : base()
        {
        }
        public GameObjectHasNaturalEquipmentMod(GameObjectHasNaturalEquipmentMod<T> Source)
            : base(Source)
        {
        }

        public virtual T GetNaturalEquipmentMod(GameObject GameObject)
        {
            if (!base.Check(GameObject))
            {
                return null;
            }
            if (GameObject.TryGetPart(out T targetMod))
            {
                return targetMod;
            }
            NaturalEquipmentOperator naturalEquipmentOperator = GameObject.NaturalEquipmentOperator();
            SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = naturalEquipmentOperator.GetNaturalEquipmentMods();
            foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in naturalEquipmentMods)
            {
                if (naturalEquipmentMod is T unappliedTargetMod)
                {
                    return unappliedTargetMod;
                }
            }
            return null;
        }

        public override bool Check(GameObject GameObject)
        {
            return GetNaturalEquipmentMod(GameObject) != null;
        }
    }
}
