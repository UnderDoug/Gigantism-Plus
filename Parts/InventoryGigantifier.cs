using System;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class InventoryGigantifier : IScribedPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject GO = E.Object;
            if (GO == null) goto Exit;
            if (GO != ParentObject) goto Exit; // skip if the created Object isn't this part's ParentObject

            Debug.Header(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");
            Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0);
            
            GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

            Debug.Footer(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");
            
            Exit:
            GO.RemovePart(this);
            return base.HandleEvent(E);
        }
    } // public class InventoryGigantifier : IScribedPart
}
