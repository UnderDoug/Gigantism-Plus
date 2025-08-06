using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NatEquipModBodyPartTypeIs<T> : ICondition<ModNaturalEquipment<T>>
        where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
    {
        public string BodyPartType;

        public NatEquipModBodyPartTypeIs()
            : base()
        {
            BodyPartType = null;
        }
        public NatEquipModBodyPartTypeIs(string BodyPartType)
            : this()
        {
            this.BodyPartType = BodyPartType;
        }
        public NatEquipModBodyPartTypeIs(NatEquipModBodyPartTypeIs<T> Source)
            : base(Source)
        {
            BodyPartType = Source.BodyPartType;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                typeof(T).Name
            };
        }

        public override bool Check(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return base.Check(NaturalEquipmentMod)
                && !BodyPartType.IsNullOrEmpty()
                && NaturalEquipmentMod.BodyPartType == BodyPartType;
        }
    }
}
