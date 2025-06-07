using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    [GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
    public class BeforeDescribeModificationEvent<T> : IDescribeModificationEvent<BeforeDescribeModificationEvent<T>, T> 
        where T 
        : IModification
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModificationEvent<T>));

        public BeforeDescribeModificationEvent()
        {
        }

        public static BeforeDescribeModificationEvent<T> FromPool(
            GameObject Object,
            string Adjective,
            string ObjectNoun,
            List<DescriptionElement> WeaponDescriptions,
            List<DescriptionElement> GeneralDescriptions,
            string Context = null)
        {
            BeforeDescribeModificationEvent<T> E = FromPool();
            if (E !=null)
            {
                E.Object = Object;
                E.Adjective = Adjective;
                E.ObjectNoun = ObjectNoun;
                E.WeaponDescriptions = WeaponDescriptions ?? new();
                E.GeneralDescriptions = GeneralDescriptions ?? new();
                E.Context = Context;
            }
            return E;
        }
        public static BeforeDescribeModificationEvent<T> Send(
            GameObject Object,
            string Adjective,
            string ObjectNoun = null,
            List<DescriptionElement> WeaponDescriptions = null,
            List<DescriptionElement> GeneralDescriptions = null,
            string Context = null)
        {
            return FromPool(
                Object: Object,
                Adjective: Adjective, 
                ObjectNoun: ObjectNoun, 
                WeaponDescriptions: WeaponDescriptions, 
                GeneralDescriptions: GeneralDescriptions, 
                Context: Context
                )?.Send();
        }
    }
}