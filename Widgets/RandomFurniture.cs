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
        public string Quality;
        public string Utility;

        public RandomFurniture()
        {
            ChanceIn100 = 100;
            Gigantic = false;
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
            Debug.Entry(4,
                $"{typeof(RandomFurniture).Name}." + 
                $"{nameof(IsDesiredFurniture)}" + 
                $"(Blueprint: {Blueprint.Name} [" + 
                $"Gigantic: {Gigantic}, " + 
                $"Class: \"{Class}\", " + 
                $"Quality: \"{Quality}\", " + 
                $"Utility: \"{Utility}\"])",
                Indent: 0);

            bool haveClass = !Class.IsNullOrEmpty();
            bool haveQuality = !Quality.IsNullOrEmpty();
            bool haveUtility = !Utility.IsNullOrEmpty();

            Debug.LoopItem(4, $"Gigantic", Indent: 0,
                Good: Gigantic);
            Debug.LoopItem(4, $"haveClass", Indent: 0,
                Good: haveClass);
            Debug.LoopItem(4, $"haveQuality", Indent: 0,
                Good: haveQuality);
            Debug.LoopItem(4, $"haveUtility", Indent: 0,
                Good: haveUtility);

            if (!Blueprint.InheritsFrom("Furniture"))
                return false;
            Debug.CheckYeh(4, $"Is Furniture", Indent: 0);

            if (Blueprint.HasTag("BaseObject"))
                return false;
            Debug.CheckYeh(4, $"Not BaseObject", Indent: 0);

            if (Gigantic != Blueprint.Parts.ContainsKey("ModGigantic"))
                return false;
            Debug.CheckYeh(4, $"Gigantic matches HasPart ModGigantic", Indent: 0);

            if (haveClass)
            {
                if (!Blueprint.TryGetTag($"Class", out string @class) || !@class.Is($"{Class.ToLower()}"))
                    return false;
                Debug.CheckYeh(4, $"Class matches {Class.ToLower()}", Indent: 0);
            }

            if (haveQuality)
            {
                if (!Blueprint.TryGetTag($"Quality", out string quality) || !quality.Is($"{Quality.ToLower()}"))
                    return false;
                Debug.CheckYeh(4, $"Quality matches {Quality.ToLower()}", Indent: 0);
            }

            if (haveUtility)
            {
                if (!Blueprint.TryGetTag($"Utility", out string utility) || !utility.Is($"{Utility.ToLower()}"))
                    return false;
                Debug.CheckYeh(4, $"Utility matches {Utility.ToLower()}", Indent: 0);
            }
            else
            {
                if (Blueprint.TryGetTag($"Utility", out string utility) && utility.Is($"medical".ToLower()))
                    return false;
                Debug.CheckYeh(4, $"Utility not medical when not specified", Indent: 0);
            }
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
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class RandomFurniture : IScribedPart
}
