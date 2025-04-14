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
    public class RandomBoulderOrDebris : IScribedPart
    {
        public string TileColor;
        public string DetailColor;

        public List<string> Blueprints = new()
        {
            "SmallBoulder",
            "MediumBoulder",
            "LargeBoulder",
            "Rubble",
        };
        public RandomBoulderOrDebris()
        {
            TileColor = string.Empty;
            DetailColor = string.Empty;
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
                GameObject newObject = GameObjectFactory.Factory.CreateObject(Blueprints.GetRandomElement());
                Render render = newObject?.Render;
                if (render != null)
                {
                    render.TileColor = TileColor ?? render.TileColor;
                    render.ColorString = TileColor ?? render.TileColor;
                    render.DetailColor = DetailColor ?? render.DetailColor;
                }
                E.ReplacementObject = newObject;
                ParentObject.RemovePart(this);
            }
            return base.HandleEvent(E);
        }
    } //!-- public class RandomBoulderOrDebris : IScribedPart
}
