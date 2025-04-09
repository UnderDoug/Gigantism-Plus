using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public class DescribeModGiganticEvent : ModPooledEvent<DescribeModGiganticEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public string ObjectNoun;

        public List<List<string>> WeaponDescriptions;

        public List<List<string>> GeneralDescriptions;

        private BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent;

        public DescribeModGiganticEvent()
        {
        }

        public DescribeModGiganticEvent(GameObject Object, string ObjectNoun)
            : this()
        {
            DescribeModGiganticEvent @new = FromPool(Object, ObjectNoun, new(), new(), new());
            this.Object = @new.Object;
            this.ObjectNoun = @new.ObjectNoun;
            WeaponDescriptions = @new.WeaponDescriptions;
            GeneralDescriptions = @new.GeneralDescriptions;
            BeforeDescribeModGiganticEvent = @new.BeforeDescribeModGiganticEvent;
        }
        public DescribeModGiganticEvent(GameObject Object, string ObjectNoun, BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this(Object, ObjectNoun)
        {
            this.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
        }

        public DescribeModGiganticEvent(
            GameObject Object,
            string ObjectNoun,
            List<List<string>> WeaponDescriptions,
            List<List<string>> GeneralDescriptions)
            : this(Object, ObjectNoun)
        {
            this.WeaponDescriptions = WeaponDescriptions;
            this.GeneralDescriptions = GeneralDescriptions;
        }
        public DescribeModGiganticEvent(
            GameObject Object,
            string ObjectNoun,
            List<List<string>> WeaponDescriptions,
            List<List<string>> GeneralDescriptions,
            BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this(Object, ObjectNoun, WeaponDescriptions, GeneralDescriptions)
        {
            this.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
        }
        public DescribeModGiganticEvent(DescribeModGiganticEvent Source)
            : this()
        {
            Object = Source.Object;
            ObjectNoun = Source.ObjectNoun;
            WeaponDescriptions = Source.WeaponDescriptions;
            GeneralDescriptions = Source.GeneralDescriptions;
            BeforeDescribeModGiganticEvent = Source.BeforeDescribeModGiganticEvent;
        }
        public DescribeModGiganticEvent(BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this(BeforeDescribeModGiganticEvent.Object, BeforeDescribeModGiganticEvent.ObjectNoun)
        {
            this.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{typeof(DescribeModGiganticEvent).Name}";
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            ObjectNoun = null;
            WeaponDescriptions = null;
            GeneralDescriptions = null;
            BeforeDescribeModGiganticEvent = null;
        }

        public DescribeModGiganticEvent Send()
        {
            bool flag = The.Game.HandleEvent(this) && Object.HandleEvent(this);

            if (flag && Object.HasRegisteredEvent(GetRegisteredEventID()))
            {
                Event @event = Event.New(GetRegisteredEventID());
                @event.SetParameter("Object", Object);
                @event.SetParameter("ObjectNoun", ObjectNoun);
                @event.SetParameter("WeaponDescriptions", WeaponDescriptions);
                @event.SetParameter("GeneralDescriptions", GeneralDescriptions);
                @event.SetParameter("BeforeDescribeModGiganticEvent", BeforeDescribeModGiganticEvent);
                Object.FireEvent(@event);
            }
            return this;
        }

        public static DescribeModGiganticEvent FromPool(GameObject Object, string ObjectNoun, List<List<string>> WeaponDescriptions, List<List<string>> GeneralDescriptions, BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
        {
            DescribeModGiganticEvent afterDescribeModGiganticEvent = FromPool();
            afterDescribeModGiganticEvent.Object = Object;
            afterDescribeModGiganticEvent.ObjectNoun = ObjectNoun;
            afterDescribeModGiganticEvent.WeaponDescriptions = WeaponDescriptions;
            afterDescribeModGiganticEvent.GeneralDescriptions = GeneralDescriptions;
            afterDescribeModGiganticEvent.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
            return afterDescribeModGiganticEvent;
        }

        public List<List<string>> IterateDataBucketTags(GameObjectBlueprint GigantismPlusModGiganticDescriptions, string When, string Where)
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

        public string Process()
        {
            List<List<string>> weaponDescriptions = new();
            List<List<string>> generalDescriptions = new();
            string objectNoun = Object.GetObjectNoun();

            if (BeforeDescribeModGiganticEvent != null)
            {
                if (BeforeDescribeModGiganticEvent.WeaponDescriptions != null)
                {
                    weaponDescriptions.AddRange(BeforeDescribeModGiganticEvent.WeaponDescriptions);
                }
                if (BeforeDescribeModGiganticEvent.GeneralDescriptions != null)
                {
                    generalDescriptions.AddRange(BeforeDescribeModGiganticEvent.GeneralDescriptions);
                }
            }
            string beforeObjectNoun = BeforeDescribeModGiganticEvent.ObjectNoun;
            objectNoun = beforeObjectNoun ?? objectNoun;

            GameObjectBlueprint GigantismPlusModGiganticDescriptions = GameObjectFactory.Factory.GetBlueprint(MODGIGANTIC_DESCRIPTIONBUCKET);
            weaponDescriptions.AddRange(IterateDataBucketTags(GigantismPlusModGiganticDescriptions, "Before", "Weapon"));
            generalDescriptions.AddRange(IterateDataBucketTags(GigantismPlusModGiganticDescriptions, "Before", "General"));

            weaponDescriptions.AddRange(WeaponDescriptions);
            generalDescriptions.AddRange(GeneralDescriptions);

            weaponDescriptions.AddRange(IterateDataBucketTags(GigantismPlusModGiganticDescriptions, "After", "Weapon"));
            generalDescriptions.AddRange(IterateDataBucketTags(GigantismPlusModGiganticDescriptions, "After", "General"));

            ObjectNoun ??= objectNoun;

            StringBuilder SB = Event.NewStringBuilder();
            ObjectNoun = $"{(Object.IsPlural ? Grammar.Pluralize(ObjectNoun) : ObjectNoun)} ";
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
                    SB.Append(Grammar.MakeAndList(processedWeaponDescription) + ". ");
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
                SB.Append(Grammar.MakeAndList(processedGeneralDescription) + ".");
            }
            else if (weaponDescriptions.Count == 0)
            {
                SB.Append(", really big. Like, massive!");
            }

            return Event.FinalizeString(SB);
        }
    }
}