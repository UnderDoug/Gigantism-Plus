using System;
using System.Collections.Generic;

using Qud.API;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class WallOrNot : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(WallOrNot));

        public string Blueprint;
        public int Chance;

        public WallOrNot()
        {
            Blueprint = "Shale";
            Chance = 100;
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
                E.ReplacementObject = GameObjectFactory.Factory.CreateObject(Blueprint) ?? E.ReplacementObject;
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
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class WallOrNot : IScribedPart
}
