using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public class GetGiganticCreatureHandler : IEventHandler, IModEventHandler<GetGiganticCreatureEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(GetGiganticCreatureHandler));

        private static readonly GetGiganticCreatureHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, GetGiganticCreatureEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<GetGiganticCreatureHandler, GetGiganticCreatureEvent>();
        }

        public bool HandleEvent(GetGiganticCreatureEvent E)
        {
            return true;
        }
    }
    public class SetGiganticCreatureHandler : IEventHandler, IModEventHandler<SetGiganticCreatureEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(SetGiganticCreatureHandler));

        private static readonly SetGiganticCreatureHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, GetGiganticCreatureEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<SetGiganticCreatureHandler, SetGiganticCreatureEvent>();
        }

        public bool HandleEvent(GetGiganticCreatureEvent E)
        {
            return true;
        }
    }

    public class GetGiganticEquipmentHandler : IEventHandler, IModEventHandler<GetGiganticEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(GetGiganticEquipmentHandler));

        private static readonly GetGiganticEquipmentHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, GetGiganticEquipmentEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<GetGiganticEquipmentHandler, GetGiganticEquipmentEvent>();
        }

        public bool HandleEvent(GetGiganticEquipmentEvent E)
        {
            return true;
        }
    }
    public class SetGiganticEquipmentHandler : IEventHandler, IModEventHandler<SetGiganticEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(SetGiganticEquipmentHandler));

        private static readonly SetGiganticEquipmentHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, GetGiganticEquipmentEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<SetGiganticEquipmentHandler, SetGiganticEquipmentEvent>();
        }

        public bool HandleEvent(GetGiganticEquipmentEvent E)
        {
            return true;
        }
    }
}
