using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;

namespace Mods.GigantismPlus
{
    [Serializable]
    public class PseudoGigantism : IActivePart
    {
        public PseudoGigantism()
        {
            WorksOnSelf = true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID;
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            if (!E.Actor.IsGiganticCreature && IsObjectActivePartSubject(E.Actor) && IsReady(UseCharge: true, IgnoreCharge: false, IgnoreLiquid: false, IgnoreBootSequence: false, IgnoreBreakage: false, IgnoreRust: false, IgnoreEMP: false, IgnoreRealityStabilization: false, IgnoreSubject: false, IgnoreLocallyDefinedFailure: false, 1, null, UseChargeIfUnpowered: false, 0L, null))
            {
                E.Decreases++;
                if (!E.Object.IsGiganticEquipment && E.SlotType != "Floating Nearby")
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
    } //!--- public class PsudoGigantic : IActivePart
}