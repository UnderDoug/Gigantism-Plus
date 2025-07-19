using System;

using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModImprovedManagedBurrowingClaws : ModImprovedMutationBase<UD_ManagedBurrowingClaws>
    {
        public ModImprovedManagedBurrowingClaws()
        {
        }

        public ModImprovedManagedBurrowingClaws(int Tier)
            : base(Tier)
        {
        }
    }
}