using System;
using System.Collections.Generic;
using System.Linq;

using XRL.World.Parts.Mutation;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;
using XRL.UI;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class Wrassler : IScribedPart
    {
        public Guid WrassleID;
        public bool Bestow = false;
        public bool beenBestowed = false;

        public Wrassler()
        {
            WrassleID = Guid.NewGuid();
        }
        public Wrassler(Guid source)
            : this()
        {
            WrassleID = source;
        }
        public Wrassler(Wrassler source)
            : this(source.WrassleID)
        {
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject)
            {
                BestowWrassleGear();
            }
            return base.HandleEvent(E);
        }

        public static Wrassler BestowWrassleGear(GameObject Object, out bool Bestowed)
        {
            Wrassler wrassler = null;
            List<string> wrassleGearBlueprints = new()
            {
                "WrassleFace",
                "WrassleSuit",
                "WrassleCape",
                "WrassleGloves",
                "WrassleBoots",
            };
            List<GameObject> wrassleGearObjects = new();
            foreach (string item in wrassleGearBlueprints)
            {
                GameObject wrassleGearObject = GameObjectFactory.Factory.CreateObject(item);
                if (wrassleGearObject != null && wrassleGearObject.TryGetPart(out WrassleGear wrassleGear) && Object.TryGetPart(out wrassler))
                {
                    wrassleGear.WrassleID = wrassler.WrassleID;
                    wrassleGear.RandomizeTile = true;
                    wrassleGear.ApplyFlair();
                    if (!wrassleGearObjects.Contains(wrassleGearObject)) wrassleGearObjects.Add(wrassleGearObject);
                }
            }
            foreach (GameObject wrassleGearObject in wrassleGearObjects)
            {
                Object.Inventory.AddObjectToInventory(wrassleGearObject);
            }
            Bestowed = true;
            return wrassler;
        }

        public Wrassler BestowWrassleGear()
        {
            return BestowWrassleGear(ParentObject, out beenBestowed);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        [WishCommand(Command = "flex muscles")]
        public static void PlayerWasslerWish()
        {
            Wrassler wrassler = The.Player.RequirePart<Wrassler>().BestowWrassleGear();
            string wrassleID = wrassler.WrassleID.ToString();
            bool doMessage = !The.Player.HasPart<Wrassler>() || The.Player.GetStringProperty("WrassleID") != wrassleID;
            The.Player.SetStringProperty("WrassleID", wrassler.WrassleID.ToString());
            foreach (GameObject item in The.Player.Inventory.GetObjects())
            {
                if (!item.InheritsFrom("BaseWrassleGear")) continue;
                The.Player.EquipObject(item, item.Armor.WornOn);
            }
            if (doMessage)
            {
                Popup.Show($"Sick gains {The.Player.DisplayName}! Check your inventory.");
            }
        }

        [WishCommand(Command = "new wrassleID please")]
        public static void PlayerWassleIDWish()
        {
            Wrassler wrassler = The.Player.RequirePart<Wrassler>();

            string oldWrassleID = wrassler.WrassleID.ToString();
            wrassler.WrassleID = Guid.NewGuid();
            Popup.Show($"Stealing identity...");
            Popup.Show($"...");
            Popup.Show($"... Done.");
            Popup.Show($"Old: {oldWrassleID}; New: {wrassler.WrassleID.ToString()}");
        }
    } //!-- public class Wrassler : IScribedPart
}
