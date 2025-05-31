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
    public class BeforeDescribeModGiganticEvent : ModPooledEvent<BeforeDescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModGiganticEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = GetRegisteredEventID();

        public GameObject Object;

        public string ObjectNoun;

        public List<List<string>> WeaponDescriptions;

        public List<List<string>> GeneralDescriptions;

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

        public static BeforeDescribeModGiganticEvent FromPool(GameObject Object, string ObjectNoun, List<List<string>> WeaponDescriptions, List<List<string>> GeneralDescriptions, string Context = null)
        {
            BeforeDescribeModGiganticEvent beforeDescribeModGiganticEvent = FromPool();
            beforeDescribeModGiganticEvent.Object = Object;
            beforeDescribeModGiganticEvent.ObjectNoun = ObjectNoun;
            beforeDescribeModGiganticEvent.WeaponDescriptions = WeaponDescriptions ?? new();
            beforeDescribeModGiganticEvent.GeneralDescriptions = GeneralDescriptions ?? new();
            beforeDescribeModGiganticEvent.Context = Context;
            return beforeDescribeModGiganticEvent;
        }
        public static BeforeDescribeModGiganticEvent CollectFor(GameObject Object, string ObjectNoun = null, List<List<string>> WeaponDescriptions = null, List<List<string>> GeneralDescriptions = null, string Context = null)
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
                    @event.SetParameter($"{nameof(Object)}", Object);
                    @event.SetParameter($"{nameof(ObjectNoun)}", ObjectNoun);
                    @event.SetParameter($"{nameof(WeaponDescriptions)}", WeaponDescriptions);
                    @event.SetParameter($"{nameof(GeneralDescriptions)}", GeneralDescriptions);
                    @event.SetParameter($"{nameof(Context)}", Context);
                    proceed = Object.FireEvent(@event);
                }
            }
            return E;
        }

        public static List<List<string>> AddDescription(List<List<string>> Descriptions, string Relationship, string Effect)
        {
            Descriptions.Add(new() { Relationship, Effect });
            return Descriptions;
        }
        public List<List<string>> AddWeaponDescription(string Relationship, string Effect)
        {
            return AddDescription(WeaponDescriptions, Relationship, Effect);
        }
        public List<List<string>> AddGeneralDescription(string Relationship, string Effect)
        {
            return AddDescription(GeneralDescriptions, Relationship, Effect);
        }

        public static List<List<string>> RemoveDescription(string Relationship, string Effect, List<List<string>> Descriptions)
        {
            List<List<string>> elementsToRemove = new()
            {
                new List<string>() { Relationship, Effect },
            };
            List<List<string>> InumerateDescriptions = new(Descriptions);
            if (!InumerateDescriptions.IsNullOrEmpty())
            {
                foreach (List<string> entry in InumerateDescriptions)
                {
                    if (elementsToRemove.Contains(entry))
                    {
                        Descriptions.Remove(entry);
                    }
                }
            }
            return Descriptions;
        }
        public List<List<string>> RemoveWeaponDescription(string Relationship, string Effect)
        {
            return RemoveDescription(Relationship, Effect, WeaponDescriptions);
        }
        public List<List<string>> RemoveGeneralDescription(string Relationship, string Effect)
        {
            return RemoveDescription(Relationship, Effect, GeneralDescriptions);
        }
    }
}