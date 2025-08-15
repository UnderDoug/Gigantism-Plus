using System;
using XRL.Core;

namespace XRL.World.Parts
{
    [Serializable]
    public class SecretExoframeColorizer : IScribedPart
    {
        public GameObject ExoframeObject = null;
        public const string HadFrameCountProperty = "HADTHEGIGANTICEXOFRAME";
        public int HadExoframeCount = 0;

        public bool HasBecoome = false;

        public static readonly int ICON_COLOR_PRIORITY = 999;

        private bool MutationColor = XRL.UI.Options.MutationColor;

        public string OldBleedLiquid;

        public string OldBleedColor;

        public string OldBleedPrefix;

        public override bool Render(RenderEvent E)
        {
            bool flag = true;
            if (ParentObject.IsPlayerControlled())
            {
                if ((XRLCore.FrameTimer.ElapsedMilliseconds & 0x7F) == 0L)
                {
                    MutationColor = XRL.UI.Options.MutationColor;
                }
                if (!MutationColor)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                E.ApplyColors("&O", "W", ICON_COLOR_PRIORITY, ICON_COLOR_PRIORITY);
            }
            return base.Render(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            SecretExoframeColorizer secretExoframePart = base.DeepCopy(Parent, MapInv) as SecretExoframeColorizer;
            secretExoframePart.ExoframeObject = null;
            return secretExoframePart;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.WriteGameObject(ExoframeObject);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            ExoframeObject = Reader.ReadGameObject();
        }
    }
}