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
    public class DescribeModGiganticEvent : ModPooledEvent<DescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModGiganticEvent));

        public new static readonly int CascadeLevel = CASCADE_ALL;

        public static readonly string RegisteredEventID = GetRegisteredEventID();

        public BeforeDescribeModGiganticEvent BeforeEvent;

        public GameObject Object;

        public string ObjectNoun;

        public List<List<string>> WeaponDescriptions;

        public List<List<string>> GeneralDescriptions;

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
            DescribeModGiganticEvent afterDescribeModGiganticEvent = FromPool();
            afterDescribeModGiganticEvent.BeforeEvent = BeforeEvent;
            afterDescribeModGiganticEvent.Object = BeforeEvent.Object;
            afterDescribeModGiganticEvent.ObjectNoun = BeforeEvent.ObjectNoun;
            afterDescribeModGiganticEvent.WeaponDescriptions = new();
            afterDescribeModGiganticEvent.GeneralDescriptions = new();
            afterDescribeModGiganticEvent.Context = BeforeEvent.Context;
            return afterDescribeModGiganticEvent;
        }

        public static DescribeModGiganticEvent Send(BeforeDescribeModGiganticEvent BeforeEvent)
        {
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
                    proceed = The.Game.HandleEvent(E);
                }
                if (proceed && objectWantsMin)
                {
                    proceed = E.Object.HandleEvent(E);
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
                }
            }
            return E;
        }
        public string Process(bool PluralizeObject = true)
        {
            List<List<string>> weaponDescriptions = new();
            List<List<string>> generalDescriptions = new();
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
            GameObjectBlueprint GigantismPlusModGiganticDescriptions = GameObjectFactory.Factory.GetBlueprint(MODGIGANTIC_DESCRIPTIONBUCKET);
            weaponDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "Before", "Weapon"));
            generalDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "Before", "General"));

            weaponDescriptions.AddRange(WeaponDescriptions);
            generalDescriptions.AddRange(GeneralDescriptions);

            weaponDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "After", "Weapon"));
            generalDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "After", "General"));

            StringBuilder SB = Event.NewStringBuilder();

            bool isPlural = Object.IsPlural && PluralizeObject;
            ObjectNoun = $"{(isPlural ? Grammar.Pluralize(ObjectNoun) : ObjectNoun)} ";
            string DemonstrativePronoun = $"{Grammar.MakeTitleCase(Object.NearDemonstrative())} ";
            SB.Append("Gigantic".OptionalColorGigantic(Colorfulness)).Append(": ")
                .Append(DemonstrativePronoun).Append(ObjectNoun);

            bool capitalizeSecondList = false;
            if (weaponDescriptions.Count != 0)
            {
                List<string> processedWeaponDescription = new();
                foreach (List<string> weaponEnrty in weaponDescriptions)
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
            foreach (List<string> entry in generalDescriptions)
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

        public static List<List<string>> IterateDataBucketTags(GameObject Object, GameObjectBlueprint GigantismPlusModGiganticDescriptions, string When, string Where)
        {
            List<List<string>> Output = new();
            foreach ((string location, string value) in GigantismPlusModGiganticDescriptions.Tags)
            {
                string[] locationArray = location.Split("::", StringSplitOptions.RemoveEmptyEntries);
                if (locationArray.Length != 4) continue;

                (string targetEvent, string targetList, string conditionPart, string who) = (locationArray[0], locationArray[1], locationArray[2], locationArray[2]);
                if (targetEvent != When) continue;
                if (targetList != "General" && targetList != "Weapon")
                {
                    Debug.Entry(1, $"WARN [{who}]: {targetList} in {location} is not a valid list. \"Weapon\" or \"General\" required.", Indent: 0);
                    continue;
                }
                if (!Object.HasPart(conditionPart)) continue;

                string[] descriptionArray = value.Split(";", StringSplitOptions.RemoveEmptyEntries);
                if (descriptionArray.Length != 2) continue;

                if (descriptionArray[0] == "'null'") descriptionArray[0] = null;
                List<string> description = new() { descriptionArray[0], descriptionArray[1] };
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