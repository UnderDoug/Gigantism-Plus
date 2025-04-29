using System;

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
            Delay = 0;
            Times = 1;
            TimeBetween = 0;
            MaxTotalDuration = 15;
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
                    $"{nameof(SonorousRumble)}." +
                    $"{nameof(HandleEvent)}({nameof(EnteredElementEvent)} E) " +
                    $"Speaker: {The.Speaker.DebugName}",
                    Indent: 0);
                float startTime = XRLCore.FrameTimer.ElapsedMilliseconds;
                float testTime = startTime;
                float currentTime = startTime;
                int times = Times;
                MaxTotalDuration = 15.0f;
                Debug.Entry(4, $"startTime", $"{startTime}", Indent: 1);
                Debug.Entry(4, $"testTime", $"{testTime}", Indent: 1);
                Debug.Entry(4, $"currentTime", $"{currentTime}", Indent: 1);
                Debug.Entry(4, $"times", $"{times}", Indent: 1);
                Debug.Entry(4, $"MaxTotalDuration", $"{MaxTotalDuration}", Indent: 1);

                Debug.Entry(4, $"> while (times > 0 && (currentTime - startTime) < (MaxTotalDuration * 1000))", Indent: 1);
                Debug.Divider(4, HONLY, Count: 40, Indent: 1);
                while (times > 0 && (currentTime - startTime) < (MaxTotalDuration * 1000))
                {
                    currentTime = XRLCore.FrameTimer.ElapsedMilliseconds;
                    float testDuration = 1000 * (times == Times ? Delay : TimeBetween);
                    float elapsedTime = currentTime - testTime;
                    if ((XRLCore.FrameTimer.ElapsedMilliseconds % 2000L) == 0L) Debug.Entry(4, $"testDuration: {testDuration}, elapsedTime: {elapsedTime}", Indent: 2);

                    if (elapsedTime > testDuration)
                    {
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                        Debug.Entry(4, $"Rumble!", Indent: 2);
                        times--;
                        testTime = currentTime + (1000 * Rumble(speakerGigantism.Level, 0.2f, 2.0f));
                        Debug.Entry(4, $"times", $"{times}", Indent: 2);
                        Debug.Entry(4, $"testTime", $"{testTime}", Indent: 2);
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2);
                    }
                }
                Debug.Divider(4, HONLY, Count: 40, Indent: 1);
                Debug.Entry(4, $"x while (times > 0 && (currentTime - startTime) < (MaxTotalDuration * 1000)) >//", Indent: 1);
            }
            return base.HandleEvent(E);
        }
    }
}