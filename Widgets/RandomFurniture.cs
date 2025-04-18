using Qud.API;

using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class RandomFurniture : IScribedPart
    {
        public int ChanceIn100;
        public bool Gigantic;

        public string Class;
        public List<string> ExcludedClasses;

        public string Quality;
        public List<string> ExcludedQualities;

        public string Utility;
        public List<string> ExcludedUtilities;

        public RandomFurniture()
        {
            ChanceIn100 = 100;
            Gigantic = false;
            ExcludedClasses = new();
            ExcludedQualities = new();
            ExcludedUtilities = new();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            if (E.Object == ParentObject && RndGP.Next(1, 100) <= ChanceIn100)
            {
                string @class = string.Empty;
                ExcludedClasses = new();
                if (!Class.IsNullOrEmpty())
                {
                    if (!Class.Contains(","))
                    {
                        if (Class.Substring(0, 1) == "!")
                        {
                            ExcludedClasses.Add(Class.Substring(1));
                        }
                        else
                        {
                            @class = Class;
                        }
                    }
                    else
                    {
                        string[] classes = Class.Split(',');
                        foreach (string entry in classes)
                        {
                            if (entry.Substring(0, 1) == "!")
                            {
                                ExcludedClasses.Add(entry.Substring(1));
                            }
                            else
                            {
                                @class ??= entry;
                            }
                        }
                    }
                }
                Class = @class;

                string quality = string.Empty;
                ExcludedQualities = new();
                if (!Quality.IsNullOrEmpty())
                {
                    if (!Quality.Contains(","))
                    {
                        if (Quality.Substring(0, 1) == "!")
                        {
                            ExcludedQualities.Add(Quality.Substring(1));
                        }
                        else
                        {
                            quality = Quality;
                        }
                    }
                    else
                    {
                        string[] qualities = Quality.Split(',');
                        foreach (string entry in qualities)
                        {
                            if (entry.Substring(0, 1) == "!")
                            {
                                ExcludedQualities.Add(entry.Substring(1));
                            }
                            else
                            {
                                quality ??= entry;
                            }
                        }
                    }
                }
                Quality = quality;

                string utility = string.Empty;
                ExcludedUtilities = new();
                if (!Utility.IsNullOrEmpty())
                {
                    if (!Utility.Contains(","))
                    {
                        if (Utility.Substring(0, 1) == "!")
                        {
                            ExcludedUtilities.Add(Utility.Substring(1));
                        }
                        else
                        {
                            utility = Utility;
                        }
                    }
                    else
                    {
                        string[] utilities = Utility.Split(',');
                        foreach (string entry in utilities)
                        {
                            if (entry.Substring(0, 1) == "!")
                            {
                                ExcludedUtilities.Add(entry.Substring(1));
                            }
                            else
                            {
                                utility ??= entry;
                            }
                        }
                    }
                }
                Utility = utility;

                GameObjectBlueprint furnitureBlueprint = EncountersAPI.GetAnObjectBlueprintModel((GameObjectBlueprint blueprint)
                => IsDesiredFurniture(blueprint));

                GameObject furnitureObject = GameObjectFactory.Factory.CreateObject(furnitureBlueprint);
                if (Gigantic && furnitureObject != null && !furnitureObject.HasPart<ModGigantic>()) furnitureObject.ApplyModification("ModGigantic", Creation: true);
                if (furnitureObject != null) E.ReplacementObject = furnitureObject;

                ParentObject.RemovePart(this);
            }
            return base.HandleEvent(E);
        }

        public bool IsDesiredFurniture(GameObjectBlueprint Blueprint)
        {
            /*Debug.Entry(4,
                $"{typeof(RandomFurniture).Name}." + 
                $"{nameof(IsDesiredFurniture)}" + 
                $"(Blueprint: {Blueprint.Name} [" + 
                $"Gigantic: {Gigantic}, " + 
                $"Class: \"{Class}\", " + 
                $"Quality: \"{Quality}\", " + 
                $"Utility: \"{Utility}\"])",
                Indent: 0);*/

            bool haveClass = !Class.IsNullOrEmpty();
            bool haveQuality = !Quality.IsNullOrEmpty();
            bool haveUtility = !Utility.IsNullOrEmpty();
            /*
            Debug.LoopItem(4, $"Gigantic", Indent: 0,
                Good: Gigantic);
            Debug.LoopItem(4, $"haveClass", Indent: 0,
                Good: haveClass);
            Debug.LoopItem(4, $"haveQuality", Indent: 0,
                Good: haveQuality);
            Debug.LoopItem(4, $"haveUtility", Indent: 0,
                Good: haveUtility);
            */
            if (!Blueprint.InheritsFrom("Furniture"))
                return false;
            //Debug.CheckYeh(4, $"Is Furniture", Indent: 0);

            if (Blueprint.HasTag("BaseObject"))
                return false;
            //Debug.CheckYeh(4, $"Not BaseObject", Indent: 0);

            if (Gigantic != Blueprint.Parts.ContainsKey("ModGigantic"))
                return false;
            //Debug.CheckYeh(4, $"Gigantic matches HasPart ModGigantic", Indent: 0);
            Blueprint.TryGetTag($"Class", out string @class);
            if (haveClass)
            {
                if (!@class.Is($"{Class.ToLower()}"))
                    return false;
                //Debug.CheckYeh(4, $"Class matches {Class.ToLower()}", Indent: 0);
            }
            if (ExcludedClasses.Contains(@class))
            {
                return false;
            }

            Blueprint.TryGetTag($"Quality", out string quality);
            if (haveQuality)
            {
                if (!quality.Is($"{Quality.ToLower()}"))
                    return false;
                //Debug.CheckYeh(4, $"Quality matches {Quality.ToLower()}", Indent: 0);
            }
            if (ExcludedQualities.Contains(quality))
            {
                return false;
            }

            Blueprint.TryGetTag($"Utility", out string utility);
            if (haveUtility)
            {
                if (!utility.Is($"{Utility.ToLower()}"))
                    return false;
                //Debug.CheckYeh(4, $"Utility matches {Utility.ToLower()}", Indent: 0);
            }
            if (ExcludedUtilities.Contains(utility))
            {
                return false;
            }

            //Debug.CheckYeh(4, $"Give us this blueprint, please.", Indent: 0);
            return true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("EnteredCell");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "EnteredCell" && ParentObject.CurrentZone != null)
            {
                string thing = Class ?? "Furniture";
                //Debug.Entry(4, $"WARN: {typeof(RandomFurniture).Name} Failed to create {thing}", Indent: 0);
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class RandomFurniture : IScribedPart
}
