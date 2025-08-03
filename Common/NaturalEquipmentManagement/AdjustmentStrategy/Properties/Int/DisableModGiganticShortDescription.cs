using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class DisableModGiganticShortDescription : SetIntProperty
    {
        public DisableModGiganticShortDescription()
            : base()
        {
            PropertyName = "ModGiganticNoShortDescription";
            Amount = 1;
        }

        public override bool Apply(GameObject Subject)
        {
            if (base.Apply(Subject))
            {
                Subject.SetIntProperty(PropertyName, (int)Amount, true);
            }
            return IsApplied();
        }
    }
}
