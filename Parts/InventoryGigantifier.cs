using System;
using HNPS_GigantismPlus;
using XRL.Wish;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class InventoryGigantifier : IScribedPart
    {
        public bool IsMerchant => ParentObject.HasPart<GenericInventoryRestocker>();
        public bool IsGigantic => ParentObject.HasPart<GigantismPlus>();

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (IsGigantic && !IsMerchant && ID == AfterObjectCreatedEvent.ID)
                || (IsGigantic && IsMerchant && ID == PooledEvent<StockedEvent>.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject GO = E.Object;
            if (GO != null && GO == ParentObject)
            {
                Debug.Header(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");
                Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0);
            
                GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

                Debug.Footer(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StockedEvent E)
        {
            GameObject GO = E.Object;
            if (GO != null && GO == ParentObject)
            {
                Debug.Header(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)");
                Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0);

                GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

                Debug.Footer(3, nameof(InventoryGigantifier), $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)");
            }
            return base.HandleEvent(E);
        }

    } // public class InventoryGigantifier : IScribedPart

    [HasWishCommand]
    public class GigantifyInventoryWishHandler
    {
        [WishCommand(Command = "HNPS_GigantifyInventory")]
        public static void GigantifyInventory()
        {
            GameObject player = The.Player;
            player.GigantifyInventory(true, true);
        }
    }
}
