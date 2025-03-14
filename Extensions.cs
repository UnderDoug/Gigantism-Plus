using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;

namespace HNPS_GigantismPlus
{
    public static class Extensions
    {
        // checks if part is managed externally
        public static bool IsExternallyManagedLimb(this BodyPart part)  // Renamed method
        {
            if (part?.Manager == null) return false;

            // Check for HelpingHands or AgolgotChord managed parts
            if (part.Manager.EndsWith("::HelpingHands") || part.Manager.EndsWith("::AgolgotChord"))
                return true;

            // Check for Nephal claws (Agolgot parts that don't use the manager)
            if (!string.IsNullOrEmpty(part.DefaultBehaviorBlueprint) &&
                part.DefaultBehaviorBlueprint.StartsWith("Nephal_Claw"))
                return true;

            return false;
        } //!-- public static bool IsExternallyManagedLimb(BodyPart part)

        public static DieRoll AdjustDieCount(this DieRoll DieRoll, int Amount)
        {
            Debug.Entry(4, $"@ AdjustDieCount(this DieRoll DieRoll: {DieRoll.ToString()}, int Amount: {Amount})", Indent: 6);
            if (DieRoll == null)
            {
                Debug.Entry(4, "AdjustDieCount", "DieRoll null", Indent: 7);
                return null;
            }
            int type = DieRoll.Type;
            if (DieRoll.LeftValue > 0)
            {
                Debug.Entry(4, "AdjustDieCount", "DieRoll.LeftValue > 0", Indent: 7);
                DieRoll.LeftValue += Amount;
                Debug.Entry(4, "DieRoll.LeftValue", $"{DieRoll.LeftValue}", Indent: 8);
                Debug.Entry(4, "Collapse ^^<<", Indent: 8);
                return DieRoll;
            }
            else
            {
                Debug.Entry(4, "AdjustDieCount", "DieRoll.LeftValue == 0", Indent: 7);
                Debug.Entry(4, "AdjustDieCount", "Recursing >>VV", Indent: 7);
                if (DieRoll.RightValue > 0) return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.RightValue);
                return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.Right);
            }
        }
        public static string AdjustDieCount(this string DieRoll, int Amount)
        {
            DieRoll dieRoll = new(DieRoll);
            return dieRoll.AdjustDieCount(Amount).ToString();
        }
        public static bool AdjustDieCount(this MeleeWeapon MeleeWeapon, int Amount)
        {
            MeleeWeapon.BaseDamage = MeleeWeapon.BaseDamage.AdjustDieCount(Amount);
            return true;
        }

        public static int GetNaturalWeaponModsCount(this GameObject GO)
        {
            return GO.GetIntProperty("ModNaturalWeaponCount");
        }
        public static bool HasNaturalWeaponMods(this GameObject GO)
        {
            return GO.GetNaturalWeaponModsCount() > 0;
        }

        public static string BonusOrPenalty(this int Int) 
        {
            return Int >= 0 ? "bonus" : "penalty";
        }

        public static string BonusOrPenalty(this string SignedInt)
        {
            if (int.TryParse(SignedInt, out int Int))
                return Int >= 0 ? "bonus" : "penalty";
            throw new ArgumentException($"int.TryParse(SignedInt) failed to parse \"{SignedInt}\". SignedInt must be capable of conversion to int.");
        }

        public static StringBuilder AppendGigantic(this StringBuilder sb, string value)
        {
            sb.AppendColored("gigantic", value);
            return sb;
        }
        public static StringBuilder AppendRule(this StringBuilder sb, string value)
        {
            // different from AppendRules (plural) since this doesn't force a new-line.
            sb.AppendColored("rules", value);
            return sb;
        }

        public static string Color(this string Text, string Color)
        {
            return "{{" + Color + "|" + Text + "}}";
        }
        public static string MaybeColor(this string Text, string Color, bool Pretty = true)
        {
            if (Pretty && Color != "") return Text.Color(Color);
            return Text;
        }

        public static string OptionalColor(this string Text, string Color, string FallbackColor = "", int Option = 3)
        {
            // 3: Most Colorful
            // 2: Vanilla Only
            // 1: Plain Text
            return Text.MaybeColor(Color, Option > 2).MaybeColor(FallbackColor, Option > 1);
        }
        public static string OptionalColorGigantic(this string Text, int Option = 3)
        {
            return Text.OptionalColor(Color: "gigantic", FallbackColor: "w", Option);
        }
        public static string OptionalColorGiant(this string Text, int Option = 3)
        {
            return Text.OptionalColor(Color: "giant", FallbackColor: "w", Option);
        }

        // ripped from the CyberneticPropertyModifier part, converted into extension.
        // Props must equal "string:int;string:int;string:int" where
        // string   is an IntProperty
        // int      is the value
        // ;        delimits each pair.
        // Example: "ChargeRangeModifier:2;JumpRangeModifier:1"
        public static Dictionary<string, int> ParseIntProps(this string Props)
        {
            Dictionary<string, int> dictionary = new();
            string[] array = Props.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(array2[0], Convert.ToInt32(array2[1]));
            }
            return dictionary;
        }

        // as above, but for int:int progressions (good for single value level progressions).
        // Props must equal "string:string;string:string;string:string" where
        // string   is a StringProperty
        // string      is the value
        // ;        delimits each pair.
        // Example: "StringProp:StringValue;AnotherStringProp:SecondValue"
        public static Dictionary<string, string> ParseStringProps(this string Props)
        {
            Dictionary<string, string> dictionary = new();
            string[] array = Props.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(array2[0], array2[1]);
            }
            return dictionary;
        }

        // as above, but for int:int progressions (good for single value level progressions).
        // Progression must equal "int:int;int:int;int:int" where
        // int      is the progression "interval"
        // int      is the value being progression
        // ;        delimits each pair.
        // Example: "1:2;3:3;6:4;9:5" starts at 2, and increases 1 every 3rd "interval"
        public static Dictionary<int, int> ParseIntProgInt(this string Progression)
        {
            Dictionary<int, int> dictionary = new();
            string[] array = Progression.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                dictionary.Add(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]));
            }
            return dictionary;
        }

        // as above, but for int:DieRoll progressions (good for level-based damage progressions).
        // Progression must equal "int:(string)DieRoll;int:(string)DieRoll;int:(string)DieRoll" where
        // int              is the progression "interval"
        // (string)DieRoll  is string formatted DieRoll being progression
        // ;                delimits each pair.
        // Example: "1:1d2;3:1d3;6:1d4;9:1d5" starts at 1d2, and increases d1 every 3rd "interval"
        public static Dictionary<int, DieRoll> ParseIntProgDieRoll(this string Progression)
        {
            Dictionary<int, DieRoll> dictionary = new();
            string[] array = Progression.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(':');
                DieRoll dieRoll = new DieRoll(array2[1]);
                dictionary.Add(Convert.ToInt32(array2[0]), dieRoll);
            }
            return dictionary;
        }

        // Similar to above, but it takes a series of string and int properties, intermixed, and gives them to two appropriately typed dictionaries.
        public static bool ParseProps(this string Props, out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps)
        {
            Dictionary<string, string> stringDictionary = new();
            Dictionary<string, int> intDictionary = new();
            string[] props = Props.Split(';');
            for (int i = 0; i < props.Length; i++)
            {
                string[] prop = props[i].Split(':');
                if (int.TryParse(prop[1], out int result))
                {
                    intDictionary.Add(prop[0], result);
                }
                else
                {
                    stringDictionary.Add(prop[0], prop[1]);
                }
            }
            StringProps = stringDictionary;
            IntProps = intDictionary;
            if (StringProps.Count == 0 && IntProps.Count == 0)
                return false;
            return true;
        }

        public static UD_ManagedBurrowingClaws ConvertToManaged(this BurrowingClaws burrowingClaws)
        {
            UD_ManagedBurrowingClaws managedBurrowingClaws = new()
            {
                Level = burrowingClaws.Level
            };

            return managedBurrowingClaws;
        }
        public static UD_ManagedCrystallinity ConvertToManaged(this Crystallinity crystallinity)
        {
            UD_ManagedCrystallinity managedCrystallinity = new()
            {
                Level = crystallinity.Level,
                RefractAdded = crystallinity.RefractAdded
            };

            return managedCrystallinity;
        }

        public static bool TryGetNaturalWeaponCyberneticsList(this Body body, out string FistReplacement)
        {
            List<GameObject> cyberneticsList = (from c in body.GetInstalledCybernetics()
                                                where c.HasPart<CyberneticsFistReplacement>() == true
                                                select c).ToList();
            if (cyberneticsList == null)
            {
                FistReplacement = string.Empty;
                return false;
            }
            int highest = -1;
            string[] rank = new string[4]
            {
                "CarbideFist",
                "FulleriteFist",
                "CrysteelFist",
                "RealHomosapien_ZetachromeFist"
            };
            foreach (GameObject handbone in cyberneticsList)
            {
                string fistObject = handbone.GetPart<CyberneticsFistReplacement>().FistObject;
                int index = Array.IndexOf(rank, fistObject);
                if (index > highest) highest = index;
                if (highest == rank.Length - 1) break;
            }
            if (highest == -1)
            {
                FistReplacement = string.Empty;
                return false;
            }
            FistReplacement = rank[highest];
            return true;
        }

        public static void GigantifyInventory(this GameObject Creature, bool Option = true, bool GrenadeOption = false)
        {
            string creatureDebugName = Creature.DebugName;
            string creatureBlueprint = Creature.Blueprint;


            bool creatureIsMerchant = Creature.IsMerchant();
            int merchantChance = 1;
            int merchantGrenadeOdds = 2;
            int merchantTradeGoodsOdds = 4;
            int merchantTonicsOdds = 4;
            int merchantRareTonicsOdds = 10;

            if (!Option) goto Exit; // skip if Option disabled
            if (!Creature.IsCreature) goto Exit; // skip non-creatures
            if (!Creature.HasPart<GigantismPlus>())goto Exit; // skip non-gigantic creatures
            if (Creature.Inventory == null) goto Exit; // skip objects without inventory
            if (Creature.GetIntProperty("InventoryGigantified") > 0) goto Exit; // skip if inventory has already been gigantified.

            Debug.Entry(3, $"* GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption})", Indent: 1);
            Debug.Divider(3, Indent: 1);

            Debug.Entry(3, "Making inventory items gigantic for creature", creatureBlueprint, Indent: 1);

            // Create a copy of the items list to avoid modifying during enumeration
            List<GameObject> itemsToProcess = new(Creature.Inventory.Objects);

            Debug.Entry(3, "> foreach (var item in itemsToProcess)", Indent: 1);
            Debug.Divider(4, "-", Count: 25, Indent: 1);
            foreach (GameObject item in itemsToProcess)
            {
                string ItemDebug = item.DebugName;
                string ItemName = item.Blueprint;
                Debug.DiveIn(3, $"{ItemDebug}", Indent: 1);
                int NoThanks = 0;
                // Can the item have the gigantic modifier applied?
                if (ItemModding.ModificationApplicable("ModGigantic", item))
                {
                    Debug.LoopItem(4, "can be made ModGigantic", Indent: 3);
                    // Is the item already gigantic? Don't attempt to apply it again.
                    if (item.HasPart<ModGigantic>())
                    {
                        Debug.LoopItem(4, "already gigantic", "NoThanks++; x/", Indent: 3);
                        NoThanks++;
                    }
                    else
                    {
                        Debug.LoopItem(4, "not gigantic", Indent: 3);
                    }

                    // Is the item a grenade, and is the option not set to include them?
                    if (item.HasTag("Grenade"))
                    {
                        if (!GrenadeOption || creatureIsMerchant)
                        {
                            if (!GrenadeOption) Debug.LoopItem(4, "grenade (excluded)", "NoThanks++; x/", Indent: 3);
                            if (creatureIsMerchant) Debug.LoopItem(4, "grenade (isMerchant)", "NoThanks++; x/", Indent: 3);
                            NoThanks++;

                            if (creatureIsMerchant && merchantChance.ChanceIn(merchantGrenadeOdds))
                            {
                                Debug.LoopItem(4,
                                    $"but! MerchanctChance {merchantChance} in {merchantGrenadeOdds}",
                                    $"NoThanks--;",
                                    Indent: 4);
                                NoThanks--;
                            }
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, "not grenade", Indent: 3);
                    }

                    // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                    if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                    {
                        Debug.LoopItem(4, "TradeGoods", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantChance.ChanceIn(merchantTradeGoodsOdds))
                        {
                            Debug.LoopItem(4,
                                $"but! MerchanctChance {merchantChance} in {merchantTradeGoodsOdds}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, "not TradeGoods", Indent: 3);
                    }

                    // Is the item a non-rare tonic? Double doses are basically useless in the early game
                    if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.Entry(4, "Tonics_NonRare", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantChance.ChanceIn(merchantTonicsOdds))
                        {
                            Debug.LoopItem(4,
                                $"but! MerchanctChance {merchantChance} in {merchantTonicsOdds}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, "not Tonics_NonRare", Indent: 3);
                    }

                    // Is the item a rare tonic? Double doses are basically useless in the early game
                    if (item.HasPart<Tonic>() && !item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.Entry(4, "Rare Tonic", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantChance.ChanceIn(merchantRareTonicsOdds))
                        {
                            Debug.LoopItem(4,
                                $"but! MerchanctChance {merchantChance} in {merchantRareTonicsOdds}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.LoopItem(4, "not Rare Tonics", Indent: 3);
                    }

                    Debug.Entry(3, $"NoThanks", $"{NoThanks}", Indent: 2);
                    if (NoThanks > 0) goto Skip;

                    Debug.Entry(3, $"Gigantifying {ItemName}", Indent: 2);
                    ItemModding.ApplyModification(item, "ModGigantic");
                    if (!item.HasPart<ModGigantic>()) Debug.Entry(3, ItemName, "<!!> Gigantification Failed", Indent: 2);
                    else Debug.Entry(3, ItemName, "has been Gigantified", Indent: 2);
                    Debug.DiveOut(3, $"{ItemDebug}", Indent: 1);
                    continue;
                }
                else
                {
                    Debug.LoopItem(4, "cannot be made gigantic x/", Indent: 3);
                }
            Skip:
                Debug.Entry(3, "/x Skipping", Indent: 2);
                Debug.DiveOut(3, $"{ItemDebug}", Indent: 1);
            }
            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "x foreach (GameObject item in itemsToProcess) >//", Indent: 1);

            // Now equip all items that should be equipped

            Debug.LoopItem(3, "Creature.WantToReequip()", Indent: 1);
            Creature.WantToReequip();

            Debug.Divider(3, Indent: 1);
            Debug.Entry(3, $"x GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption}) ]//", Indent: 1);
            
            Exit:
            return;
        } //!-- public static void GigantifyInventory(this GameObject Creature, bool Option = true, bool GrenadeOption = false)
        
        public static void SetSwingSound(this GameObject Object, string Path)
        {
            if (Path != null && Path != "")
                Object.SetStringProperty("SwingSound", Path);
        }
        public static void SetBlockedSound(this GameObject Object, string Path)
        {
            if (Path != null && Path != "")
                Object.SetStringProperty("BlockedSound", Path);
        }
        public static void SetEquipmentFrameColors(this GameObject Object, string Path = null)
        {
            Object.SetStringProperty("EquipmentFrameColors", Path, true);
        }

        public static void CheckAffectedEquipmentSlots(this GameObject Actor)
        {
            Body Body = Actor?.Body;
            Debug.Entry(3, $"* CheckAffectedEquipmentSlots(this GameObject Actor: {Actor.ShortDisplayName})");
            if (Actor == null || Body == null)
            {
                Debug.Entry(3, "x (Actor == null || Body == null)");
                return;
            }

            List<GameObject> list = Event.NewGameObjectList();
            Debug.Entry(3, "* foreach (BodyPart bodyPart in Body.LoopParts())");
            foreach (BodyPart bodyPart in Body.LoopParts())
            {
                GameObject equipped = bodyPart.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    Debug.Entry(3, "- Part", equipped.DebugName);
                    list.Add(equipped);
                    int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                    int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, bodyPart.Type, true);
                    if (partCountEquippedOn != slotsRequiredFor && bodyPart.TryUnequip(true, true, false, false) && partCountEquippedOn > slotsRequiredFor)
                    {
                        equipped.SplitFromStack();
                        bodyPart.Equip(equipped, new int?(0), true, false, false, true);
                    }
                }
            }
            Debug.Entry(3, $"x CheckAffectedEquipmentSlots(this GameObject Actor: {Actor.ShortDisplayName}) *//");
        }

        public static IPart RequirePart(this GameObject Object, string Part, bool DoRegistration = true, bool Creation = false)
        {
            if (Object.HasPart(Part))
            {
                return Object.GetPart(Part);
            }
            GamePartBlueprint gamePartBlueprint = new(Part);
            IPart part = gamePartBlueprint.Reflector?.GetInstance() ?? (Activator.CreateInstance(gamePartBlueprint.T) as IPart);
            part.ParentObject = Object;
            gamePartBlueprint.InitializePartInstance(part);
            return Object.AddPart(part, DoRegistration: DoRegistration,Creation: Creation);
        }
    }
}
