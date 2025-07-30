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

        public List<DescriptionElement> WeaponDescriptions;

        public List<DescriptionElement> GeneralDescriptions;

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
            Object = null;
            Adjective = null;
            ObjectNoun = null;
            WeaponDescriptions = null;
            GeneralDescriptions = null;
            Context = null;
            base.Reset();
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
                    @event.SetParameter(nameof(WeaponDescriptions), WeaponDescriptions);
                    @event.SetParameter(nameof(GeneralDescriptions), GeneralDescriptions);
                    @event.SetParameter(nameof(Context), Context);
                    proceed = Object.FireEvent(@event);
                    Adjective = @event.GetStringParameter(nameof(Adjective));
                    ObjectNoun = @event.GetStringParameter(nameof(ObjectNoun));
                    WeaponDescriptions = @event.GetParameter(nameof(WeaponDescriptions)) as List<DescriptionElement>;
                    GeneralDescriptions = @event.GetParameter(nameof(GeneralDescriptions)) as List<DescriptionElement>;
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
            ClearWeaponElements();
            ClearGeneralElements();
        }
        public List<DescriptionElement> ClearWeaponElements()
        {
            return WeaponDescriptions = new();
        }
        public List<DescriptionElement> ClearGeneralElements()
        {
            return GeneralDescriptions = new();
        }
        public static List<DescriptionElement> AddElement(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            Descriptions ??= new();
            Descriptions.Add(new(Verb, Effect));
            return Descriptions;
        }
        public List<DescriptionElement> AddWeaponElement(string Verb, string Effect)
        {
            return WeaponDescriptions = AddElement(WeaponDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> AddGeneralElement(string Verb, string Effect)
        {
            return GeneralDescriptions = AddElement(GeneralDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> AddWeaponElement(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return AddElement(WeaponDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> AddGeneralElement(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return AddElement(GeneralDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> AddWeaponElement(DescriptionElement DescriptionElement)
        {
            return AddElement(WeaponDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> AddGeneralElement(DescriptionElement DescriptionElement)
        {
            return AddElement(GeneralDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
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
                        Descriptions = AddElement(Descriptions, descriptionElement.Verb, descriptionElement.Effect);
                    }
                }
            }
            return Descriptions;
        }
        public List<DescriptionElement> AddWeaponElements(List<DescriptionElement> AddDescriptions)
        {
            return WeaponDescriptions = AddElements(WeaponDescriptions, AddDescriptions);
        }
        public List<DescriptionElement> AddGeneralElements(List<DescriptionElement> AddDescriptions)
        {
            return GeneralDescriptions = AddElements(GeneralDescriptions, AddDescriptions);
        }

        public static List<DescriptionElement> AddElements(List<DescriptionElement> Descriptions, List<List<string>> AddDescriptions)
        {
            Descriptions ??= new();
            List<DescriptionElement> addDescriptions = new();
            if (!AddDescriptions.IsNullOrEmpty())
            {
                foreach (List<string> descriptionStringElement in AddDescriptions)
                {
                    addDescriptions.Add(new(descriptionStringElement));
                }
            }
            return AddElements(Descriptions, addDescriptions);
        }
        public List<DescriptionElement> AddWeaponElements(List<List<string>> AddDescriptions)
        {
            return WeaponDescriptions = AddElements(WeaponDescriptions, AddDescriptions);
        }
        public List<DescriptionElement> AddGeneralElements(List<List<string>> AddDescriptions)
        {
            return GeneralDescriptions = AddElements(GeneralDescriptions, AddDescriptions);
        }

        public static List<DescriptionElement> RemoveElement(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            Descriptions ??= new();
            Descriptions.RemoveAll(x => x.Verb == Verb && x.Effect == Effect);
            return Descriptions;
        }
        public List<DescriptionElement> RemoveWeaponElement(string Verb, string Effect)
        {
            return WeaponDescriptions = RemoveElement(WeaponDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> RemoveGeneralElement(string Verb, string Effect)
        {
            return GeneralDescriptions = RemoveElement(GeneralDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> RemoveWeaponElement(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return WeaponDescriptions = RemoveElement(WeaponDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveGeneralElement(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return GeneralDescriptions = RemoveElement(GeneralDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveWeaponElement(DescriptionElement DescriptionElement)
        {
            return WeaponDescriptions = RemoveElement(WeaponDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveGeneralElement(DescriptionElement DescriptionElement)
        {
            return GeneralDescriptions = RemoveElement(GeneralDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }

        public static List<DescriptionElement> RemoveElements(List<DescriptionElement> Descriptions, List<DescriptionElement> RemoveDescriptions)
        {
            Descriptions ??= new();
            if (!RemoveDescriptions.IsNullOrEmpty())
            {
                foreach (DescriptionElement descriptionElement in RemoveDescriptions)
                {
                    Descriptions = RemoveElement(Descriptions, descriptionElement.Verb, descriptionElement.Effect);
                }
            }
            return Descriptions;
        }
        public List<DescriptionElement> RemoveWeaponElements(List<DescriptionElement> DescriptionElements)
        {
            return WeaponDescriptions = RemoveElements(WeaponDescriptions, DescriptionElements);
        }
        public List<DescriptionElement> RemoveGeneralElements(List<DescriptionElement> DescriptionElements)
        {
            return GeneralDescriptions = RemoveElements(GeneralDescriptions, DescriptionElements);
        }

        public static List<DescriptionElement> RemoveElements(List<DescriptionElement> Descriptions, List<List<string>> RemoveDescriptions)
        {
            Descriptions ??= new();
            List<DescriptionElement> removeDescriptions = new();
            if (!RemoveDescriptions.IsNullOrEmpty())
            {
                foreach (List<string> descriptionStringElement in RemoveDescriptions)
                {
                    removeDescriptions.Add(new(descriptionStringElement));
                }
            }
            return RemoveElements(Descriptions, removeDescriptions);
        }
        public List<DescriptionElement> RemoveWeaponElements(List<List<string>> DescriptionElements)
        {
            return WeaponDescriptions = RemoveElements(WeaponDescriptions, DescriptionElements);
        }
        public List<DescriptionElement> RemoveGeneralElements(List<List<string>> DescriptionElements)
        {
            return GeneralDescriptions = RemoveElements(GeneralDescriptions, DescriptionElements);
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