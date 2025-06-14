﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;

using Kobold;

using XRL;
using XRL.UI;
using XRL.Rules;
using XRL.Language;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.World.ObjectBuilders;
using XRL.World.AI.GoalHandlers;
using XRL.World.Text.Delegates;
using XRL.World.Text.Attributes;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [HasVariableReplacer]
    public static class Utils
    {
        private static bool doDebug => true;
        public static bool getDoDebug (string MethodName)
        {
            if (MethodName == nameof(TryGetTilePath))
                return false;

            if (MethodName == nameof(AddAccumulatedNaturalEquipmentTo))
                return false;

            if (MethodName == nameof(ExplodingDie))
                return false;

            if (MethodName == nameof(SwapMutationEnrtyClass))
                return false;

            if (MethodName == nameof(ManagedVanillaMutationOptionHandler))
                return false;

            if (MethodName == nameof(SwapMutationCategory))
                return false;

            if (MethodName == nameof(Rumble))
                return true;

            return doDebug;
        }

        public static ModInfo ThisMod => ModManager.GetMod(MOD_ID);
        public static ModInfo UD_Blink_Mutation => ModManager.GetMod(UD_BLINK_MUTATION_MOD_ID);

        public static Gigantified Gigantifier = new();
        public static GiantHero GiantHeroBuilder = new();
        public static WrassleGiantHero WrassleGiantHeroBuilder = new();

        public struct DescriptionElement
        {
            private static bool doDebug => getClassDoDebug(nameof(DescriptionElement));

            public string Verb;
            public string Effect;

            public DescriptionElement(string Verb, string Effect)
            {
                this.Verb = Verb;
                this.Effect = Effect;
            }

            public DescriptionElement(List<string> Source)
            {
                Verb = null;
                Effect = null;
                if (!Source.IsNullOrEmpty())
                {
                    Verb = Source[0];
                    if (Source.Count > 1)
                    {
                        Effect = Source[1];
                    }
                }
            }

            public readonly List<string> ToList()
            {
                return new List<string>()
                {
                    Verb,
                    Effect,
                };
            }

            public override readonly string ToString()
            {
                if (Verb == "")
                {
                    return "It " + Effect;
                }
                if (Verb == null)
                {
                    return "It is " + Effect;
                }
                return Grammar.ThirdPerson(Verb, PrependSpace: false) + " " + Effect;
            }

            public readonly string ToString(GameObject Object)
            {
                if (Verb == "")
                {
                    return Object.It + " " + Effect;
                }
                if (Verb == null)
                {
                    return Object.Itis + " " + Effect;
                }
                return Object.GetVerb(Verb, PrependSpace: false) + " " + Effect;
            }
        }

        public static List<DescriptionElement> IterateDataBucketTags(GameObject Object, GameObjectBlueprint GigantismPlusModGiganticDescriptions, string When, string Where)
        {
            List<DescriptionElement> Output = new();
            foreach ((string location, string value) in GigantismPlusModGiganticDescriptions.Tags)
            {
                string[] locationArray = location.Split("::", StringSplitOptions.RemoveEmptyEntries);
                if (locationArray.Length != 4) continue;

                (string targetEvent, string targetList, string conditionPart, string who) = (locationArray[0], locationArray[1], locationArray[2], locationArray[2]);
                if (targetEvent != When) continue;
                if (targetList != "General" && targetList != "Weapon")
                {
                    Debug.Warn(1,
                        nameof(Utils),
                        nameof(IterateDataBucketTags),
                        $"[{who}]: {targetList} in {location} is not a valid list. \"Weapon\" or \"General\" required.", Indent: 0);
                    continue;
                }
                if (!Object.HasPart(conditionPart)) continue;

                string[] descriptionArray = value.Split(";", StringSplitOptions.RemoveEmptyEntries);
                if (descriptionArray.Length != 2) continue;

                if (descriptionArray[0] == "'null'") descriptionArray[0] = null;
                DescriptionElement description = new(descriptionArray[0], descriptionArray[1]);
                if (targetEvent == When)
                {
                    if (targetList == Where)
                    {
                        Output.Add(description);
                    }
                }
            }
            return Output;
        }

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

        [VariableReplacer]
        public static string optionalColor(DelegateContext Context)
        {
            string Text = string.Empty;
            string Color = string.Empty;
            string Fallback = string.Empty;
            if (!Context.Parameters.IsNullOrEmpty())
            {
                if (Context.Parameters[0] != null)
                    Text = Context.Parameters[0];
                if (Context.Parameters[1] != null)
                    Color = Context.Parameters[1];
                if (Context.Parameters[2] != null)
                    Fallback = Context.Parameters[2];
            }
            return Text.OptionalColor(Color, Fallback, Colorfulness);
        }

        [VariableReplacer]
        public static string optionalColorYuge(DelegateContext Context)
        {
            string Text = string.Empty;
            if (!Context.Parameters.IsNullOrEmpty())
            {
                if (Context.Parameters[0] != null)
                    Text = Context.Parameters[0];
            }
            return Text.OptionalColorYuge(Colorfulness);
        }

        [VariableReplacer]
        public static string optionalColorGigantic(DelegateContext Context)
        {
            string Text = string.Empty;
            if (!Context.Parameters.IsNullOrEmpty())
            {
                if (Context.Parameters[0] != null)
                    Text = Context.Parameters[0];
            }
            return Text.OptionalColorGigantic(Colorfulness);
        }

        public static bool RegisterGameLevelEventHandlers()
        {
            Debug.Entry(1, $"Registering XRLGame Event Handlers...", Indent: 1);
            bool flag = The.Game != null;
            if (flag)
            {
                CrayonsGetColorHandler.Register();
                BeforeModGiganticAppliedHandler.Register();
                AfterModGiganticAppliedHandler.Register();
                BeforeDescribeModGiganticHandler.Register();
                DescribeModGiganticHandler.Register();
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
            "Tiles",
            "Tiles2",
            "Walls",
            "Walls2",
            "Widgets",
        };
        public static string BuildCustomTilePath(string DisplayName)
        {
            return Grammar.MakeTitleCase(ColorUtility.StripFormatting(DisplayName)).Replace(" ", "");
        }
        public static bool TryGetTilePath(string TileName, out string TilePath, bool IsWholePath = false)
        {
            Debug.Entry(3, $"@ Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath)", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));

            bool wasFound = false;
            bool inCache;
            Debug.Entry(4, $"? if (_TilePathCache.TryGetValue(TileName, out TilePath))", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
            if (inCache = !_TilePathCache.TryGetValue(TileName, out TilePath))
            {
                Debug.Entry(4, $"_TilePathCache does not contain {TileName}", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                Debug.Entry(4, $"x if (_TilePathCache.TryGetValue(TileName, out TilePath)) ?//", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));

                Debug.Entry(4, $"Attempting to add \"{TileName}\" to _TilePathCache", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                if (!wasFound && !_TilePathCache.TryAdd(TileName, TilePath))
                    Debug.Entry(3, $"!! Adding \"{TileName}\" to _TilePathCache failed", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));

                if (IsWholePath)
                {
                    if (SpriteManager.HasTextureInfo(TileName))
                    {
                        TilePath = TileName;
                        _TilePathCache[TileName] = TileName;
                        Debug.CheckYeh(4, $"Tile: \"{TileName}\", Added entry to _TilePathCache", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Tile: \"{TileName}\"", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    }
                }
                else
                {
                    Debug.Entry(4, $"Listing subfolders", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    Debug.Entry(4, $"> foreach (string subfolder  in TileSubfolders)", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    foreach (string subfolder in TileSubfolders)
                    {
                        Debug.LoopItem(4, $" \"{subfolder}\"", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    }
                    Debug.Entry(4, $"x foreach (string subfolder  in TileSubfolders) >//", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));

                    Debug.Entry(4, $"> foreach (string subfolder in TileSubfolders)", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    Debug.Divider(3, "-", Count: 25, Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    foreach (string subfolder in TileSubfolders)
                    {
                        string path = subfolder;
                        if (path != "") path += "/";
                        path += TileName;
                        if (SpriteManager.HasTextureInfo(path))
                        {
                            TilePath = path;
                            _TilePathCache[TileName] = TilePath;
                            Debug.CheckYeh(4, $"Tile: \"{path}\", Added entry to _TilePathCache", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                        }
                        else
                        {
                            Debug.CheckNah(4, $"Tile: \"{path}\"", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
                        }
                    }
                    Debug.Divider(3, "-", Count: 25, Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                    Debug.Entry(4, $"x foreach (string subfolder in TileSubfolders) >//", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
                }
            }
            else
            {
                Debug.Entry(3, $"_TilePathCache contains {TileName}", TilePath ?? "null", Indent: 3, Toggle: getDoDebug(nameof(TryGetTilePath)));
            }
            string foundLocation = 
                inCache 
                ? "_TilePathCache" 
                : IsWholePath 
                    ? "files" 
                    : "supplied subfolders";

            Debug.Entry(3, $"Tile \"{TileName}\" {(TilePath == null ? "not" : "was")} found in {foundLocation}", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));

            wasFound = TilePath != null;
            Debug.Entry(3, $"x Utils.TryGetTilePath(string TileName: {TileName}, out string TilePath) @//", Indent: 2, Toggle: getDoDebug(nameof(TryGetTilePath)));
            return wasFound;
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

        // !! This is currently not firing from any of the NaturalEquipmentMod Mutations but it has code that will make implementing the cybernetics adjustments easier.
        // The supplied part has the supplied blueprint created and assigned to it, saving the supplied previous behavior.
        // The supplied stats are assigned to the new part.
        public static void AddAccumulatedNaturalEquipmentTo(GameObject Creature, BodyPart Part, string BlueprintName, GameObject OldDefaultBehavior, string BaseDamage, int MaxStrBonus, int HitBonus, string AssigningMutation)
        {
            Debug.Divider(3, "-", 40, Indent: 4, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
            Debug.Entry(3, "* Utils.AddAccumulatedNaturalEquipmentTo()", Indent: 4, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));

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
                    Debug.Entry(3, "Part.DefaultBehavior not null, assigning stats", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));

                    Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation, false);

                    MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                    // weapon.BaseDamage = BaseDamage;
                    // if (HitBonus != 0) weapon.HitBonus = HitBonus;
                    // weapon.MaxStrengthBonus = MaxStrBonus;

                    Debug.Entry(3, "Checking for HandBones", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    Debug.Entry(3, "* if (TryGetNaturalWeaponCyberneticsList(Part.ParentBody, out string FistReplacement))", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    if (Part.ParentBody.TryGetNaturalWeaponCyberneticsList(out string FistReplacement))
                    {
                        Debug.Entry(3, $"HandBones Found: {FistReplacement}", Indent: 6);
                        switch (FistReplacement)
                        {
                            case "RealHomosapien_ZetachromeFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(4)", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                                weapon.AdjustDamageDieSize(4);
                                break;
                            case "CrysteelFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(3)", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                                weapon.AdjustDamageDieSize(3);
                                break;
                            case "FulleriteFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(2)", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                                weapon.AdjustDamageDieSize(2);
                                break;
                            case "CarbideFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(1)", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                                weapon.AdjustDamageDieSize(1);
                                break;
                            default:
                                Debug.Entry(3, "-FistReplacement has no assiciated bonus", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                                break;
                        }
                    }
                    else
                    {
                        Debug.Entry(3, $"No HandBones Found", Indent: 6, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    }
                    Debug.Entry(3, "x if (TryGetNaturalWeaponCyberneticsList(Part.ParentBody, out string FistReplacement)) >//", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));

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
                        desc._Short += $" This appendage is being {exoframe.GetAugmentAdjective()} by a {exoframe.ImplantObject.DisplayName}.";

                        Render render = Part.DefaultBehavior.GetPart<Render>();
                        render.ColorString = exoframe.AugmentTileColorString;
                        render.DetailColor = exoframe.AugmentTileDetailColor;
                        render.Tile = exoframe.AugmentTile;

                        Part.DefaultBehavior.SetStringProperty("SwingSound", exoframe.AugmentSwingSound);
                        Part.DefaultBehavior.SetStringProperty("BlockedSound", exoframe.AugmentBlockedSound);
                    }

                    Debug.Entry(4, $"]|> hand.DefaultBehavior = {BlueprintName}", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    Debug.Entry(4, $"]|> MaxStrBonus: {weapon.MaxStrengthBonus}", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    Debug.Entry(4, $"]|> Base: {weapon.BaseDamage}", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    Debug.Entry(4, $"]|> Hit: {weapon.HitBonus}", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                }
                else
                {
                    Debug.Entry(3, $"part.DefaultBehavior was null, invalid blueprint name \"{BlueprintName}\"", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                    Part.DefaultBehavior = OldDefaultBehavior;
                    Debug.Entry(3, $"OldDefaultBehavior reassigned", Indent: 5, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
                }

                // make Creature not gigantic if they're prentending not to be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = false;
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"", Indent: 4, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
            }

            Debug.Entry(2, "x public void AddAccumulatedNaturalEquipmentTo() ]//", Indent: 4, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
            Debug.Divider(3, "-", 40, Indent: 4, Toggle: getDoDebug(nameof(AddAccumulatedNaturalEquipmentTo)));
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
                Indent: Indent, Toggle: getDoDebug(nameof(ExplodingDie)));

            int High = DieRoll.Max();
            int oldIndent = Indent;

            Debug.Entry(4, $"Explodes on High Roll: {High}", Indent: Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
            Begin:
            if (Limit != 0 && High >= Limit)
            {
                Debug.Entry(4, "Limit 0 or DieRoll.Max() >= Limit", Indent: ++Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
                Debug.Entry(4, "Exiting", Indent: Indent--, Toggle: getDoDebug(nameof(ExplodingDie)));
                Number = Limit;
                goto Exit;
            }

            Debug.Entry(4, $"Rollin' the Die!", Indent: Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
            int Result = DieRoll.Resolve();
            if (Result == High)
            {
                Debug.Entry(4, $"continue: {Result}, Success!", Indent: ++Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
                Debug.Entry(4, $"Increasing Number by {Step}", Indent: Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
                Debug.Entry(4, $"Sending Number for another roll!", Indent: Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
                Number += Step;
                Indent++;
                // Number = ExplodingDie(Number += Step, DieRoll, Step, Limit, ++Indent);
                goto Begin;
            }
            else
            {
                Debug.Entry(4, $"continue: {Result}, Failure!", Indent: ++Indent, Toggle: getDoDebug(nameof(ExplodingDie)));
            }

            Exit:
            Debug.Entry(4, $"Final Number: {Number}", Indent: oldIndent, Toggle: getDoDebug(nameof(ExplodingDie)));
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
                Indent: Indent, Toggle: getDoDebug(nameof(SwapMutationEnrtyClass)));
            Debug.Entry(4,
                $"Entry.DisplayName: {Entry.Name ?? "[Nameless]"} | Entry.Class: {Entry.Class} | Destination Class: {Class}",
                Indent: Indent, Toggle: getDoDebug(nameof(SwapMutationEnrtyClass)));

            if (Entry.Class != Class)
            {
                Debug.Entry(4, $"Classes don't match, swapping", Indent: Indent + 1, Toggle: getDoDebug(nameof(SwapMutationEnrtyClass)));
                Entry.Class = Class;
            }
            else
            {
                Debug.Entry(4, $"Classes already match, no action necessary", Indent: Indent + 1, Toggle: getDoDebug(nameof(SwapMutationEnrtyClass)));
            }
                
            Debug.Entry(4,
                $"x {nameof(Utils)}.{nameof(SwapMutationEnrtyClass)}(MutationEntry Entry, string Class, int Indent = 0) @//",
                Indent: Indent, Toggle: getDoDebug(nameof(SwapMutationEnrtyClass)));
        }

        public static void ManagedVanillaMutationOptionHandler()
        {
            Debug.Entry(4, $"* {nameof(Utils)}.{nameof(ManagedVanillaMutationOptionHandler)}()", Indent: 1, Toggle: getDoDebug(nameof(ManagedVanillaMutationOptionHandler)));
            List<(string, string, string)> MutationEntries = new()
            {
                ("Burrowing Claws", "BurrowingClaws", "UD_ManagedBurrowingClaws"),
                ("Crystallinity", "Crystallinity", "UD_ManagedCrystallinity")
            };
            Debug.Entry(4, $"> foreach ((string Name, string Vanilla, string Managed) entry in MutationEntries)", Indent: 1, Toggle: getDoDebug(nameof(ManagedVanillaMutationOptionHandler)));
            foreach ((string Name, string Vanilla, string Managed) in MutationEntries)
            {
                Debug.LoopItem(4, $"Name: {Name} | Vanilla: {Vanilla} | Managed: {Managed}", Indent: 1, Toggle: getDoDebug(nameof(ManagedVanillaMutationOptionHandler)));
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
            Debug.Entry(4, $"x foreach ((string Name, string Vanilla, string Managed) entry in MutationEntries) >//", Indent: 1, Toggle: getDoDebug(nameof(ManagedVanillaMutationOptionHandler)));
            Debug.Entry(4, $"x {nameof(Utils)}.{nameof(ManagedVanillaMutationOptionHandler)}() *//", Indent: 1, Toggle: getDoDebug(nameof(ManagedVanillaMutationOptionHandler)));
        }
        public static void SwapMutationCategory(string MutationName, string OutOfCategory, string IntoCategory)
        {
            Debug.Header(3, 
                $"{MutationName}", 
                $"SwapMutationCategory(MutationName, " +
                $"OutOfCategory: \"{OutOfCategory}\", " +
                $"IntoCategory: \"{IntoCategory}\")", 
                Toggle: getDoDebug(nameof(SwapMutationCategory)));

            MutationEntry MutationEntry = MutationFactory.GetMutationEntryByName(MutationName);

            Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", 
                Indent: 1, Toggle: getDoDebug(nameof(SwapMutationCategory)));
            foreach (MutationCategory category in MutationFactory.GetCategories())
            {
                Debug.LoopItem(4, category.Name, Indent: 2, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                if (category.Name == IntoCategory)
                {
                    Debug.DiveIn(4, $"Found Category: \"{IntoCategory}\"", 
                        Indent: 2, Toggle: getDoDebug(nameof(SwapMutationCategory)));

                    Debug.Entry(3, $"Adding \"{MutationEntry.Name}\" Mutation to \"{IntoCategory}\" Category", 
                        Indent: 3, Toggle: getDoDebug(nameof(SwapMutationCategory)));

                    category.Add(MutationEntry);
                    category.Entries.Sort((x, y) => x.Name.CompareTo(y.Name));

                    Debug.Entry(4, $"Displaying all entries in \"{IntoCategory}\" Category", 
                        Indent: 3, Toggle: getDoDebug(nameof(SwapMutationCategory)));

                    Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", 
                        Indent: 3, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                    foreach (MutationEntry entry in category.Entries)
                    {
                        Debug.LoopItem(4, entry.Name, Indent: 4, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                    }

                    Debug.DiveOut(3, $"x {IntoCategory} //", 
                        Indent: 2, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                }
                if (category.Name == OutOfCategory)
                {
                    Debug.DiveIn(3, $"Found Category: \"{OutOfCategory}\"", 
                        Indent: 2, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                    Debug.Entry(3, $"Removing \"{MutationEntry.Name}\" from \"{OutOfCategory}\" Category", 
                        Indent: 3, Toggle: getDoDebug(nameof(SwapMutationCategory)));

                    category.Entries.RemoveAll(r => r == MutationEntry);

                    Debug.DiveOut(3, $"x {OutOfCategory} //", 
                        Indent: 2, Toggle: getDoDebug(nameof(SwapMutationCategory)));
                }
            }
            Debug.Entry(4, "x foreach (MutationCategory category in MutationFactory.GetCategories()) >//", 
                Indent: 1, Toggle: getDoDebug(nameof(SwapMutationCategory)));

            Debug.Footer(3, 
                $"{MutationName}", 
                $"SwapMutationCategory(MutationName, " +
                $"OutOfCategory: \"{OutOfCategory}\", " +
                $"IntoCategory: \"{IntoCategory}\")", 
                Toggle: getDoDebug(nameof(SwapMutationCategory)));
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
        public static string MakeAndList(IReadOnlyList<string> Words, bool Serial = true, bool IgnoreCommas = false)
        {
            List<string> replacedList = new();
            foreach (string entry in Words)
            {
                replacedList.Add(entry.Replace(",", ";;"));
            }
            string andList = Grammar.MakeAndList(replacedList, Serial);
            return andList.Replace(";;", ",");
        }
        public static string Quote(string @string)
        {
            return $"\"{@string}\"";
        }

        public static BookInfo GetBook(string BookName)
        {
            BookUI.Books.TryGetValue(BookName, out BookInfo Book);
            return Book;
        }

        public static float Rumble(float Cause, float DurationFactor = 1.0f, float DurationMax = 1.0f, bool Async = false)
        {
            float duration = Math.Min(DurationMax, Cause * DurationFactor);
            CombatJuice.cameraShake(duration, Async: Async);
            Debug.Entry(4, 
                $"* {nameof(Rumble)}:"
                + $" Duration ({duration}),"
                + $" Cause ({Cause}),"
                + $" DurationFactor ({DurationFactor}), "
                + $"DurationMax({DurationMax})", 
                Toggle: getDoDebug(nameof(Rumble)));
            return duration;
        }
        public static float Rumble(double Cause, float DurationFactor = 1.0f, float DurationMax = 1.0f, bool Async = true)
        {
            return Rumble((float)Cause, DurationFactor, DurationMax, Async);
        }
        public static float Rumble(int Cause, float DurationFactor = 1.0f, float DurationMax = 1.0f, bool Async = true)
        {
            return Rumble((float)Cause, DurationFactor, DurationMax, Async);
        }
    } //!-- public static class Utils

}