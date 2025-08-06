using System.Collections.Generic;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IReadOnlyCollection<IAdjustment>
    {
        public int Count => Length;

        public Adjustments(IReadOnlyCollection<IAdjustment> Conditions)
            : this(Conditions as IReadOnlyList<IAdjustment>)
        {
        }
    }
}
