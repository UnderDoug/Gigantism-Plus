using System;
using System.Collections.Generic;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Secrets;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsGiganticExoframe : IPart
    {
        [NonSerialized]
        private GameObject _User;
        public GameObject User => _User ??= ParentObject.Implantee;

        public GameObject ImplantObject;
        
        // XML Set Properties.
        public string Model = "Alpha";
        public string AugmentAdjectiveColor = "b";
        public string AugmentTile = "GiganticManipulator.png";
        public string AugmentTileColorString = "&c";
        public string AugmentTileDetailColor = "b";
        public string AugmentedSwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fullerite_swing";
        public string AugmentedBlockSound = "Sounds/Melee/multiUseBlock/sfx_melee_fullerite_blocked";
        public int JumpDistanceBonus = 0;
        public double StunningForceLevelFactor = 0.5;

        public string GetShortAugmentAdjective(bool Pretty = true)
        {
            return ("augmented").MaybeColor(AugmentAdjectiveColor, Pretty);
        }
        public string GetAugmentAdjective(bool Pretty = true)
        {
            return ($"E{ ("F").MaybeColor("c", Pretty) }-{GetShortAugmentAdjective(Pretty)}").MaybeColor("Y", Pretty);
        }

        public GameObject _ManipulatorObject;

        public string ManipulatorBlueprintName
        {
            get
            {
                return "GiganticExoframeManipulator" + Model;
            }

        }

        [NonSerialized]
        protected GameObjectBlueprint _ManipulatorBlueprint;

        public GameObjectBlueprint ManipulatorBlueprint
        {
            get
            {
                _ManipulatorBlueprint ??= GameObjectFactory.Factory.GetBlueprint(ManipulatorBlueprintName);
                return _ManipulatorBlueprint;
            }
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            CyberneticsGiganticExoframe exoframe = base.DeepCopy(Parent, MapInv) as CyberneticsGiganticExoframe;
            exoframe._ManipulatorObject = null;
            exoframe._User = null;
            exoframe.ImplantObject = null;
            return exoframe;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
                || ID == CanEnterInteriorEvent.ID;
        }

        public virtual void OnImplanted(GameObject Object)
        {
            Debug.Entry(2, $"* public virtual void OnImplanted({Object.DisplayName})");
            
            _User = Object;

            Become(Object, Model, ImplantObject);

            Debug.Entry(2, $"x public virtual void OnImplanted({Object.DisplayName}) ]//");
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Object)
        {
            Debug.Entry(3, $"* public virtual void OnUnimplanted({Object.DisplayName})");

            CheckEquipment(Object, Object.Body);

            
            Unbecome(Object, Model, ImplantObject);
            Debug.Entry(3, $"x public virtual void OnUnimplanted({Object.DisplayName}) ]//");
        } //!--- public override void OnUnimplanted(GameObject Object)

        public override bool HandleEvent(ImplantedEvent E)
        {
            Debug.Entry(3, "@START CyberneticsGiganticExoframe.HandleEvent(UnimplantedEvent E)");
            Debug.Entry(3, $"TARGET Implantee is {E.Implantee.DisplayName}");

            ImplantObject = E.Item;

            OnImplanted(E.Implantee);

            Debug.Entry(3, "x return base.HandleEvent(E) ]//");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnimplantedEvent E)
        {
            Debug.Entry(3, "@START CyberneticsGiganticExoframe.HandleEvent(UnimplantedEvent E)");
            Debug.Entry(3, $"TARGET Unimplantee is {E.Implantee.DisplayName}");

            OnUnimplanted(E.Implantee);

            Debug.Entry(3, "x return base.HandleEvent(E) ]//");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you install this cybernetic despite being a disparate size to you.
            if (E.Object.HasPart<CyberneticsBaseItem>())
            {
                if (!E.Actor.IsGiganticCreature && E.Object.IsGiganticEquipment)
                    E.Decreases++;
                else if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment)
                    E.Increases++;

                E.CanBeTooSmall = false;
            }
            return base.HandleEvent(E);
        }

        public void CheckEquipment(GameObject Actor, Body Body)
        {
            Debug.Entry(3, "* public void CheckEquipment(GameObject Actor, Body Body)");
            if (Actor == null || Body == null)
            { 
                Debug.Entry(3, "x (Actor == null || Body == null)");
                return;
            }

            List<GameObject> list = Event.NewGameObjectList();
            Debug.Entry(3, "* foreach (BodyPart bodyPart in Body.LoopParts())");
            foreach (BodyPart bodyPart in Body.LoopParts())
            {
                GameObject equipped = bodyPart.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    Debug.Entry(3, "- Part", equipped.DebugName);
                    list.Add(equipped);
                    int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                    int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, bodyPart.Type, true);
                    if (partCountEquippedOn != slotsRequiredFor && bodyPart.TryUnequip(true, true, false, false) && partCountEquippedOn > slotsRequiredFor)
                    {
                        equipped.SplitFromStack();
                        bodyPart.Equip(equipped, new int?(0), true, false, false, true);
                    }
                }
            }
            Debug.Entry(3, "x public void CheckEquipment(GameObject Actor, Body Body)");
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        // These prevent the cybernetic in question from being disassembled.
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("CanBeDisassembled");
            base.Register(Object, Registrar);
        }
        public void CanBeDisassembled()
        {
            Event CanBeDisassembled = Event.New("CanBeDisassembled");
            ParentObject.FireEvent(CanBeDisassembled);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "CanBeDisassembled")
            {
                return false;
            }

            return base.FireEvent(E);
        }
    }
}
