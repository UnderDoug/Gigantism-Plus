using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    [GameEvent(Base = true, Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public abstract class IDescribeModGiganticEvent<T> : ModPooledEvent<T> where T : IDescribeModGiganticEvent<T>, new()
    {
        private static bool doDebug => getClassDoDebug("IDescribeModGiganticEvent");

        public static string RegisteredEventID => typeof(T).Name;

        public GameObject Object;

        public string ObjectNoun;

        public List<DescriptionElement> WeaponDescriptions;

        public List<DescriptionElement> GeneralDescriptions;

        public string Context;

        public IDescribeModGiganticEvent()
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
            ObjectNoun = null;
            WeaponDescriptions = null;
            GeneralDescriptions = null;
            Context = null;
        }
        public T Send()
        {
            int indent = Debug.LastIndent;

            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && haveObject && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Object.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = gameWants || objectWantsMin || objectWantsStr;

            bool proceed = anyWants;

            if (proceed)
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
                    @event.SetParameter(nameof(ObjectNoun), ObjectNoun);
                    @event.SetParameter(nameof(WeaponDescriptions), WeaponDescriptions);
                    @event.SetParameter(nameof(GeneralDescriptions), GeneralDescriptions);
                    @event.SetParameter(nameof(Context), Context);
                    proceed = Object.FireEvent(@event);
                    WeaponDescriptions = @event.GetParameter(nameof(WeaponDescriptions)) as List<DescriptionElement>;
                    GeneralDescriptions = @event.GetParameter(nameof(GeneralDescriptions)) as List<DescriptionElement>;
                }
            }

            Debug.LastIndent = indent;
            return (T)this;
        }

        public static List<DescriptionElement> AddDescription(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            Descriptions.Add(new(Verb, Effect));
            return Descriptions;
        }
        public List<DescriptionElement> AddWeaponDescription(string Verb, string Effect)
        {
            return WeaponDescriptions = AddDescription(WeaponDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> AddGeneralDescription(string Verb, string Effect)
        {
            return GeneralDescriptions = AddDescription(GeneralDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> AddWeaponDescription(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return AddDescription(WeaponDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> AddGeneralDescription(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return AddDescription(GeneralDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> AddWeaponDescription(DescriptionElement DescriptionElement)
        {
            return AddDescription(WeaponDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> AddGeneralDescription(DescriptionElement DescriptionElement)
        {
            return AddDescription(GeneralDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }

        public static List<DescriptionElement> RemoveDescription(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"{typeof(BeforeDescribeModGiganticEvent).Name}." +
                $"{nameof(RemoveDescription)}(Verb: {Verb ?? NULL}, Effect: {Effect ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            Descriptions.RemoveAll(x => x.Verb == Verb && x.Effect == Effect);
            return Descriptions;
        }
        public List<DescriptionElement> RemoveWeaponDescription(string Verb, string Effect)
        {
            return WeaponDescriptions = RemoveDescription(WeaponDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> RemoveGeneralDescription(string Verb, string Effect)
        {
            return GeneralDescriptions = RemoveDescription(GeneralDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> RemoveWeaponDescription(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return WeaponDescriptions = RemoveDescription(WeaponDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveGeneralDescription(List<string> Entry)
        {
            DescriptionElement descriptionElement = new(Entry);
            return GeneralDescriptions = RemoveDescription(GeneralDescriptions, descriptionElement.Verb, descriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveWeaponDescription(DescriptionElement DescriptionElement)
        {
            return WeaponDescriptions = RemoveDescription(WeaponDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }
        public List<DescriptionElement> RemoveGeneralDescription(DescriptionElement DescriptionElement)
        {
            return GeneralDescriptions = RemoveDescription(GeneralDescriptions, DescriptionElement.Verb, DescriptionElement.Effect);
        }
    }
}