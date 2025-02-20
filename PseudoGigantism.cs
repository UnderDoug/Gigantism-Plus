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
            bool isReady =
                    IsReady(
                        UseCharge: true,
                        IgnoreCharge: false,
                        IgnoreLiquid: false,
                        IgnoreBootSequence: false,
                        IgnoreBreakage: false,
                        IgnoreRust: false,
                        IgnoreEMP: false,
                        IgnoreRealityStabilization: false,
                        IgnoreSubject: false,
                        IgnoreLocallyDefinedFailure: false,
                        MultipleCharge: 1,
                        ChargeUse: null,
                        UseChargeIfUnpowered: false,
                        GridMask: 0L,
                        PowerLoadLevel: null
                );

            if (!E.Actor.IsGiganticCreature && IsObjectActivePartSubject(E.Actor) && isReady)
            {
                E.Decreases++;
                if (!E.Object.IsGiganticEquipment && E.SlotType != "Floating Nearby" && E.SlotType != "Thrown Weapon")
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