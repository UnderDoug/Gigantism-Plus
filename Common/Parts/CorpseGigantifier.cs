using System;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public class CorpseGigantifier : IScribedPart
    {
        private static bool doDebug => Options.getDoDebug(nameof(CorpseGigantifier));

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == DroppedEvent.ID;
        }
        public override bool HandleEvent(DroppedEvent E)
        {
            if (E.Actor is GameObject creature
                && E.Item is GameObject item
                && creature.IsGiganticCreature
                && item.InheritsFrom("Corpse"))
            {
                Debug.Entry(4,
                    $"{nameof(CorpseGigantifier)}." +
                    $"{nameof(HandleEvent)}({nameof(CorpseGigantifier)} E) " +
                    $"{nameof(creature)}: {creature?.DebugName ?? NULL}, " +
                    $"{nameof(item)}: {item?.DebugName ?? NULL}",
                    Indent: 0, Toggle: doDebug);
                item.ApplyModification(nameof(ModGigantic), Actor: creature);
                item.RemovePart(this);
            }
            return base.HandleEvent(E);
        }
    }
}
