using System;
using System.Collections.Generic;
using System.Linq;

using ConsoleLib.Console;

using Kobold;

using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.Language;
using XRL.World.Text.Delegates;
using XRL.World.Text.Attributes;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [HasVariableReplacer]
    public static class Utils
    {
        public static ModInfo ThisMod => ModManager.GetMod(MOD_ID);

        [VariableReplacer]
        public static string nbsp(DelegateContext Context)
        {
            string nbsp = "\xFF";
            string output = nbsp;
            if (!Context.Parameters.IsNullOrEmpty() && int.TryParse(Context.Parameters[0], out int count))
            {
                for (int i = 1; i < count; i++)
                {
                    output += nbsp;
                }
            }
            return output;
        }

        public static bool RegisterGameLevelEventHandlers()
        {
            Debug.Entry(1, $"Registering XRLGame Event Handlers...", Indent: 1);
            bool flag = The.Game != null;
            if (flag)
            {
                BeforeModGiganticAppliedHandler.Register();
                AfterModGiganticAppliedHandler.Register();
                BeforeDescribeModGiganticHandler.Register();
                DescribeModGiganticHandler.Register();
                MeleeWeapon_AfterObjectCreatedHandler.Register();
                // ExampleHandler.Register();
            }
            else
            {
                Debug.Entry(2, $"The.Game is null, unable to register any events.", Indent: 2);

            }
            Debug.LoopItem(1, $"Event Handler Registration Finished", Indent: 1, Good: flag);
            return flag;
        }

        [ModSensitiveStaticCache(CreateEmptyInstance = true)]
        private static Dictionary<string, string> _TilePathCache = new();
        private static readonly List<string> TileSubfolders = new()
        {
            "",
            "Abilities",
            "Assets",
            "Blueprints",
            "Creatures",
            "Items",
            "Mutations",
            "NaturalWeapons",
            "Terrain",
            "Tiles"
        };
        public static string BuildCustomTilePath(string DisplayName)
        {
            return Grammar.MakeTitleCase(ColorUtility.StripFormatting(DisplayName)).Replace(" ", "");
        }
        public static bool TryGetTilePath(string TileName, out string TilePath)
        {
            Debug.Entry(3, $"@ Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath)", Indent: 2);

            bool found = false;
            bool cache = false;
            Debug.Entry(4, $"? if (_TilePathCache.TryGetValue(TileName, out TilePath))", Indent: 2);
            if (cache = !_TilePathCache.TryGetValue(TileName, out TilePath))
            {
                Debug.Entry(4, $"_TilePathCache does not contain {TileName}", Indent: 3);
                Debug.Entry(4, $"x if (_TilePathCache.TryGetValue(TileName, out TilePath)) ?//", Indent: 2);

                Debug.Entry(4, $"Attempting to add \"{TileName}\" to _TilePathCache", Indent: 3);
                if (!found && !_TilePathCache.TryAdd(TileName, TilePath))
                    Debug.Entry(3, $"!! Adding \"{TileName}\" to _TilePathCache failed", Indent: 3);

                Debug.Entry(4, $"Listing subfolders", Indent: 2);
                Debug.Entry(4, $"> foreach (string subfolder  in TileSubfolders)", Indent: 2);
                foreach (string subfolder in TileSubfolders)
                {
                    Debug.LoopItem(4, $" \"{subfolder}\"", Indent: 3);
                }
                Debug.Entry(4, $"x foreach (string subfolder  in TileSubfolders) >//", Indent: 2);

                Debug.Entry(4, $"> foreach (string subfolder in TileSubfolders)", Indent: 2);
                Debug.Divider(3, "-", Count: 25, Indent: 2);
                foreach (string subfolder in TileSubfolders)
                {
                    string path = subfolder;
                    if (path != "") path += "/";
                    path += TileName;
                    if (SpriteManager.HasTextureInfo(path))
                    {
                        TilePath = path;
                        _TilePathCache[TileName] = TilePath;
                        Debug.CheckYeh(4, $"Tile: \"{path}\", Added entry to _TilePathCache", Indent: 3);
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Tile: \"{path}\"", Indent: 3);
                    }
                }
                Debug.Divider(3, "-", Count: 25, Indent: 2);
                Debug.Entry(4, $"x foreach (string subfolder in TileSubfolders) >//", Indent: 2);
            }
            else
            {
                Debug.Entry(3, $"_TilePathCache contains {TileName}", TilePath ?? "null", Indent: 3);
            }

            string foundLocation = cache ? "_TilePathCache" : "supplied subfolders";
            Debug.Entry(3, $"Tile \"{TileName}\" {(TilePath == null ? "not" : "was")} found in {foundLocation}", Indent: 2);

            found = TilePath != null;
            Debug.Entry(3, $"x Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath) @//", Indent: 2);
            return found;
        }

        public static string WeaponDamageString(int DieSize, int DieCount, int Bonus)
        {
            string output = $"{DieSize}d{DieCount}";

            if (Bonus > 0)
            {
                output += $"+{Bonus}";
            }
            else if (Bonus < 0)
            {
                output += Bonus;
            }

            return output;
        }

        public static Random RndGP = Stat.GetSeededRandomGenerator("HNPS_GigantismPlus");

        // !! This is currently not firing from any of the NaturalEquipmentMod Mutations but it has code that will make implementing the cybernetics adjustments easier.
        // The supplied part has the supplied blueprint created and assigned to it, saving the supplied previous behavior.
        // The supplied stats are assigned to the new part.
        public static void AddAccumulatedNaturalEquipmentTo(GameObject Creature, BodyPart Part, string BlueprintName, GameObject OldDefaultBehavior, string BaseDamage, int MaxStrBonus, int HitBonus, string AssigningMutation)
        {
            Debug.Divider(3, "-", 40, Indent: 4);
            Debug.Entry(3, "* Utils.AddAccumulatedNaturalEquipmentTo()", Indent: 4);

            if (Part != null && Part.Type == "Hand" && !Part.IsExternallyManagedLimb())
            {
                // make Creature gigantic temporarily if they normally would be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = true;

                if (AssigningMutation != "HNPS_GigantismPlus")
                {
                    Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);
                }
                else
                {
                    if (Creature.HasPart<ElongatedPaws>())
                    {
                        ItemModding.ApplyModification(Part.DefaultBehavior, "ModElongatedNaturalWeapon", Actor: Creature);
                    }
                    ItemModding.ApplyModification(Part.DefaultBehavior, "ModGiganticNaturalWeapon", Actor: Creature);
                }

                if (Part.DefaultBehavior != null)
                {
                    Debug.Entry(3, "Part.DefaultBehavior not null, assigning stats", Indent: 5);

                    Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation, false);

                    MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                    // weapon.BaseDamage = BaseDamage;
                    // if (HitBonus != 0) weapon.HitBonus = HitBonus;
                    // weapon.MaxStrengthBonus = MaxStrBonus;

                    Debug.Entry(3, "Checking for HandBones", Indent: 5);
                    Debug.Entry(3, "* if (TryGetNaturalWeaponCyberneticsList(Part.ParentBody, out string FistReplacement))", Indent: 5);
                    if (Part.ParentBody.TryGetNaturalWeaponCyberneticsList(out string FistReplacement))
                    {
                        Debug.Entry(3, $"HandBones Found: {FistReplacement}", Indent: 6);
                        switch (FistReplacement)
                        {
                            case "RealHomosapien_ZetachromeFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(4)", Indent: 6);
                                weapon.AdjustDamageDieSize(4);
                                break;
                            case "CrysteelFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(3)", Indent: 6);
                                weapon.AdjustDamageDieSize(3);
                                break;
                            case "FulleriteFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(2)", Indent: 6);
                                weapon.AdjustDamageDieSize(2);
                                break;
                            case "CarbideFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(1)", Indent: 6);
                                weapon.AdjustDamageDieSize(1);
                                break;
                            default:
                                Debug.Entry(3, "-FistReplacement has no assiciated bonus", Indent: 6);
                                break;
                        }
                    }
                    else
                    {
                        Debug.Entry(3, $"No HandBones Found", Indent: 6);
                    }
                    Debug.Entry(3, "x if (TryGetNaturalWeaponCyberneticsList(Part.ParentBody, out string FistReplacement)) >//", Indent: 5);

                    var cybernetics = Part.ParentBody.GetBody().Cybernetics;
                    if (cybernetics != null && cybernetics.TryGetPart(out CyberneticsGiganticExoframe exoframe))
                    {
                        Part.DefaultBehavior.RequirePart<Metal>();

                        if (exoframe.AugmentAdjectiveColor == "zetachrome")
                        {
                            Part.DefaultBehavior.RequirePart<Zetachrome>();
                            Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "mCmC");
                        }
                        if (exoframe.AugmentAdjectiveColor == "crysteel")
                        {
                            Part.DefaultBehavior.RequirePart<Crysteel>();
                            Part.DefaultBehavior.SetIntProperty("Flawless", 1);
                            Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "KGKG");
                        }

                        if (exoframe.Model == "YES") Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "WOWO");

                        Part.DefaultBehavior.DisplayName = exoframe.GetAugmentAdjective() + " " + Part.DefaultBehavior.ShortDisplayName;

                        Description desc = Part.DefaultBehavior.GetPart<Description>();
                        desc._Short += $" This appendage is being {exoframe.GetShortAugmentAdjective()} by a {exoframe.ImplantObject.DisplayName}.";

                        Render render = Part.DefaultBehavior.GetPart<Render>();
                        render.ColorString = exoframe.AugmentTileColorString;
                        render.DetailColor = exoframe.AugmentTileDetailColor;
                        render.Tile = exoframe.AugmentTile;

                        Part.DefaultBehavior.SetStringProperty("SwingSound", exoframe.AugmentSwingSound);
                        Part.DefaultBehavior.SetStringProperty("BlockedSound", exoframe.AugmentBlockedSound);
                    }

                    Debug.Entry(4, $"]|> hand.DefaultBehavior = {BlueprintName}", Indent: 5);
                    Debug.Entry(4, $"]|> MaxStrBonus: {weapon.MaxStrengthBonus}", Indent: 5);
                    Debug.Entry(4, $"]|> Base: {weapon.BaseDamage}", Indent: 5);
                    Debug.Entry(4, $"]|> Hit: {weapon.HitBonus}", Indent: 5);
                }
                else
                {
                    Debug.Entry(3, $"part.DefaultBehavior was null, invalid blueprint name \"{BlueprintName}\"", Indent: 5);
                    Part.DefaultBehavior = OldDefaultBehavior;
                    Debug.Entry(3, $"OldDefaultBehavior reassigned", Indent: 5);
                }

                // make Creature not gigantic if they're prentending not to be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = false;
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"", Indent: 4);
            }

            Debug.Entry(2, "x public void AddAccumulatedNaturalEquipmentTo() ]//", Indent: 4);
            Debug.Divider(3, "-", 40, Indent: 4);
        } //!-- public void AddGiganticFistTo(BodyPart part)

        // Ripped wholesale from ModGigantic.
        public static string GetProcessedItem(List<string> item, bool second, List<List<string>> items, GameObject obj)
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
        } //!-- public static string GetProcessedItem(List<string> item, bool second, List<List<string>> items, GameObject obj)

        public static int ExplodingDie(int Number, DieRoll DieRoll, int Step = 1, int Limit = 0, int Indent = 0)
        {
            Debug.Entry(4,
                $"ExplodingDie(Number: {Number}, DieRoll: {DieRoll}, Step: {Step}, Limit: {Limit})",
                Indent: Indent);

            int High = DieRoll.Max();
            int oldIndent = Indent;

            Debug.Entry(4, $"Explodes on High Roll: {High}", Indent: Indent);
            Begin:
            if (Limit != 0 && High >= Limit)
            {
                Debug.Entry(4, "Limit 0 or DieRoll.Max() >= Limit", Indent: ++Indent);
                Debug.Entry(4, "Exiting", Indent: Indent--);
                Number = Limit;
                goto Exit;
            }

            Debug.Entry(4, $"Rollin' the Die!", Indent: Indent);
            int Result = DieRoll.Resolve();
            if (Result == High)
            {
                Debug.Entry(4, $"Result: {Result}, Success!", Indent: ++Indent);
                Debug.Entry(4, $"Increasing Number by {Step}", Indent: Indent);
                Debug.Entry(4, $"Sending Number for another roll!", Indent: Indent);
                Number += Step;
                Indent++;
                // Number = ExplodingDie(Number += Step, DieRoll, Step, Limit, ++Indent);
                goto Begin;
            }
            else
            {
                Debug.Entry(4, $"Result: {Result}, Failure!", Indent: ++Indent);
            }

            Exit:
            Debug.Entry(4, $"Final Number: {Number}", Indent: oldIndent);
            return Number;
        }

        public static int ExplodingDie(int Number, string DieRoll, int Step = 1, int Limit = 0, int Indent = 0)
        {
            DieRoll dieRoll = new(DieRoll);
            return ExplodingDie(Number, dieRoll, Step, Limit, Indent);
        }

        public static void SwapMutationEnrtyClass(MutationEntry Entry, string Class, int Indent = 0)
        {
            Debug.Entry(4, 
                $"@ {nameof(Utils)}.{nameof(SwapMutationEnrtyClass)}(MutationEntry Entry, string Class, int Indent = 0)",
                Indent: Indent);
            Debug.Entry(4,
                $"Entry.DisplayName: {Entry.DisplayName ?? "[Nameless]"} | Entry.Class: {Entry.Class} | Destination Class: {Class}",
                Indent: Indent);

            if (Entry.Class != Class)
            {
                Debug.Entry(4, $"Classes don't match, swapping", Indent: Indent+1);
                Entry.Class = Class;
            }
            else
            {
                Debug.Entry(4, $"Classes already match, no action necessary", Indent: Indent + 1);
            }
                
            Debug.Entry(4,
                $"x {nameof(Utils)}.{nameof(SwapMutationEnrtyClass)}(MutationEntry Entry, string Class, int Indent = 0) @//",
                Indent: Indent);
        }

        public static void ManagedVanillaMutationOptionHandler()
        {
            Debug.Entry(4, $"* {nameof(Utils)}.{nameof(ManagedVanillaMutationOptionHandler)}()", Indent: 1);
            List<(string, string, string)> MutationEntries = new()
            {
                ("Burrowing Claws", "BurrowingClaws", "UD_ManagedBurrowingClaws"),
                ("Crystallinity", "Crystallinity", "UD_ManagedCrystallinity")
            };
            Debug.Entry(4, $"> foreach ((string Name, string Vanilla, string Managed) entry in MutationEntries)", Indent: 1);
            foreach ((string Name, string Vanilla, string Managed) in MutationEntries)
            {
                Debug.LoopItem(4, $"Name: {Name} | Vanilla: {Vanilla} | Managed: {Managed}", Indent: 1);
                MutationEntry vanillaMutation = MutationFactory.GetMutationEntryByName(Name);
                if ((bool)EnableManagedVanillaMutations)
                {
                    SwapMutationEnrtyClass(vanillaMutation, Managed, Indent: 2);
                }
                else
                {
                    SwapMutationEnrtyClass(vanillaMutation, Vanilla, Indent: 2);
                }
            }
            Debug.Entry(4, $"x foreach ((string Name, string Vanilla, string Managed) entry in MutationEntries) >//", Indent: 1);
            Debug.Entry(4, $"x {nameof(Utils)}.{nameof(ManagedVanillaMutationOptionHandler)}() *//", Indent: 1);
        }

        /*
        // method to swap Gigantism mutation category between Physical and PhysicalDefects
        public static void SwapMutationCategory(MutationEntry MutationEntry, string OutOfCategory, string IntoCategory)
        {
            SwapMutationCategory(MutationEntry.Name, OutOfCategory, IntoCategory);
        } //!--- public static void SwapMutationCategory(this MutationEntry MutationEntry, string OutOfCategory, string IntoCategory)

        public static void SwapMutationCategory(BaseMutation Mutation, string OutOfCategory, string IntoCategory)
        {
            SwapMutationCategory(Mutation?.GetMutationEntry()?.Name, OutOfCategory, IntoCategory);
        } //!--- public static void SwapMutationCategory(this MutationEntry MutationEntry, string OutOfCategory, string IntoCategory)

        public static void SwapMutationCategory(string MutationName, string OutOfCategory, string IntoCategory)
        {
            if (MutationFactory.TryGetMutationEntry(MutationName, out MutationEntry MutationEntry))
            {
                Debug.Header(3, MutationEntry.DisplayName, $"{nameof(SwapMutationCategory)}(OutOfCategory: \"{OutOfCategory}\", IntoCategory: \"{IntoCategory}\")");

                List<MutationCategory> mutationCategories = MutationFactory.GetCategories().Vomit(4, "mutationCategories", DivAfter: Debug.HONLY, Indent: 1);

                MutationCategory outOfCategory = mutationCategories.Find((x) => x.Name == OutOfCategory);
                MutationCategory intoCategory = mutationCategories.Find((x) => x.Name == IntoCategory);

                outOfCategory.Entries.Vomit(4, "outOfCategoryEntries | Before:", DivAfter: Debug.HONLY, Indent: 1);
                intoCategory.Entries.Vomit(4, "intoCategoryEntries | Before:", Indent: 1);
                Debug.Divider(4, Debug.HONLY, 40, Indent: 1);

                foreach (MutationCategory category in MutationFactory.GetCategories())
                {
                    if (category.Name == IntoCategory)
                    {
                        if (!category.Entries.Contains(MutationEntry))
                        {
                            Debug.Entry(4, $"Mutation \"{MutationEntry.DisplayName}\" not found in IntoCategory \"{intoCategory.Name}\"", Indent: 2);
                            Debug.Entry(4, $"Adding it", Indent: 3);
                            category.Entries.Add(MutationEntry);
                            Debug.Entry(4, $"Attempting to Sort", Indent: 3);
                            category.Entries.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));
                            MutationEntry.Type = IntoCategory;
                        }
                    }
                    if (category.Name == OutOfCategory)
                    {
                        if (outOfCategory.Entries.Contains(MutationEntry))
                        {
                            Debug.Entry(4, $"Mutation \"{MutationEntry.DisplayName}\" found in OutOfCategory \"{outOfCategory.Name}\"", Indent: 2);
                            Debug.Entry(4, $"Removing it", Indent: 3);
                            outOfCategory.Entries.Remove(MutationEntry);
                        }
                    }
                }

                Debug.Divider(4, Debug.HONLY, 40, Indent: 1);
                outOfCategory.Entries.Vomit(4, "outOfCategoryEntries |  After:", DivAfter: Debug.HONLY, Indent: 1);
                intoCategory.Entries.Vomit(4, "intoCategoryEntries |  After:", Indent: 1);

                Debug.Footer(3, MutationEntry.Mutation.GetMutationClass(), $"{nameof(SwapMutationCategory)}(OutOfCategory: \"{OutOfCategory}\", IntoCategory: \"{IntoCategory}\")");
            }
        }
        */

        public static void SwapMutationCategory(string MutationName, string OutOfCategory, string IntoCategory)
        {
            Debug.Header(3, 
                $"{MutationName}", 
                $"SwapMutationCategory(MutationName, OutOfCategory: \"{OutOfCategory}\", IntoCategory: \"{IntoCategory}\")");

            MutationEntry MutationEntry = MutationFactory.GetMutationEntryByName(MutationName);

            Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 1);
            foreach (MutationCategory category in MutationFactory.GetCategories())
            {
                Debug.LoopItem(4, category.Name, Indent: 2);
                if (category.Name == IntoCategory)
                {
                    Debug.DiveIn(4, $"Found Category: \"{IntoCategory}\"", Indent: 2);

                    Debug.Entry(3, $"Adding \"{MutationEntry.DisplayName}\" Mutation to \"{IntoCategory}\" Category", Indent: 3);
                    category.Add(MutationEntry);
                    category.Entries.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

                    Debug.Entry(4, $"Displaying all entries in \"{IntoCategory}\" Category", Indent: 3);
                    Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 3);
                    foreach (MutationEntry entry in category.Entries)
                    {
                        Debug.LoopItem(4, entry.DisplayName, Indent: 4);
                    }
                    Debug.DiveOut(3, $"x {IntoCategory} //", Indent: 2);
                }
                if (category.Name == OutOfCategory)
                {
                    Debug.DiveIn(3, $"Found Category: \"{OutOfCategory}\"", Indent: 2);
                    Debug.Entry(3, $"Removing \"{MutationEntry.DisplayName}\" from \"{OutOfCategory}\" Category", Indent: 3);
                    category.Entries.RemoveAll(r => r == MutationEntry);
                    Debug.DiveOut(3, $"x {OutOfCategory} //", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (MutationCategory category in MutationFactory.GetCategories()) >//", Indent: 1);
            Debug.Footer(3, 
                $"{MutationName}", 
                $"SwapMutationCategory(MutationName, OutOfCategory: \"{OutOfCategory}\", IntoCategory: \"{IntoCategory}\")");
        } //!-- private void SwapMutationCategory(bool Before = true)

        public static GameObjectBlueprint GetGameObjectBlueprint(string Blueprint)
        {
            GameObjectFactory.Factory.Blueprints.TryGetValue(Blueprint, out GameObjectBlueprint GameObjectBlueprint);
            return GameObjectBlueprint;
        }
        public static bool TryGetGameObjectBlueprint(string Blueprint, out GameObjectBlueprint GameObjectBlueprint)
        {
            GameObjectBlueprint = GetGameObjectBlueprint(Blueprint);
            return !GameObjectBlueprint.Is(null);
        }
    } //!-- public static class Utils

}