using System;
using System.Collections.Generic;

using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class BonyProtrusions : BaseDefaultEquipmentMutation
    {
        public GameObject BonyHandsObject;
        public GameObject BonyHeadObject;
        public GameObject BonyFeetObject;

        public string HandsBlueprintName => "Bony Claws";
        public string HeadBlueprintName => "Bony Corona";
        public string FeetBlueprintName => "Bony Feet";

        [NonSerialized]
        protected GameObjectBlueprint _HandsBlueprint;
        protected GameObjectBlueprint _HeadBlueprint;
        protected GameObjectBlueprint _FeetBlueprint;

        public GameObjectBlueprint HandsBlueprint => _HandsBlueprint ??= GameObjectFactory.Factory.GetBlueprint(HandsBlueprintName);
        public GameObjectBlueprint HeadBlueprint => _HeadBlueprint ??= GameObjectFactory.Factory.GetBlueprint(HeadBlueprintName);
        public GameObjectBlueprint FeetBlueprint => _FeetBlueprint ??= GameObjectFactory.Factory.GetBlueprint(FeetBlueprintName);

        public BonyProtrusions()
        {
        }

        public override bool CanLevel() 
        { 
            return false; 
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Bony protusions on your head, hands, and feet make it impossible to wear equipment designed for these limbs.";
        }

        public override string GetLevelText(int Level)
        {
            return "";
        }

        public override void OnRegenerateDefaultEquipment(Body Body)
        {
            if (!GameObject.Validate(ref BonyHandsObject)) BonyHandsObject = GameObject.Create(HandsBlueprint);
            if (!GameObject.Validate(ref BonyHeadObject)) BonyHeadObject = GameObject.Create(HeadBlueprint);
            if (!GameObject.Validate(ref BonyFeetObject)) BonyFeetObject = GameObject.Create(FeetBlueprint);

            List<(BodyPart BodyPart, GameObject BonyObject)> partObjectPairs = new()
            {
                ( RequireRegisteredSlot(Body, HandsBlueprint.GetPartParameter<string>("Armor", "WornOn")),   BonyHandsObject ),
                ( RequireRegisteredSlot(Body, HeadBlueprint.GetPartParameter<string>("Armor", "WornOn")),    BonyHeadObject ),
                ( RequireRegisteredSlot(Body, FeetBlueprint.GetPartParameter<string>("Armor", "WornOn")),    BonyFeetObject ),
            };
            foreach ((BodyPart BodyPart, GameObject BonyObject) in partObjectPairs)
            {
                if (BodyPart != null 
                    && BodyPart.Equipped != BonyObject 
                    && (BodyPart.Equipped == null || BodyPart.ForceUnequip(Silent: true)) 
                    && !ParentObject.ForceEquipObject(BonyObject, BodyPart, Silent: true, 0))
                {
                    MetricsManager.LogError(
                        $"{typeof(BonyProtrusions).Name} " + 
                        $"force equip of {BonyObject?.ShortDisplayNameStripped ?? "null"} " + 
                        $"on {BodyPart?.Name ?? "null"} failed");
                }
            }
            base.OnRegenerateDefaultEquipment(Body);
        }

        public override bool Unmutate(GameObject GO)
        {
            CleanUpMutationEquipment(GO, ref BonyHandsObject);
            CleanUpMutationEquipment(GO, ref BonyHeadObject);
            CleanUpMutationEquipment(GO, ref BonyFeetObject);
            return base.Unmutate(GO);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BonyProtrusions bonyProtrusions = base.DeepCopy(Parent, MapInv) as BonyProtrusions;
            bonyProtrusions.BonyHandsObject = null;
            bonyProtrusions.BonyHeadObject = null;
            bonyProtrusions.BonyFeetObject = null;
            return bonyProtrusions;
        }

    } //!-- public class BonyProtrusions : BaseDefaultEquipmentMutation

}
