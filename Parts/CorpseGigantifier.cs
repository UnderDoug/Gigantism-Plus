using System;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class CorpseGigantifier : IScribedPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == DroppedEvent.ID;
        }

        public override bool HandleEvent(DroppedEvent E)
        {
            if (E.Actor.HasPart<GigantismPlus>() && E.Item.InheritsFrom("Corpse"))
            {
                Debug.Entry(4,
                    $"{typeof(CorpseGigantifier).Name}." +
                    $"{nameof(HandleEvent)}({typeof(CorpseGigantifier).Name} E) " + 
                    $"E.Actor: [{E.Actor.ID}:{E.Actor.ShortDisplayNameStripped}], " + 
                    $"E.Item: [{E.Item.ID}:{E.Item.ShortDisplayNameStripped}]",
                    Indent: 0);
                ModGigantic modGigantic = new();
                E.Item.ApplyModification(modGigantic);
                E.Item.RemovePart(this);
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    } //!-- public class CorpseGigantifier : IScribedPart
}
