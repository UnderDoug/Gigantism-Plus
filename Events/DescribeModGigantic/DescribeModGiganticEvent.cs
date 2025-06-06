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
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class DescribeModGiganticEvent : ModPooledEvent<DescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModGiganticEvent));

        public struct DescriptionElement
        {
            private static bool doDebug => getClassDoDebug(nameof(DescriptionElement));

            public string Verb;
            public string Effect;

            public DescriptionElement(string Verb, string Effect)
            {
                this.Verb = Verb;
                this.Effect = Effect;
            }

            public DescriptionElement(List<string> Source)
            {
                Verb = null;
                Effect = null;
                if (!Source.IsNullOrEmpty())
                {
                    Verb = Source[0];
                    if (Source.Count > 1)
                    {
                        Effect = Source[1];
                    }
                }
            }

            public readonly List<string> ToList()
            {
                return new List<string>()
                {
                    Verb,
                    Effect,
                };
            }

            public override readonly string ToString()
            {
                if (Verb == "")
                {
                    return  "It " + Effect;
                }
                if (Verb == null)
                {
                    return "It is " + Effect;
                }
                return Grammar.ThirdPerson(Verb, PrependSpace: false) + " " + Effect;
            }

            public readonly string ToString(GameObject Object)
            {
                if (Verb == "")
                {
                    return Object.It + " " + Effect;
                }
                if (Verb == null)
                {
                    return Object.Itis + " " + Effect;
                }
                return Object.GetVerb(Verb, PrependSpace: false) + " " + Effect;
            }
        }

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = GetRegisteredEventID();

        public BeforeDescribeModGiganticEvent BeforeEvent;

        public GameObject Object;

        public string ObjectNoun;

        public List<DescriptionElement> WeaponDescriptions;

        public List<DescriptionElement> GeneralDescriptions;

        public string Context;

        public DescribeModGiganticEvent()
        {
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }
        public static string GetRegisteredEventID()
        {
            return $"{nameof(DescribeModGiganticEvent)}";
        }

        public override void Reset()
        {
            base.Reset();
            BeforeEvent = null;
            Object = null;
            ObjectNoun = null;
            WeaponDescriptions = null;
            GeneralDescriptions = null;
            Context = null;
        }

        public static DescribeModGiganticEvent FromPool(BeforeDescribeModGiganticEvent BeforeEvent)
        {
            DescribeModGiganticEvent E = FromPool();
            E.BeforeEvent = BeforeEvent;
            E.Object = BeforeEvent.Object;
            E.ObjectNoun = BeforeEvent.ObjectNoun;
            E.WeaponDescriptions = new();
            E.GeneralDescriptions = new();
            E.Context = BeforeEvent.Context;
            return E;
        }

        public static DescribeModGiganticEvent Send(BeforeDescribeModGiganticEvent BeforeEvent)
        {
            int indent = Debug.LastIndent;

            DescribeModGiganticEvent E = FromPool(BeforeEvent);

            bool haveGame = The.Game != null;
            bool haveObject = E.Object != null;

            bool gameWants = haveGame && The.Game.WantEvent(ID, CascadeLevel);

            bool objectWantsMin = haveObject && E.Object.WantEvent(ID, CascadeLevel);
            bool objectWantsStr = haveObject && E.Object.HasRegisteredEvent(RegisteredEventID);

            bool anyWants = gameWants || objectWantsMin || objectWantsStr;

            bool proceed = anyWants;

            if (proceed)
            {
                if (proceed && gameWants)
                {
                    Debug.Entry(4, $"{nameof(gameWants)}", Indent: indent + 1, Toggle: doDebug);
                    proceed = The.Game.HandleEvent(E);
                    Debug.Entry(4, $"{proceed}", Indent: indent + 2, Toggle: doDebug);
                }
                if (proceed && objectWantsMin)
                {
                    Debug.Entry(4, $"{nameof(objectWantsMin)}", Indent: indent + 1, Toggle: doDebug);
                    proceed = E.Object.HandleEvent(E);
                    Debug.Entry(4, $"{proceed}", Indent: indent + 2, Toggle: doDebug);
                }
                if (proceed && objectWantsStr)
                {
                    Event @event = Event.New(RegisteredEventID);
                    @event.SetParameter($"{nameof(E.Object)}", E.Object);
                    @event.SetParameter($"{nameof(E.ObjectNoun)}", E.ObjectNoun);
                    @event.SetParameter($"{nameof(E.WeaponDescriptions)}", E.WeaponDescriptions);
                    @event.SetParameter($"{nameof(E.GeneralDescriptions)}", E.GeneralDescriptions);
                    @event.SetParameter($"{nameof(E.Context)}", E.Context);
                    proceed = E.Object.FireEvent(@event);
                    E.WeaponDescriptions = @event.GetParameter(nameof(E.WeaponDescriptions)) as List<DescriptionElement>;
                    E.GeneralDescriptions = @event.GetParameter(nameof(E.GeneralDescriptions)) as List<DescriptionElement>;
                }
            }

            Debug.LastIndent = indent;
            return E;
        }
        public string Process(bool PluralizeObject = true)
        {
            List<DescriptionElement> weaponDescriptions = new();
            List<DescriptionElement> generalDescriptions = new();
            ObjectNoun ??= Object.GetObjectNoun();

            if (BeforeEvent != null)
            {
                if (!BeforeEvent.WeaponDescriptions.IsNullOrEmpty())
                {
                    weaponDescriptions.AddRange(BeforeEvent.WeaponDescriptions);
                }
                if (!BeforeEvent.GeneralDescriptions.IsNullOrEmpty())
                {
                    generalDescriptions.AddRange(BeforeEvent.GeneralDescriptions);
                }
            }

            if (!WeaponDescriptions.IsNullOrEmpty())
            {
                weaponDescriptions.AddRange(WeaponDescriptions);
            }
            if (!GeneralDescriptions.IsNullOrEmpty())
            {
                generalDescriptions.AddRange(GeneralDescriptions);
            }

            StringBuilder SB = Event.NewStringBuilder();

            bool isPlural = Object.IsPlural && PluralizeObject;
            ObjectNoun = $"{(isPlural ? Grammar.Pluralize(ObjectNoun) : ObjectNoun)} ";
            SB.Append("Gigantic".OptionalColorGigantic(Colorfulness)).Append(": ")
                .Append(Object.IndicativeProximal).Append(" ").Append(ObjectNoun);

            bool capitalizeSecondList = false;
            if (weaponDescriptions.Count != 0)
            {
                List<string> processedWeaponDescription = new();
                foreach (DescriptionElement weaponEnrty in weaponDescriptions)
                {
                    processedWeaponDescription.Add(weaponEnrty.GetProcessedItem(second: false, weaponDescriptions, Object));
                }
                if (!processedWeaponDescription.IsNullOrEmpty())
                {
                    SB.Append(Utils.MakeAndList(processedWeaponDescription, IgnoreCommas: true) + ". ");
                    capitalizeSecondList = true;
                }
            }
            List<string> processedGeneralDescription = new();
            foreach (DescriptionElement entry in generalDescriptions)
            {
                processedGeneralDescription.Add(entry.GetProcessedItem(second: capitalizeSecondList, generalDescriptions, Object));
            }
            if (!processedGeneralDescription.IsNullOrEmpty())
            {
                SB.Append(Utils.MakeAndList(processedGeneralDescription, IgnoreCommas: true) + ".");
            }
            else if (weaponDescriptions.Count == 0)
            {
                SB.Append($"{Object.Are()} really big. Like, massive! Yuge!");
            }

            BeforeEvent.Reset();
            Reset();
            return Event.FinalizeString(SB);
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

        public static List<DescriptionElement> IterateDataBucketTags(GameObject Object, GameObjectBlueprint GigantismPlusModGiganticDescriptions, string When, string Where)
        {
            List<DescriptionElement> Output = new();
            foreach ((string location, string value) in GigantismPlusModGiganticDescriptions.Tags)
            {
                string[] locationArray = location.Split("::", StringSplitOptions.RemoveEmptyEntries);
                if (locationArray.Length != 4) continue;

                (string targetEvent, string targetList, string conditionPart, string who) = (locationArray[0], locationArray[1], locationArray[2], locationArray[2]);
                if (targetEvent != When) continue;
                if (targetList != "General" && targetList != "Weapon")
                {
                    Debug.Warn(1, 
                        nameof(DescribeModGiganticEvent), 
                        nameof(IterateDataBucketTags), 
                        $"[{who}]: {targetList} in {location} is not a valid list. \"Weapon\" or \"General\" required.", Indent: 0);
                    continue;
                }
                if (!Object.HasPart(conditionPart)) continue;

                string[] descriptionArray = value.Split(";", StringSplitOptions.RemoveEmptyEntries);
                if (descriptionArray.Length != 2) continue;

                if (descriptionArray[0] == "'null'") descriptionArray[0] = null;
                DescriptionElement description = new(descriptionArray[0], descriptionArray[1]);
                if (targetEvent == When)
                {
                    if (targetList == Where)
                    {
                        Output.Add(description);
                    }
                }
            }
            return Output;
        }
    }
}