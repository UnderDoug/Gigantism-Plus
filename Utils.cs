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
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public static class Utils
    {
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
            Debug.Divider(3, Count: 40, Indent: 4);
            Debug.Entry(3, $"@ Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath)", Indent: 5);

            bool found = false;
            Debug.Entry(4, $"? if (_TilePathCache.TryGetValue(TileName, out TilePath))", Indent: 5);
            if (_TilePathCache.TryGetValue(TileName, out TilePath))
            {
                Debug.Entry(3, $"_TilePathCache contains {TileName}", TilePath ?? "null", Indent: 6);
                goto Exit;
            }
            Debug.Entry(4, $"_TilePathCache does not contain {TileName}", Indent: 6);
            Debug.Entry(4, $"x if (_TilePathCache.TryGetValue(TileName, out TilePath)) ?//", Indent: 5);

            Debug.Entry(4, $"Attempting to add \"{TileName}\" to _TilePathCache", Indent: 6);
            if (!_TilePathCache.TryAdd(TileName, TilePath)) 
                Debug.Entry(3, $"!! Adding \"{TileName}\" to _TilePathCache failed", Indent: 6);

            Debug.Entry(4, $"Listing subfolders", Indent: 5);
            Debug.Entry(4, $"> foreach (string subfolder  in TileSubfolders)", Indent: 5);
            foreach (string subfolder in TileSubfolders)
            {
                Debug.LoopItem(4, $"{subfolder}", Indent: 6);
            }
            Debug.Entry(4, $"x foreach (string subfolder  in TileSubfolders) >//", Indent: 5);

            Debug.Entry(4, $"> foreach (string subfolder in TileSubfolders)", Indent: 5);
            Debug.Divider(3, "-", Count: 25, Indent: 5);
            foreach (string subfolder in TileSubfolders)
            {
                string path = subfolder;
                if (path != "") path += "/";
                path += TileName;
                Debug.Entry(4, $"Does Tile: \"{path}\" exist?", Indent: 6);
                if (SpriteManager.HasTextureInfo(path))
                {
                    Debug.DiveIn(4, $"Yes.", Indent: 7);
                    Debug.Entry(3, $"out Tile = {path}", Indent: 7);
                    TilePath = path;
                    _TilePathCache[TileName] = TilePath;
                    Debug.Entry(3, $"Added entry to _TilePathCache", Indent: 7);
                    Debug.DiveOut(4, "TilePath Exists", Indent: 6);
                    break;
                }
                Debug.Entry(4, $"No.", Indent: 7);
            }
            Debug.Divider(3, "-", Count: 25, Indent: 5);
            Debug.Entry(4, $"x foreach (string subfolder in TileSubfolders) >//", Indent: 5);

            Debug.Entry(3, $"Tile \"{TileName}\" {(TilePath == null ? "not" : "was")} found in supplied subfolders", Indent: 5);

            Exit:
            found = TilePath != null;
            Debug.Entry(3, $"x Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath) @//", Indent: 5);
            Debug.Divider(3, Count: 40, Indent: 4);
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

        // !! This is currently not firing from any of the NaturalWeaponSubpart Mutations but it has code that will make implementing the cybernetics adjustments easier.
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
                $"ExplodingDie(Number: {Number}, DieRoll: {DieRoll.ToString()}, Step: {Step}, Limit: {Limit})",
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

        public static void ManagedVanillaMutation()
        {
            Debug.Entry(4, $"* {nameof(Utils)}.{nameof(ManagedVanillaMutation)}()", Indent: 1);
            List<(string, string, string)> MutationEntries = new List<(string Name, string Vanilla, string Managed)>
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
            Debug.Entry(4, $"x {nameof(Utils)}.{nameof(ManagedVanillaMutation)}() *//", Indent: 1);
        }

    } //!-- public static class Utils

} //!-- namespace HNPS_GigantismPlus