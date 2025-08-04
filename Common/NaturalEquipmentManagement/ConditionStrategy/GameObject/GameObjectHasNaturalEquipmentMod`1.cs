using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectHasNaturalEquipmentMod<T> : GameObjectHasPart<ModNaturalEquipment<T>>
        where T
        : IPart
        , IManagedDefaultNaturalEquipment<T>
        , new()
    {
        private static bool doDebug => getClassDoDebug("NotCondition");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                // nameof(Check),
                // nameof(NotCheck),
            };
            List<object> dontList = new()
            {
                nameof(GetNaturalEquipmentMod),
            };

            return Options.getDoDebug(what, doList, dontList, doDebug);
        }

        public string ModificationName => nameof(ModNaturalEquipment<T>) + $"<{typeof(T).Name}>";

        public GameObjectHasNaturalEquipmentMod()
            : base()
        {
        }
        public GameObjectHasNaturalEquipmentMod(GameObjectHasNaturalEquipmentMod<T> Source)
            : base(Source)
        {
        }

        public virtual ModNaturalEquipment<T> GetNaturalEquipmentMod(GameObject GameObject)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(GetNaturalEquipmentMod));
            Debug.Entry(3,
                $"* {GetType().Name[..^2] + $"<{ModificationName}>"}."
                + $"{nameof(GetNaturalEquipmentMod)}("
                + $"{nameof(GameObject)}: {GameObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            if (GameObject == null)
            {
                Debug.CheckNah(3, $"{nameof(GameObject)} is null", Indent: indent + 2, Toggle: doDebug);
                Debug.LastIndent = indent;
                return null;
            }
            if (GameObject.TryGetPart(out ModNaturalEquipment<T> targetMod))
            {
                Debug.CheckYeh(3, $"{nameof(GameObject)} has {ModificationName}", Indent: indent + 2, Toggle: doDebug);
                Debug.LastIndent = indent;
                return targetMod;
            }

            SortedDictionary<int, ModNaturalEquipmentBase> naturalEquipmentMods = GameObject.GetPrioritisedNaturalEquipmentMods();

            Debug.Entry(3, $"Looping {nameof(Extensions.GetPrioritisedNaturalEquipmentMods)}...", 
                Indent: indent + 2, Toggle: doDebug);
            foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in naturalEquipmentMods)
            {
                Debug.LoopItem(3, $"{nameof(naturalEquipmentMod)}: {naturalEquipmentMod.GetType().Name}", 
                    Indent: indent + 3, Toggle: doDebug);
                if (naturalEquipmentMod.GetType().InheritsFrom(typeof(ModNaturalEquipment<T>), Silent: false))
                {
                    Debug.CheckYeh(3, $"{naturalEquipmentMod.GetType().Name} is target mod", Indent: indent + 4, Toggle: doDebug);
                    Debug.LastIndent = indent;
                    return (ModNaturalEquipment<T>)naturalEquipmentMod;
                }
            }
            Debug.CheckNah(3, $"{ModificationName} wasn't found", Indent: indent + 4, Toggle: doDebug);
            Debug.LastIndent = indent;
            return null;
        }

        public override List<string> AddToString()
        {
            List<string> additions = new(base.AddToString());
            int elementIndex = -1;
            string modNaturalEquipment = nameof(ModNaturalEquipment<T>);
            if (!additions.IsNullOrEmpty())
            {
                for (int i = 0; i < additions.Count; i++)
                {
                    if (additions[i].Contains(modNaturalEquipment))
                    {
                        elementIndex = i;
                        break;
                    }
                }
            }
            if (elementIndex != -1)
            {
                additions[elementIndex] = ModificationName;
            }
            return additions;
        }

        public override bool Check(GameObject GameObject)
        {
            return Check(GameObject, out _);
        }

        public virtual bool Check(GameObject GameObject, out ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return (NaturalEquipmentMod = GetNaturalEquipmentMod(GameObject)) != null || base.Check(GameObject);
        }
    }
}
