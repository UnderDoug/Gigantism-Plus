using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class PseudoGigantism : IScribedPart
    {
        public override void Attach()
        {
            base.Attach();
            ParentObject.CheckEquipmentSlots();
        }

        public override void Remove()
        {
            ParentObject.CheckEquipmentSlots();
            base.Remove();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID;
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            if (!E.Actor.IsGiganticCreature)
            {
                E.Decreases++;
                
                E.CanBeTooSmall = 
                    !E.Object.IsGiganticEquipment 
                 && !E.SlotType.Is("Floating Nearby")
                 && !E.Object.HasPart<CyberneticsBaseItem>() 
                 && !E.Object.HasTagOrProperty("GiganticEquippable");
            }

            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    } //!-- public class PsudoGigantic : IPart
}
