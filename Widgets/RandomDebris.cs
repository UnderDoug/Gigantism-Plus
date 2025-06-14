﻿using System;
using System.Collections.Generic;

using Qud.API;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class RandomDebris : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(RandomDebris));

        public string TileColor;
        public string DetailColor;
        public string ColorLike;
        public bool Invert;

        public Dictionary<string, int> debrisBlueprints;

        public RandomDebris()
        {
            TileColor = string.Empty;
            DetailColor = string.Empty;
            ColorLike = string.Empty;
            Invert = false;
            debrisBlueprints = new()
            {
                { "SmallBoulder", 10 },
                { "MediumBoulder", 20 },
                { "LargeBoulder", 30 },
                { "Rubble", 70 },
            };
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
                GameObject newObject = GameObjectFactory.Factory.CreateObject(debrisBlueprints.Sample());
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
