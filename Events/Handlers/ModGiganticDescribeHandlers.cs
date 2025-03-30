using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
    {
        private static readonly BeforeDescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, BeforeDescribeModGiganticEvent.ID);

            return (bool)The.Game?.WasEventRegistered<BeforeDescribeModGiganticHandler, BeforeDescribeModGiganticEvent>(BeforeDescribeModGiganticEvent.ID);
        }

        public bool HandleEvent(BeforeDescribeModGiganticEvent E)
        {
            GameObject Object = E.Object;

            /* Check out ObjectBlueprints/Data.xml -> GigantismPlusModGiganticDescriptions
             * 
            if (Object.HasPart<LightSource>())
            {
                E.GeneralDescriptions.Add(new List<string> { "illuminate", "twice as far" });
            }
            */
            return true;
        }

    } //!-- public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>

    public class AfterDescribeModGiganticHandler : IEventHandler, IModEventHandler<AfterDescribeModGiganticEvent>
    {
        private static readonly AfterDescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, AfterDescribeModGiganticEvent.ID);

            return (bool)The.Game?.WasEventRegistered<AfterDescribeModGiganticHandler, AfterDescribeModGiganticEvent>(AfterDescribeModGiganticEvent.ID);
        }

        public bool HandleEvent(AfterDescribeModGiganticEvent E)
        {
            GameObject Object = E.Object;

            if (Object.HasPart<Backpack>())
            {
                E.GeneralDescriptions.Add(new List<string> { "support", "twice and a half as much weight" });
            }

            if (Object.HasPart<Armor>() && Object.GetPart<Armor>().CarryBonus > 0)
            {
                E.GeneralDescriptions.Add(new List<string> { "have", "a quarter more carry capcity" });
            }

            return true;
        }
    } //!-- public class AfterDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
}
