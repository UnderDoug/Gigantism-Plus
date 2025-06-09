using ConsoleLib.Console;
using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Core;
using XRL.Rules;
using XRL.Wish;
using XRL.World.AI.GoalHandlers;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Skills.Cooking;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
    public class Gigantified : IObjectBuilder
    {
        private static bool doDebug => getClassDoDebug(nameof(Gigantified));
        public static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                '!',    // Warn
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

        public string NamePrefix;

        public Gigantified() 
        { 
            Initialize();
        }
        public Gigantified(string NamePrefix) 
        {
            Initialize();
            this.NamePrefix = NamePrefix;
        }

        public override void Initialize()
        {
            NamePrefix = GetNamePrefix();
        }

        public static string GetNamePrefix()
        {
            return "gigantic".MaybeColor("gigantic");
        }

        public override void Apply(GameObject Object, string Context = "")
        {
            int Level = GetLevel();
            int Stews = GetStews();
            int ObjectTier = GetObjectTier(Object);
            int Tier = GetTier(ObjectTier);
            Gigantify(Object, Level, Stews, Tier, NamePrefix, Context);
        }

        public static int GetLevel(int StartsAt = 1, string DieString = "1d3", int Step = 2, int Limit = 16, int Indent = 2)
        {
            return ExplodingDie(StartsAt, DieString, Step, Limit, Indent);
        }
        public static int GetStews(int StartsAt = 0, string DieString = "1d2", int Step = 1, int Limit = 0, int Indent = 2)
        {
            return ExplodingDie(StartsAt, DieString, Step, Limit, Indent);
        }
        public static int GetObjectTier(GameObject Object)
        {
            if (Object == null || Object.GetBlueprint() == null)
            {
                return 0;
            }
            return (int)Math.Floor(Object.GetBlueprint().Stat("Level") / 5.0);
        }
        public static int GetTier(int StartsAt = 0, string DieString = "1d3", int Step = 1, int Indent = 2)
        {
            return ExplodingDie(StartsAt, DieString, Step, 8, Indent);
        }

        public static bool GigantifyMutant(GameObject Creature, int Level = 1, int? Stews = null, string NamePrefix = "", string Context = "")
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(GigantifyMutant)}"
                + $"(Creature: {Creature?.DebugName ?? NULL},"
                + $" Level: {Level},"
                + $" Stews: {Stews},"
                + $" NamePrefix: {NamePrefix},"
                + $" Context: {Context.Quote()})",
                Indent: indent, Toggle: getDoDebug());

            bool success = false;
            if (Creature != null && Creature.IsCreature && !Creature.IsTrueKin())
            {
                Debug.CheckYeh(4, $"Have Creature that isn't TrueKin", Indent: indent + 1, Toggle: getDoDebug());

                NamePrefix = NamePrefix == "gigantic" ? NamePrefix.MaybeColor("gigantic") : NamePrefix;

                Debug.Entry(4, nameof(NamePrefix), NamePrefix, Indent: indent + 1, Toggle: getDoDebug());

                Mutations mutations = Creature.GetPart<Mutations>();

                if (!mutations.HasMutation(nameof(GigantismPlus)))
                {
                    mutations.AddMutation(new GigantismPlus());
                }
                GigantismPlus gigantismPlus = mutations.GetMutation(nameof(GigantismPlus)) as GigantismPlus;

                Debug.Entry(4, $"Calculating Level/RA Split...", Indent: indent + 1, Toggle: getDoDebug());
                int level = Level > 10 ? 10 : Level;
                int rapidAdvances = Level > 10 ? Level - 10 : 0;
                gigantismPlus.BaseLevel = level;

                Debug.LoopItem(4, nameof(level), $"{level}", Indent: indent + 2, Toggle: getDoDebug());
                Debug.LoopItem(4, nameof(rapidAdvances), $"{rapidAdvances}", Indent: indent + 2, Toggle: getDoDebug());

                if (rapidAdvances > 0)
                {
                    gigantismPlus.SetRapidLevelAmount(rapidAdvances.RapidAdvancementCeiling(MinAdvances: 1), Sync: true);
                }

                Debug.Entry(4, $"Rolling Stews...", Indent: indent + 1, Toggle: getDoDebug());
                int startingStews = Stews ?? Stat.Roll("2d2");

                Debug.LoopItem(4, nameof(startingStews), $"{startingStews}", Indent: indent + 2, Toggle: getDoDebug());

                Creature.SetIntProperty(GNT_START_STEWS_PROPLABEL, startingStews);
                if (!Creature.TryGetPart(out StewBelly stewBelly))
                {
                    stewBelly = Creature.RequirePart<StewBelly>();
                }
                Debug.Entry(4, $"Processing Starting Stews...", Indent: indent + 1, Toggle: getDoDebug());
                stewBelly.ProcessStartingStews();

                Debug.Entry(4, $"Applying NamePrefix...", Indent: indent + 1, Toggle: getDoDebug());
                if (!NamePrefix.IsNullOrEmpty() && Context != "Hero" && Context != "Unique")
                {
                    Debug.CheckYeh(4, $"Creature eligible for NamePrefix", Indent: indent + 2, Toggle: getDoDebug());
                    Creature.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
                }
                else
                {
                    Debug.CheckNah(4, $"Creature ineligible for NamePrefix", Indent: indent + 2, Toggle: getDoDebug());
                }

                success = mutations.HasMutation(gigantismPlus.GetMutationClass());
            }
            
            Debug.Entry(4,
                $"x {nameof(GigantifyMutant)}"
                + $"(Creature: {Creature?.DebugName ?? NULL},"
                + $" Level: {Level},"
                + $" Stews: {Stews},"
                + $" NamePrefix: {NamePrefix},"
                + $" Context: {Context.Quote()}) *//",
                Indent: indent, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return success;
        }

        public static bool GigantifyTrueKin(GameObject Creature, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(GigantifyTrueKin)}"
                + $"(Creature: {Creature?.DebugName ?? NULL},"
                + $" Tier: {Tier},"
                + $" NamePrefix: {NamePrefix},"
                + $" Context: {Context.Quote()})",
                Indent: indent, Toggle: getDoDebug());

            bool success = false;
            if (Creature != null && Creature.IsCreature && Creature.IsTrueKin())
            {
                Debug.CheckYeh(4, $"Have Creature that is TrueKin", Indent: indent + 1, Toggle: getDoDebug());

                Debug.Entry(4, $"Getting list of Exoframes...", Indent: indent + 1, Toggle: getDoDebug());
                SortedDictionary<int, string> exoframeList = new();
                foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.GetBlueprintsInheritingFrom("BaseGiganticExoframe"))
                {
                    Debug.Divider(4, HONLY, 25, Indent: indent + 2, Toggle: doDebug);
                    string exoframeBlueprint = blueprint.Name;
                    int exoframeTier = blueprint.Tier;
                    Debug.LoopItem(4, $"{nameof(exoframeBlueprint)}: {blueprint.Name}, {nameof(exoframeTier)}: {blueprint.Tier}", Indent: indent + 2, Toggle: getDoDebug());

                    if (exoframeBlueprint == "THEGIGANTICEXOFRAME")
                    {
                        if (!1.ChanceIn(100000) && Context != "GODKING")
                        {
                            Debug.CheckNah(4, $"NO GOD TODAY", Indent: indent + 3, Toggle: getDoDebug());
                            continue;
                        }
                        else
                        {
                            exoframeList = new()
                            {
                                { exoframeTier, exoframeBlueprint }
                            };
                            Debug.CheckYeh(4, $"WE A GOD, BOIS", Indent: indent + 3, Toggle: getDoDebug());
                            break;
                        }
                    }
                    Debug.CheckYeh(4, $"Added to List", Indent: indent + 3, Toggle: getDoDebug());
                    exoframeList.Add(exoframeTier, exoframeBlueprint);
                }
                Debug.Divider(4, HONLY, 25, Indent: indent + 2, Toggle: doDebug);

                string exoframe = string.Empty;
                Debug.Entry(4, $"Getting Tier appropriate Exoframe ({Tier})...", Indent: indent + 1, Toggle: getDoDebug());
                foreach ((int exoframeTier, string exoframeBlueprint) in exoframeList)
                {
                    Debug.Divider(4, HONLY, 25, Indent: indent + 2, Toggle: doDebug);
                    Debug.LoopItem(4, $"{nameof(exoframeBlueprint)}: {exoframeBlueprint}, {nameof(exoframeTier)}: {exoframeTier}", Indent: indent + 2, Toggle: getDoDebug());
                    if (exoframeList.Count == 1 || exoframe == string.Empty)
                    {
                        Debug.CheckYeh(4, $"Granted: Only 1, or none assigned", Indent: indent + 3, Toggle: getDoDebug());
                        exoframe = exoframeBlueprint;
                        continue;
                    }
                    if (Tier >= exoframeTier)
                    {
                        Debug.CheckYeh(4, $"Upgraded! Tier matches or exceeds Exoframe Tier", Indent: indent + 3, Toggle: getDoDebug());
                        exoframe = exoframeBlueprint;
                    }
                    else
                    {
                        Debug.CheckNah(4, $"Denied! Insufficient Tier for Exoframe", Indent: indent + 3, Toggle: getDoDebug());
                    }
                }
                Debug.Divider(4, HONLY, 25, Indent: indent + 2, Toggle: doDebug);
                Debug.LoopItem(4, nameof(exoframe), exoframe, Indent: indent + 2, Toggle: getDoDebug());

                Debug.Entry(4, $"Getting Exoframe Object...", Indent: indent + 1, Toggle: getDoDebug());
                GameObject exoframeObject = GameObjectFactory.Factory.CreateObject(exoframe);
                Debug.LoopItem(4, nameof(exoframeObject), exoframeObject?.DebugName ?? NULL, Indent: indent + 2, Toggle: getDoDebug());
                if (exoframeObject != null && exoframeObject.TryGetPart(out CyberneticsGiganticExoframe exoframeCybernetic))
                {
                    Debug.CheckYeh(4, $"{nameof(exoframeObject)} not null, and have {nameof(CyberneticsGiganticExoframe)} part", Indent: indent + 1, Toggle: getDoDebug());

                    NamePrefix = exoframeCybernetic.GetNaturalEquipmentColoredAdjective();
                    Debug.LoopItem(4, nameof(NamePrefix), exoframe, Indent: indent + 2, Toggle: getDoDebug());

                    Debug.Entry(4, $"Checking Context...", Indent: indent + 1, Toggle: getDoDebug());
                    Debug.LoopItem(4, nameof(Context), Context, Indent: indent + 2, Toggle: getDoDebug());
                    if (Context == "Initialization" || Context == "GameStarted" || Context == "Wish" || Context == "Creation" || Context == "Sample")
                    {
                        Debug.Entry(4, $"Context requires {nameof(CyberneticsHasImplants)}...", Indent: indent + 1, Toggle: getDoDebug());

                        string addImplant = exoframe + "@body";
                        Creature.RequirePart<CyberneticsHasImplants>();
                        CyberneticsHasImplants hasImplants = Creature.RequirePart<CyberneticsHasImplants>();

                        Debug.LoopItem(4, nameof(hasImplants.Implants), hasImplants.Implants, Indent: indent + 2, Toggle: getDoDebug());
                        if (hasImplants.Implants.IsNullOrEmpty())
                        {
                            Debug.Entry(4, $"{nameof(hasImplants.Implants)} Empty, Adding...", Indent: indent + 1, Toggle: getDoDebug());
                            hasImplants.Implants = addImplant;
                        }
                        else if (hasImplants.Implants.Contains(","))
                        {
                            Debug.Entry(4, $"{nameof(hasImplants.Implants)} has multiple entires, Inserting...", Indent: indent + 1, Toggle: getDoDebug());

                            List<string> implants = new(hasImplants.Implants.Split(","));
                            bool found = false;
                            for (int i = 0; i < implants.Count(); i++)
                            {
                                Debug.LoopItem(4, $"{i}] {implants[i]}", Indent: indent + 3, Toggle: getDoDebug());
                                if (implants[i].Contains("@body"))
                                {
                                    Debug.CheckYeh(4, $"replaceing [{implants[i]}] with [{addImplant}]", Indent: indent + 4, Toggle: getDoDebug());
                                    found = true;
                                    implants[i] = addImplant;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                Debug.CheckYeh(4, $"existing entry not found, adding [{addImplant}]", Indent: indent + 3, Toggle: getDoDebug());
                                implants.Add(addImplant);
                            }
                            hasImplants.Implants = implants.Join(",");
                        }
                        else
                        {
                            Debug.Entry(4, $"{nameof(hasImplants.Implants)} has single entry, Prepending or replacing...", Indent: indent + 1, Toggle: getDoDebug());
                            Debug.LoopItem(4, hasImplants.Implants, Indent: indent + 2, Toggle: getDoDebug());
                            if (hasImplants.Implants.Contains("@body"))
                            {
                                Debug.CheckYeh(4, $"replaceing [{hasImplants.Implants}] with [{addImplant}]", Indent: indent + 3, Toggle: getDoDebug());
                                hasImplants.Implants = addImplant;
                            }
                            else
                            {
                                Debug.CheckYeh(4, $"existing entry not found, prepending [{addImplant}]", Indent: indent + 3, Toggle: getDoDebug());
                                hasImplants.Implants = $"{addImplant},{hasImplants.Implants}";
                            }
                        }
                        Debug.LoopItem(4, nameof(hasImplants.Implants), hasImplants.Implants, Indent: indent + 2, Toggle: getDoDebug());
                        success = hasImplants.Implants.Contains(addImplant);
                        exoframeObject?.Obliterate();
                    }
                    else
                    {
                        Debug.Entry(4, $"Context allows direct implantation...", Indent: indent + 1, Toggle: getDoDebug());
                        Creature.Body.GetBody().Implant(exoframeObject, Silent: true);
                        success = Creature.Body.HasInstalledCybernetics(exoframe);
                    }

                    Debug.LoopItem(4, $"{nameof(success)}?", $"{success}", Good: success, Indent: indent + 1, Toggle: getDoDebug());

                    if (success)
                    {
                        Debug.Entry(4, $"Performing Color Changes...", Indent: indent + 1, Toggle: getDoDebug());

                        Render render = Creature.Render;
                        string tileColor = render.TileColor.IsNullOrEmpty() ? render.ColorString : render.TileColor; ;
                        render.ColorString = "&" + exoframeCybernetic.AugmentTileDetailColor;
                        render.TileColor = "&" + exoframeCybernetic.AugmentTileDetailColor;

                        if (render.DetailColor == exoframeCybernetic.AugmentTileDetailColor)
                        {
                            render.DetailColor = ColorUtility.FindLastForeground(tileColor)?.ToString() ?? Crayons.GetRandomColor();
                        }

                        if (!NamePrefix.IsNullOrEmpty())
                        {
                            Creature.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
                        }
                    }
                }
                else
                {
                    Debug.Warn(4,
                        nameof(Gigantified),
                        nameof(GigantifyTrueKin),
                        $"{nameof(exoframeObject)} is null or lacks {nameof(CyberneticsGiganticExoframe)} part", 
                        Indent: indent + 1);
                }
            }
            else
            {
                if (Creature == null)
                {
                    Debug.Warn(4,
                        nameof(Gigantified),
                        nameof(GigantifyTrueKin),
                        $"{nameof(Creature)} is null", Indent: 
                        indent + 1);
                }
                if (Creature != null && !Creature.IsCreature)
                {
                    Debug.Warn(4,
                        nameof(Gigantified),
                        nameof(GigantifyTrueKin),
                        $"{nameof(Creature)} is not a Creature", 
                        Indent: indent + 1);
                }
            }

            Debug.Entry(4,
                $"x {nameof(GigantifyTrueKin)}"
                + $"(Creature: {Creature?.DebugName ?? NULL},"
                + $" Tier: {Tier},"
                + $" NamePrefix: {NamePrefix},"
                + $" Context: {Context.Quote()}) *//",
                Indent: indent, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return success;
        }

        public static bool Gigantify(GameObject Creature, int Level = 1, int Stews = 0, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            Debug.Header(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}" +
                $"(Object: {Creature?.DebugName ?? NULL}," +
                $" Level: {Level}," +
                $" Tier: {Tier})",
                Toggle: doDebug);

            bool gigantified = 
                GigantifyMutant(Creature, Level, Stews, NamePrefix, Context) 
             || GigantifyTrueKin(Creature, Tier, NamePrefix, Context);
            
            Debug.Footer(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}" +
                $"(Object: {Creature?.DebugName ?? NULL}," +
                $" Level: {Level}," +
                $" Tier: {Tier})",
                Toggle: doDebug);

            return gigantified;
        }
        public bool Gigantify(GameObject Creature, int Level = 1, int Stews = 0, int Tier = 1, string Context = "")
        {
            return Gigantify(Creature, Level, Stews, Tier, NamePrefix, Context);
        }

        [WishCommand("gigantic", null)]
        public static void Wish(string Blueprint)
        {
            GameObject @object = The.Player;

            if (Blueprint == null)
            {
                WishResult wishResult = WishSearcher.SearchForBlueprint(Blueprint);
                @object = GameObjectFactory.Factory.CreateObject(wishResult.Result, 0, 0, null, null, null, "Wish");
            }
            if (@object != null)
            {
                int Level = ExplodingDie(1, "1d2", Step: 1, Limit: 16, Indent: 2);
                int Stews = ExplodingDie(0, "1d2", Step: 1, Indent: 2);
                int objectTier = (int)Math.Floor(@object.GetBlueprint().Stat("Level") / 5.0);
                int Tier = ExplodingDie(objectTier, "1d3", Step: 1, Limit: 8, Indent: 2);
                Gigantify(@object, Level, Stews, Tier, "gigantic".MaybeColor("gigantic"));
                @object.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);
            }

            The.PlayerCell.getClosestEmptyCell().AddObject(@object);
        }
    }
}
