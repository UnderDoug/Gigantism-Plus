using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
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
            string creatureBlueprint = Creature.Blueprint;

            bool creatureIsMerchant = Creature.HasPart<GenericInventoryRestocker>();
            (DieRoll die, int high) merchantBaseChance = (new("1d5"), 4);
            (DieRoll die, int high) merchantGrenades = (new("1d2"), 2);
            (DieRoll die, int high) merchantTradeGoods = (new("1d4"), 4);
            (DieRoll die, int high) merchantTonics = (new("1d4"), 4);
            (DieRoll die, int high) merchantRareTonics = (new("1d10"), 10);

            if (!Option) goto Exit; // skip if Option disabled
            if (!Creature.IsCreature) goto Exit; // skip non-creatures
            if (Creature.Inventory == null) goto Exit; // skip objects without inventoryalready been gigantified.

            Debug.Entry(3, $"* GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption})", Indent: 1);
            Debug.Divider(3, Indent: 1);

            Debug.Entry(3, "Making inventory items gigantic for creature", creatureBlueprint, Indent: 1);
            Debug.Entry(3, $"Creature is merchant", creatureIsMerchant ? "Yeh" : "Nah", Indent: 1);

            // Create a copy of the items list to avoid modifying during enumeration
            List<GameObject> itemsToProcess = new(Creature.GetInventoryAndEquipment());

            Debug.Entry(3, "> foreach (GameObject item in itemsToProcess)", Indent: 1);
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
                    Debug.CheckYeh(4, "eligible to be made ModGigantic", Indent: 3);
                    // Is the item already gigantic? Don't attempt to apply it again.
                    if (item.HasPart<ModGigantic>())
                    {
                        Debug.CheckNah(4, "already gigantic", "NoThanks++; x/", Indent: 3);
                        NoThanks++;
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not already gigantic", Indent: 3);
                    }

                    // Is the item a grenade, and is the option not set to include them?
                    if (item.HasTag("Grenade"))
                    {
                        if (!GrenadeOption || creatureIsMerchant)
                        {
                            if (!GrenadeOption)
                            { 
                                Debug.CheckNah(4, "grenade (excluded)", "NoThanks++; x/", Indent: 3); 
                                NoThanks++; 
                            }
                            if (creatureIsMerchant) 
                            { 
                                Debug.CheckNah(4, "grenade (isMerchant)", "NoThanks++; x/", Indent: 3); 
                                NoThanks++; 
                            }

                            if (creatureIsMerchant && merchantGrenades.die.Resolve() >= merchantGrenades.high)
                            {
                                Debug.LoopItem(4,
                                    $"but!] merchantGrenades {merchantGrenades.die} rolled at or above {merchantGrenades.high}",
                                    $"NoThanks--;",
                                    Indent: 4);
                                NoThanks--;
                            }
                        }
                        else if (GrenadeOption)
                        {
                            Debug.CheckYeh(4, "grenade (included)", Indent: 3);
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not grenade", Indent: 3);
                    }

                    // Is the item a trade good? We don't want gigantic copper nuggets making the start too easy
                    if (item.HasTag("DynamicObjectsTable:TradeGoods"))
                    {
                        Debug.CheckNah(4, "TradeGoods", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantTradeGoods.die.Resolve() >= merchantTradeGoods.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantTradeGoods {merchantTradeGoods.die} rolled at or above {merchantTradeGoods.high}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not TradeGoods", Indent: 3);
                    }

                    // Is the item a non-rare tonic? Double doses are basically useless in the early game
                    if (item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.CheckNah(4, "Tonics_NonRare", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantTonics.die.Resolve() >= merchantTonics.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantTonics {merchantTonics.die} rolled at or above {merchantTonics.high}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not Tonics_NonRare", Indent: 3);
                    }

                    // Is the item a rare tonic? Double doses are basically useless in the early game
                    if (item.HasPart<Tonic>() && !item.HasTag("DynamicObjectsTable:Tonics_NonRare"))
                    {
                        Debug.CheckNah(4, "Rare Tonic", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (creatureIsMerchant && merchantRareTonics.die.Resolve() >= merchantRareTonics.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantRareTonics {merchantRareTonics.die} rolled at or above {merchantRareTonics.high}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4, "not Rare Tonics", Indent: 3);
                    }

                    // Is the item held by a merchant, and did their roll fail?
                    if (creatureIsMerchant)
                    {
                        Debug.CheckNah(4, "creatureIsMerchant is True", "NoThanks++; x/", Indent: 3);
                        NoThanks++;

                        if (merchantBaseChance.die.Resolve() >= merchantBaseChance.high)
                        {
                            Debug.LoopItem(4,
                                $"but!] merchantBaseChance {merchantBaseChance.die} rolled at or above {merchantBaseChance.high}",
                                $"NoThanks--;",
                                Indent: 4);
                            NoThanks--;
                        }
                        else 
                        {
                            Debug.LoopItem(4, 
                                $"and!] merchantBaseChance {merchantBaseChance.die} rolled below {merchantBaseChance.high}",
                                "Bummer!",
                                Indent: 4);
                        }
                    }
                    else
                    {
                        Debug.CheckYeh(4,
                            $"creatureIsMerchant is False",
                            Indent: 3);
                    }

                    Debug.Entry(3, $"NoThanks", $"{NoThanks}", Indent: 2);
                    if (NoThanks > 0) goto Skip;

                    Debug.Entry(3, $"Gigantifying {ItemName}", Indent: 2);
                    item.ApplyModification("ModGigantic");
                    if (!item.HasPart<ModGigantic>()) Debug.Entry(3, ItemName, "<!!> Gigantification Failed", Indent: 2);
                    else Debug.Entry(3, ItemName, "has been Gigantified", Indent: 2);

                    Debug.DiveOut(3, $"{ItemDebug}", Indent: 1);
                    continue;
                }
                else
                {
                    Debug.CheckNah(4, "ineligible to be made ModGigantic x/", Indent: 3);
                }
            Skip:
                Debug.Entry(3, "/x Skipping", Indent: 2);
                Debug.DiveOut(3, $"{ItemDebug}", Indent: 1);
            }
            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "x foreach (GameObject item in itemsToProcess) >//", Indent: 1);

            // Now equip all items that should be equipped

            Debug.Entry(3, "Creature.WantToReequip()", Indent: 1);
            Creature.WantToReequip();

            Debug.Divider(3, Indent: 1);
            Debug.Entry(3, $"x GigantifyInventory(Option: {Option}, GrenadeOption: {GrenadeOption}) *//", Indent: 1);
            
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
        public static void SetEquipmentFrameColors(this GameObject Object, string TopLeft_Left_Right_BottomRight = null)
        {
            Object.SetStringProperty("EquipmentFrameColors", TopLeft_Left_Right_BottomRight, true);
        }

        public static void CheckAffectedEquipmentSlots(this GameObject Actor)
        {
            Debug.Entry(3, $"* {nameof(CheckAffectedEquipmentSlots)}(this GameObject Actor: {Actor.DebugName})");
            Body Body = Actor?.Body;
            if (Body != null)
            {
                List<GameObject> list = Event.NewGameObjectList();
                Debug.Entry(3, "> foreach (BodyPart bodyPart in Body.LoopParts())");
                foreach (BodyPart bodyPart in Body.LoopParts())
                {
                    Debug.Entry(3, "bodyPart", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Name}]", Indent: 1);
                    GameObject equipped = bodyPart.Equipped;
                    if (equipped != null && !list.Contains(equipped))
                    {
                        Debug.LoopItem(3, "equipped", $"[{equipped.ID}:{equipped.ShortDisplayName}]", Indent: 2);
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
                Debug.Entry(3, "x foreach (BodyPart bodyPart in Body.LoopParts()) >//");
            }
            else
            {
                Debug.Entry(4, $"no body on which to perform check, aborting ");
            }
            Debug.Entry(3, $"x {nameof(CheckAffectedEquipmentSlots)}(this GameObject Actor: {Actor.DebugName}) *//");
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
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            IModification ModPart = ModPartName.ConvertToModification();
            return (ModNaturalEquipment<T>)ModPart;
        }

        public static T GetManagedNaturalEquipmentCompatiblePart<T>(this GameObject Object) 
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
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
            return Object.GetBlueprint().InheritsFrom(Blueprint);
        }

        // https://stackoverflow.com/a/32184652
        public static bool SetPropertyValue(this object @object, string PropertyName, object Value)
        {
            PropertyInfo property = @object?.GetType()?.GetProperty(PropertyName);
            if (property == null) return false;
            Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            object safeValue = (Value == null) ? null : Convert.ChangeType(Value, type);

            property.SetValue(@object, safeValue, null);
            return property?.GetValue(@object, null) != null;
        }
        // https://stackoverflow.com/a/1965659
        public static bool SetFieldValue(this object @object, string FieldName, object Value)
        {
            FieldInfo field = @object?.GetType()?.GetField(FieldName);
            if (field == null) return false;
            Type type = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
            object safeValue = (Value == null) ? null : Convert.ChangeType(Value, type);

            field.SetValue(@object, safeValue);
            return field?.GetValue(@object) != null;
        }

        public static string NearDemonstrative(this GameObject Object)
        {
            return Object.IsPlural ? "these" : "this";
        }
        public static string FarDemonstrative(this GameObject Object)
        {
            return Object.IsPlural ? "those" : "that";
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
            if (!Object.Understood())
            {
                return "artifact";
            }
            if (Object.IsCreature)
            {
                return "creature";
            }
            if (Object.HasPart<CyberneticsBaseItem>())
            {
                return "implant";
            }
            if (Object.InheritsFrom("Tonic"))
            {
                return "tonic";
            }
            if (Object.HasPart<Medication>())
            {
                return "medication";
            }
            if (Object.InheritsFrom("Energy Cell"))
            {
                return "energy cell";
            }
            if (Object.InheritsFrom("LightSource"))
            {
                return "light source";
            }
            if (Object.InheritsFrom("Tool"))
            {
                return "tool";
            }
            if (Object.InheritsFrom("BaseRecoiler"))
            {
                return "recoiler";
            }
            if (Object.InheritsFrom("BaseNugget"))
            {
                return "nugget";
            }
            if (Object.InheritsFrom("Gemstone"))
            {
                return "gemstone";
            }
            if (Object.InheritsFrom("Random Figurine"))
            {
                return "figurine";
            }
            if (Object.HasPart<Applicator>())
            {
                return "applicator";
            }
            if (Object.HasPart<Chair>())
            {
                return "chair";
            }
            if (Object.TryGetPart(out MissileWeapon missileWeapon))
            {
                if (missileWeapon.Skill.Contains("Shotgun") 
                    || Object.InheritsFrom("BaseShotgun") 
                    || (Object.TryGetPart(out MagazineAmmoLoader loader) && loader.AmmoPart == "AmmoShotgunShell"))
                {
                    return "shotgun";
                }
                if (Object.InheritsFrom("BaseHeavyWeapon"))
                {
                    return "heavy weapon";
                }
                if (Object.InheritsFrom("BaseBow"))
                {
                    return "bow";
                }
                if (Object.InheritsFrom("BaseRifle"))
                {
                    return "rifle";
                }
                if (Object.InheritsFrom("BasePistol"))
                {
                    return "pistol";
                }
                return "missile weapon";
            }
            if (Object.TryGetPart(out ThrownWeapon thrownWeapon) && !thrownWeapon.IsImprovised())
            {
                if (Object.InheritsFrom("BaseBoulder"))
                {
                    return "boulder";
                }
                if (Object.InheritsFrom("BaseStone"))
                {
                    return "stone";
                }
                if (Object.InheritsFrom("Grenade"))
                {
                    return "grenade";
                }
                if (Object.InheritsFrom("BaseDagger"))
                {
                    return "dagger";
                }
                return "thrown weapon";
            }
            if (Object.TryGetPart(out MeleeWeapon meleeWeapon) && !meleeWeapon.IsImprovised())
            {
                if (Object.HasPart<NaturalEquipmentManager>())
                {
                    return Object.Render.DisplayName;
                }
                if (Object.InheritsFrom("BaseCudgel"))
                {
                    return "cudgel";
                }
                if (Object.InheritsFrom("BaseAxe"))
                {
                    return "axe";
                }
                if (Object.InheritsFrom("BaseLongBlade"))
                {
                    return "long blade";
                }
                if (Object.InheritsFrom("BaseDagger"))
                {
                    return "short blade";
                }
                return "weapon";
            }
            if (Object.TryGetPart(out Armor armor))
            {
                if (!Object.IsPluralIfKnown)
                {
                    if (armor.WornOn == "Back")
                    {
                        if (Object.Blueprint == "Gas Tumbler")
                        {
                            return "tumbler";
                        }
                        if (armor.CarryBonus > 0 || Object.Blueprint == "Gyrocopter Backpack" || Object.Blueprint == "SkybearJetpack")
                        {
                            return "pack";
                        }
                        return "cloak";
                    }
                    if (armor.WornOn == "Head")
                    {
                        return "helmet";
                    }
                    if (armor.WornOn == "Face")
                    {
                        if (Object.InheritsFrom("BaseMask"))
                        {
                            return "mask";
                        }
                    }
                    if (armor.WornOn == "Body")
                    {
                        if (Object.Blueprint.Contains("Plate"))
                        {
                            return "plate";
                        }
                        if (armor.AV > 2)
                        {
                            return "suit";
                        }
                        return "vest";
                    }
                    if (armor.WornOn == "Arm")
                    {
                        if (Object.InheritsFrom("BaseUtilityBracelet"))
                        {
                            return "utility bracelet";
                        }
                        if (Object.InheritsFrom("BaseBracelet"))
                        {
                            return "bracelet";
                        }
                        if (Object.InheritsFrom("BaseArmlet"))
                        {
                            return "armlet";
                        }
                    }
                    return "armor";
                }
                if (armor.WornOn == "Back")
                {
                    if (Object.Blueprint == "Mechanical Wings")
                    {
                        return "wings";
                    }
                }
                if (armor.WornOn == "Face")
                {
                    if (Object.HasPart<Spectacles>())
                    {
                        return "spectacle";
                    }
                    if (Object.InheritsFrom("BaseEyewear"))
                    {
                        return "goggle";
                    }
                    if (Object.InheritsFrom("BaseFaceJewelry"))
                    {
                        return "jewelry";
                    }
                    if (Object.Blueprint == "VISAGE")
                    {
                        return "scanner";
                    }
                }
                if (armor.WornOn == "Hands")
                {
                    if (Object.HasPart<Metal>())
                    {
                        return "gauntlet";
                    }
                    return "glove";
                }
                if (armor.WornOn == "Feet")
                {
                    if (Object.InheritsFrom("BaseBoot"))
                    {
                        if (Object.HasPart<Metal>())
                        {
                            return "sabaton";
                        }
                        return "boot";
                    }
                    return "shoe";
                }
            }
            if (Object.HasPart<LiquidVolume>())
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
                    if (Object.HasPart<Shield>() && !Object.IsPluralIfKnown)
                    {
                        return "shield";
                    }
                    if (!Object.Takeable)
                    {
                        return "object";
                    }
                    return "item";
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
             && (MeleeWeapon.BaseDamage == "1d2" || MeleeWeapon.BaseDamage == "1d2+3") 
             && MeleeWeapon.Ego == 0 
             && MeleeWeapon.Skill == "Cudgel" 
             && MeleeWeapon.Stat == "Strength" 
             && MeleeWeapon.Slot == "Hand" 
             && MeleeWeapon.Attributes.IsNullOrEmpty();

            bool hasImprovisedProp = 
                MeleeWeapon.ParentObject.HasTagOrStringProperty("IsImprovisedMelee") 
             && MeleeWeapon.ParentObject.GetStringProperty("IsImprovisedMelee") != "false";

            MeleeWeapon @default = new();

            return MeleeWeapon.SameAs(@default) 
                || MeleeWeapon.IsImprovisedWeapon() 
                || isImprovisedButGigantic 
                || MeleeWeapon.ParentObject.GetIntProperty("IsImprovisedMelee") > 0
                || hasImprovisedProp;
        }
    }
}
