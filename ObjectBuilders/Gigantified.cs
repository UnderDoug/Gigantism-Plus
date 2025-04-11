using System;
using System.Collections.Generic;

using ConsoleLib.Console;

using XRL.Rules;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using XRL.World.Skills.Cooking;
using XRL.Core;

namespace XRL.World.ObjectBuilders
{
    [Serializable]
    [HasWishCommand]
    public class Gigantified : IObjectBuilder
    {
        public string NamePrefix;
        public string DieRoll;

        public override void Initialize()
        {
            NamePrefix = "gigantic".MaybeColor("gigantic");
            DieRoll = "1d3";
        }

        public static string GetNamePrefix()
        {
            Gigantified gigantified = new();
            gigantified.Initialize();
            return gigantified.NamePrefix;
        }

        public override void Apply(GameObject Object, string Context = "")
        {
            int Level = ExplodingDie(1, DieRoll, Step: 2, Limit: 16, Indent: 2);
            int objectTier = (int)Math.Floor(Object.GetBlueprint().Stat("Level") / 5.0);
            int Tier = ExplodingDie(objectTier, DieRoll, Step: 1, Limit: 8, Indent: 2);
            Gigantify(Object, Level, Tier, NamePrefix, Context);
        }

        public static void Gigantify(GameObject Object, int Level = 1, int Tier = 1, string NamePrefix = "", string Context = "")
        {
            Debug.Header(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Object.DebugName}, Level: {Level}, Tier: {Tier})"
                );
            if (!Object.IsTrueKin()) // not true kin
            {
                NamePrefix = NamePrefix == "gigantic" ? NamePrefix.MaybeColor("gigantic") : NamePrefix;

                Mutations mutations = Object.GetPart<Mutations>();
                GigantismPlus gigantismPlus = new();

                if (mutations.HasMutation(gigantismPlus.GetMutationClass()))
                {
                    mutations.GetMutation(gigantismPlus.GetMutationClass()).Level += Level;
                    return;
                }

                mutations.AddMutation(new GigantismPlus(), Level);

                if (!NamePrefix.IsNullOrEmpty())
                {
                    Object.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
                }
            }
            else // is true kin
            {
                SortedDictionary<int,string> exoframeList = new();
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
                foreach (KeyValuePair<int, string> entry in exoframeList)
                {
                    if (exoframeList.Count == 1 || exoframe == string.Empty) exoframe = entry.Value;
                    if (Tier >= entry.Key) exoframe = entry.Value;
                }

                string addImplant = exoframe + "@body";
                Object.RequirePart<CyberneticsHasImplants>();
                CyberneticsHasImplants hasImplants = Object.GetPart<CyberneticsHasImplants>();
                hasImplants.Implants = hasImplants.Implants == "" ? addImplant : addImplant+",";

                GameObject exoframeObject = GameObjectFactory.create(exoframe);
                CyberneticsGiganticExoframe exoframeCybernetic = exoframeObject.GetPart<CyberneticsGiganticExoframe>();

                NamePrefix = exoframeCybernetic.GetNaturalEquipmentColoredAdjective();

                Render render = Object.Render;
                string tileColor = render.TileColor.IsNullOrEmpty() ? render.ColorString : render.TileColor; ;
                render.ColorString = "&" + exoframeCybernetic.AugmentTileDetailColor;
                render.TileColor = "&" + exoframeCybernetic.AugmentTileDetailColor;
                if (render.DetailColor == exoframeCybernetic.AugmentTileDetailColor)
                {
                        render.DetailColor = ColorUtility.FindLastForeground(tileColor)?.ToString() ?? Crayons.GetRandomColor();
                    }
                if (!NamePrefix.IsNullOrEmpty())
                {
                    Object.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
                }
            }
            Debug.Footer(4, 
                $"{nameof(Gigantified)}", 
                $"{nameof(Gigantify)}(Object: {Object.DebugName}, Level: {Level}, Tier: {Tier})"
                );
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
                Gigantified gigantified = new();
                gigantified.Initialize();

                int Level = ExplodingDie(1, "1d2", Step: 1, Limit: 16, Indent: 2);
                int objectTier = (int)Math.Floor(@object.GetBlueprint().Stat("Level") / 5.0);
                int Tier = ExplodingDie(objectTier, gigantified.DieRoll, Step: 1, Limit: 8, Indent: 2);
                Gigantify(@object, Level, Tier, "gigantic".MaybeColor("gigantic"));
                @object.GigantifyInventory(EnableGiganticNPCGear, EnableGiganticNPCGear_Grenades);
            }

            The.PlayerCell.getClosestEmptyCell().AddObject(@object);
        }
    }
}
