using System;

using XRL;
using XRL.World;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;

namespace XRL.World.Conversations.Parts
{
    [Serializable]
    public class SonorousRumble : IConversationPart
    {
        public override bool WantEvent(int ID, int Propagation)
        {
            return base.WantEvent(ID, Propagation)
                || ID == EnteredElementEvent.ID;
        }

        public override bool HandleEvent(EnteredElementEvent E)
        {
            if (The.Speaker.TryGetPart(out GigantismPlus speakerGigantism) && !The.Player.HasPart<GigantismPlus>())
            {
                Rumble(speakerGigantism.Level, 0.5f, 2.5f);
            }
            return base.HandleEvent(E);
        }
    }
}