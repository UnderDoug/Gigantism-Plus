using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public interface IDescribeModificationHandler<T>
        : IModEventHandler<BeforeDescribeModificationEvent<T>>
        , IModEventHandler<DescribeModificationEvent<T>>
        where T : IModification
    {
    }
}
