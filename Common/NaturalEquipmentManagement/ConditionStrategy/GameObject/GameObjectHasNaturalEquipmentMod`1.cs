using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;

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
            int indent = Debug.LastIndent;
            Debug.Entry(3,
                $"* {nameof(GameObjectHasNaturalEquipmentMod<T>)}."
                + $"{nameof(GetNaturalEquipmentMod)}("
                + $"{nameof(GameObject)}: {GameObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: true);

            if (GameObject == null)
            {
                Debug.CheckNah(3, $"{nameof(GameObject)} is null", Indent: indent + 2, Toggle: true);
                Debug.LastIndent = indent;
                return null;
            }
            if (GameObject.TryGetPart(out T targetMod))
            {
                Debug.CheckYeh(3, $"{nameof(GameObject)} has {typeof(T).Name}", Indent: indent + 2, Toggle: true);
                Debug.LastIndent = indent;
                return targetMod;
            }
            NaturalEquipmentOperator naturalEquipmentOperator = GameObject.NaturalEquipmentOperator();
            SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = naturalEquipmentOperator.GetNaturalEquipmentMods();

            Debug.Entry(3, $"Looping {nameof(naturalEquipmentOperator.GetNaturalEquipmentMods)}...", 
                Indent: indent + 2, Toggle: true);
            foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in naturalEquipmentMods)
            {
                Debug.LoopItem(3, $"{nameof(naturalEquipmentMod)}: {naturalEquipmentMod.GetType().Name}", 
                    Indent: indent + 3, Toggle: true);
                if (naturalEquipmentMod.GetType().InheritsFrom(typeof(T), Silent: false))
                {
                    Debug.CheckYeh(3, $"{naturalEquipmentMod.GetType().Name} is target mod", Indent: indent + 4, Toggle: true);
                    Debug.LastIndent = indent;
                    return (T)naturalEquipmentMod;
                }
            }
            Debug.CheckNah(3, $"Target mod wasn't found", Indent: indent + 4, Toggle: true);
            Debug.LastIndent = indent;
            return null;
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject) || GetNaturalEquipmentMod(GameObject) != null;
        }
    }
}
