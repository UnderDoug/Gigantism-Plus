using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NatEquipModAdjectiveIs<T> : ICondition<ModNaturalEquipment<T>>
        where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
    {
        public string Adjective;

        public NatEquipModAdjectiveIs()
            : base()
        {
            Adjective = null;
        }
        public NatEquipModAdjectiveIs(string Adjective)
            : this()
        {
            this.Adjective = Adjective;
        }
        public NatEquipModAdjectiveIs(NatEquipModAdjectiveIs<T> Source)
            : base(Source)
        {
            Adjective = Source.Adjective;
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
                && !Adjective.IsNullOrEmpty()
                && NaturalEquipmentMod.Adjective == Adjective;
        }
    }
}
