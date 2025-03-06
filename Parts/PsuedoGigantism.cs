using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class PseudoGigantism : IPart
    {
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
                if (!E.Object.IsGiganticEquipment && E.SlotType != "Floating Nearby" && !E.Object.HasPart<CyberneticsBaseItem>() && !E.Object.HasTagOrProperty("GiganticEquippable"))
                {
                    E.CanBeTooSmall = true;
                }
            }

            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    } //!-- public class PsudoGigantic : IPart
}
