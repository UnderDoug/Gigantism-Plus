using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using NUnit.Framework;
using System.Collections.Generic;
using Qud.API;
using static FastNoise;

namespace XRL.World.Parts
{
    [Serializable]
    public class WallOrDebris : IScribedPart
    {

        public int WallChanceIn100;
        public string TileColor;
        public string DetailColor;
        public bool InvertColor;

        public string Wall;
        public Dictionary<string, int> debrisBlueprints;

        private string Blueprint;

        public WallOrDebris()
        {
            WallChanceIn100 = 50;
            Wall = "Shale";
            debrisBlueprints = new()
            {
                { "SmallBoulder", 10 },
                { "MediumBoulder", 20 },
                { "LargeBoulder", 30 },
                { "Rubble", 70 },
            };
            InvertColor = false;
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
                int byChance = RndGP.Next(1, 100);
                if (byChance <= WallChanceIn100)
                {
                    Blueprint = Wall;
                }
                else
                {
                    Blueprint = debrisBlueprints.Sample();

                    if (Blueprint.IsNullOrEmpty())
                    {
                        Debug.Entry(4, 
                            $"WARN: {typeof(WallOrDebris).Name} Failed to get Debris from weighted list. " + 
                            $"Blueprint set by fallback to Wall \n{Wall}\n", 
                            Indent: 0);
                        Blueprint = Wall;
                    }
                }

                GameObject newObject = GameObjectFactory.Factory.CreateObject(Blueprint);

                if (!Blueprint.Is(Wall))
                {
                    GameObjectBlueprint wallBlueprint = GameObjectFactory.Factory.GetBlueprintIfExists(Wall);
                    if (wallBlueprint != null && wallBlueprint.Parts.ContainsKey("Render"))
                    {
                        GamePartBlueprint renderBlueprint = wallBlueprint.Parts["Render"];

                        if (renderBlueprint.TryGetParameter(InvertColor ? "TileColor" : "DetailColor", out string tileColor))
                            TileColor = $"&{tileColor}";
                        if (renderBlueprint.TryGetParameter(InvertColor ? "DetailColor" : "TileColor", out string detailColor))
                            DetailColor = detailColor.Replace("&", "");
                    }
                    Render render = newObject?.Render;
                    if (render != null)
                    {
                        render.ColorString = TileColor ?? render.TileColor;
                        render.TileColor = TileColor ?? render.TileColor;
                        render.DetailColor = DetailColor ?? render.DetailColor;
                    }
                }
                E.ReplacementObject = newObject;
                ParentObject.RemovePart(this);
            }
            return base.HandleEvent(E);
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
                string thing = Blueprint  ?? "Wall or Debris";
                Debug.Entry(4, $"WARN: {typeof(WallOrDebris).Name} Failed to create {thing}", Indent: 0);
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class WallOrDebris : IScribedPart
}
