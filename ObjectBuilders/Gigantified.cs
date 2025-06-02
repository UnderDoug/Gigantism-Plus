using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;

using XRL.Core;
using XRL.Rules;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Skills.Cooking;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
    public class Gigantified : IObjectBuilder
    {
        private static bool doDebug => getClassDoDebug(nameof(Gigantified));

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
            if (Creature == null || !Creature.IsCreature || Creature.IsTrueKin()) return false;
            NamePrefix = NamePrefix == "gigantic" ? NamePrefix.MaybeColor("gigantic") : NamePrefix;

            Mutations mutations = Creature.GetPart<Mutations>();
            GigantismPlus gigantismPlus = new();

            if (!mutations.HasMutation(gigantismPlus.GetMutationClass()))
            {
                mutations.AddMutation(new GigantismPlus());
            }
            gigantismPlus = mutations.GetMutation(gigantismPlus.GetMutationClass()) as GigantismPlus;

            int level = Level > 10 ? 10 : Level;
            int rapidAdvances = Level > 10 ? Level - 10 : 0;
            gigantismPlus.BaseLevel = level;
            if (rapidAdvances > 0)
            {
                gigantismPlus.SetRapidLevelAmount(rapidAdvances.RapidAdvancementCeiling(MinAdvances: 1), Sync: true);
            }

            int startingStews = Stews ?? Stat.Roll("2d2");
            Creature.SetIntProperty(GNT_START_STEWS_PROPLABEL, startingStews);
            if (Creature.TryGetPart(out StewBelly stewBelly))
            {
                stewBelly.ProcessStartingStews();
            }

            if (!NamePrefix.IsNullOrEmpty() && Context != "Hero" && Context != "Unique")
            {
                Creature.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
            }
            return mutations.HasMutation(gigantismPlus.GetMutationClass());
        }

        public static bool GigantifyTrueKin(GameObject Creature, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            if (Creature == null || !Creature.IsCreature || !Creature.IsTrueKin()) return false;

            SortedDictionary<int, string> exoframeList = new();
            foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.GetBlueprintsInheritingFrom("BaseGiganticExoframe"))
            {
                string exoframeBlueprint = blueprint.Name;
                int exoframeTier = blueprint.Tier;
                if (exoframeBlueprint == "THEGIGANTICEXOFRAME")
                {
                    if (!1.ChanceIn(100000) && Context != "GODKING")
                    {
                        continue;
                    }
                    else
                    {
                        exoframeList = new()
                            {
                                { exoframeTier, exoframeBlueprint }
                            };
                        break;
                    }
                }
                exoframeList.Add(exoframeTier, exoframeBlueprint);
            }

            string exoframe = string.Empty;
            foreach ((int exoframeTier, string exoframeBlueprint) in exoframeList)
            {
                if (exoframeList.Count == 1 || exoframe == string.Empty)
                    exoframe = exoframeBlueprint;
                if (Tier >= exoframeTier)
                    exoframe = exoframeBlueprint;
            }

            GameObject exoframeObject = GameObjectFactory.Factory.CreateObject(exoframe);
            CyberneticsGiganticExoframe exoframeCybernetic = exoframeObject.GetPart<CyberneticsGiganticExoframe>();
            exoframeCybernetic.MapAugmentAdjustments();

            NamePrefix = exoframeCybernetic.GetNaturalEquipmentColoredAdjective();

            bool success = false;
            if (Context == "Initialization" || Context == "GameStarted" || Context == "Wish" || Context == "Creation" || Context == "Sample")
            {
                string addImplant = exoframe + "@body";
                Creature.RequirePart<CyberneticsHasImplants>();
                CyberneticsHasImplants hasImplants = Creature.GetPart<CyberneticsHasImplants>();

                if (hasImplants.Implants.IsNullOrEmpty())
                {
                    hasImplants.Implants = addImplant;
                }
                else if (hasImplants.Implants.Contains(","))
                {
                    List<string> implants = new(hasImplants.Implants.Split(","));
                    bool found = false;
                    for (int i = 0; i < implants.Count(); i++)
                    {
                        if (implants[i].Contains("@body"))
                        {
                            found = true;
                            implants[i] = addImplant;
                        }
                    }
                    if (!found)
                    {
                        implants.Add(addImplant);
                    }
                    hasImplants.Implants = implants.Join(",");
                }
                else
                {
                    hasImplants.Implants = $"{addImplant},{hasImplants.Implants}";
                }
                exoframeObject.Obliterate();
                success = hasImplants.Implants.Contains(addImplant);
            }
            else
            {
                Creature.Body.GetBody().Implant(exoframeObject, Silent: true);
                success = Creature.Body.HasInstalledCybernetics(exoframe);
            }

            if (success)
            {
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
            
            return success;
        }

        public static bool Gigantify(GameObject Creature, int Level = 1, int Stews = 0, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            Debug.Header(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Creature.DebugName}, Level: {Level}, Tier: {Tier})",
                Toggle: doDebug);

            bool gigantified = 
                GigantifyMutant(Creature, Level, Stews, NamePrefix, Context) 
             || GigantifyTrueKin(Creature, Tier, NamePrefix, Context);
            
            Debug.Footer(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Creature.DebugName}, Level: {Level}, Tier: {Tier})",
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
