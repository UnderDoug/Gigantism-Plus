using Genkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Text;

using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.World.ObjectBuilders;
using XRL.World.Parts.Skill;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using XRL.UI;
using Microsoft.CodeAnalysis;

namespace HNPS_GigantismPlus
{
    public static class Extensions
    {
        private static bool doDebug => true;
        private static bool getDoDebug(string MethodName)
        {
            if (MethodName == nameof(GigantifyInventory))
                return true;

            if (MethodName == nameof(CheckEquipmentSlots))
                return false;

            if (MethodName == nameof(TagIsIncludedOrNotExcluded))
                return false;

            if (MethodName == nameof(MakeIncludeExclude))
                return false;

            if (MethodName == nameof(PullInsideFromEdges))
                return false;

            if (MethodName == nameof(PullInsideFromEdge))
                return false;

            if (MethodName == nameof(GetNumberedTileVariants))
                return false;

            return doDebug;
        }

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

        public static int GetDieCount(this DieRoll DieRoll)
        {
            if (DieRoll == null)
            {
                return 0;
            }
            if (DieRoll.LeftValue > 0)
            {
                return DieRoll.LeftValue;
            }
            else
            {
                return DieRoll.Left.GetDieCount();
            }
        }
        public static int GetDieCount(this string DieRoll)
        {
            DieRoll dieRoll = new(DieRoll);
            return dieRoll.GetDieCount();
        }

        public static DieRoll AdjustDieCount(this DieRoll DieRoll, int Amount)
        {
            if (DieRoll == null)
            {
                return null;
            }
            int type = DieRoll.Type;
            if (DieRoll.LeftValue > 0)
            {
                DieRoll.LeftValue += Amount;
                return DieRoll;
            }
            else
            {
                if (DieRoll.RightValue > 0) return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.RightValue);
                return new(type, DieRoll.Left.AdjustDieCount(Amount), DieRoll.Right);
            }
        } //!-- public static DieRoll AdjustDieCount(this DieRoll DieRoll, int Amount)
       
        public static string AdjustDieCount(this string DieRoll, int Amount)
        {
            DieRoll dieRoll = new(DieRoll);
            return dieRoll.AdjustDieCount(Amount).ToString();
        }
        public static bool AdjustDamageDieCount(this MeleeWeapon MeleeWeapon, int Amount)
        {
            MeleeWeapon.BaseDamage = MeleeWeapon.BaseDamage.AdjustDieCount(Amount);
            return true;
        }
        public static bool AdjustDamageDieCount(this ThrownWeapon ThrownWeapon, int Amount)
        {
            ThrownWeapon.Damage = ThrownWeapon.Damage.AdjustDieCount(Amount);
            return true;
        }
        public static bool AdjustDamageDieCount(this Projectile Projectile, int Amount)
        {
            Projectile.BaseDamage = Projectile.BaseDamage.AdjustDieCount(Amount);
            return true;
        }

        public static int GetNaturalWeaponModsCount(this GameObject GO)
        {
            List<ModNaturalEquipmentBase> naturalEquipmentMods = GO.GetPartsDescendedFrom<ModNaturalEquipmentBase>();
            return naturalEquipmentMods.Count;
        }
        public static bool HasNaturalWeaponMods(this GameObject GO)
        {
            return GO.HasPartDescendedFrom<ModNaturalEquipmentBase>();
        }

        public static string BonusOrPenalty(this int Int) 
        {
            return Int >= 0 ? "bonus" : "penalty";
        }
        public static string BonusOrPenalty(this string SignedInt)
        {
            if (int.TryParse(SignedInt, out int Int))
                return Int >= 0 ? "bonus" : "penalty";
            throw new ArgumentException(
                $"{nameof(BonusOrPenalty)}(this string SignedInt): " +
                $"int.TryParse(SignedInt) failed to parse \"{SignedInt}\". " +
                $"SignedInt must be capable of conversion to int.");
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
        public static string OptionalColorYuge(this string Text, int Option = 3)
        {
            return Text.OptionalColor(Color: "yuge", FallbackColor: "w", Option);
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
                Level = burrowingClaws.Level,
                DigUpActivatedAbilityID = burrowingClaws.DigUpActivatedAbilityID,
                DigDownActivatedAbilityID = burrowingClaws.DigDownActivatedAbilityID,
                EnableActivatedAbilityID = burrowingClaws.EnableActivatedAbilityID,
            };

            return managedBurrowingClaws;
        }
        public static UD_ManagedCrystallinity ConvertToManaged(this Crystallinity crystallinity)
        {
            UD_ManagedCrystallinity managedCrystallinity = new()
            {
                Level = crystallinity.Level,
                RefractAdded = crystallinity.RefractAdded,
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

        public static void GigantifyInventory(this GameObject Creature, bool Option = true, bool GrenadeOption = false, bool Wish = false, bool Force = false)
        {
            string creatureBlueprint = Creature.Blueprint;

            bool creatureIsMerchant = Creature.HasPart<GenericInventoryRestocker>();
            (DieRoll die, int high) merchantBaseChance = (new("1d5"), 4);
            (DieRoll die, int high) merchantGrenades = (new("1d2"), 2);
            (DieRoll die, int high) merchantTradeGoods = (new("1d4"), 4);
            (DieRoll die, int high) merchantTonics = (new("1d4"), 4);
            (DieRoll die, int high) merchantRareTonics = (new("1d10"), 10);

            if (Creature.ID.Is("1") && !Creature.HasPart<GigantismPlus>() && !Force) return; // redundancy, just in case.
            if (!Option && !Force) return; // skip if Option disabled
            if (!Creature.IsCreature && !Force) return; // skip non-creatures
            if (Creature.Inventory == null) return; // skip creatures without inventory

            Debug.Entry(3, $"* GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption}, Force: {Force})", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            Debug.Divider(3, Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));

            if (Force)
            {
                Option = Force;
                GrenadeOption = Force;
            }

            Debug.Entry(3, "Making inventory items gigantic for creature", creatureBlueprint, Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            Debug.Entry(3, $"Creature is merchant", creatureIsMerchant ? "Yeh" : "Nah", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));

            // Create a copy of the items list to avoid modifying during enumeration
            List<GameObject> itemsToProcess = new(Creature.GetInventoryAndEquipment());

            Debug.Entry(3, "> foreach (GameObject item in itemsToProcess)", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            foreach (GameObject item in itemsToProcess)
            {
                string ItemDebug = item.DebugName;
                string ItemName = item.Blueprint;
                Debug.DiveIn(3, $"{ItemDebug}", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
                int NoThanks = 0;
                // Can the item have the gigantic modifier applied?
                if (ItemModding.ModificationApplicable("ModGigantic", item))
                {
                    Debug.CheckYeh(4, "eligible to be made ModGigantic", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    // Is the item already gigantic? Don't attempt to apply it again.
                    if (item.HasPart<ModGigantic>())
                    {
                        Debug.CheckNah(4, "already gigantic", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        NoThanks++;
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not already gigantic", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    // Is the item a grenade, and is the option not set to include them?
                    if (item.HasTag("Grenade"))
                    {
                        if (!GrenadeOption || creatureIsMerchant)
                        {
                            if (!GrenadeOption)
                            { 
                                Debug.CheckNah(4, "grenade (excluded)", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory))); 
                                NoThanks++; 
                            }
                            if (creatureIsMerchant) 
                            { 
                                Debug.CheckNah(4, "grenade (isMerchant)", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory))); 
                                NoThanks++; 
                            }

                            if (creatureIsMerchant && merchantGrenades.die.Resolve() >= merchantGrenades.high)
                            {
                                Debug.LoopItem(4,
                                    $"but!] merchantGrenades {merchantGrenades.die} rolled at or above {merchantGrenades.high}",
                                    $"NoThanks--;",
                                    Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                                NoThanks--;
                            }
                        }
                        else if (GrenadeOption)
                        {
                            Debug.CheckYeh(4, "grenade (included)", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not grenade", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                    if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                    {
                        Debug.CheckNah(4, "TradeGoods", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        NoThanks++;

                        if (creatureIsMerchant && merchantTradeGoods.die.Resolve() >= merchantTradeGoods.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantTradeGoods {merchantTradeGoods.die} rolled at or above {merchantTradeGoods.high}",
                                $"NoThanks--;",
                                Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not TradeGoods", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    // Is the item a non-rare tonic? Double doses are basically useless in the early game
                    if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.CheckNah(4, "Tonics_NonRare", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        NoThanks++;

                        if (creatureIsMerchant && merchantTonics.die.Resolve() >= merchantTonics.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantTonics {merchantTonics.die} rolled at or above {merchantTonics.high}",
                                $"NoThanks--;",
                                Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not Tonics_NonRare", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    // Is the item a rare tonic? Double doses are basically useless in the early game
                    if (item.HasPart<Tonic>() && !item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.CheckNah(4, "Rare Tonic", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        NoThanks++;

                        if (creatureIsMerchant && merchantRareTonics.die.Resolve() >= merchantRareTonics.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantRareTonics {merchantRareTonics.die} rolled at or above {merchantRareTonics.high}",
                                $"NoThanks--;",
                                Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not Rare Tonics", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    // Is the item held by a merchant, and did their roll fail?
                    if (creatureIsMerchant)
                    {
                        Debug.CheckNah(4, "creatureIsMerchant is True", "NoThanks++; x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        NoThanks++;

                        if (merchantBaseChance.die.Resolve() >= merchantBaseChance.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantBaseChance {merchantBaseChance.die} rolled at or above {merchantBaseChance.high}",
                                $"NoThanks--;",
                                Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                            NoThanks--;
                        }
                        else 
                        {
                            Debug.LoopItem(4, 
                                $"and!] merchantBaseChance {merchantBaseChance.die} rolled below {merchantBaseChance.high}",
                                "Bummer!",
                                Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4,
                            $"creatureIsMerchant is False",
                            Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    }

                    Debug.Entry(3, $"NoThanks", $"{NoThanks}", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    if (NoThanks > 0 && !Wish)
                    {
                        Debug.Entry(3, "/x Skipping", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        Debug.DiveOut(3, $"{ItemDebug}", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
                        continue;
                    }
                    string byWish = Wish ? ", by Wish!" : "";
                    Debug.Entry(3, $"Gigantifying {ItemName}{byWish}", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    item.ApplyModification("ModGigantic");
                    if (!item.HasPart<ModGigantic>()) Debug.Entry(3, ItemName, "/!\\ Gigantification Failed", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    else Debug.Entry(3, ItemName, "has been Gigantified", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));

                    Debug.DiveOut(3, $"{ItemDebug}", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
                }
                else
                {
                    Debug.CheckNah(4, "ineligible to be made ModGigantic x/", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    Debug.Entry(3, "/x Skipping", Indent: 2, Toggle: getDoDebug(nameof(GigantifyInventory)));
                    Debug.DiveOut(3, $"{ItemDebug}", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
                }
            }
            Debug.Divider(4, "-", Count: 25, Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            Debug.Entry(4, "x foreach (GameObject item in itemsToProcess) >//", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));

            // Now equip all items that should be equipped
            if (!Wish)
            {
                Debug.Entry(3, "Creature.WantToReequip()", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
                Creature.WantToReequip();
            }

            Debug.Divider(3, Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
            Debug.Entry(3, $"x GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption}) *//", Indent: 1, Toggle: getDoDebug(nameof(GigantifyInventory)));
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
        public static void SetEquipmentFrameColors(this GameObject Object, string TopLeft_Left_Right_BottomRight = null)
        {
            Object.SetStringProperty("EquipmentFrameColors", TopLeft_Left_Right_BottomRight, true);
        }

        public static void CheckEquipmentSlots(this GameObject Actor)
        {
            Debug.Entry(3, $"* {nameof(CheckEquipmentSlots)}(this GameObject Actor: {Actor.DebugName})", Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
            Body Body = Actor?.Body;
            if (Body != null)
            {
                List<GameObject> list = Event.NewGameObjectList();
                Debug.Entry(3, "> foreach (BodyPart bodyPart in Actor.LoopParts())", Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
                foreach (BodyPart bodyPart in Body.LoopParts())
                {
                    Debug.Entry(3, "bodyPart", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Name}]", Indent: 1, Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
                    GameObject equipped = bodyPart.Equipped;
                    if (equipped != null && !list.Contains(equipped))
                    {
                        Debug.LoopItem(3, "equipped", $"[{equipped.ID}:{equipped.ShortDisplayName}]", Indent: 2, Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
                        list.Add(equipped);
                        int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                        int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, bodyPart.Type, true);
                        if (!partCountEquippedOn.Is(slotsRequiredFor) 
                            && bodyPart.TryUnequip(true, true, false, false) 
                            && partCountEquippedOn > slotsRequiredFor)
                        {
                            equipped.SplitFromStack();
                            bodyPart.Equip(equipped, new int?(0), true, false, false, true);
                        }
                    }
                }
                Debug.Entry(3, "x foreach (BodyPart bodyPart in Actor.LoopParts()) >//", Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
            }
            else
            {
                Debug.Entry(4, $"no body on which to perform check, aborting ", Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
            }
            Debug.Entry(3, $"x {nameof(CheckEquipmentSlots)}(this GameObject Actor: {Actor.DebugName}) *//", Toggle: getDoDebug(nameof(CheckEquipmentSlots)));
        }

        public static IPart RequirePart(this GameObject Object, IPart Part, bool DoRegistration = true, bool Creation = false)
        {
            if (Object.HasPart(Part.Name))
            {
                return Object.GetPart(Part.Name);
            }
            Part.ParentObject = Object;
            return Object.AddPart(Part, DoRegistration: DoRegistration, Creation: Creation);
        }
        public static IPart RequirePart(this GameObject Object, string Part, bool DoRegistration = true, bool Creation = false)
        {
            if (Object.HasPart(Part))
            {
                return Object.GetPart(Part);
            }
            GamePartBlueprint gamePartBlueprint = new(Part);
            if (gamePartBlueprint == null) return null;
            IPart part = gamePartBlueprint.Reflector?.GetInstance() ?? (Activator.CreateInstance(gamePartBlueprint.T) as IPart);
            part.ParentObject = Object;
            gamePartBlueprint.InitializePartInstance(part);
            return Object.AddPart(part, DoRegistration: DoRegistration, Creation: Creation);
        }

        public static IPart ConvertToPart(this string Part)
        {
            GamePartBlueprint gamePartBlueprint = new(Part);
            IPart part = gamePartBlueprint.Reflector?.GetInstance() ?? (Activator.CreateInstance(gamePartBlueprint.T) as IPart);
            return part;
        }

        public static IModification ConvertToModification(this string ModPartName)
        {
            IModification ModPart;
            Type type = ModManager.ResolveType("XRL.World.Parts." + ModPartName);
            if (type == null)
            {
                MetricsManager.LogError("ConvertToModification", "Couldn'type resolve unknown mod part: " + ModPartName);
                return null;
            }
            ModPart = Activator.CreateInstance(type) as IModification;
            if (ModPart == null)
            {
                if (Activator.CreateInstance(type) is not IPart)
                {
                    MetricsManager.LogError("failed to load " + type);
                }
                else
                {
                    MetricsManager.LogError(type?.ToString() + " is not an IModification");
                }
                return null;
            }
            return ModPart;
        }

        public static ModNaturalEquipment<T> ConvertToNaturalWeaponModification<T>(this string ModPartName) 
            where T : IPart, IModEventHandler<ManageDefaultEquipmentEvent>, IManagedDefaultNaturalEquipment<T>, new()
        {
            IModification ModPart = ModPartName.ConvertToModification();
            return (ModNaturalEquipment<T>)ModPart;
        }

        public static T GetManagedNaturalEquipmentCompatiblePart<T>(this GameObject Object) 
            where T 
            : IPart
            , IManagedDefaultNaturalEquipment<T>
            , new()
        {
            T part = Object?.GetPart<T>();
            if (part != null) return part;
            List<GameObject> Cybernetics = Object?.Body?.GetInstalledCybernetics();
            if (Cybernetics == null) return null;
            foreach (GameObject cybernetic in Cybernetics)
            {
                part = cybernetic.GetPart<T>();
                if (part != null) return part;
            }
            return null;
        }

        public static bool ApplyNaturalEquipmentModification(this GameObject obj, ModNaturalEquipmentBase ModPart, GameObject Actor) 
        {
            return obj.ApplyModification(ModPart, Actor: Actor);
        }

        public static bool IsDefaultEquipmentOf(this GameObject Object, BodyPart BodyPart)
        {
            return BodyPart.DefaultBehavior == Object;
        }

        public static BodyPart EquippingPart(this GameObject Object)
        {
            Body body = Object?.Equipped?.Body;
            if (body != null)
            {
                foreach (BodyPart part in body.LoopParts())
                {
                    if (part.DefaultBehavior == Object)
                        return part;
                    if (part.Equipped == Object)
                        return part;
                    if (part.Cybernetics == Object)
                        return part;
                }
            }
            return null;
        }

        public static bool InheritsFrom(this GameObject Object, string Blueprint)
        {
            return Object.Blueprint.Is(Blueprint) || Object.GetBlueprint().InheritsFrom(Blueprint);
        }

        // partially repurposed from https://stackoverflow.com/a/32184652
        public static bool SetPropertyValue(this object @object, string PropertyName, object Value)
        {
            PropertyInfo property = @object?.GetType()?.GetProperty(PropertyName);
            if (property == null) return false;
            Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            object safeValue = (Value == null) ? null : Convert.ChangeType(Value, type);

            property.SetValue(@object, safeValue, null);
            return property?.GetValue(@object, null) != null;
        }
        // partially repurposed from https://stackoverflow.com/a/1965659
        public static bool SetFieldValue(this object @object, string FieldName, object Value)
        {
            FieldInfo field = @object?.GetType()?.GetField(FieldName);
            if (field == null) return false;
            Type type = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
            object safeValue = (Value == null) ? null : Convert.ChangeType(Value, type);

            field.SetValue(@object, safeValue);
            return field?.GetValue(@object) != null;
        }
        public static bool SetPropertyOrFieldValue(this object @object, string PropertyOrField, object Value)
        {
            return @object.SetPropertyValue(PropertyOrField, Value) || @object.SetFieldValue(PropertyOrField, Value);
        }

        public static string NearDemonstrative(this GameObject Object)
        {
            return Object.IsPlural ? "these" : "this";
        }
        public static string FarDemonstrative(this GameObject Object)
        {
            return Object.IsPlural ? "those" : "that";
        }

        public static bool Is(this string @this, string @string)
        {
            return @this == @string;
        }
        public static bool Is(this int @this, int @int)
        {
            return @this == @int;
        }
        public static bool Is(this float @this, float @float)
        {
            return @this == @float;
        }
        public static bool Is(this double @this, double @double)
        {
            return @this == @double;
        }
        public static bool Is<T>(this T @this, T @object)
            where T : class
        {
            return @this == @object;
        }

        public static string GetProcessedItem(this List<string> item, bool second, List<List<string>> items, GameObject obj)
        {
            if (item[0] == "")
            {
                if (second && item == items[0])
                {
                    return obj.It + " " + item[1];
                }
                return item[1];
            }
            if (item[0] == null)
            {
                if (second && item == items[0])
                {
                    return obj.Itis + " " + item[1];
                }
                if (item != items[0])
                {
                    bool flag = true;
                    foreach (List<string> item2 in items)
                    {
                        if (item2[0] != null)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return item[1];
                    }
                }
                return obj.GetVerb("are", PrependSpace: false) + " " + item[1];
            }
            if (second && item == items[0])
            {
                return obj.It + obj.GetVerb(item[0]) + " " + item[1];
            }
            return obj.GetVerb(item[0], PrependSpace: false) + " " + item[1];
        }

        public static string GetObjectNoun(this GameObject Object)
        {
            if (Object == null)
                return null;

            if (!Object.Understood())
                return "artifact";

            if (Object.InheritsFrom("FoldingChair") 
                && Object.HasPart<ModGigantic>())
                return "folding chair";

            if (Object.IsCreature)
                return "creature";

            if (Object.HasPart<CyberneticsBaseItem>())
                return "implant";

            if (Object.InheritsFrom("Tonic"))
                return "tonic";

            if (Object.HasPart<Medication>())
                return "medication";

            if (Object.InheritsFrom("Energy Cell"))
                return "energy cell";

            if (Object.InheritsFrom("LightSource"))
                return "light source";

            if (Object.InheritsFrom("Tool"))
                return "tool";

            if (Object.InheritsFrom("BaseRecoiler"))
                return "recoiler";

            if (Object.InheritsFrom("BaseNugget"))
                return "nugget";

            if (Object.InheritsFrom("Gemstone"))
                return "gemstone";

            if (Object.InheritsFrom("Random Figurine"))
                return "figurine";

            if (Object.HasPart<Applicator>())
                return "applicator";

            if (Object.HasPart<Tombstone>())
                return "tombstone";

            if (Object.TryGetPart(out MissileWeapon missileWeapon))
            {
                if (missileWeapon.Skill.Contains("Shotgun") 
                    || Object.InheritsFrom("BaseShotgun") 
                    || (Object.TryGetPart(out MagazineAmmoLoader loader) && loader.AmmoPart.Is("AmmoShotgunShell")))
                    return "shotgun";

                if (Object.InheritsFrom("BaseHeavyWeapon"))
                    return "heavy weapon";

                if (Object.InheritsFrom("BaseBow"))
                    return "bow";

                if (Object.InheritsFrom("BaseRifle"))
                    return "rifle";

                if (Object.InheritsFrom("BasePistol"))
                    return "pistol";

                return "missile weapon";
            }

            if (Object.TryGetPart(out ThrownWeapon thrownWeapon) && !thrownWeapon.IsImprovised())
            {
                if (Object.InheritsFrom("BaseBoulder"))
                    return "boulder";

                if (Object.InheritsFrom("BaseStone"))
                    return "stone";

                if (Object.InheritsFrom("Grenade"))
                    return "grenade";

                if (Object.InheritsFrom("BaseDagger"))
                    return "dagger";

                return "thrown weapon";
            }

            if (Object.TryGetPart(out MeleeWeapon meleeWeapon) && !meleeWeapon.IsImprovised())
            {
                if (Object.HasPart<NaturalEquipmentManager>())
                    return Object.Render.DisplayName;

                if (Object.InheritsFrom("BaseCudgel"))
                    return "cudgel";

                if (Object.InheritsFrom("BaseAxe"))
                    return "axe";

                if (Object.InheritsFrom("BaseLongBlade"))
                    return "long blade";

                if (Object.InheritsFrom("BaseDagger"))
                    return "short blade";

                return "weapon";
            }

            if (Object.TryGetPart(out Armor armor))
            {
                if (!Object.IsPluralIfKnown)
                {
                    switch (armor.WornOn)
                    {
                        case "Back":
                            {
                                if (Object.Blueprint.Is("Mechanical Wings"))
                                    return "wing";

                                if (Object.Blueprint.Is("Gas Tumbler"))
                                    return "tumbler";

                                if (armor.CarryBonus > 0
                                    || Object.Blueprint.Is("Gyrocopter Backpack")
                                    || Object.Blueprint.Is("SkybearJetpack"))
                                    return "pack";

                                return "cloak";
                            }
                        case "Head":
                            {
                                if (armor.AV < 3)
                                    return "hat";

                                return "helmet";
                            }
                        case "Face":
                            {
                                if (Object.InheritsFrom("BaseMask"))
                                    return "mask";

                                break;
                            }
                        case "Body":
                            {
                                if (Object.Blueprint.Contains("Plate"))
                                    return "plate";

                                if (armor.AV > 2)
                                    return "suit";

                                return "vest";
                            }
                        case "Arm":
                            {
                                if (Object.InheritsFrom("BaseUtilityBracelet"))
                                    return "utility bracelet";

                                if (Object.InheritsFrom("BaseBracelet"))
                                    return "bracelet";

                                if (Object.InheritsFrom("BaseArmlet"))
                                    return "armlet";

                                break;
                            }
                        default:
                            return "armor";
                    };
                }
                switch (armor.WornOn)
                {
                    case "Back":
                        {
                            if (Object.Blueprint.Is("Mechanical Wings"))
                                return "wing";

                            if (Object.Blueprint.Is("Gas Tumbler"))
                                return "tumbler";

                            break;
                        }
                    case "Face":
                        {
                            if (Object.HasPart<Spectacles>())
                                return "spectacle";

                            if (Object.InheritsFrom("BaseEyewear"))
                                return "goggle";

                            if (Object.InheritsFrom("BaseFaceJewelry"))
                                return "jewelry";

                            if (Object.Blueprint.Is("VISAGE"))
                                return "scanner";

                            break;
                        }
                    case "Hands":
                        {
                            if (Object.HasPart<Metal>())
                                return "gauntlet";

                            return "glove";
                        }
                    case "Feet":
                        {
                            if (Object.InheritsFrom("BaseBoot"))
                            {
                                if (Object.HasPart<Metal>())
                                    return "sabaton";
                                return "boot";
                            }

                            return "shoe";
                        }
                    default:
                        break;
                }
            }
            if (Object.InheritsFrom("Furniture"))
            {
                string bodyType = Object.GetPropertyOrTag("BodyType");
                string @class = Object.GetPropertyOrTag("Class");
                if (!@class.IsNullOrEmpty())
                {
                    if (!bodyType.IsNullOrEmpty())
                    {
                        if (bodyType.Is("Pillow"))
                        {
                            return "seat";
                        }
                    }
                    return @class;
                }
                if (!bodyType.IsNullOrEmpty())
                {
                    return bodyType.SplitCamelCase().ToLower();
                }
                if (Object.InheritsFrom("Statue") || Object.InheritsFrom("Random Statue"))
                {
                    return "statue";
                }
                if (Object.InheritsFrom("Eater Hologram"))
                {
                    return "hologram";
                }
                if (Object.InheritsFrom("Switch"))
                {
                    return "switch";
                }
                if (Object.InheritsFrom("Sign"))
                {
                    return "sign";
                }
                if (Object.InheritsFrom("BaseBookshelf"))
                {
                    return "bookshelf";
                }
                if (Object.HasPart<MergeConduit>())
                {
                    return "power conduit";
                }
                if (Object.HasPart<Container>() || Object.HasPart<LiquidVolume>())
                {
                    return "container";
                }
                if (Object.TryGetPart(out LightSource lightSource) && lightSource.Radius > 0)
                {
                    return "light source";
                }
            }
            if (Object.HasPart<Chair>())
            {
                return "chair";
            }
            if (Object.HasPart<Bed>())
            {
                return "bed";
            }
            if (Object.HasPart<Container>() || Object.HasPart<LiquidVolume>())
            {
                return "container";
            }
            switch (Scanning.GetScanTypeFor(Object))
            {
                case Scanning.Scan.Tech:
                    return "artifact";
                case Scanning.Scan.Bio:
                    return "organism";
                default:
                    if (Object.HasPart<XRL.World.Parts.Shield>() && !Object.IsPluralIfKnown)
                    {
                        return "shield";
                    }
                    if (!Object.Takeable)
                    {
                        return Object.Render?.DisplayName ?? "object";
                    }
                    return Object.Render?.DisplayName ?? "item";
            }
        }

        public static bool IsImprovised(this ThrownWeapon ThrownWeapon)
        {
            bool hasImprovisedProp =
                ThrownWeapon.ParentObject.HasTagOrStringProperty("IsImprovisedThrown")
             && ThrownWeapon.ParentObject.GetStringProperty("IsImprovisedThrown") != "false";

            ThrownWeapon @default = new();
            return ThrownWeapon.SameAs(@default)
                || ThrownWeapon.ParentObject.GetIntProperty("IsImprovisedThrown") > 0
                || hasImprovisedProp;
        }

        public static bool IsImprovised(this MeleeWeapon MeleeWeapon)
        {
            bool isImprovisedButGigantic = MeleeWeapon.ParentObject.HasPart<ModGigantic>()
             && MeleeWeapon.MaxStrengthBonus == 0 
             && MeleeWeapon.PenBonus == 0 
             && MeleeWeapon.HitBonus == 0 
             && (MeleeWeapon.BaseDamage.Is("1d2") || MeleeWeapon.BaseDamage.Is("1d2+3")) 
             && MeleeWeapon.Ego == 0 
             && MeleeWeapon.Skill.Is("Cudgel") 
             && MeleeWeapon.Stat.Is("Strength") 
             && MeleeWeapon.Slot.Is("Hand") 
             && MeleeWeapon.Attributes.IsNullOrEmpty();

            bool hasImprovisedProp = 
                MeleeWeapon.ParentObject.HasTagOrStringProperty("IsImprovisedMelee") 
             && !MeleeWeapon.ParentObject.GetStringProperty("IsImprovisedMelee").Is("false");

            MeleeWeapon @default = new();

            return MeleeWeapon.SameAs(@default) 
                || MeleeWeapon.IsImprovisedWeapon() 
                || isImprovisedButGigantic 
                || MeleeWeapon.ParentObject.GetIntProperty("IsImprovisedMelee") > 0
                || hasImprovisedProp;
        }

        public static List<GameObject> GetNaturalEquipment(this Body Body)
        {
            static bool filter(GameObject GO) { return GO.HasPart<NaturalEquipment>() || GO.HasTag("NaturalGear"); }
            return Body.GetEquippedObjects(filter);
        }

        public static T DrawRandomElement<T>(this List<T> Bag, T ExceptForElement = null, List<T> ExceptForElements = null)
            where T : class
        {
            return Bag.DrawSeededElement(Guid.Empty, ExceptForElement, ExceptForElements);
        }
        public static T DrawSeededElement<T>(this List<T> Bag, Guid Seed, T ExceptForElement = null, List<T> ExceptForElements = null)
            where T : class
        {
            if (Bag.IsNullOrEmpty()) return null;
            List<T> drawBag = new();
            drawBag.AddRange(Bag);
            ExceptForElements ??= new();
            if (drawBag.Contains(ExceptForElement)) drawBag.Remove(ExceptForElement);
            foreach (T exceptForElement in ExceptForElements)
            {
                if (drawBag.Contains(exceptForElement)) drawBag.Remove(exceptForElement);
            }
            if (drawBag.IsNullOrEmpty()) return null;
            T output = null;
            if (Seed != Guid.Empty)
            {
                string seed = Seed.ToString();
                int low = 0;
                int high = (drawBag.Count - 1) * 7;
                int roll = Stat.SeededRandom(seed, low, high) % (drawBag.Count - 1);
                output = drawBag.ElementAt(roll);
            }
            output ??= drawBag.GetRandomElement();
            Bag.Remove(output);
            return output;
        }
        public static T DrawElement<T>(this List<T> Bag, T Element)
            where T : class
        {
            T output = (!Bag.IsNullOrEmpty() || Bag.Remove(Element)) ? Element : null;
            return output;
        }

        public static T DrawRandomElement<T>(this Dictionary<string, List<T>> Bag, string FromSubBag = "", T ExceptForElement = null, List<T> ExceptForElements = null)
            where T : class
        {
            return Bag.DrawSeededElement(Guid.Empty, FromSubBag, ExceptForElement, ExceptForElements);
        }
        public static T DrawSeededElement<T>(this Dictionary<string, List<T>> Bag, Guid Seed, string FromSubBag = "", T ExceptForElement = null, List<T> ExceptForElements = null)
            where T : class
        {
            List<T> drawBag = new();
            ExceptForElements ??= new();
            bool haveTargetedSubBag = FromSubBag != "" && Bag.ContainsKey(FromSubBag);
            if (haveTargetedSubBag)
            {
                drawBag = Bag[FromSubBag];
            }
            else
            {
                foreach ((_, List<T> subBag) in Bag)
                {
                    foreach (T element in subBag)
                    {
                        if (!drawBag.Contains(element)) drawBag.Add(element);
                    }
                }
            }

            T output = drawBag.DrawSeededElement(Seed, ExceptForElement, ExceptForElements);

            if (haveTargetedSubBag)
            {
                if (Bag[FromSubBag].Contains(output)) Bag[FromSubBag].Remove(output);
            }
            else
            {
                foreach ((_, List<T> subBag) in Bag)
                {
                    if (subBag.Contains(output)) subBag.Remove(output);
                }
            }
            return output;
        }
        public static T DrawElement<T>(this Dictionary<string, List<T>> Bag, T Element, string FromSubBag = "")
            where T : class
        {
            List<T> drawBag = new();
            bool haveTargetedSubBag = FromSubBag != "" && Bag.ContainsKey(FromSubBag);
            if (haveTargetedSubBag)
            {
                drawBag = Bag[FromSubBag];
            }
            else
            {
                foreach ((_, List<T> subBag) in Bag)
                {
                    foreach (T element in subBag)
                    {
                        if (!drawBag.Contains(element)) drawBag.Add(element);
                    }
                }
            }

            T output = (!drawBag.IsNullOrEmpty() && drawBag.Contains(Element)) ? Element : null;

            if (haveTargetedSubBag)
            {
                if (Bag[FromSubBag].Contains(output)) Bag[FromSubBag].Remove(output);
            }
            else
            {
                foreach ((_, List<T> subBag) in Bag)
                {
                    if (subBag.Contains(output)) subBag.Remove(output);
                }
            }
            return output;
        }
        public static bool Contains<T>(this Dictionary<string, List<T>> Bag, T Element, out string Key, string FromSubBag = "")
            where T : class
        {
            List<T> peekBag = new();
            bool haveTargetedSubBag = FromSubBag != "" && Bag.ContainsKey(FromSubBag);
            bool haveElement = false;
            Key = null;
            if (haveTargetedSubBag)
            {
                peekBag = Bag[FromSubBag];
            }
            else
            {
                foreach ((string key, List<T> subBag) in Bag)
                {
                    foreach (T element in subBag)
                    {
                        if (peekBag.Contains(element))
                        {
                            Key = key;
                            haveElement = true;
                        }
                    }
                }
            }
            return haveElement;
        }
        public static bool Contains<T>(this Dictionary<string, List<T>> Bag, T Element, string FromSubBag = "")
            where T : class
        {
            return Bag.Contains(Element, out _, FromSubBag);
        }

        public static void Gigantify(this GameObject Object, string Context = "")
        {
            Gigantified gigantified = new();
            gigantified.Initialize();
            gigantified.Apply(Object, Context);
        }

        public static GameObjectBlueprint GetGameObjectBlueprint(this GameObject GameObject)
        {
            return Utils.GetGameObjectBlueprint(GameObject?.Blueprint);
        }
        public static bool TryGetGameObjectBlueprint(this GameObject GameObject, out GameObjectBlueprint GameObjectBlueprint)
        {
            GameObjectBlueprint = GameObject.GetGameObjectBlueprint();
            return !GameObjectBlueprint.Is(null);
        }

        public static string DebugName(this BodyPart BodyPart)
        {
            return $"[{BodyPart.ID}:{BodyPart.Name}]::{BodyPart.Description}";
        }

        public static int Between(this int @int, int Min, int Max)
        {
            return Math.Min(Math.Max(@int, Min), Max);
        }
        public static double Between(this double @double, double Min, double Max)
        {
            return Math.Min(Math.Max(@double, Min), Max);
        }
        public static float Between(this float @float, float Min, float Max)
        {
            return Math.Min(Math.Max(@float, Min), Max);
        }

        public static int RapidAdvancementCeiling(this int @int, int MinAdvances = 0)
        {
            MinAdvances = MinAdvances > 0 ? (int)Math.Ceiling(MinAdvances / 3.0) : 0;
            return (int)Math.Max(MinAdvances, Math.Ceiling(@int / 3.0)) * 3;
        }

        public static int RapidAdvancementFloor(this int @int, int MinAdvances = 0)
        {
            MinAdvances = MinAdvances > 0 ? (int)Math.Ceiling(MinAdvances / 3.0) : 0;
            return (int)Math.Max(MinAdvances, Math.Floor(@int / 3.0)) * 3;
        }

        public static int RapidAdvancementRound(this int @int, int MinAdvances = 0)
        {
            MinAdvances = MinAdvances > 0 ? (int)Math.Ceiling(MinAdvances / 3.0) : 0;
            return (int)Math.Max(MinAdvances, Math.Round(@int / 3.0)) * 3;
        }

        public static string Are(this GameObject Object)
        {
            return Object.IsPlural ? "are" : "is";
        }
        public static string SplitCamelCase(this string @string)
        {
            return Regex.Replace(
                Regex.Replace(
                    @string,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static T Sample<T>(this Dictionary<T, int> WeightedList)
            where T : class
        {
            T Output = default;
            int weightMax = 0;
            foreach ((_, int entryWeight) in WeightedList)
            {
                weightMax += entryWeight;
            }
            int ticket = RndGP.Next(1, weightMax);
            int weightCurrent = 0;
            foreach ((T entryT, int entryWeight) in WeightedList)
            {
                weightCurrent += entryWeight;
                if (ticket <= weightCurrent)
                {
                    Output = entryT;
                    break;
                }
            }
            return Output;
        }
        public static T Draw<T>(this Dictionary<T, int> WeightedList)
            where T : class
        {
            T Output = WeightedList.Sample();
            if(--WeightedList[Output] == 0)
                WeightedList.Remove(Output);
            return Output;
        }
        public static void AddTicket<T>(this Dictionary<T, int> WeightedList, T Ticket)
            where T : class
        {
            if (WeightedList.ContainsKey(Ticket))
            {
                WeightedList[Ticket]++;
            }
            else
            {
                WeightedList.Add(Ticket, 1);
            }
        }

        public static bool TagIsIncludedOrNotExcluded(this GameObjectBlueprint Blueprint, string TagName, Dictionary<string, bool> IncludeExclude)
        {
            Debug.Entry(4, 
                $"* ({Blueprint.Name})."
                + $"{nameof(TagIsIncludedOrNotExcluded)}" 
                + $"(string TagName: {TagName}, Dictionary<string, bool> IncludeExclude)",
                Indent: 0, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));

            if (!IncludeExclude.IsNullOrEmpty())
            {

                Debug.CheckYeh(4, $"IncludeExclude Not Empty", Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));

                List<string> includeTags = new();
                List<string> excludeTags = new();

                Debug.Entry(4, $"IncludeExclude values:", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                foreach ((string entryValue, bool entryInclude) in IncludeExclude)
                {
                    if (entryInclude) includeTags.Add(entryValue);
                    else excludeTags.Add(entryValue);
                    Debug.LoopItem(4, $"{(entryInclude ? "include" : "exclude")}: {entryValue}", 
                        Indent: 2, Good: entryInclude, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                }

                bool noBlueprintTagValue = !Blueprint.TryGetTag(TagName, out string blueprintTagValue);
                bool noBlueprintPart = true;
                foreach ((string part,_) in Blueprint.Parts)
                {
                    if (includeTags.Contains(part.ToLower()))
                    {
                        noBlueprintPart = false;
                        break;
                    }
                }
                bool noBlueprintTagOrPart = noBlueprintTagValue && noBlueprintPart;
                if (!includeTags.IsNullOrEmpty() && noBlueprintTagOrPart)
                {
                    Debug.CheckNah(4, $"includeTags not empty and {Blueprint.Name} doesn't have \"{TagName}\" tag or equivalent Part", 
                        Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                    return false; 
                }
                else if (noBlueprintTagOrPart)
                {
                    Debug.CheckYeh(4, 
                        $"includeTags is empty and {Blueprint.Name} doesn't have \"{TagName}\" tag or equivalent Part, " + 
                        $"exclusions are irrelevant",
                        Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                    return true;
                }

                Debug.CheckYeh(4, $"{TagName}: \"{blueprintTagValue}\"", Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                List<string> blueprintTagValues = new();
                if (blueprintTagValue != null)
                {
                    if (blueprintTagValue.Contains(","))
                    {
                        Debug.Entry(4, $"blueprintTagValue contains \",\"", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                        string[] tagsArray = blueprintTagValue.Split(',');
                        foreach (string value in tagsArray)
                        {
                            Debug.LoopItem(4, $" \"{value}\" added to values list", Indent: 3, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                            blueprintTagValues.Add(value);
                        }
                    }
                    else
                    {
                        Debug.Entry(4, $"blueprintTagValue doesn't contain \",\"", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                        Debug.LoopItem(4, $" \"{blueprintTagValue}\" added to values list", Indent: 3, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                        blueprintTagValues.Add(blueprintTagValue);
                    }
                }
                foreach ((string part, _) in Blueprint.Parts)
                {
                    blueprintTagValues.Add(part.ToLower());
                }

                List<string> inclusionMatches = new();
                List<string> exclusionMatches = new();

                Debug.Entry(4, $"blueprintTagValues Matches:", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                foreach (string tagValue in blueprintTagValues)
                {
                    if (includeTags.Contains(tagValue))
                    {
                        Debug.CheckYeh(4, $"includeTags contains {tagValue}", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                        inclusionMatches.Add(tagValue);
                    }
                    if (excludeTags.Contains(tagValue))
                    {
                        Debug.CheckNah(4, $"excludeTags contains {tagValue}", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                        exclusionMatches.Add(tagValue);
                    }
                }

                Debug.Entry(4, $"inclusionMatches:", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                foreach (string entry in inclusionMatches)
                {
                    Debug.CheckYeh(4, $"{entry}", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                }
                if (inclusionMatches.IsNullOrEmpty())
                    Debug.CheckNah(4, $"Empty", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));

                Debug.Entry(4, $"exclusionMatches:", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                foreach (string entry in exclusionMatches)
                {
                    Debug.CheckNah(4, $"{entry}", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
                }
                if (exclusionMatches.IsNullOrEmpty())
                    Debug.CheckYeh(4, $"Empty", Indent: 2, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));

                if (!includeTags.IsNullOrEmpty() && inclusionMatches.IsNullOrEmpty())
                    return false;
                Debug.CheckYeh(4, $"includeTags was Empty, or there was at least one inclusionMatch", Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));

                if (!excludeTags.IsNullOrEmpty() && !exclusionMatches.IsNullOrEmpty())
                    return false;
                Debug.CheckYeh(4, $"excludeTags was Empty, or there were no exclusionMatchs", Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
            }
            Debug.CheckYeh(4, $"IncludeExclude was empty or filter allowed blueprint through", Indent: 1, Toggle: getDoDebug(nameof(TagIsIncludedOrNotExcluded)));
            return true;
        }

        public static void MakeIncludeExclude(this string valueString, Dictionary<string, bool> @return)
        {
            Debug.Entry(4,
                $"* {valueString}."
                + $"{nameof(MakeIncludeExclude)}"
                + $"(Dictionary<string, bool> @return)",
                Indent: 0, Toggle: getDoDebug(nameof(MakeIncludeExclude)));

            if (!valueString.IsNullOrEmpty())
            {
                Debug.Entry(4, $"value not empty or null", Indent: 1, Toggle: getDoDebug(nameof(MakeIncludeExclude)));
                if (valueString.Contains(","))
                {
                    Debug.Entry(4, $"valueString contains \",\"", Indent: 1, Toggle: getDoDebug(nameof(MakeIncludeExclude)));
                    string[] classesArray = valueString.Split(',');
                    foreach (string entry in classesArray)
                    {
                        bool isNot = entry.StartsWith("!");
                        string value = isNot ? entry.Substring(1) : entry;
                        Debug.LoopItem(4, $"{value}: {(!isNot ? "include" : "exclude")}", Indent: 2, Good: !isNot, Toggle: getDoDebug(nameof(MakeIncludeExclude)));
                        @return.Add(value.ToLower(), !isNot);
                    }
                }
                else
                {
                    Debug.Entry(4, $"valueString doesn't contain \",\"", Indent: 1, Toggle: getDoDebug(nameof(MakeIncludeExclude)));

                    bool isNot = valueString.StartsWith("!");
                    string value = isNot ? valueString.Substring(1) : valueString;
                    Debug.LoopItem(4, $"{value}: {(!isNot ? "include" : "exclude")}", Indent: 2, Good: !isNot, Toggle: getDoDebug(nameof(MakeIncludeExclude)));
                    @return.Add(value.ToLower(), !isNot);
                }
            }
        }
        public static string GetOwner(this GameObjectBlueprint Blueprint)
        {
            return Blueprint?.GetPartParameter<string>("Physics", "Owner");
        }
        public static bool HasOwner(this GameObjectBlueprint Blueprint)
        {
            return !Blueprint.GetOwner().IsNullOrEmpty();
        }

        public static string Quote(this string @string)
        {
            return Utils.Quote($"{@string}");
        }

        public static Dictionary<string,List<Cell>> GetHutRegion(this Zone Z, Rect2D R, bool Round = false)
        {
            string Inner = "Inner";
            string Outer = "Outer";
            string Corners = "Corners";
            string NorthEdge = "NorthEdge";
            string SouthEdge = "SouthEdge";
            string EastEdge = "EastEdge";
            string WestEdge = "WestEdge";
            string Door = "Door";
            Dictionary<string, List<Cell>> Region = new()
            {
                { Inner, new() },
                { Outer, new() },
                { Corners, new() },
                { NorthEdge, new() },
                { SouthEdge, new() },
                { EastEdge, new() },
                { WestEdge, new() },
                { Door, new() },
            };
            Rect2D r = R.ReduceBy(1, 1);
            Cell cell;
            for (int i = r.y1; i <= r.y2; i++)
            {
                for (int j = r.x1; j <= r.x2; j++)
                {
                    if ((cell = Z.GetCell(j, i)) == null) continue;
                    Region[Inner].Add(cell);
                }
            }
            if (Round)
            {
                for (int k = R.x1 + 1; k <= R.x2 - 1; k++)
                {
                    if ((cell = Z.GetCell(k, R.y1)) != null)
                        Region[Outer].Add(cell);
                    if ((cell = Z.GetCell(k, R.y2)) != null)
                        Region[Outer].Add(cell);
                }
                for (int l = R.y1 + 1; l <= R.y2 - 1; l++)
                {
                    if ((cell = Z.GetCell(R.x1, l)) != null)
                        Region[Outer].Add(cell);
                    if ((cell = Z.GetCell(R.x2, l)) != null)
                        Region[Outer].Add(cell);
                }

                if ((cell = Z.GetCell(R.x1 + 1, R.y1 + 1)) != null)
                    Region[Outer].Add(cell);
                if ((cell = Z.GetCell(R.x2 - 1, R.y1 + 1)) != null)
                    Region[Outer].Add(cell);
                if ((cell = Z.GetCell(R.x1 + 1, R.y2 - 1)) != null)
                    Region[Outer].Add(cell);
                if ((cell = Z.GetCell(R.x2 - 1, R.y2 - 1)) != null)
                    Region[Outer].Add(cell);
            }
            else
            {
                for (int m = R.x1; m <= R.x2; m++)
                {
                    if ((cell = Z.GetCell(m, R.y1)) != null)
                        Region[Outer].Add(cell);
                    if ((cell = Z.GetCell(m, R.y2)) != null)
                        Region[Outer].Add(cell);
                }
                for (int n = R.y1; n <= R.y2; n++)
                {
                    if ((cell = Z.GetCell(R.x1, n)) != null)
                        Region[Outer].Add(cell);
                    if ((cell = Z.GetCell(R.x2, n)) != null)
                        Region[Outer].Add(cell);
                }
            }
            foreach (Cell outerCell in Region[Outer])
            {
                if (outerCell.Y == R.y1) Region[NorthEdge].Add(outerCell);
                if (outerCell.Y == R.y2) Region[SouthEdge].Add(outerCell);
                if (outerCell.X == R.x2) Region[EastEdge].Add(outerCell);
                if (outerCell.X == R.x1) Region[WestEdge].Add(outerCell);
                if ((outerCell.X == R.x1 || outerCell.X == R.x2) && (outerCell.Y == R.y1 || outerCell.Y == R.y2))
                    Region[Corners].Add(outerCell);
            }
            if (R.Door != null)
            {
                Cell door = Z.GetCell(R.Door);
                string doorSide = R.GetCellSide(R.Door) switch
                {
                    "N" => NorthEdge,
                    "S" => SouthEdge,
                    "E" => EastEdge,
                    "W" => WestEdge,
                    _ => null,
                };
                if (doorSide != null)
                {
                    Cell newDoor = Region[doorSide].GetRandomElement();
                    
                    Dictionary<string,List<Cell>> Edges = new()
                    {
                        { NorthEdge, Region[NorthEdge] },
                        { SouthEdge, Region[SouthEdge] },
                        { EastEdge, Region[EastEdge] },
                        { WestEdge, Region[WestEdge] },
                    };
                    Point2D newDoor2D = newDoor.PullInsideFromEdges(Edges, doorSide);
                    R.Door.x = newDoor2D.x;
                    R.Door.y = newDoor2D.y;
                }
                door = Z.GetCell(R.Door);
                Region[Door].Add(door);
            }
            return Region;
        }

        public static List<Cell> GetOrdinalAdjacentCells(this Cell @this, bool bLocalOnly = false, bool BuiltOnly = true, bool IncludeThis = false)
        {
            List<Cell> exclusionList = new(Cell.DirectionListCardinalOnly.Length);
            string[] directionListCardinalOnly = Cell.DirectionListCardinalOnly;
            foreach (string direction in directionListCardinalOnly)
            {
                Cell cell = (bLocalOnly ? @this.GetLocalCellFromDirection(direction) : @this.GetCellFromDirection(direction));
                if (cell != null)
                {
                    exclusionList.Add(cell);
                }
            }

            List<Cell> cellsList = (bLocalOnly ? @this.GetLocalAdjacentCells() : @this.GetAdjacentCells());
            foreach (Cell excludeCell in exclusionList)
            {
                if (cellsList.Contains(excludeCell)) cellsList.Remove(excludeCell);
            }

            if (IncludeThis)
            {
                cellsList.Add(@this);
            }

            return cellsList;
        }

        public static Point2D PullInsideFromEdges(this Cell Cell, Dictionary<string,List<Cell>> Edges, string DoorSide = "")
        {
            Debug.Entry(4,
                $"@ {typeof(Extensions).Name}."
                + $"{nameof(PullInsideFromEdges)}"
                + $"(Cell: {Cell}, List<Cell> Edge, " 
                + $"string DoorSide: {(DoorSide.IsNullOrEmpty() ? $"".Quote() : DoorSide.Quote())})",
                Indent: 0, Toggle: getDoDebug(nameof(PullInsideFromEdges)));

            Point2D output = new(Cell.X, Cell.Y);
            foreach ((string Side, List<Cell> Edge) in Edges)
            {
                Debug.Entry(4, $"Side: {Side}", Indent: 1, Toggle: getDoDebug(nameof(PullInsideFromEdges)));
                if (DoorSide == "" || Side == DoorSide)
                    output = Cell.PullInsideFromEdge(Edge);
            }
            Debug.Entry(4,
                $"x {typeof(Extensions).Name}."
                + $"{nameof(PullInsideFromEdges)}"
                + $"(Cell: [{Cell}], List<Cell> Edge, "
                + $"string DoorSide: {(DoorSide.IsNullOrEmpty() ? $"".Quote() : DoorSide.Quote())}) @//",
                Indent: 0, Toggle: getDoDebug(nameof(PullInsideFromEdges)));
            return output;
        }

        public static Point2D PullInsideFromEdge(this Cell Cell, List<Cell> Edge)
        {
            Debug.Entry(4,
                $"* {typeof(Extensions).Name}."
                + $"{nameof(PullInsideFromEdge)}"
                + $"(Cell: {Cell}, List<Cell> Edge)",
                Indent: 1, Toggle: getDoDebug(nameof(PullInsideFromEdge)));

            Point2D output = new(Cell.X, Cell.Y);
            if (Edge != null && Edge.Contains(Cell))
            {
                List<int> Xs = new();
                List<int> Ys = new();
                string XsString = string.Empty;
                string YsString = string.Empty;
                foreach (Cell cell in Edge)
                {
                    if (!Xs.Contains(cell.X)) Xs.Add(cell.X);
                    if (!Ys.Contains(cell.Y)) Ys.Add(cell.Y);
                }
                foreach (int X in Xs)
                {
                    XsString += XsString == "" ? $"{X}" : $",{X}";
                }
                foreach (int Y in Ys)
                {
                    YsString += YsString == "" ? $"{Y}" : $",{Y}";
                }
                Debug.Entry(4, $"Xs: {XsString}", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                Debug.Entry(4, $"Ys: {YsString}", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                if (Xs.Count > 1 &&  Ys.Count > 1)
                {
                    Debug.Entry(2, 
                        $"WARN [GigantismPlus]: " + 
                        $"{typeof(Extensions).Name}." + 
                        $"{nameof(PullInsideFromEdge)}() " + 
                        $"List<Cell> Edge must be a straight line.", 
                        Indent: 0, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                    return output;
                }
                bool edgeIsLat = Xs.Count > 1;
                Debug.Entry(4, $"edgeIsLat: {edgeIsLat}", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                int max = int.MinValue;
                int min = int.MaxValue;
                if (edgeIsLat)
                {
                    foreach(int x in Xs)
                    {
                        max = Math.Max(max, x);
                        min = Math.Min(min, x);
                    }
                    if (output.x <= min)
                    {
                        Debug.Entry(4, $"output.x ({output.x}) >= min ({min}): output.x = min + 1", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                        output.x = min + 1;
                    }
                    if (output.x >= max)
                    {
                        Debug.Entry(4, $"output.x ({output.x}) <= max ({max}): output.x = max - 1", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                        output.x = max - 1;
                    }
                }
                else
                {
                    foreach (int y in Ys)
                    {
                        max = Math.Max(max, y);
                        min = Math.Min(min, y);
                    }
                    if (output.y <= min)
                    {
                        Debug.Entry(4, $"output.y ({output.y}) >= min ({min}): output.y = min + 1", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                        output.y = min + 1;
                    }
                    if (output.y >= max)
                    {
                        Debug.Entry(4, $"output.y ({output.y}) <= max ({max}): output.y = max - 1", Indent: 2, Toggle: getDoDebug(nameof(PullInsideFromEdge)));
                        output.y = max - 1;
                    }
                }
            }
            Debug.Entry(4,
                $"x {typeof(Extensions).Name}."
                + $"{nameof(PullInsideFromEdge)}"
                + $"(Cell: {Cell}, List<Cell> Edge) *//",
                Indent: 1, Toggle: getDoDebug(nameof(PullInsideFromEdge)));

            return output;
        }

        public static List<string> GetNumberedTileVariants(this string Source)
        {
            Debug.Entry(4,
                $"* {typeof(Extensions).Name}."
                + $"{nameof(GetNumberedTileVariants)}"
                + $"(string Source: {Source})",
                Indent: 0, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));
            List<string> output = new();
            
            string[] sourcePieces = Source.Split("~");
            string pathBefore = sourcePieces[0];
            string pathAfter = sourcePieces[2];

            Debug.Entry(4, $"pathBefore: {pathBefore}, pathAfter: {pathAfter}", Indent: 1, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));
            Debug.Entry(4, $"sourcePieces[1] {sourcePieces[1]}", Indent: 1, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));
            
            string[] pathRange = sourcePieces[1].Split('-');
            int first = int.Parse(pathRange[0]);
            int last = int.Parse(pathRange[1]);

            Debug.Entry(4, $"first {first} - last: {last}", Indent: 1, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));

            for (int i = first; i <= last; i++)
            {
                Debug.Entry(4, $"i: {i}", Indent: 2, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));

                int padding = Math.Max(2, last.ToString().Length);
                string number = $"{i}".PadLeft(padding, '0');
                string path = $"{pathBefore}{number}{pathAfter}";
                Debug.Entry(4, $"path: {path}", Indent: 2, Toggle: getDoDebug(nameof(GetNumberedTileVariants)));
                if (!output.Contains(path)) output.Add(path);
            }

            return output;
        }

        public static List<string> CommaExpansion(this string String)
        {
            string[] stringPieces = String.Split(",");
            List<string> output = new();
            for (int i = 0; i < stringPieces.Count(); i++)
            {
                if (!output.Contains(stringPieces[i])) output.Add(stringPieces[i]);
            }
            return output;
        }

        public static bool SeededRandomBool(this Guid Seed, int ChanceIn = 2)
        {
            int High = ChanceIn * 7;
            return Stat.SeededRandom(Seed.ToString(), 0, High) % ChanceIn == 0;
        }

        public static string Join<T>(this List<T> List, string Delimiter = ",")
            where T : IConvertible
        {
            string output = string.Empty;
            if (!List.IsNullOrEmpty())
            {
                foreach (T item in List)
                {
                    output += $"{(output == string.Empty ? "" : Delimiter)}{item}";
                }
            }
            return output;
        }

        public static bool TrySplit(this string String, string Delimiter, out string[] split)
        {
            if (!String.IsNullOrEmpty() && String.Contains(Delimiter))
            {
                split = String.Split(Delimiter);
                return true;
            }
            split = null;
            return false;
        }

        public static bool TrySplitToList(this string String, string Delimiter, out List<string> List)
        {
            List<string> strings = new();
            if (!String.IsNullOrEmpty() && String.TrySplit(Delimiter, out string[] split))
            {
                foreach (string item in split)
                {
                    if (!strings.Contains(item)) strings.Add(item);
                }
                List = strings;
                return true;
            }
            List = new();
            return false;
        }

        public static bool ContainsCI(this string String, string Value)
        {
            return String.ToLower().Contains(Value.ToLower());
        }

        public static bool IsGivingStewful(this string NamePiece)
        {
            return NamePiece.ContainsCI("Stew")
                || NamePiece.ContainsCI("Cook");
        }
        public static bool IsGivingStewless(this string NamePiece)
        {
            return NamePiece.ContainsCI("Hanker")
                || NamePiece.ContainsCI("Gains Seeker");
        }
        public static bool IsGivingThoughtful(this string NamePiece)
        {
            return NamePiece.ContainsCI("Thought")
                || NamePiece.ContainsCI("Think")
                || NamePiece.ContainsCI("Book")
                || NamePiece.ContainsCI("Listen")
                || NamePiece.ContainsCI("Philosoph");
        }
        public static bool IsGivingTough(this string NamePiece)
        {
            return NamePiece.ContainsCI("Stone")
                || NamePiece.ContainsCI("Pillar")
                || NamePiece.ContainsCI("Solid")
                || NamePiece.ContainsCI("Sturdy")
                || NamePiece.ContainsCI("Mov") // Immovable, Unmoving
                || NamePiece.ContainsCI("Stop")
                || NamePiece.ContainsCI("Mountain")
                || NamePiece.ContainsCI("Thic") // Thick, Thicc
                || NamePiece.ContainsCI("Hefty");
        }
        public static bool IsGivingStrong(this string NamePiece)
        {
            return NamePiece.ContainsCI("Gain")
                || NamePiece.ContainsCI("Swole")
                || NamePiece.ContainsCI("Mighty")
                || NamePiece.ContainsCI("Mountain")
                || NamePiece.ContainsCI("Slap");
        }
        public static bool IsGivingResilient(this string NamePiece)
        {
            return NamePiece.ContainsCI("Waits")
                || NamePiece.ContainsCI("Sits")
                || NamePiece.ContainsCI("Silent")
                || NamePiece.ContainsCI("Still")
                || NamePiece.ContainsCI("Tall")
                || NamePiece.ContainsCI("Mountain")
                || NamePiece.ContainsCI("Keeps Going");
        }
        public static bool IsGivingPopular(this string NamePiece)
        {
            return NamePiece.ContainsCI("Altruis");
        }
        public static bool IsGivingWrassler(this string NamePiece)
        {
            return NamePiece.ContainsCI("Folding Chair");
        }
        public static bool IsGivingTrulyImmense(this string NamePiece)
        {
            return NamePiece.ContainsCI("Belly")
                || NamePiece.ContainsCI("Heft")
                || NamePiece.ContainsCI("Considerable Size")
                || NamePiece.ContainsCI("Bellows")
                || NamePiece.ContainsCI("Rumbles")
                || NamePiece.ContainsCI("Rotund")
                || NamePiece.ContainsCI("Giant")
                || NamePiece.ContainsCI("Immense")
                || NamePiece.ContainsCI("Enormous")
                || NamePiece.ContainsCI("Huge")
                || NamePiece.ContainsCI("Yuge")
                || NamePiece.ContainsCI("Somewhat Big")
                || NamePiece.ContainsCI("Really Big");
        }

        public static List<BaseSkill> AddSkills(this GameObject Creature, List<string> Skills)
        {
            List<BaseSkill> output = new();
            foreach (var skill in Skills)
            {
                if (Creature.HasSkill(skill)) continue;
                BaseSkill outputSkill = Creature.AddSkill(skill);
                if (!output.Contains(outputSkill)) output.Add(outputSkill);
            }
            return output;
        }

        public static bool TryAdd<T>(this List<T> List, T Item)
            where T : class
        {
            if (!List.Contains(Item))
            {
                List.Add(Item);
                return true;
            }
            return false;
        }

        public static bool TryGetBook(this string BookName, out BookInfo Book)
        {
            return BookUI.Books.TryGetValue(BookName, out Book);
        }
        public static bool TryGetListBookPages(this string BookName, out List<string> Pages)
        {
            List<string> pages = new();
            if (BookName.TryGetBook(out BookInfo Book) && !Book.Pages.IsNullOrEmpty())
            {
                foreach (BookPage page in Book.Pages)
                {
                    pages.TryAdd(page.FullText.Trim());
                }
            }
            Pages = pages;
            return !Pages.IsNullOrEmpty();
        }
        public static List<string> BookPagesAsList(this string BookName)
        {
            BookName.TryGetListBookPages(out List<string> pages);
            return pages;
        }

        public static List<Location2D> ToLocation2DList(this List<Cell> CellList)
        {
            if (CellList.IsNullOrEmpty()) return null;
            List<Location2D> Location2DList = new();
            foreach (Cell cell in CellList)
            {
                Location2DList.TryAdd(cell.Location);
            }
            return Location2DList;
        }
        public static List<string> ToStringList(this List<Location2D> Location2DList)
        {
            if (Location2DList.IsNullOrEmpty()) return null;
            List<string> stringList = new();
            foreach (Location2D location in Location2DList)
            {
                stringList.TryAdd(location.ToString());
            }
            return stringList;
        }
        public static List<string> ToStringList(this List<Cell> CellList)
        {
            return CellList.ToLocation2DList().ToStringList();
        }

        public static List<Cell> ToCellList(this List<string> StringList, Zone Z)
        {
            List<Cell> cellList = new();
            foreach (string @string in StringList)
            {
                string[] coord = @string.Split(",");
                cellList.TryAdd(Z.GetCell(int.Parse(coord[0]), int.Parse(coord[1])));
            }
            return cellList;
        }
        public static List<string> ToStringCoordList(this string String)
        {
            List<string> stringList = new();
            if (!String.Contains(";"))
            {
                stringList.TryAdd(String);
            }
            else
            {
                string[] coords = String.Split(";");
                foreach (string coord in coords)
                {
                    stringList.TryAdd(coord);
                }
            }
            return stringList;
        }

        public static bool OverlapsWith<T>(this List<T> List, List<T> TestList)
        {
            bool overlaps = false;
            if (!List.IsNullOrEmpty() && !TestList.IsNullOrEmpty())
            {
                foreach (T entry in List)
                {
                    if (TestList.Contains(entry))
                    {
                        overlaps = true;
                        break;
                    }
                }
            }
            return overlaps;
        }

        public static Cell GetCellOppositePivotCell(this Cell Origin, Cell Pivot)
        {
            if (Origin == null || Origin == Pivot || !Origin.GetAdjacentCells().Contains(Pivot)) return null;
            return Pivot.GetCellFromDirection(Origin.GetDirectionFromCell(Pivot));
        }

        public static string YehNah(this bool Condition, bool Flip = false)
        {
            // Input |  Flip
            // true  != false   == true
            // true  != true    == false
            // false != false   == false
            // false != true    == true
            return Condition != Flip ? TICK.Color("G") : CROSS.Color("R");
        }
        public static string ThisManyTimes(this string @string, int Times = 1)
        {
            string output = @string;

            for (int i = 0; i < Times; i++)
            {
                output += @string;
            }

            return output;
        }

        public static bool HasDiggableVaultableObject(this Cell cell)
        {
            foreach (GameObject vaultable in cell.GetObjectsWithPart(nameof(Vaultable)))
            {
                if (vaultable.HasStringProperty("Diggable") || vaultable.HasStringProperty("WasDiggable"))
                {
                    return true;
                }
            }
            return false;
        }

    } //!-- Extensions
}
