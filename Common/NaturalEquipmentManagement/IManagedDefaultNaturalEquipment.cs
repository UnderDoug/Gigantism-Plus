using System;
using System.Collections.Generic;

using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World
{
    public interface IManagedDefaultNaturalEquipment 
        : IModEventHandler<GetNaturalEquipmentOperatorsEvent>
        , IModEventHandler<BeforeUpdateBodyPartsEvent>
        , IModEventHandler<BodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
        , IModEventHandler<GetNaturalEquipmentModsEvent>
        , IModEventHandler<BeforeManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<ManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<AfterManageDefaultNaturalEquipmentEvent>
        , IModEventHandler<BeforeRapidAdvancementEvent>
        , IModEventHandler<AfterRapidAdvancementEvent>
    {
        public int Level { get; set; }

        public abstract void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Operator, BodyPart TargetBodyPart);
    }
}