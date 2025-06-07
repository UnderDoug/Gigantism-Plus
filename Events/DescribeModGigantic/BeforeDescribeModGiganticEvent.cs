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
    public class BeforeDescribeModGiganticEvent : IDescribeModGiganticEvent<BeforeDescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModGiganticEvent));

        public BeforeDescribeModGiganticEvent()
        {
        }

        public static BeforeDescribeModGiganticEvent FromPool(
            GameObject Object, 
            string ObjectNoun, 
            List<DescriptionElement> WeaponDescriptions, 
            List<DescriptionElement> GeneralDescriptions, 
            string Context = null)
        {
            BeforeDescribeModGiganticEvent E = FromPool();
            if (E !=null)
            {
                E.Object = Object;
                E.ObjectNoun = ObjectNoun;
                E.WeaponDescriptions = WeaponDescriptions ?? new();
                E.GeneralDescriptions = GeneralDescriptions ?? new();
                E.Context = Context;
            }
            return E;
        }
        public static BeforeDescribeModGiganticEvent Send(
            GameObject Object,
            string ObjectNoun = null,
            List<DescriptionElement> WeaponDescriptions = null,
            List<DescriptionElement> GeneralDescriptions = null,
            string Context = null)
        {
            return FromPool(
                Object: Object, 
                ObjectNoun: ObjectNoun, 
                WeaponDescriptions: WeaponDescriptions, 
                GeneralDescriptions: GeneralDescriptions, 
                Context: Context
                )?.Send();
        }
    }
}