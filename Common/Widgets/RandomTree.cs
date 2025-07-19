using System;
using System.Collections.Generic;

using Qud.API;

using XRL.Rules;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class RandomTree : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(RandomTree));

        public bool Nearby;
        public int Count;
        public bool Persist;
        public int ChancePer;

        public RandomTree()
        {
            Nearby = false;
            Count = 0;
            Persist = false;
            ChancePer = 100;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (!Nearby && ID == ObjectCreatedEvent.ID);
        }
        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            if (E.Object == ParentObject)
            {
                E.ReplacementObject = GameObjectFactory.Factory.CreateObject("Dogthorn Tree");
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
                Cell cell = ParentObject.CurrentCell;
                List<Cell> cells = cell.GetAdjacentCells();
                cells.Add(cell);
                int count = Count;
                while(count > 0 && !cells.IsNullOrEmpty())
                {
                    bool ByChance = Stat.Roll(1, 100) <= ChancePer;
                    cell = cells.DrawRandomToken();
                    if (cell.IsEmpty() && ByChance)
                    {
                        cell.AddObject(GameObjectFactory.Factory.CreateObject("Dogthorn Tree"));
                        count--;
                    }
                    else if (!Persist && ByChance)
                    {
                        count--;
                    }
                }
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class RandomTree : IScribedPart
}
