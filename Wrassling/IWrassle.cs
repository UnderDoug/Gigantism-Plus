using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Capabilities;

using static XRL.UD_QudWrasslingEntertainment;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World
{
    public interface IWrassle
        : IModEventHandler<AddWrassleIDEvent>
        , IModEventHandler<UpdateWrassleIDEvent>
        , IModEventHandler<WrassleIDUpdatedEvent>
    {
        public virtual WrassleID WrassleID => GetWrassleID();

        public virtual string PrimaryColor => WrassleID?.PrimaryColor;
        public virtual string SecondaryColor => WrassleID?.SecondaryColor;

        public abstract WrassleID GetWrassleID();
        public abstract WrassleID SetWrassleID(Guid WrassleID);
        public abstract WrassleID SetWrassleID(WrassleID WrassleID);
        public abstract WrassleID SetWrassleID(IWrassle WrasslePart);
        public abstract void ClearWrassleID();
        public abstract bool ClearCachedWrassleID();
        public abstract Guid NewWrassleID();
        public abstract void OnUpdatedWrassleID();
    }
}
