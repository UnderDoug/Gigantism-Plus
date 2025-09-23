using System;
using System.Collections.Generic;

using XRL.Rules;
using XRL.Wish;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class InventoryGigantifier : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(InventoryGigantifier));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                nameof(GigantifyInventory),
            };
            List<object> dontList = new()
            {
            };

            return Options.getDoDebug(what, doList, dontList, doDebug);
        }
        public bool IsMerchant => ParentObject != null && ParentObject.HasPart<GenericInventoryRestocker>();
        public bool IsSecretGiant => ParentObject != null && ParentObject.HasPropertyOrTag("SecretGiantVillager");
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

        public static void GigantifyInventory(GameObject Creature, bool Option = true, bool GrenadeOption = false, bool Wish = false, bool Force = false, string Context = null)
        {
            if (Creature == null) return; // need to have a creature.

            bool doDebug = getDoDebug(nameof(GigantifyInventory));

            string creatureBlueprint = Creature?.Blueprint;

            bool creatureIsMerchant = Creature.HasPart<GenericInventoryRestocker>();
            bool creatureIsSecretGiantVillager = Creature.HasPropertyOrTag("SecretGiantVillager");
            bool creatureIsSecretGiantGutsmonger = Creature.InheritsFrom("Giant Gutsmonger");

            (DieRoll die, int high) merchantBaseChance = !creatureIsSecretGiantVillager ? (new("1d5"), 4) : (new("1d7"), 2);
            (DieRoll die, int high) merchantGrenades = !creatureIsSecretGiantVillager ? (new("1d2"), 2) : (new("1d5"), 2);
            (DieRoll die, int high) merchantTradeGoods = !creatureIsSecretGiantVillager ? (new("1d4"), 4) : (new("1d5"), 3);
            (DieRoll die, int high) merchantTonics = !creatureIsSecretGiantVillager ? (new("1d4"), 4) : (new("1d5"), 2);
            (DieRoll die, int high) merchantRareTonics = !creatureIsSecretGiantVillager ? (new("1d10"), 10) : (new("1d5"), 3);

            string secretGiantExtra = !creatureIsSecretGiantVillager
                ? $""
                : $" ({nameof(creatureIsSecretGiantVillager)})"
                ;

            if (Creature.ID == "1" && !Creature.HasPart<GigantismPlus>() && !Force)
            {
                return; // redundancy, just in case.
            }
            if (!Option && !Force)
            {
                return; // skip if Option disabled
            }
            if (!Creature.IsCreature && !Force)
            {
                return; // skip non-creatures
            }
            if (Creature.Inventory == null)
            {
                return; // skip creatures without inventory
            }

            Debug.Entry(3,
                $"* {nameof(GigantifyInventory)}("
                + $"{nameof(Option)}: {Option}, "
                + $"{nameof(GrenadeOption)}: {GrenadeOption}, "
                + $"{nameof(Force)}: {Force})",
                Indent: 1, Toggle: doDebug);
            Debug.Divider(3, Indent: 1, Toggle: doDebug);

            if (Force)
            {
                Option = Force;
                GrenadeOption = Force;
            }

            Debug.Entry(3, "Making inventory items gigantic for creature", creatureBlueprint, Indent: 1, Toggle: doDebug);
            Debug.Entry(3, $"Creature is merchant", creatureIsMerchant ? "Yeh" : "Nah", Indent: 1, Toggle: doDebug);

            // Create a copy of the items list to avoid modifying during enumeration
            List<GameObject> itemsToProcess = new(Creature.GetInventoryAndEquipment());

            bool wantstoUpdateBody = false;
            if (!itemsToProcess.IsNullOrEmpty())
            {
                Debug.Entry(3, "> foreach (GameObject item in itemsToProcess)",
                    Indent: 1, Toggle: doDebug);
                Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: doDebug);
                foreach (GameObject item in itemsToProcess)
                {
                    string ItemDebug = item.DebugName;
                    string ItemName = item.Blueprint;
                    bool inventoryGigantifierAlwaysAllow = item.HasTagOrProperty("InventoryGigantifierAlwaysAllow");
                    bool inventoryGigantifierAlwaysStockGiant = item.HasTagOrProperty("InventoryGigantifierAlwaysStockGiant");
                    bool itemIsCybernetic = item.HasPart<CyberneticsBaseItem>();
                    bool itemLiquidContainer = item.InheritsFrom("WaterContainer");

                    string alwaysStockGiantExtra = !inventoryGigantifierAlwaysStockGiant
                        ? $""
                        : $" ({nameof(inventoryGigantifierAlwaysStockGiant)})"
                        ;

                    Debug.LoopItem(3, $"{nameof(ItemName)}", ItemName, Indent: 1, Toggle: doDebug);

                    int NoThanks = 0;
                    Debug.DiveIn(3, $"{ItemDebug}", Indent: 1, Toggle: doDebug);
                    // Can the item have the gigantic modifier applied?
                    if (ItemModding.ModificationApplicable("ModGigantic", item)
                        || (creatureIsSecretGiantGutsmonger && itemIsCybernetic)
                        || (creatureIsSecretGiantVillager && itemLiquidContainer))
                    {
                        Debug.CheckYeh(4, "eligible to be made ModGigantic", Indent: 2, Toggle: doDebug);
                        // Is the item already gigantic? Don't attempt to apply it again.
                        if (item.HasPart<ModGigantic>())
                        {
                            Debug.CheckNah(4, "already gigantic", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                            NoThanks++;
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not already gigantic", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item a natural equipment the creature starts with? don't gigantify.
                        if (item.IsNaturalEquipment())
                        {
                            Debug.CheckNah(4, "Natural Equipment", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                            NoThanks++;
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not natural equipment", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item a grenade, and is the option not set to include them?
                        if (item.IsGrenade())
                        {
                            if (!GrenadeOption || creatureIsMerchant)
                            {
                                if (!GrenadeOption && !creatureIsMerchant)
                                {
                                    Debug.CheckNah(4, "grenade (excluded)", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                                    NoThanks++;
                                }
                                if (creatureIsMerchant)
                                {
                                    Debug.CheckNah(4, "grenade (isMerchant)", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                                    NoThanks++;
                                }
                                if (creatureIsMerchant && merchantGrenades.die.Resolve() >= merchantGrenades.high
                                    || inventoryGigantifierAlwaysStockGiant)
                                {
                                    Debug.LoopItem(4,
                                        $"but!] {nameof(merchantGrenades)}" +
                                        $"{secretGiantExtra}{alwaysStockGiantExtra} " +
                                        $"{merchantGrenades.die} " +
                                        $"rolled at or above {merchantGrenades.high}",
                                        $"NoThanks--;",
                                        Indent: 2, Toggle: doDebug);
                                    NoThanks--;
                                }
                                else if (item.HasTagOrProperty("InventoryGigantifierAlwaysAllow"))
                                {
                                    Debug.LoopItem(4,
                                        $"but!] {nameof(inventoryGigantifierAlwaysAllow)}",
                                        $"NoThanks--;",
                                        Indent: 2, Toggle: doDebug);
                                    NoThanks--;
                                }
                            }
                            else if (GrenadeOption)
                            {
                                Debug.CheckYeh(4, "grenade (included)", Indent: 2, Toggle: doDebug);
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not grenade", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                        if (item.IsTradeGood())
                        {
                            Debug.CheckNah(4, "TradeGoods", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                            NoThanks++;

                            if (creatureIsMerchant && merchantTradeGoods.die.Resolve() >= merchantTradeGoods.high
                                    || inventoryGigantifierAlwaysStockGiant)
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(merchantTradeGoods)}" +
                                    $"{secretGiantExtra}{alwaysStockGiantExtra} {merchantTradeGoods.die} " +
                                    $"rolled at or above {merchantTradeGoods.high}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                            else if (item.HasTagOrProperty("InventoryGigantifierAlwaysAllow"))
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(inventoryGigantifierAlwaysAllow)}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not TradeGoods", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item a non-rare tonic? Double doses are basically useless in the early game
                        if (item.IsBasicTonic())
                        {
                            Debug.CheckNah(4, "Tonics_NonRare", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                            NoThanks++;

                            if (creatureIsMerchant && merchantTonics.die.Resolve() >= merchantTonics.high
                                || inventoryGigantifierAlwaysStockGiant)
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(merchantTonics)}" +
                                    $"{secretGiantExtra}{alwaysStockGiantExtra} " +
                                    $"{merchantTonics.die} " +
                                    $"rolled at or above {merchantTonics.high}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                            else if (item.HasTagOrProperty("InventoryGigantifierAlwaysAllow"))
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(inventoryGigantifierAlwaysAllow)}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not Tonics_NonRare", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item a rare tonic? Double doses are basically useless in the early game
                        if (item.IsRareTonic())
                        {
                            Debug.CheckNah(4, "Rare Tonic", "NoThanks++; x/", Indent: 2, Toggle: doDebug);
                            NoThanks++;

                            if (creatureIsMerchant && merchantRareTonics.die.Resolve() >= merchantRareTonics.high)
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(merchantRareTonics)}" +
                                    $"{secretGiantExtra}{alwaysStockGiantExtra} " +
                                    $"{merchantRareTonics.die} " +
                                    $"rolled at or above {merchantRareTonics.high}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                            else if (item.HasTagOrProperty("InventoryGigantifierAlwaysAllow"))
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(inventoryGigantifierAlwaysAllow)}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4, "not Rare Tonics", Indent: 2, Toggle: doDebug);
                        }

                        // Is the item held by a merchant, and did their roll fail?
                        if (creatureIsMerchant)
                        {
                            Debug.CheckNah(4, $"{nameof(creatureIsMerchant)} is {creatureIsMerchant}", "NoThanks++; x/",
                                Indent: 2, Toggle: doDebug);
                            NoThanks++;

                            if (merchantBaseChance.die.Resolve() >= merchantBaseChance.high
                                    || inventoryGigantifierAlwaysStockGiant)
                            {
                                Debug.LoopItem(4,
                                    $"but!] {nameof(merchantBaseChance)}" +
                                    $"{secretGiantExtra}{alwaysStockGiantExtra} " +
                                    $"{merchantBaseChance.die} " +
                                    $"rolled at or above {merchantBaseChance.high}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: doDebug);
                                NoThanks--;
                            }
                            else
                            {
                                Debug.LoopItem(4,
                                    $"and!] {nameof(merchantBaseChance)}{secretGiantExtra}{alwaysStockGiantExtra} {merchantBaseChance.die} rolled below {merchantBaseChance.high}",
                                    "Bummer!",
                                    Indent: 2, Toggle: doDebug);
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4,
                                $"{nameof(creatureIsMerchant)} is {creatureIsMerchant}",
                                Indent: 2, Toggle: doDebug);
                        }

                        Debug.Entry(3, $"NoThanks", $"{NoThanks}", Indent: 2, Toggle: doDebug);
                        Debug.Entry(3, $"Checking if item is Cybernetic and in inventory of Secret Giant Gutsmonger and 7 in 10...",
                            Indent: 2, Toggle: doDebug);

                        if (!(creatureIsSecretGiantGutsmonger && itemIsCybernetic && 7.in10()))
                        {
                            Debug.CheckNah(4, "item is not Cybernetic in inventory of Secret Giant Gutsmonger and 7 in 10",
                                Indent: 3, Toggle: doDebug);
                            if (NoThanks > 0 && !Wish && !(creatureIsSecretGiantVillager && item.HasPart<CyberneticsBaseItem>() && 7.in10()))
                            {
                                Debug.Entry(3, $"Skipped {ItemDebug} //", Indent: 2, Toggle: doDebug);
                                Debug.DiveOut(3, $"{ItemDebug}", Indent: 1, Toggle: doDebug);
                                continue;
                            }
                        }
                        else
                        {
                            Debug.CheckYeh(4, "item is Cybernetic in inventory of Secret Giant Gutsmonger and 7 in 10",
                                Indent: 3, Toggle: doDebug);
                        }

                        string byWish = Wish ? ", by Wish!" : "";
                        Debug.Entry(3,
                            $"Gigantifying {ItemName}{byWish}",
                            Indent: 2, Toggle: doDebug);

                        item.ApplyModification("ModGigantic");
                        if (!item.HasPart<ModGigantic>())
                        {
                            Debug.Warn(2,
                                nameof(InventoryGigantifier),
                                nameof(GigantifyInventory),
                                $"Gigantification of {ItemName} Failed",
                                Indent: 0);
                        }
                        else
                        {
                            if (creatureIsMerchant)
                            {
                                item.ModIntProperty("_stock", 1);
                            }
                            Debug.Entry(3,
                                ItemName, "has been Gigantified",
                                Indent: 2, Toggle: doDebug);
                        }

                        Debug.DiveOut(3, $"Completed {ItemDebug} //", Indent: 1, Toggle: doDebug);
                    }
                    else
                    {
                        Debug.CheckNah(4, "ineligible to be made ModGigantic x/", Indent: 2, Toggle: doDebug);

                        if (item.IsNaturalEquipment() && !item.TryGetPart(out NaturalEquipmentOperator @operator))
                        {
                            NaturalEquipmentManager manager = Creature.RequirePart<NaturalEquipmentManager>();
                            @operator.Manager = manager;
                            wantstoUpdateBody = true;
                        }
                        Debug.DiveOut(3, $"Skipped {ItemDebug} //", Indent: 1, Toggle: doDebug);
                    }
                }
                Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: doDebug);
                Debug.Entry(4, "x foreach (GameObject item in itemsToProcess) >//", Indent: 1, Toggle: doDebug);
            }

            // Now equip all items that should be equipped
            if (!Wish)
            {
                Debug.Entry(3, "Creature.WantToReequip()", Indent: 1, Toggle: doDebug);
                Creature.WantToReequip();
            }

            if (wantstoUpdateBody)
            {
                Creature.Body.UpdateBodyParts();
            }

            Debug.Divider(3, Indent: 1, Toggle: doDebug);
            Debug.Entry(3,
                $"x {nameof(GigantifyInventory)}("
                + $"{nameof(Option)}: {Option}, "
                + $"{nameof(GrenadeOption)}: {GrenadeOption}, "
                + $"{nameof(Force)}: {Force})"
                + $" *//",
                Indent: 1, Toggle: doDebug);
        }

        [WishCommand(Command = "HNPS_GigantifyInventory")]
        public static void GigantifyInventory()
        {
            GameObject player = The.Player;
            player.GigantifyInventory(Force: true, Wish: true);
        }

    }
}
