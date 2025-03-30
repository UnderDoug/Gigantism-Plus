using System;
using System.Collections.Generic;
using System.Text;
using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public class AfterDescribeModGiganticEvent : ModPooledEvent<AfterDescribeModGiganticEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public string ObjectNoun;

        public List<List<string>> WeaponDescriptions;

        public List<List<string>> GeneralDescriptions;

        private BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent;

        public AfterDescribeModGiganticEvent()
        {
        }

        public AfterDescribeModGiganticEvent(GameObject Object, string ObjectNoun)
            : this()
        {
            AfterDescribeModGiganticEvent @new = FromPool(Object, ObjectNoun, new(), new(), new());
            this.Object = @new.Object;
            this.ObjectNoun = @new.ObjectNoun;
            WeaponDescriptions = @new.WeaponDescriptions;
            GeneralDescriptions = @new.GeneralDescriptions;
            BeforeDescribeModGiganticEvent = @new.BeforeDescribeModGiganticEvent;
        }
        public AfterDescribeModGiganticEvent(GameObject Object, string ObjectNoun, BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this()
        {
            AfterDescribeModGiganticEvent @new = FromPool(Object, ObjectNoun, new(), new(), BeforeDescribeModGiganticEvent);
            this.Object = @new.Object;
            this.ObjectNoun = @new.ObjectNoun;
            WeaponDescriptions = @new.WeaponDescriptions;
            GeneralDescriptions = @new.GeneralDescriptions;
            this.BeforeDescribeModGiganticEvent = @new.BeforeDescribeModGiganticEvent;
        }

        public AfterDescribeModGiganticEvent(
            GameObject Object,
            string ObjectNoun,
            List<List<string>> WeaponDescriptions,
            List<List<string>> GeneralDescriptions)
            : this(Object, ObjectNoun)
        {
            this.WeaponDescriptions = WeaponDescriptions;
            this.GeneralDescriptions = GeneralDescriptions;
        }
        public AfterDescribeModGiganticEvent(
            GameObject Object,
            string ObjectNoun,
            List<List<string>> WeaponDescriptions,
            List<List<string>> GeneralDescriptions,
            BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this(Object, ObjectNoun, WeaponDescriptions, GeneralDescriptions)
        {
            this.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
        }
        public AfterDescribeModGiganticEvent(AfterDescribeModGiganticEvent Source)
            : this()
        {
            Object = Source.Object;
            ObjectNoun = Source.ObjectNoun;
            WeaponDescriptions = Source.WeaponDescriptions;
            GeneralDescriptions = Source.GeneralDescriptions;
            BeforeDescribeModGiganticEvent = Source.BeforeDescribeModGiganticEvent;
        }
        public AfterDescribeModGiganticEvent(BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
            : this()
        {
            this.BeforeDescribeModGiganticEvent = BeforeDescribeModGiganticEvent;
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{typeof(AfterDescribeModGiganticEvent).Name}";
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

        public AfterDescribeModGiganticEvent Send()
        {
            bool flag = The.Game.HandleEvent(this) || Object.HandleEvent(this);

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

        public static AfterDescribeModGiganticEvent FromPool(GameObject Object, string ObjectNoun, List<List<string>> WeaponDescriptions, List<List<string>> GeneralDescriptions, BeforeDescribeModGiganticEvent BeforeDescribeModGiganticEvent)
        {
            AfterDescribeModGiganticEvent afterDescribeModGiganticEvent = FromPool();
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

            if (Object.LiquidVolume != null)
            {
                generalDescriptions.Add(new List<string> { "hold", "twice as much liquid" });
            }
            if (Object.HasPart<EnergyCell>())
            {
                generalDescriptions.Add(new List<string> { "have", "twice the energy capacity" });
            }
            if (Object.HasPartDescendedFrom<IGrenade>())
            {
                generalDescriptions.Add(new List<string> { "have", "twice as large a radius of effect" });
            }
            if (Object.HasPart<Tonic>())
            {
                generalDescriptions.Add(new List<string> { "contain", "double the tonic dosage" });
            }
            if (Object.GetIntProperty("Currency") > 0)
            {
                generalDescriptions.Add(new List<string> { null, "much more valuable" });
            }
            
            bool isDefaultBehavior = Object.EquipAsDefaultBehavior();
            if (!isDefaultBehavior)
            {
                generalDescriptions.Add(new List<string> { null, "much heavier than usual" });
            }

            MeleeWeapon meleeWeapon = Object.GetPart<MeleeWeapon>();
            bool isDefaultBehaviorOrFloating = isDefaultBehavior || Object.IsEntirelyFloating();
            if (meleeWeapon != null && Object.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
                if (meleeWeapon.Skill == "Cudgel")
                {
                    weaponDescriptions.Add(new List<string> { null, "twice as effective targetEvent you Slam with " + Object.them });
                }
                else if (meleeWeapon.Skill == "Axe")
                {
                    weaponDescriptions.Add(new List<string> { "cleave", "for -3 AV" });
                }
            }
            else if (Object.HasPart<MissileWeapon>())
            {
                weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
            }
            else if (Object.HasPart<ThrownWeapon>())
            {
                if (!Object.HasPartDescendedFrom<IGrenade>())
                {
                    weaponDescriptions.Add(new List<string> { "have", "+3 damage" });
                }
            }

            bool improvisedMelee = false;
            if (meleeWeapon != null && meleeWeapon.IsImprovised()) improvisedMelee = true;

            if (!isDefaultBehaviorOrFloating)
            {
                if (Object.UsesSlots == null && 
                    (!improvisedMelee
                    || (Object.TryGetPart(out ThrownWeapon thrownWeapon) && !thrownWeapon.IsImprovised())
                    || Object.HasPart<MissileWeapon>()
                    || Object.HasPart<Shield>()))
                {
                    generalDescriptions.Add(new List<string> { "", Object.it + " must be wielded " + (Object.UsesTwoSlots ? "four" : "two") + "-handed by non-gigantic creatures" });
                }
                else
                {
                    generalDescriptions.Add(new List<string> { "", "can only be equipped by gigantic creatures" });
                }
            }

            if (Object.HasPart<DiggingTool>() || Object.HasPart<Drill>())
            {
                weaponDescriptions.Add(new List<string> { "dig", "twice as fast" });
            }

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
            else
            {
                SB.Append("thing is really big. Like, massive!");
            }

            return Event.FinalizeString(SB);
        }
    }
}