using ConsoleLib.Console;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;

namespace XRL.World.Effects
{
    public class HNPS_RadarHighlightedEffect : Effect
    {
        public string TileColor;

        public HNPS_RadarHighlightedEffect()
        {
            Duration = DURATION_INDEFINITE;
        }
        public HNPS_RadarHighlightedEffect(string TileColor)
            : this()
        {
            this.TileColor = TileColor;
        }

        public override int GetEffectType()
        {
            return TYPE_EQUIPMENT;
        }
        public override bool SameAs(Effect e)
        {
            return false;
        }
        public override string GetDescription()
        {
            return null;
        }

        public bool CheckVisible()
        {
            return Object.CurrentCell.GetLight() > LightLevel.Light
                && Object.CurrentCell.GetLight() < LightLevel.Omniscient
                && !Object.HasEffect(typeof(SensePsychicEffect), fx => (fx as SensePsychicEffect).Listener == The.Player);
        }

        public override bool FinalRender(RenderEvent E, bool bAlt)
        {
            if (!TileColor.IsNullOrEmpty() && !E.UI && CheckVisible())
            {
                string color = TileColor[^1].ToString();
                E.ColorString = $"&{color}";
                E.DetailColor = $"p"; // customer color #007f7f, based on UnityEngine.Color ColorDarkCyan = new(0, 0.5f, 0.5f);
                E.CustomDraw = true;
                return false;
            }
            return true;
        }
    }
}
