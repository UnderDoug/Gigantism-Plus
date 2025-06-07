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
    public class DescribeModificationEvent<T> : IDescribeModificationEvent<DescribeModificationEvent<T>, T>
        where T : IModification
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModificationEvent<T>));

        public BeforeDescribeModificationEvent<T> BeforeEvent;

        public DescribeModificationEvent()
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

        public static DescribeModificationEvent<T> FromPool(BeforeDescribeModificationEvent<T> BeforeEvent)
        {
            if (BeforeEvent == null)
            {
                return null;
            }
            DescribeModificationEvent<T> E = FromPool(
                Object: BeforeEvent.Object,
                Adjective: BeforeEvent.Adjective,
                ObjectNoun: BeforeEvent.ObjectNoun,
                Context: BeforeEvent.Context);
            E.BeforeEvent = BeforeEvent;
            return E;
        }
        public static DescribeModificationEvent<T> FromPool(
            GameObject Object,
            string Adjective,
            string ObjectNoun = null,
            List<DescriptionElement> WeaponDescriptions = null,
            List<DescriptionElement> GeneralDescriptions = null,
            string Context = null)
        {
            DescribeModificationEvent<T> E = FromPool();
            if (E != null)
            {
                E.BeforeEvent = null;
                E.Object = Object;
                E.Adjective = Adjective;
                E.ObjectNoun = ObjectNoun;
                E.WeaponDescriptions = WeaponDescriptions ?? new();
                E.GeneralDescriptions = GeneralDescriptions ?? new();
                E.Context = Context;
            }
            return E;
        }

        public static DescribeModificationEvent<T> Send(BeforeDescribeModificationEvent<T> BeforeEvent)
        {
            return FromPool(BeforeEvent)?.Send();
        }
        public static DescribeModificationEvent<T> Send(
            GameObject Object,
            string Adjective,
            string ObjectNoun = null,
            string Context = null)
        {
            return Send(
                BeforeDescribeModificationEvent<T>.Send(
                    Object: Object,
                    Adjective: Adjective,
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

            string adjective = Grammar.MakeTitleCase(Adjective).Color('y');

            string objectNoun = Object != null && Object.IsPlural ? Grammar.Pluralize(ObjectNoun) : ObjectNoun;

            SB.Append(adjective).Append(": "); // "Gigantic: "
            SB.Append(Object?.IndicativeProximal).Append(" "); // "This "
            SB.Append(objectNoun).Append(" "); // "fist "
            // "Gigantic: This fist "

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
                if (typeof(T) == typeof(ModGigantic))
                {
                    SB.Append($"{Object.Are()} really big. Like, massive! Yuge!");
                }
                else
                {
                    SB.Append($"{Object.Are()} mysterious. Like, strange! Indescribable!");
                }
            }

            BeforeEvent.Reset();
            Reset();
            return Event.FinalizeString(SB);
        }
    }
}