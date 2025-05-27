using System;
using System.Collections.Generic;
using System.Text;

using XRL.UI;
using XRL.World.Tinkering;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModificationApplicator : IScribedPart
    {
        public string Mod;

        public bool IsConsumed;

        public string ConsumedVerb;

        public string ConsumedMessage;

        public string Become;

        public string NoItemsMessage;

        public ModificationApplicator()
        {
            Mod = "";
                // ModLacquered
            
            IsConsumed = true;
            
            ConsumedVerb = "get";
                // get
            
            ConsumedMessage = "used up entriely.";
                // (gets) used up entriely.
            
            NoItemsMessage = "You have no items that this can be used on.";
                // You have no items that can be lacquered.
            
            Become = "";
                // (Becomes) covered in a rust-proof lacquer.
        }

        private bool isCompletelyConfigured
        {
            get
            {
                return !Mod.IsNullOrEmpty()
                    && !(IsConsumed && (ConsumedVerb.IsNullOrEmpty() || ConsumedMessage.IsNullOrEmpty()))
                    && !NoItemsMessage.IsNullOrEmpty()
                    && !Become.IsNullOrEmpty();
            }
        }

        public override void Attach()
        {
            base.Attach();
            ParentObject.RequirePart<Applicator>();
        }

        public override bool WantEvent(int ID, int Cascade)
        {
            return base.WantEvent(ID, Cascade)
                || (isCompletelyConfigured && ID == InventoryActionEvent.ID);
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == "Apply" && isCompletelyConfigured)
            {
                if (!E.Actor.CheckFrozen(Telepathic: false, Telekinetic: true))
                {
                    return false;
                }
                if (E.Item.IsBroken() || E.Item.IsRusted() || E.Item.IsEMPed())
                {
                    E.Actor.Fail(ParentObject.Does("do", int.MaxValue, null, null, null, AsIfKnown: false, Single: false, NoConfusion: false, NoColor: false, Stripped: false, WithoutTitles: true, Short: true, BaseOnly: false, WithIndefiniteArticle: false, null, IndicateHidden: false, Pronoun: true, SecondPerson: true, null) + " nothing.");
                    return false;
                }
                List<GameObject> objects = E.Actor.Inventory.GetObjects((GameObject o) => CanApply(o, E.Actor));
                if (objects.Count == 0)
                {
                    if (ParentObject.Understood())
                    {
                        E.Actor.Fail(ParentObject.Does("do", int.MaxValue, null, null, null, AsIfKnown: false, Single: false, NoConfusion: false, NoColor: false, Stripped: false, WithoutTitles: true, Short: true, BaseOnly: false, WithIndefiniteArticle: false, null, IndicateHidden: false, Pronoun: true, SecondPerson: true, null) + " nothing.");
                    }
                    else
                    {
                        E.Actor.Fail(NoItemsMessage);
                    }
                    return false;
                }
                GameObject applicant = PickItem.ShowPicker(objects, null, PickItem.PickItemDialogStyle.SelectItemDialog, E.Actor);
                if (applicant == null)
                {
                    return false;
                }
                applicant.SplitFromStack();
                if (E.Actor.IsPlayer())
                {
                    ParentObject.MakeUnderstood();
                }
                string message = applicant.Does("become", int.MaxValue, null, null, null, AsIfKnown: false, Single: false, NoConfusion: false, NoColor: false, Stripped: false, WithoutTitles: true, Short: true, BaseOnly: false, WithIndefiniteArticle: false, null, IndicateHidden: false, Pronoun: false, SecondPerson: true, null) + " " + Become;
                bool isUnderstood = applicant.Understood();
                if (!ItemModding.ApplyModification(applicant, Mod, DoRegistration: true, E.Actor))
                {
                    E.Actor.Fail("Nothing happens.");
                    applicant.CheckStack();
                    return false;
                }
                if (isUnderstood && !applicant.Understood())
                {
                    applicant.MakeUnderstood();
                }
                applicant.CheckStack();

                if (E.Actor.IsPlayer())
                {
                    Popup.Show(message);
                }
                if (IsConsumed)
                {
                    if (E.Actor.IsPlayer())
                    {
                        Popup.Show(ParentObject.Does(ConsumedVerb, int.MaxValue, null, null, null, AsIfKnown: false, Single: true, NoConfusion: false, NoColor: false, Stripped: false, WithoutTitles: true, Short: true, BaseOnly: false, WithIndefiniteArticle: false, null, IndicateHidden: false, Pronoun: false, SecondPerson: true, null) + " " + ConsumedMessage);
                    }
                    ParentObject.Destroy();
                }
            }
            return base.HandleEvent(E);
        }

        private bool CanApply(GameObject Item, GameObject Actor)
        {
            return ItemModding.ModificationApplicable(Mod, Item, Actor);
        }
    }
}
