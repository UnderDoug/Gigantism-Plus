using System;
using HNPS_GigantismPlus;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class InventoryGigantifier : IScribedPart
    {
        public bool IsMerchant => ParentObject.HasPart<GenericInventoryRestocker>();

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID
                || ID == PooledEvent<StockedEvent>.ID;
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject GO = E.Object;
            if (GO == null) goto Remove;
            if (GO != ParentObject) goto Remove; // skip if the created Object isn't this part's ParentObject
            if (!GO.HasPart<GigantismPlus>()) goto Remove; // skip non-gigantic creatures
            if (IsMerchant) goto Exit;

            Debug.Header(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");
            Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0);
            
            GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

            Debug.Footer(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");

        Remove:
            GO.RemovePart(this);
        Exit:
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StockedEvent E)
        {
            GameObject GO = E.Object;
            if (GO == null) goto Exit;
            if (GO != ParentObject) goto Exit; // skip if the created Object isn't this part's ParentObject
            if (!GO.HasPart<GigantismPlus>()) goto Remove; // skip non-gigantic creatures
            if (!IsMerchant) goto Remove; // remove this part if the Object still has this part after creation but is not a merchant

            Debug.Header(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)");
            Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0);

            GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

            Debug.Footer(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)");
            goto Exit;

        Remove:
            GO.RemovePart(this);
        Exit:
            return base.HandleEvent(E);
        }

    } // public class InventoryGigantifier : IScribedPart
}
