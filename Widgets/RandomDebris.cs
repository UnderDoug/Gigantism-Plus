using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using NUnit.Framework;
using System.Collections.Generic;
using Qud.API;

namespace XRL.World.Parts
{
    [Serializable]
    public class RandomDebris : IScribedPart
    {
        public string TileColor;
        public string DetailColor;
        public string ColorLike;
        public bool Invert;

        public List<string> Blueprints = new()
        {
            "SmallBoulder",
            "MediumBoulder",
            "LargeBoulder",
            "Rubble",
            "Rubble",
        };
        public RandomDebris()
        {
            TileColor = string.Empty;
            DetailColor = string.Empty;
            ColorLike = string.Empty;
            Invert = false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            if (E.Object == ParentObject)
            {
                if (!ColorLike.Is(string.Empty))
                {
                    GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprintIfExists(ColorLike);
                    if (blueprint != null && blueprint.Parts.ContainsKey("Render"))
                    {
                        GamePartBlueprint renderBlueprint = blueprint.Parts["Render"];

                        // These are swapped, but that's a typical difference between walls and other objects
                        if (renderBlueprint.TryGetParameter(Invert ? "TileColor" : "DetailColor", out string tileColor))
                            TileColor = $"&{tileColor}";
                        if (renderBlueprint.TryGetParameter(Invert ? "DetailColor" : "TileColor", out string detailColor))
                            DetailColor = detailColor.Replace("&","");
                    }
                }
                GameObject newObject = GameObjectFactory.Factory.CreateObject(Blueprints.GetRandomElement());
                Render render = newObject?.Render;
                if (render != null)
                {
                    render.ColorString = TileColor ?? render.TileColor;
                    render.TileColor = TileColor ?? render.TileColor;
                    render.DetailColor = DetailColor ?? render.DetailColor;
                }
                E.ReplacementObject = newObject;
                ParentObject.RemovePart(this);
            }
            return base.HandleEvent(E);
        }
    } //!-- public class RandomDebris : IScribedPart
}
