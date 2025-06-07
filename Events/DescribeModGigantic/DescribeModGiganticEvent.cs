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
    public class DescribeModGiganticEvent : IDescribeModGiganticEvent<DescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModGiganticEvent));

        public BeforeDescribeModGiganticEvent BeforeEvent;

        public DescribeModGiganticEvent()
        {
        }

        public override void Reset()
        {
            base.Reset();
            if (BeforeEvent != null)
            {
                BeforeEvent.Reset();
                BeforeEvent = null;
            }
        }

        public static DescribeModGiganticEvent FromPool(BeforeDescribeModGiganticEvent BeforeEvent)
        {
            if (BeforeEvent == null)
            {
                return null;
            }
            DescribeModGiganticEvent E = FromPool(
                Object: BeforeEvent.Object, 
                ObjectNoun: BeforeEvent.ObjectNoun,
                Context: BeforeEvent.Context);
            E.BeforeEvent = BeforeEvent;
            return E;
        }
        public static DescribeModGiganticEvent FromPool(
            GameObject Object, 
            string ObjectNoun = null, 
            List<DescriptionElement> WeaponDescriptions = null, 
            List<DescriptionElement> GeneralDescriptions = null, 
            string Context = null)
        {
            DescribeModGiganticEvent E = FromPool();
            if (E != null)
            {
                E.BeforeEvent = null;
                E.Object = Object;
                E.ObjectNoun = ObjectNoun;
                E.WeaponDescriptions = WeaponDescriptions ?? new();
                E.GeneralDescriptions = GeneralDescriptions ?? new();
                E.Context = Context;
            }
            return E;
        }

        public static DescribeModGiganticEvent Send(BeforeDescribeModGiganticEvent BeforeEvent)
        {
            return FromPool(BeforeEvent)?.Send();
        }
        public static DescribeModGiganticEvent Send(
            GameObject Object,
            string ObjectNoun = null,
            string Context = null)
        {
            return Send(
                BeforeDescribeModGiganticEvent.Send(
                    Object: Object,
                    ObjectNoun: ObjectNoun,
                    Context: Context)
                );
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
    }
}