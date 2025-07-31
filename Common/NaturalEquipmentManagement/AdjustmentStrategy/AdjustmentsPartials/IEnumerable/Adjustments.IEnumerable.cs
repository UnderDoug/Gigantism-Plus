using System.Collections;

using XRL;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
