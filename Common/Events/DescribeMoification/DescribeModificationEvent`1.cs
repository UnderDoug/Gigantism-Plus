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

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public override void Reset()
        {
            if (BeforeEvent != null)
            {
                BeforeEvent.Reset();
                BeforeEvent = null;
            }
            base.Reset();
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
                WeaponDescriptions: BeforeEvent.PrimaryDescriptions,
                GeneralDescriptions: BeforeEvent.SecondaryDescriptions,
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
                E.PrimaryDescriptions = WeaponDescriptions ?? new();
                E.SecondaryDescriptions = GeneralDescriptions ?? new();
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
            List<DescriptionElement> WeaponDescriptions = null,
            List<DescriptionElement> GeneralDescriptions = null,
            string Context = null)
        {
            return Send(
                BeforeDescribeModificationEvent<T>.Send(
                    Object: Object,
                    Adjective: Adjective,
                    ObjectNoun: ObjectNoun,
                    WeaponDescriptions: WeaponDescriptions, 
                    GeneralDescriptions: GeneralDescriptions,
                    Context: Context)
                );
        }
        public static DescribeModificationEvent<T> Send(
            GameObject Object,
            string Adjective,
            out List<DescriptionElement> WeaponDescriptions,
            out List<DescriptionElement> GeneralDescriptions,
            string ObjectNoun = null,
            string Context = null)
        {
            WeaponDescriptions = new();
            GeneralDescriptions = new();
            return Send(
                BeforeDescribeModificationEvent<T>.Send(
                    Object: Object,
                    Adjective: Adjective,
                    ObjectNoun: ObjectNoun,
                    WeaponDescriptions: WeaponDescriptions,
                    GeneralDescriptions: GeneralDescriptions,
                    Context: Context)
                );
        }
        public string Process(bool PluralizeObject = true)
        {
            List<DescriptionElement> primaryDescriptions = new();
            List<DescriptionElement> secondaryDescriptions = new();
            ObjectNoun ??= Object.GetObjectNoun();

            if (BeforeEvent != null)
            {
                if (!BeforeEvent.PrimaryDescriptions.IsNullOrEmpty())
                {
                    primaryDescriptions = AddElements(primaryDescriptions, BeforeEvent.PrimaryDescriptions);
                }
                if (!BeforeEvent.SecondaryDescriptions.IsNullOrEmpty())
                {
                    secondaryDescriptions = AddElements(secondaryDescriptions, BeforeEvent.SecondaryDescriptions);
                }
            }

            primaryDescriptions ??= AddElements(primaryDescriptions, PrimaryDescriptions);
            secondaryDescriptions ??= AddElements(secondaryDescriptions, SecondaryDescriptions);

            primaryDescriptions.Sort();
            secondaryDescriptions.Sort();

            StringBuilder SB = Event.NewStringBuilder();

            string adjective = Grammar.MakeTitleCase(Adjective).Color('y');

            string objectNoun = Object != null && Object.IsPlural ? Grammar.Pluralize(ObjectNoun) : ObjectNoun;

            SB.Append(adjective).Append(": "); // "Gigantic: "
            SB.Append(Object?.IndicativeProximal).Append(" "); // "This "
            SB.Append(objectNoun).Append(" "); // "fist "
            // "Gigantic: This fist "

            if (primaryDescriptions.IsNullOrEmpty() && secondaryDescriptions.IsNullOrEmpty())
            {
                secondaryDescriptions ??= new();
                if (typeof(T).InheritsFrom(typeof(ModGigantic)))
                {
                    secondaryDescriptions.Add(new(null, "really big. Like, massive! Yuge"));
                }
                else if (typeof(T).InheritsFrom(typeof(ModNaturalEquipmentBase)))
                {
                    secondaryDescriptions.Add(new("gain", "some manner of adjustments"));
                }
                else
                {
                    secondaryDescriptions.Add(new(null, "mysterious. Like, strange! Indescribable"));
                }
            }

            bool isFirstList = true;
            if (!primaryDescriptions.IsNullOrEmpty())
            {
                SB.AppendDescription(Object, primaryDescriptions, isFirstList);
                isFirstList = false;
            }
            if (!secondaryDescriptions.IsNullOrEmpty())
            {
                SB.AppendDescription(Object, secondaryDescriptions, isFirstList);
            }

            Reset();
            return Event.FinalizeString(SB);
        }

        public static implicit operator DescribeModificationEvent<IModification>(DescribeModificationEvent<T> E)
        {
            return E as DescribeModificationEvent<IModification>;
        }
        public static implicit operator DescribeModificationEvent<T>(DescribeModificationEvent<IModification>  E)
        {
            return E as DescribeModificationEvent<T>;
        }

        public static implicit operator DescribeModificationEvent<T>(BeforeDescribeModificationEvent<T> E)
        {
            return E as DescribeModificationEvent<T>;
        }
        public static implicit operator DescribeModificationEvent<T>(BeforeDescribeModificationEvent<IModification> E)
        {
            return E as DescribeModificationEvent<T>;
        }
    }
}