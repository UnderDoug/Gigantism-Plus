using System;
using System.Collections.Generic;
using static FastNoise;

using Qud.API;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class WallOrDebris : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(WallOrDebris));

        public int WallChance;
        public bool SmallBoulders;
        public string TileColor;
        public string DetailColor;
        public bool InvertColor;

        public string Wall;
        public Dictionary<string, int> debrisBlueprints;

        private string Blueprint;

        public WallOrDebris()
        {
            WallChance = 50;
            SmallBoulders = true;
            Wall = "Shale";
            debrisBlueprints = new()
            {
                { "SmallBoulder", SmallBoulders ? 10 : 0 },
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
                string debugHeader = 
                    $"[{ParentObject.ID}:{ParentObject.Blueprint}] -> " + 
                    $"{nameof(WallOrDebris)}." +
                    $"{nameof(HandleEvent)}({nameof(ObjectCreatedEvent)} E)";

                if (WallChance.in100())
                {
                    Blueprint = Wall;
                    Debug.Entry(4, $"{debugHeader} resolved into {Blueprint}", Indent: 0, Toggle: doDebug);
                }
                else
                {
                    string debrisBlueprint = debrisBlueprints.Sample();
                    Blueprint = debrisBlueprint;

                    if (Blueprint.IsNullOrEmpty())
                    {
                        Debug.Warn(4, 
                            $"{nameof(WallOrDebris)}",
                            $"{nameof(HandleEvent)}({nameof(ObjectCreatedEvent)})",
                            $"Failed to get Debris from weighted list. " + 
                            $"Blueprint set by fallback to Wall {Wall.Quote()}", 
                            Indent: 0);
                        Blueprint = Wall;
                    }
                    Debug.Entry(4, $"{debugHeader} resolved into {debrisBlueprint}, {Blueprint} is what we got", 
                        Indent: 0, Toggle: doDebug);
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
                string thing = Blueprint ?? "Wall or Debris";
                Debug.Warn(4,
                    $"{nameof(WallOrDebris)}",
                    $"{nameof(HandleEvent)}({nameof(ObjectCreatedEvent)})",
                    $"Failed to create {thing}",
                    Indent: 0);
                
                if (ParentObject.Obliterate())
                {
                    return false;
                }
            }
            return base.FireEvent(E);
        }
    } //!-- public class WallOrDebris : IScribedPart
}
