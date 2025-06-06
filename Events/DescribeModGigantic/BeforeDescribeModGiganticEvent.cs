using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.DescribeModGiganticEvent;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class BeforeDescribeModGiganticEvent : ModPooledEvent<BeforeDescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModGiganticEvent));

        public static readonly string RegisteredEventID = GetRegisteredEventID();

        public GameObject Object;

        public string ObjectNoun;

        public List<DescriptionElement> WeaponDescriptions;

        public List<DescriptionElement> GeneralDescriptions;

        public string Context;

        public BeforeDescribeModGiganticEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
        public static string GetRegisteredEventID()
        {
            return $"{nameof(BeforeDescribeModGiganticEvent)}";
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

        public static BeforeDescribeModGiganticEvent FromPool(GameObject Object, string ObjectNoun, List<DescriptionElement> WeaponDescriptions, List<DescriptionElement> GeneralDescriptions, string Context = null)
        {
            BeforeDescribeModGiganticEvent E = FromPool();
            E.Object = Object;
            E.ObjectNoun = ObjectNoun;
            E.WeaponDescriptions = WeaponDescriptions ?? new();
            E.GeneralDescriptions = GeneralDescriptions ?? new();
            E.Context = Context;
            return E;
        }
        public static BeforeDescribeModGiganticEvent CollectFor(GameObject Object, string ObjectNoun = null, List<DescriptionElement> WeaponDescriptions = null, List<DescriptionElement> GeneralDescriptions = null, string Context = null)
        {
            BeforeDescribeModGiganticEvent E = FromPool(Object, ObjectNoun, WeaponDescriptions, GeneralDescriptions, Context);

            bool haveGame = The.Game != null;
            bool haveObject = Object != null;

            bool gameWants = haveGame && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && Object.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = gameWants || objectWantsMin || objectWantsStr;

            bool proceed = anyWants;

            if (proceed)
            {
                if (proceed && gameWants)
                {
                    proceed = The.Game.HandleEvent(E);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = Object.HandleEvent(E);
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(RegisteredEventID);
                    @event.SetParameter($"{nameof(E.Object)}", E.Object);
                    @event.SetParameter($"{nameof(E.Object)}", E.Object);
                    @event.SetParameter($"{nameof(E.WeaponDescriptions)}", E.WeaponDescriptions);
                    @event.SetParameter($"{nameof(E.GeneralDescriptions)}", E.GeneralDescriptions);
                    @event.SetParameter($"{nameof(E.Context)}", E.Context);
                    proceed = Object.FireEvent(@event);
                    E.WeaponDescriptions = @event.GetParameter(nameof(E.WeaponDescriptions)) as List<DescriptionElement>;
                    E.GeneralDescriptions = @event.GetParameter(nameof(E.GeneralDescriptions)) as List<DescriptionElement>;
                }
            }
            return E;
        }

        public static List<DescriptionElement> AddDescription(List<DescriptionElement> Descriptions, string Verb, string Effect)
        {
            Descriptions.Add(new(Verb, Effect));
            return Descriptions;
        }
        public List<DescriptionElement> AddWeaponDescription(string Verb, string Effect)
        {
            return AddDescription(WeaponDescriptions, Verb, Effect);
        }
        public List<DescriptionElement> AddGeneralDescription(string Verb, string Effect)
        {
            return AddDescription(GeneralDescriptions, Verb, Effect);
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
                $"{nameof(BeforeDescribeModGiganticEvent)}." +
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