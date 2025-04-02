using System;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModImprovedGigantismPlus : ModImprovedMutationBase<GigantismPlus>
    {
        public ModImprovedGigantismPlus()
        {
        }

        public ModImprovedGigantismPlus(int Tier)
            : base(Tier)
        {
        }
    }
}