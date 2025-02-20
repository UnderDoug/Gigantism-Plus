using System;
using System.Collections.Generic;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;

/*namespace XRL.World.Parts
{
    */[Serializable]
    public class CyberneticsMassiveExoframe : IPart
    {
        /* Potentially redundant
         * 
        [Serializable]
        public class CompactedExoframe : IActivePart // Inner marker class
        {
            public CompactedExoframe() 
            {
                WorksOnSelf = true;
                WorksOnImplantee = true;
            }

            public override bool WantEvent(int ID, int cascade)
            {
                return base.WantEvent(ID, cascade)
                    || ID == GetSlotsRequiredEvent.ID;
            }

            public override bool HandleEvent(GetSlotsRequiredEvent E)
            {
                bool isReady =
                    IsReady(
                        UseCharge: true,
                        IgnoreCharge: false,
                        IgnoreLiquid: false,
                        IgnoreBootSequence: false,
                        IgnoreBreakage: false,
                        IgnoreRust: false,
                        IgnoreEMP: false,
                        IgnoreRealityStabilization: false,
                        IgnoreSubject: false,
                        IgnoreLocallyDefinedFailure: false,
                        MultipleCharge: 1,
                        ChargeUse: null,
                        UseChargeIfUnpowered: false,
                        GridMask: 0L,
                        PowerLoadLevel: null
                    );

                if (!E.Actor.IsGiganticCreature && IsObjectActivePartSubject(E.Actor) && isReady)
                {
                    E.Decreases++;
                    if (!E.Object.IsGiganticEquipment && E.SlotType != "Floating Nearby" && E.SlotType != "Thrown Weapon")
                    {
                        E.CanBeTooSmall = true;
                    }
                }
                return base.HandleEvent(E);
            }

            public override bool AllowStaticRegistration()
            {
                return true;
            }
        } //!--- public class CompactedExoframe : IActivePart  
        */

        [NonSerialized]
        private GameObject _User;

        public GameObject User => _User ?? (_User = ParentObject.Implantee);

        /* Potentially redundant.
         * 
        public static readonly string COMPACT_MODE_COMMAND_NAME = "CommandToggleExoframeCompactMode";
        public Guid EnableActivatedAbilityID = Guid.Empty;

        private bool IsCompactModeFree = false;
        private int _compactModeEnergyCost = 500;
        
        public int CompactModeEnergyCost
        {
            get
            {
                if (IsCompactModeFree)
                {
                    IsCompactModeFree = false;
                    return 0;
                }
                return _compactModeEnergyCost;
            }
            private set => _compactModeEnergyCost = value;
        }

        // Stats modified by compact mode
        public int CompactModeAVModifier = 4;
        public int CompactModeDVModifier = -6;
        public int CompactModeQNModifier = -50;
        public int CompactModeMSModifier = -50;

        // private bool _isGiganticCreature = false;
        public bool IsGiganticCreature
        {
            get
            {
                return _User.IsGiganticCreature;
            }
            private set
            {
                _User.IsGiganticCreature = value;
                if (IsPseudoGiganticCreature == value)
                {
                    IsPseudoGiganticCreature = !value;
                }
            }
        }

        // private bool _isPseudoGiganticCreature = false;
        public bool IsPseudoGiganticCreature
        {
            get
            {
                return _User.HasPart<CompactedExoframe>();
            }
            set
            {
                if (value) _User.RequirePart<CompactedExoframe>();
                else _User.RemovePart<CompactedExoframe>();

                if (IsGiganticCreature == value)
                {
                    IsGiganticCreature = !value;
                }

            }
        }
        */

        public string Model = "Alpha";

        public GameObject _ManipulatorObject;

        public string ManipulatorBlueprintName
        {
            get
            {
                return "MassiveExoframeManipulator" + this.Model;
            }

        }

        [NonSerialized]
        protected GameObjectBlueprint _ManipulatorBlueprint;

        public GameObjectBlueprint ManipulatorBlueprint
        {
            get
            {
                if (_ManipulatorBlueprint == null)
                {
                    _ManipulatorBlueprint = GameObjectFactory.Factory.GetBlueprint(ManipulatorBlueprintName);
                }
                return _ManipulatorBlueprint;
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
                || ID == CanEnterInteriorEvent.ID;
        }

        public virtual void OnImplanted(GameObject Object)
        {
            Debug.Entry(2, "**public virtual void OnImplanted(GameObject Object)");
            // base
            _User = Object;

            // Require Part
            Object.RequirePart<CyberneticsMassiveExoframe>();

            /* Testing Mutation method.
             *
            // Make Gigantic
            IsGiganticCreature = true;

            // Active Ability
            EnableActivatedAbilityID = 
                AddMyActivatedAbility(
                    Name: "{{C|{{W|[}}Standard{{W|]}}/Compact}}",
                    Command: COMPACT_MODE_COMMAND_NAME,
                    Class: "Cybernetics",
                    Description: null, 
                    Icon: "&#214",
                    DisabledMessage: null,
                    Toggleable: true,
                    DefaultToggleState: false,
                    ActiveToggle: true,
                    IsAttack: false,
                    IsRealityDistortionBased: false,
                    IsWorldMapUsable: false
                    );

            ActivatedAbilityEntry abilityEntry = Object.GetActivatedAbility(EnableActivatedAbilityID);
            abilityEntry.DisplayName = "{{C|{{W|[}}Standard{{W|]}}\nCompact\n}}"; ;
            */

            // Natural Weapon
            Debug.Entry(3, "**if (_ManipulatorObject == null)");
            if (_ManipulatorObject == null)
            {
                Debug.Entry(3, "-- (_ManipulatorObject == null");
                _ManipulatorObject = GameObjectFactory.Factory.CreateObject(ManipulatorBlueprint);
            }

            /* Seeing if I can have the GigantismPlus mutation handle this by hotswapping the Weapon Object.
             * 
            Body body = Object.Body;
            if (body != null)
            {
                foreach (BodyPart part in body.GetParts())
                {
                    if (part.Type == "Hand")
                    {
                        part.DefaultBehavior = _ManipulatorObject;
                    }
                }
            } 
            */
            
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Object)
        {
            Debug.Entry(3, "**public virtual void OnUnimplanted(GameObject Object)");
            /* Might be redundant.
             * 
            // Remove manipulator from hands
            Body body = Object.Body;
            if (body != null)
            {
                foreach (BodyPart part in body.GetParts())
                {
                    if (part.Type == "Hand" && part.DefaultBehavior == _manipulatorObject)
                    {
                        part.DefaultBehavior = null;
                    }
                }
            }
            */

            /* Testing Mutation Method.
             * 
            DisengageCompactMode();
            IsGiganticCreature = false;

            if (EnableActivatedAbilityID != Guid.Empty)
            {
                RemoveMyActivatedAbility(ref EnableActivatedAbilityID);
            }
            */

            Object.RemovePart<CyberneticsMassiveExoframe>();
            Debug.Entry(3, "**Object.RemovePart<CyberneticsMassiveExoframe>()");

            /* Testing Mutation Method.
             * 
            Object.RemovePart<CompactedExoframe>();
            */

            CheckEquipment(Object, Object.Body);
            Debug.Entry(3, "**CheckEquipment(Object, Object.Body)");

        } //!--- public override void OnUnimplanted(GameObject Object)

        public override bool HandleEvent(ImplantedEvent E)
        {
            Debug.Entry(3, "**public override bool HandleEvent(ImplantedEvent E)");
            /* Temporarily Commenting this out to see if OnImplanted works instead.
             * 
            E.Implantee.IsGiganticCreature = true;
            E.Implantee.RequirePart<MassiveExoframe>();
            // Create manipulator weapon if needed
            if (_manipulatorObject == null)
            {
                _manipulatorObject = GameObjectFactory.Factory.CreateObject("MassiveExoframeManipulatorA");
            }
            
            // Apply to hands
            Body body = E.Part?.ParentBody;
            if (body != null)
            {
                foreach (BodyPart part in body.GetParts())
                {
                    if (part.Type == "Hand")
                    {
                        part.DefaultBehavior = _manipulatorObject;
                    }
                }
            }

            if (!ParentObject.HasPart<Vehicle>())
            {
                EnableActivatedAbilityID =
                    E.Implantee.AddActivatedAbility(
                        Name: "{{C|{{W|[}}Standard{{W|]}}/Compact}}",
                        Command: COMPACT_MODE_COMMAND_NAME,
                        Class: "Cybernetics",
                        Description: "Toggle between standard and compact configurations",
                        Icon: "&#214",
                        DisabledMessage: null,
                        Toggleable: true,
                        DefaultToggleState: false,
                        ActiveToggle: true,
                        IsAttack: false,
                        IsRealityDistortionBased: false,
                        IsWorldMapUsable: false
                );

                ActivatedAbilityEntry abilityEntry = E.Implantee.GetActivatedAbility(EnableActivatedAbilityID);
                if (abilityEntry != null)
                {
                    // If the above check isn't done, the game stalls on village generation if any of the generated villages
                    // want their inhabitants implanted with this (they seem to get implanted without actually be instantiatied.
                    abilityEntry.DisplayName = "{{C|{{W|[}}Standard{{W|]}}\nCompact\n}}";
                }
            }

            CheckEquipment(E.Implantee, E.Part?.ParentBody);
            */

            OnImplanted(E.Implantee);

            Debug.Entry(3, "xxreturn base.HandleEvent(E)");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnimplantedEvent E)
        {
            Debug.Entry(3, "**public override bool HandleEvent(UnimplantedEvent E)");
            /* Temporarily Commenting this out to see if OnImplanted works instead.
             *
            // Remove manipulator from hands
            Body body = E.Part?.ParentBody;
            if (body != null)
            {
                foreach (BodyPart part in body.GetParts())
                {
                    if (part.Type == "Hand" && part.DefaultBehavior == _manipulatorObject)
                    {
                        part.DefaultBehavior = null;
                    }
                }
            }

            DisengageCompactMode();
            E.Implantee.IsGiganticCreature = false;

            if (EnableActivatedAbilityID != Guid.Empty)
                E.Implantee.RemoveActivatedAbility(ref EnableActivatedAbilityID);

            E.Implantee.RemovePart<MassiveExoframe>();

            CheckEquipment(E.Implantee, E.Part?.ParentBody);
            */

            OnUnimplanted(E.Implantee);
            Debug.Entry(3, "xxreturn base.HandleEvent(E)");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(CanEnterInteriorEvent E)
        {
            /* Testing Mutation Method.
             * 
            if (ParentObject == E.Object)
                return base.HandleEvent(E);

            GameObject actor = E.Actor;
            if (actor != null && actor.IsGiganticCreature && !ParentObject.HasPart<Vehicle>())
            {
                IsCompactModeFree = true;
                CommandEvent.Send(actor, COMPACT_MODE_COMMAND_NAME);
                bool check = CanEnterInteriorEvent.Check(E.Actor, E.Object, E.Interior, ref E.Status, ref E.Action, ref E.ShowMessage);
                E.Status = check ? 0 : E.Status;
                Popup.Show("Your exoframe compacts itself to fit into the space.");
            }
            */
            return base.HandleEvent(E);
        }

        public void CheckEquipment(GameObject Actor, Body Body)
        {
            Debug.Entry(3, "**public void CheckEquipment(GameObject Actor, Body Body)");
            if (Actor == null || Body == null)
            { 
                Debug.Entry(3, "xx(Actor == null || Body == null)");
                return;
            }

            List<GameObject> list = Event.NewGameObjectList();
            Debug.Entry(3, "**foreach (BodyPart bodyPart in Body.LoopParts())");
            foreach (BodyPart bodyPart in Body.LoopParts())
            {
                GameObject equipped = bodyPart.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    Debug.Entry(3, "Part", equipped.DebugName);
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
            Debug.Entry(3, "xxpublic void CheckEquipment(GameObject Actor, Body Body)");
        }
        
        /* Potentially redundant
         * 
        public void EngageCompactMode(bool Message = false)
        {
            Debug.Entry(2, "**public void EngageCompactMode(bool Message = false)");
            if (IsPseudoGiganticCreature)
                return;

            IsPseudoGiganticCreature = true;
            Debug.Entry(2, "**IsPseudoGiganticCreature = true");

            Debug.Entry(2, "**if (!IsGiganticCreature && IsPseudoGiganticCreature)");
            if (!IsGiganticCreature && IsPseudoGiganticCreature)
            {
                Debug.Entry(3, "- Now not Gigantic, Now is PseudoGigantic");
                ParentObject.UseEnergy(CompactModeEnergyCost, "Cybernetic Exoframe Compact Mode");

                // Apply stat modifications
                _User.ModIntProperty("AV", CompactModeAVModifier);
                _User.ModIntProperty("DV", CompactModeDVModifier);
                _User.ModIntProperty("Quickness", CompactModeQNModifier);
                _User.ModIntProperty("MoveSpeed", CompactModeMSModifier);

                _User.PlayWorldSound("Sounds/Machines/sfx_machine_hydraulics");
                if (Message)
                    Popup.Show("Your exoframe compacts with hydraulic whirs and mechanical clicks.");

                ActivatedAbilityEntry abilityEntry = _User.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = "{{C|Standard\n{{W|[}}Compact{{W|]}}\n}}";
            }
        }

        public void DisengageCompactMode(bool Message = false)
        {
            Debug.Entry(2, "**public void DisengageCompactMode(bool Message = false)");
            if (!IsPseudoGiganticCreature)
                return;

            IsPseudoGiganticCreature = false;
            Debug.Entry(2, "**IsPseudoGiganticCreature = false");

            Debug.Entry(2, "**if (IsGiganticCreature && !IsPseudoGiganticCreature)");
            if (IsGiganticCreature && !IsPseudoGiganticCreature)
            {
                Debug.Entry(3, "- Now is Gigantic, Now not PseudoGigantic");
                ParentObject.UseEnergy(CompactModeEnergyCost, "Cybernetic Exoframe Standard Mode");

                // Revert stat modifications
                _User.ModIntProperty("AV", -CompactModeAVModifier);
                _User.ModIntProperty("DV", -CompactModeDVModifier);
                _User.ModIntProperty("Quickness", -CompactModeQNModifier);
                _User.ModIntProperty("MoveSpeed", -CompactModeMSModifier);

                _User.PlayWorldSound("Sounds/Machines/sfx_machine_hydraulics");
                if (Message)
                    Popup.Show("Your exoframe expands with hydraulic whirs and mechanical clicks.");

                ActivatedAbilityEntry abilityEntry = _User.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = "{{C|{{W|[}}Standard{{W|]}}\nCompact\n}}";
            }
        }

        public override bool FireEvent(Event E)
        {
            Debug.Entry(3, "**public override bool FireEvent(Event E)");
            /* Testing Mutation Method.
             * 
            if (E.ID == COMPACT_MODE_COMMAND_NAME)
            {
                Debug.Entry(2, "**MassiveExoframe.FireEvent(Event E)", E.ID);

                Debug.Entry(4, "_User", _User.DisplayName);
                if (_User.CurrentZone.ZoneWorld == "Interior" && !IsGiganticCreature)
                {
                    Debug.Entry(3, "- Parent in interior, Abort");
                    Popup.Show("This space is too small for you to disengage compact mode!");
                    return base.FireEvent(E);
                }

                if (_User.HasPart<Vehicle>())
                {
                    Debug.Entry(3, "- Parent has Vehicle, Abort");
                    return base.FireEvent(E);
                }

                // Use IActivePart's toggle methods
                ToggleMyActivatedAbility(EnableActivatedAbilityID, null, Silent: true, null);

                //  Debug
                Debug.Entry(3, "**ToggleMyActivatedAbility(EnableActivatedAbilityID, null, Silent: true, null)");
                string IsActiveAbilityToggledOn = IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID) ? "On" : "Off";
                Debug.Entry(2, "**if (_User.IsActivatedAbilityToggledOn(EnableActivatedAbilityID))", IsActiveAbilityToggledOn);
                Debug.Entry(2, "- EnableActivatedAbilityID.ToString()", EnableActivatedAbilityID.ToString());
                //! Debug

                if (IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID))
                {
                    Debug.Entry(2, "- Toggled is On");
                    EngageCompactMode(true);
                }
                else
                {
                    Debug.Entry(2, "- Toggled is Off");
                    DisengageCompactMode(true);
                }

            }
            *//*

            // The.Core.RenderBase();
            Debug.Entry(3, "xxpublic override bool FireEvent(Event E)");
            return base.FireEvent(E);
        }
        */


        public override bool AllowStaticRegistration()
        {
            return true;
        }
        
        /* Testing Mutation Method.
         *
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
             
            Registrar.Register(COMPACT_MODE_COMMAND_NAME);

            base.Register(Object, Registrar);
        }
        */
    }/*
}*/
