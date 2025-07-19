using System;
using System.Collections.Generic;

using XRL;
using XRL.Core;
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
        public float Delay;

        public int Times;

        public float TimeBetween;

        public float MaxTotalDuration;

        public SonorousRumble() 
        {
            Delay = 0f;
            Times = 1;
            TimeBetween = 0f;
            MaxTotalDuration = 15f;
        }

        public override bool WantEvent(int ID, int Propagation)
        {
            return base.WantEvent(ID, Propagation)
                || ID == EnteredElementEvent.ID;
        }

        public override bool HandleEvent(EnteredElementEvent E)
        {
            if (The.Speaker.TryGetPart(out GigantismPlus speakerGigantism) && !The.Player.HasPart<GigantismPlus>())
            {
                Debug.Entry(4,
                    $"@ {nameof(SonorousRumble)}."
                    + $"{nameof(HandleEvent)}({nameof(EnteredElementEvent)} E) " 
                    + $"Speaker: {The.Speaker.DebugName}",
                    Indent: 0);

                Debug.Entry(4, $"Part Paramaters:", Indent: 2);
                Debug.LoopItem(4, $"Delay", $"{Delay}", Indent: 2);
                Debug.LoopItem(4, $"Times", $"{Times}", Indent: 2);
                Debug.LoopItem(4, $"TimeBetween", $"{TimeBetween}", Indent: 2);
                Debug.LoopItem(4, $"MaxTotalDuration", $"{MaxTotalDuration}", Indent: 2);

                List<CombatJuiceEntry> combatJuiceEntries = new();

                float cumulativeDuration = 0f;
                int counter = 0;

                Debug.Entry(4, $"Accumulating alternating CombatJuice Entries", Indent: 1);
                for (int i = 0; i < Times; i++)
                {
                    Debug.Divider(4, HONLY, Count: 40, Indent: 2);
                    Debug.Entry(4, $"Iteration {i + 1}", Indent: 2);
                    CombatJuiceEntry delayEntry = new()
                    {
                        duration = i == 0 ? Delay : TimeBetween,
                    };

                    cumulativeDuration += delayEntry.duration;
                    if (cumulativeDuration > MaxTotalDuration) break;
                    combatJuiceEntries.TryAdd(delayEntry);

                    Debug.Entry(4, $"combatJuiceEntries", $"delayEntry added with duration {delayEntry.duration}", Indent: 3);
                    Debug.Entry(4, $"cumulativeDuration", $"{cumulativeDuration}", Indent: 3);
                    counter = i + 1;
                    Debug.Entry(4, $"counter", $"{counter}", Indent: 3);

                    Debug.Divider(4, HONLY, Count: 25, Indent: 3);

                    float durationMax = 2.0f;
                    float durationFactor = 0.25f;
                    int cause = speakerGigantism.Level;
                    float duration = Math.Min(durationMax, cause * durationFactor);
                    CombatJuiceEntryCameraShake shakeEntry = new(duration);

                    cumulativeDuration += shakeEntry.duration;
                    if (cumulativeDuration > MaxTotalDuration) break;
                    combatJuiceEntries.TryAdd(shakeEntry);

                    Debug.Entry(4, $"combatJuiceEntries", $"shakeEntry added with duration {shakeEntry.duration}", Indent: 3);
                    Debug.Entry(4, $"cumulativeDuration", $"{cumulativeDuration}", Indent: 3);
                    counter = i + 1;
                    Debug.Entry(4, $"counter", $"{counter}", Indent: 3);
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 2);
                if (counter < Times)
                {
                    Debug.Entry(4, $"/!\\ cumulativeDuration ({cumulativeDuration}) exceeds MaxTotalDurtion ({MaxTotalDuration})", Indent: 1);
                    Debug.CheckNah(4, $"Loop aborted on interation {counter}, {Times - counter} iterations early.", Indent: 1);
                }

                CombatJuiceEntry sonorousRumble = new()
                {
                    delayedEntries = combatJuiceEntries,
                    async = false,
                };

                CombatJuiceManager.enqueueEntry(sonorousRumble, true);

                Debug.Entry(4,
                    $"x {nameof(SonorousRumble)}."
                    + $"{nameof(HandleEvent)}({nameof(EnteredElementEvent)} E) "
                    + $"Speaker: {The.Speaker.DebugName} @//",
                    Indent: 0);
            }
            return base.HandleEvent(E);
        }
    }
}