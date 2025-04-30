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
        public int Chance;
        public bool? Gigantic;

        public string Class;
        public Dictionary<string, bool> Classes;

        public string Quality;
        public Dictionary<string, bool> Qualities;

        public string Utility;
        public Dictionary<string, bool> Utilities;

        public GameObjectBlueprint Blueprint;
        public Dictionary<string,GameObjectBlueprint> Blueprints;

        public RandomFurniture()
        {
            Chance = 100;
            Classes = new();
            Qualities = new();
            Utilities = new();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            if (E.Object == ParentObject && Chance.in100())
            {
                Debug.Header(4,
                    $"[{ParentObject.ID}:{ParentObject.Blueprint}] -> {typeof(RandomFurniture).Name}",
                    $"{nameof(HandleEvent)}({typeof(ObjectCreatedEvent).Name} E) [" +
                    $"Gigantic: {(Gigantic != null ? Gigantic : "null")}, " +
                    $"Class: \"{Class}\", " +
                    $"Quality: \"{Quality}\", " +
                    $"Utility: \"{Utility}\"]");

                Class.MakeIncludeExclude(Classes);
                Quality.MakeIncludeExclude(Qualities);
                Utility.MakeIncludeExclude(Utilities);
                
                Blueprint = EncountersAPI.GetAnObjectBlueprintModel((GameObjectBlueprint blueprint)
                => IsDesiredFurniture(blueprint));

                GameObject furnitureObject = GameObjectFactory.Factory.CreateObject(Blueprint);
                if (furnitureObject != null)
                {
                    E.ReplacementObject = furnitureObject;
                    ParentObject.RemovePart(this);
                }
            }
            return base.HandleEvent(E);
        }

        public bool IsDesiredFurniture(GameObjectBlueprint Blueprint)
        {
            Debug.Entry(4,
                $"[{ParentObject.ID}:{ParentObject.Blueprint}]" + 
                $"({Blueprint.Name}).{nameof(IsDesiredFurniture)}() [" + 
                $"Gigantic: {Gigantic}, " + 
                $"Class: \"{Class}\", " + 
                $"Quality: \"{Quality}\", " + 
                $"Utility: \"{Utility}\"]",
                Indent: 0);

            if (!Blueprint.InheritsFrom("Furniture"))
                return false;
            Debug.CheckYeh(4, $"Is Furniture", Indent: 0);

            if (Blueprint.HasTag("BaseObject"))
                return false;
            Debug.CheckYeh(4, $"Not BaseObject", Indent: 0);

            if (Blueprint.HasOwner())
                return false;
            Debug.CheckYeh(4, $"Doesn't have an owner", Indent: 0);

            if (Gigantic != null)
            {
                if (Gigantic != Blueprint.Parts.ContainsKey("ModGigantic"))
                    return false;
                Debug.CheckYeh(4, $"Gigantic matches HasPart ModGigantic", Indent: 0);
            }

            if (!Blueprint.TagIsIncludedOrNotExcluded("Class", Classes))
            {
                Debug.CheckNah(4, $"Class filter failed", Indent: 0);
                return false;
            }
            Debug.CheckYeh(4, $"Class filter passed", Indent: 0);

            if (!Blueprint.TagIsIncludedOrNotExcluded("Quality", Qualities))
            {
                Debug.CheckNah(4, $"Quality filter failed", Indent: 0);
                return false;
            }
            Debug.CheckYeh(4, $"Quality filter passed", Indent: 0);

            if (!Blueprint.TagIsIncludedOrNotExcluded("Utility", Utilities))
            {
                Debug.CheckNah(4, $"Utility filter failed", Indent: 0);
                return false;
            }
            Debug.CheckYeh(4, $"Utility filter passed", Indent: 0);

            Debug.CheckYeh(4, $"Give us this blueprint, please.", Indent: 0);
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
                Debug.Entry(4, $"WARN: {typeof(RandomFurniture).Name} Failed to create {thing}", Indent: 0);
                if (ParentObject.InheritsFrom("Widget"))
                    ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class RandomFurniture : IScribedPart
}
