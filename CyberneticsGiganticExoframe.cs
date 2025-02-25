using System;
using System.Collections.Generic;
using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;
using static Mods.GigantismPlus.HelperMethods;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberneticsGiganticExoframe : IPart
    {
        [NonSerialized]
        private GameObject _User;
        public GameObject User => _User ?? (_User = ParentObject.Implantee);

        public GameObject ImplantObject;
            
        public string Model = "Alpha";
        public string AugmentAdjectiveColor = "b";
        public string AugmentTile = "GiganticManipulator.png";
        public string AugmentTileColorString = "&c";
        public string AugmentTileDetailColor = "b";

        public string GetShortAugmentAdjective(bool Pretty = true)
        {
            return MaybeColorText(
                Color: AugmentAdjectiveColor,
                Pretty: Pretty,
                Text: "augmented"
                );
        }
        public string GetAugmentAdjective(bool Pretty = true)
        {
            return MaybeColorText(
                Color: "Y",
                Pretty: Pretty,
                Text: "E" + MaybeColorText("c", "F", Pretty) + "-" + GetShortAugmentAdjective(Pretty)
                );
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
                if (_ManipulatorBlueprint == null)
                {
                    _ManipulatorBlueprint = GameObjectFactory.Factory.GetBlueprint(ManipulatorBlueprintName);
                }
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

            if (Model == "YES")
            {
                Popup.Show("...");
                Popup.Show("You... You've done it...");
                Popup.Show("...You've {{W|really}} done it...");
                Popup.Show("At last, you have {{c|become}}...");
                string finalMessage = "{{Y-W-W-W-O-O-O distribution|";
                switch (Stat.TinkerRandom(1, 4))
                {
                    case 1: finalMessage += "THE FINAL KING OF QUD"; 
                        break;
                    case 2: finalMessage += "A SHINING GOLDEN GOD";
                        break;
                    case 3: finalMessage += "A GOLD PLUS VIP MEMBER";
                        break;
                    case 4: finalMessage += "REALLY REALLY YELLOW";
                        break;
                }
                finalMessage += "}}";
                Popup.Show(finalMessage);
                User.RequirePart<SecretExoframePart>();
            }

            Debug.Entry(2, $"x public virtual void OnImplanted({Object.DisplayName}) ]//");
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Object)
        {
            Debug.Entry(3, $"* public virtual void OnUnimplanted({Object.DisplayName})");

            CheckEquipment(Object, Object.Body);

            if (Model == "YES")
            {
                User.RequirePart<SecretExoframePart>();
                User.RemovePart<SecretExoframePart>();
                Popup.Show("Oh! To have tasted sweet {{Y-W-W-W-O-O-O distribution|ambrosia}}...");
            }

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

        [Serializable]
        public class SecretExoframePart : IPart
        {
            public static readonly int ICON_COLOR_PRIORITY = 999;
            private bool MutationColor = XRL.UI.Options.MutationColor;
            public override bool Render(RenderEvent E)
            {
                bool flag = true;
                if (ParentObject.IsPlayerControlled())
                {
                    if ((XRLCore.FrameTimer.ElapsedMilliseconds & 0x7F) == 0L)
                    {
                        MutationColor = XRL.UI.Options.MutationColor;
                    }
                    if (!MutationColor)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    E.ApplyColors("&O", "W", ICON_COLOR_PRIORITY, ICON_COLOR_PRIORITY);
                }
                return base.Render(E);
            }
        }

    }
}
