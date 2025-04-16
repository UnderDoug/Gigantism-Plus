using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using NUnit.Framework;
using System.Collections.Generic;
using Qud.API;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts
{
    [Serializable]
    public class RandomFurniture : IScribedPart
    {
        public int ChanceIn100;
        public bool Gigantic;
        public string Class;
        public string Tags;

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
                /*
                GameObjectBlueprint furnitureBlueprint = EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                => blueprint.DescendsFrom("Furniture")
                && !blueprint.HasTag("BaseObject")
                && !(Class.IsNullOrEmpty() || blueprint.HasTag($"{Class.ToLower()}"))
                && !blueprint.HasTag("NoLibrarian")
                && !blueprint.HasTagOrProperty("StartInLiquid")
                && !blueprint.DescendsFrom("BaseTrueKin")
                && !(blueprint.TryGetTag("Species", out string species) && species.Is("mecha")));

                furnitureBlueprint.GetxTag
                GameObject furnitureObject = GameObjectFactory.Factory.CreateObject(furnitureBlueprint);
                if (Gigantic && furnitureObject != null && !furnitureObject.HasPart<ModGigantic>()) furnitureObject.ApplyModification("ModGigantic", Creation: true);
                E.ReplacementObject = furnitureObject ?? E.ReplacementObject;
                ParentObject.RemovePart(this);
                */
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
                Debug.Entry(4, $"WARN: {typeof(RandomFurniture).Name} Failed to create {}", Indent: 0);
                ParentObject.Obliterate();
            }
            return base.FireEvent(E);
        }
    } //!-- public class RandomFurniture : IScribedPart
}
