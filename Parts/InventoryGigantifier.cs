using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using XRL.Wish;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class InventoryGigantifier : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(InventoryGigantifier));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }
        public bool IsMerchant => ParentObject != null && ParentObject.HasPart<GenericInventoryRestocker>();
        public bool IsGigantic => ParentObject != null && ParentObject.HasPart<GigantismPlus>();

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (IsGigantic && !IsMerchant && ID == AfterObjectCreatedEvent.ID)
                || (IsGigantic && IsMerchant && ID == StockedEvent.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            GameObject GO = E.Object;
            if (GO != null && GO == ParentObject && IsGigantic)
            {
                Debug.Header(3, 
                    nameof(InventoryGigantifier), 
                    $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)",
                    Toggle: doDebug);
                Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0, Toggle: doDebug);
            
                GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

                Debug.Footer(3, 
                    nameof(InventoryGigantifier), 
                    $"{nameof(HandleEvent)}({nameof(AfterObjectCreatedEvent)} E)", 
                    Toggle: doDebug);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StockedEvent E)
        {
            GameObject GO = E.Object;
            Debug.Entry(3, "TARGET", GO.DebugName, Indent: 0, Toggle: doDebug);
            if (GO != null && GO == ParentObject && IsMerchant)
            {
                Debug.Header(3, 
                    nameof(InventoryGigantifier), 
                    $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)", 
                    Toggle: doDebug);

                GO.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);

                Debug.Footer(3, 
                    nameof(InventoryGigantifier), 
                    $"{nameof(HandleEvent)}({nameof(StockedEvent)} E)", 
                    Toggle: doDebug);
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
            player.GigantifyInventory(Force: true);
        }
    }
}
