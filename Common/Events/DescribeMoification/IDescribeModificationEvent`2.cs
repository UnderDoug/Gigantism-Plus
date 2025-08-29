using System;
using System.Collections.Generic;
using System.Text;
using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.AI.GoalHandlers;
using XRL.World.Capabilities;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public abstract class IDescribeModificationEvent<T, M> : ModPooledEvent<T> 
        where T : IDescribeModificationEvent<T, M>, new()
        where M : IModification
    {
        private static bool doDebug => getClassDoDebug(typeof(T).Name);

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static string RegisteredEventID => typeof(T).Name;

        public GameObject Object;

        public string Adjective;

        public string ObjectNoun;

        public List<DescriptionElement> PrimaryDescriptions;

        public List<DescriptionElement> SecondaryDescriptions;

        public string Context;

        public IDescribeModificationEvent()
        {
        }

        public virtual string GetRegisteredEventID()
        {
            return RegisteredEventID;
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            Adjective = null;
            ObjectNoun = null;
            PrimaryDescriptions = null;
            SecondaryDescriptions = null;
            Context = null;
        }
        public T Send()
        {
            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && haveObject && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Object.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = gameWants || objectWantsMin || objectWantsStr;

            bool proceed = true;

            if (proceed && anyWants)
            {
                if (proceed && gameWants)
                {
                    proceed = The.Game.HandleEvent(this);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = Object.HandleEvent(this);
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(RegisteredEventID);
                    @event.SetParameter(nameof(Object), Object);
                    @event.SetParameter(nameof(Adjective), Adjective);
                    @event.SetParameter(nameof(ObjectNoun), ObjectNoun);
                    @event.SetParameter(nameof(PrimaryDescriptions), PrimaryDescriptions);
                    @event.SetParameter(nameof(SecondaryDescriptions), SecondaryDescriptions);
                    @event.SetParameter(nameof(Context), Context);
                    proceed = Object.FireEvent(@event);
                    Adjective = @event.GetStringParameter(nameof(Adjective));
                    ObjectNoun = @event.GetStringParameter(nameof(ObjectNoun));
                    PrimaryDescriptions = @event.GetParameter(nameof(PrimaryDescriptions)) as List<DescriptionElement>;
                    SecondaryDescriptions = @event.GetParameter(nameof(SecondaryDescriptions)) as List<DescriptionElement>;
                }
            }
            return (T)this;
        }

        public virtual string SetAdjective(string NewAdjective)
        {
            return Adjective = NewAdjective ?? Adjective;
        }
        public virtual string SetObjectNoun(string NewObjectNoun)
        {
            return ObjectNoun = NewObjectNoun ?? ObjectNoun;
        }
        public void ClearDescriptionElements()
        {
            ClearPrimaryElements();
            ClearSecondaryElements();
        }
        public List<DescriptionElement> ClearPrimaryElements()
        {
            return PrimaryDescriptions = new();
        }
        public List<DescriptionElement> ClearSecondaryElements()
        {
            return SecondaryDescriptions = new();
        }
        public static List<DescriptionElement> AddElement(List<DescriptionElement> Descriptions, int Priority, string Verb, string Effect)
        {
            Descriptions ??= new();
            DescriptionElement descriptionElement = new(Priority, Verb, Effect);
            if (!Descriptions.Contains(descriptionElement))
            {
                Descriptions.Add(descriptionElement);
            }
            return Descriptions;
        }
        public static List<DescriptionElement> AddElement(List<DescriptionElement> Descriptions, int Priority, DescriptionElement Element)
        {
            Descriptions ??= new();
            DescriptionElement descriptionElement = new(Priority, Element);
            if (!Descriptions.Contains(descriptionElement))
            {
                Descriptions.Add(descriptionElement);
            }
            return Descriptions;
        }
        public static List<DescriptionElement> AddElement(List<DescriptionElement> Descriptions, DescriptionElement Element)
        {
            Descriptions ??= new();
            if (!Descriptions.Contains(Element))
            {
                Descriptions.Add(Element);
            }
            return Descriptions;
        }
        public List<DescriptionElement> AddPrimaryElement(int Priority, string Verb, string Effect)
        {
            return PrimaryDescriptions = AddElement(PrimaryDescriptions, Priority, Verb, Effect);
        }
        public List<DescriptionElement> AddPrimaryElement(string Verb, string Effect)
        {
            return PrimaryDescriptions = AddElement(PrimaryDescriptions, 0, Verb, Effect);
        }
        public List<DescriptionElement> AddSecondaryElement(int Priority, string Verb, string Effect)
        {
            return SecondaryDescriptions = AddElement(SecondaryDescriptions, Priority, Verb, Effect);
        }
        public List<DescriptionElement> AddSecondaryElement(string Verb, string Effect)
        {
            return SecondaryDescriptions = AddElement(SecondaryDescriptions, 0, Verb, Effect);
        }
        public List<DescriptionElement> AddPrimaryElement(int Priority, List<string> Entry)
        {
            return AddElement(PrimaryDescriptions, Priority, Entry);
        }
        public List<DescriptionElement> AddPrimaryElement(List<string> Entry)
        {
            return AddElement(PrimaryDescriptions, 0, Entry);
        }
        public List<DescriptionElement> AddSecondaryElement(int Priority, List<string> Entry)
        {
            return AddElement(SecondaryDescriptions, Priority, Entry);
        }
        public List<DescriptionElement> AddSecondaryElement(List<string> Entry)
        {
            return AddElement(SecondaryDescriptions, 0, Entry);
        }
        public List<DescriptionElement> AddPrimaryElement(int Priority, DescriptionElement DescriptionElement)
        {
            return AddElement(PrimaryDescriptions, Priority, DescriptionElement);
        }
        public List<DescriptionElement> AddPrimaryElement(DescriptionElement DescriptionElement)
        {
            return AddElement(PrimaryDescriptions, 0, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> AddSecondaryElement(int Priority, DescriptionElement DescriptionElement)
        {
            return AddElement(SecondaryDescriptions, Priority, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> AddSecondaryElement(DescriptionElement DescriptionElement)
        {
            return AddElement(SecondaryDescriptions, 0, DescriptionElement.Verb, DescriptionElement.Effect);
        }

        public static List<DescriptionElement> AddElements(List<DescriptionElement> Descriptions, List<DescriptionElement> AddDescriptions)
        {
            Descriptions ??= new();
            if (!AddDescriptions.IsNullOrEmpty())
            {
                foreach (DescriptionElement descriptionElement in AddDescriptions)
                {
                    if (descriptionElement != DescriptionElement.Empty)
                    {
                        Descriptions = AddElement(Descriptions, descriptionElement);
                    }
                }
            }
            return Descriptions;
        }
        public List<DescriptionElement> AddPrimaryElements(List<DescriptionElement> AddDescriptions)
        {
            return PrimaryDescriptions = AddElements(PrimaryDescriptions, AddDescriptions);
        }
        public List<DescriptionElement> AddSecondaryElements(List<DescriptionElement> AddDescriptions)
        {
            return SecondaryDescriptions = AddElements(SecondaryDescriptions, AddDescriptions);
        }

        public static List<DescriptionElement> AddElements(List<DescriptionElement> Descriptions, List<List<string>> AddDescriptions)
        {
            Descriptions ??= new();
            List<DescriptionElement> addDescriptions = new();
            if (!AddDescriptions.IsNullOrEmpty())
            {
                foreach (List<string> descriptionStringElement in AddDescriptions)
                {
                    addDescriptions.Add(descriptionStringElement);
                }
            }
            return AddElements(Descriptions, addDescriptions);
        }
        public List<DescriptionElement> AddPrimaryElements(List<List<string>> AddDescriptions)
        {
            return PrimaryDescriptions = AddElements(PrimaryDescriptions, AddDescriptions);
        }
        public List<DescriptionElement> AddSecondaryElements(List<List<string>> AddDescriptions)
        {
            return SecondaryDescriptions = AddElements(SecondaryDescriptions, AddDescriptions);
        }

        public static List<DescriptionElement> RemoveElement(List<DescriptionElement> Descriptions, int Priority, string Verb, string Effect, bool IgnorePriority = false)
        {
            Descriptions ??= new();
            Descriptions.RemoveAll(x => (x.Priority == Priority || IgnorePriority) && x.Verb == Verb && x.Effect == Effect);
            return Descriptions;
        }
        public static List<DescriptionElement> RemoveElement(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            return RemoveElement(Descriptions, 0, Verb, Effect, true);
        }
        public static List<DescriptionElement> RemoveElement(List<DescriptionElement> Descriptions, DescriptionElement ElementToRemove, bool IgnorePriority = false)
        {
            Descriptions ??= new();
            Descriptions.RemoveAll(x => x == ElementToRemove || (IgnorePriority && x.ToString() == ElementToRemove.ToString()));
            return Descriptions;
        }
        public static List<DescriptionElement> RemoveElement(List<DescriptionElement> Descriptions, DescriptionElement ElementToRemove)
        {
            return RemoveElement(Descriptions, ElementToRemove, true);
        }
        public List<DescriptionElement> RemovePrimaryElement(int Priority, string Verb, string Effect, bool IgnorePriority = false)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, Priority, Verb, Effect, IgnorePriority);
        }
        public List<DescriptionElement> RemovePrimaryElement(string Verb, string Effect)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, 0, Verb, Effect, true);
        }
        public List<DescriptionElement> RemoveSecondaryElement(int Priority, string Verb, string Effect, bool IgnorePriority = false)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, Priority, Verb, Effect, IgnorePriority);
        }
        public List<DescriptionElement> RemoveSecondaryElement(string Verb, string Effect)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, 0, Verb, Effect, true);
        }
        public List<DescriptionElement> RemovePrimaryElement(int Priority, List<string> Entry, bool IgnorePriority = false)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, new(Priority, Entry), IgnorePriority);
        }
        public List<DescriptionElement> RemovePrimaryElement(List<string> Entry)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, Entry, true);
        }
        public List<DescriptionElement> RemoveSecondaryElement(int Priority, List<string> Entry, bool IgnorePriority = false)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, new(Priority, Entry), IgnorePriority);
        }
        public List<DescriptionElement> RemoveSecondaryElement(List<string> Entry)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, Entry, true);
        }
        public List<DescriptionElement> RemovePrimaryElement(DescriptionElement DescriptionElement, bool IgnorePriority = false)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, DescriptionElement, IgnorePriority);
        }
        public List<DescriptionElement> RemovePrimaryElement(DescriptionElement DescriptionElement)
        {
            return PrimaryDescriptions = RemoveElement(PrimaryDescriptions, DescriptionElement, true);
        }
        public List<DescriptionElement> RemoveSecondaryElement(DescriptionElement DescriptionElement, bool IgnorePriority = false)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, DescriptionElement, IgnorePriority);
        }
        public List<DescriptionElement> RemoveSecondaryElement(DescriptionElement DescriptionElement)
        {
            return SecondaryDescriptions = RemoveElement(SecondaryDescriptions, DescriptionElement, true);
        }

        public static List<DescriptionElement> RemoveElements(List<DescriptionElement> Descriptions, List<DescriptionElement> RemoveDescriptions, bool IgnorePriority = false)
        {
            Descriptions ??= new();
            if (!RemoveDescriptions.IsNullOrEmpty())
            {
                if (!IgnorePriority)
                {
                    Descriptions.RemoveAll(x => RemoveDescriptions.Contains(x));
                }
                else
                {
                    foreach (DescriptionElement descriptionElement in RemoveDescriptions)
                    {
                        Descriptions = RemoveElement(Descriptions, descriptionElement, IgnorePriority);
                    }
                }
                
            }
            return Descriptions;
        }
        public List<DescriptionElement> RemovePrimaryElements(List<DescriptionElement> DescriptionElements, bool IgnorePriority = false)
        {
            return PrimaryDescriptions = RemoveElements(PrimaryDescriptions, DescriptionElements, IgnorePriority);
        }
        public List<DescriptionElement> RemoveSecondaryElements(List<DescriptionElement> DescriptionElements, bool IgnorePriority = false)
        {
            return SecondaryDescriptions = RemoveElements(SecondaryDescriptions, DescriptionElements, IgnorePriority);
        }

        public static List<DescriptionElement> RemoveElements(List<DescriptionElement> Descriptions, int Priority, List<List<string>> RemoveDescriptions, bool IgnorePriority = false)
        {
            Descriptions ??= new();
            List<DescriptionElement> removeDescriptions = new();
            if (!RemoveDescriptions.IsNullOrEmpty())
            {
                foreach (List<string> descriptionStringElement in RemoveDescriptions)
                {
                    DescriptionElement descriptionElement = new(descriptionStringElement);
                    if (!IgnorePriority)
                    {
                        descriptionElement.Priority = Priority;
                    }
                    removeDescriptions.Add(descriptionElement);
                }
            }
            return RemoveElements(Descriptions, removeDescriptions, IgnorePriority);
        }
        public static List<DescriptionElement> RemoveElements(List<DescriptionElement> Descriptions, List<List<string>> RemoveDescriptions)
        {
            return RemoveElements(Descriptions, 0, RemoveDescriptions, true);
        }
        public List<DescriptionElement> RemovePrimaryElements(int Priority, List<List<string>> DescriptionElements, bool IgnorePriority = false)
        {
            return PrimaryDescriptions = RemoveElements(PrimaryDescriptions, Priority, DescriptionElements, IgnorePriority);
        }
        public List<DescriptionElement> RemovePrimaryElements(List<List<string>> DescriptionElements)
        {
            return PrimaryDescriptions = RemoveElements(PrimaryDescriptions, 0, DescriptionElements, true);
        }
        public List<DescriptionElement> RemoveSecondaryElements(int Priority, List<List<string>> DescriptionElements, bool IgnorePriority = false)
        {
            return SecondaryDescriptions = RemoveElements(SecondaryDescriptions, Priority, DescriptionElements, IgnorePriority);
        }
        public List<DescriptionElement> RemoveSecondaryElements(List<List<string>> DescriptionElements)
        {
            return SecondaryDescriptions = RemoveElements(SecondaryDescriptions, 0, DescriptionElements, true);
        }

        public static implicit operator DescribeModificationEvent<IModification>(IDescribeModificationEvent<T, M> E)
        {
            return E as DescribeModificationEvent<IModification>;
        }
        public static implicit operator IDescribeModificationEvent<T, M>(DescribeModificationEvent<IModification> E)
        {
            return E as IDescribeModificationEvent<T, M>;
        }

        public static implicit operator BeforeDescribeModificationEvent<IModification>(IDescribeModificationEvent<T, M> E)
        {
            return E as BeforeDescribeModificationEvent<IModification>;
        }
        public static implicit operator IDescribeModificationEvent<T, M>(BeforeDescribeModificationEvent<IModification> E)
        {
            return E as IDescribeModificationEvent<T, M>;
        }
    }
}