using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectIsForSlot : ICondition<GameObject>
    {
        public string Slot;

        public GameObjectIsForSlot()
            : base()
        {
            Slot = null;
        }
        public GameObjectIsForSlot(string Slot = null)
            : this()
        {
            this.Slot = Slot;
        }
        public GameObjectIsForSlot(BodyPart BodyPart = null)
            : this(BodyPart.Type)
        {
        }
        public GameObjectIsForSlot(GameObjectIsForSlot Source)
            : base(Source)
        {
            Slot = Source.Slot;
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                Slot ?? NULL
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                && !Slot.IsNullOrEmpty()
                && GameObject?.GetPart<Armor>()?.WornOn == Slot
                || GameObject?.GetPart<MeleeWeapon>()?.Slot == Slot;
        }
    }
}
