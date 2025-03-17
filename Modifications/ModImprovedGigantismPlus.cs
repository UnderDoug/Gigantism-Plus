using System;
using XRL.World.Parts.Mutation;

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