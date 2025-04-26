using System;
using System.Collections.Generic;
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
using System.Xml;
using System.Linq;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
    public class Gigantified : IObjectBuilder
    {
        public string NamePrefix;

        public Gigantified() 
        { 
            Initialize();
        }
        public Gigantified(string NamePrefix) 
        {
            this.NamePrefix = NamePrefix;
        }

        public override void Initialize()
        {
            NamePrefix = "gigantic".MaybeColor("gigantic");
        }

        public static string GetNamePrefix()
        {
            Gigantified gigantified = new();
            gigantified.Initialize();
            return gigantified.NamePrefix;
        }

        public override void Apply(GameObject Object, string Context = "")
        {
            int Level = ExplodingDie(1, "1d3", Step: 2, Limit: 16, Indent: 2);
            int Stews = ExplodingDie(0, "1d2", Step: 1, Indent: 2);
            int ObjectTier = (int)Math.Floor(Object.GetBlueprint().Stat("Level") / 5.0);
            int Tier = ExplodingDie(ObjectTier, "1d3", Step: 1, Limit: 8, Indent: 2);
            Gigantify(Object, Level, Stews, Tier, NamePrefix, Context);
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

            mutations.GetMutation(gigantismPlus.GetMutationClass()).Level += Level - 1;

            int startingStews = Stews ?? Stat.Roll("2d2");
            if (Creature.TryGetPart(out StewBelly stewBelly))
            {
                stewBelly.Stews = startingStews;
            }
            else
            {
                Creature.SetIntProperty(GNT_START_STEWS_PROPLABEL, startingStews);
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
                    if (!1.ChanceIn(100000))
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

            GameObject exoframeObject = GameObjectFactory.Factory.CreateSampleObject(exoframe);
            CyberneticsGiganticExoframe exoframeCybernetic = exoframeObject.GetPart<CyberneticsGiganticExoframe>();
            exoframeCybernetic.MapAugmentAdjustments();
            NamePrefix = exoframeCybernetic.GetNaturalEquipmentColoredAdjective();
            exoframeObject.Obliterate();

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

            return hasImplants.Implants.Contains(addImplant);
        }

        public static bool Gigantify(GameObject Creature, int Level = 1, int Stews = 0, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            Debug.Header(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Creature.DebugName}, Level: {Level}, Tier: {Tier})");

            bool gigantified = 
                GigantifyMutant(Creature, Level, Stews, NamePrefix, Context) 
             || GigantifyTrueKin(Creature, Tier, NamePrefix, Context);
            
            Debug.Footer(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Creature.DebugName}, Level: {Level}, Tier: {Tier})");

            return gigantified;
        }

        [WishCommand("gigantic", null)]
        public static void Wish(string Blueprint)
        {
            WishResult wishResult = WishSearcher.SearchForBlueprint(Blueprint);
            GameObject @object;
            if (GameObjectFactory.Factory.Blueprints.TryGetValue(wishResult.Result, out GameObjectBlueprint blueprint))
            {
                GamePartBlueprint gigantifiedPartBlueprint = new("XRL.World.ObjectBuilders", nameof(Gigantified))
                {
                    Name = nameof(Gigantified),
                    ChanceOneIn = 1
                };
                blueprint.Builders[gigantifiedPartBlueprint.Name] = gigantifiedPartBlueprint;
                @object = GameObjectFactory.Factory.CreateObject(blueprint, 0, 0, null, null, null, "Wish");
            }
            else
            {
                @object = GameObjectFactory.Factory.CreateObject(wishResult.Result, 0, 0, null, null, null, "Wish");
                
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
