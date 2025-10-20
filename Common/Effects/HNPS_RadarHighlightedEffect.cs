using ConsoleLib.Console;
using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.Rules;
using XRL.World;

namespace XRL.World.Effects
{
    public class HNPS_RadarHighlightedEffect : Effect
    {
        public string TileColor;
        public string DetailColor;

        public HNPS_RadarHighlightedEffect()
        {
            Duration = DURATION_INDEFINITE;
            TileColor = null;
            DetailColor = null;
        }
        public HNPS_RadarHighlightedEffect(string TileColor)
            : this()
        {
            this.TileColor = TileColor;
        }
        public HNPS_RadarHighlightedEffect(string TileColor, string DetailColor)
            : this(TileColor)
        {
            this.DetailColor = DetailColor;
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

        public bool CheckVisible(LightLevel Lit)
        {
            return Lit > LightLevel.Light
                && Lit < LightLevel.LitRadar
                && !(Object?.HasEffect(typeof(SensePsychicEffect), fx => (fx as SensePsychicEffect).Listener == The.Player)).GetValueOrDefault();
        }

        public override bool FinalRender(RenderEvent E, bool bAlt)
        {
            if (GameObject.Validate(Object)
                && !bAlt 
                && (!TileColor.IsNullOrEmpty() || !DetailColor.IsNullOrEmpty()) 
                && !E.UI && CheckVisible(E.Lit)
                && XRLCore.CurrentFrame % 60 != Stat.RandomCosmetic(0, 59))
            {
                string tileColor = "P"; // custom color #00ffff, based on UnityEngine.Color ColorBrightCyan = new(0, 1f, 1f);
                string detailColor = "p"; // custom color #007f7f, based on UnityEngine.Color ColorDarkCyan = new(0, 0.5f, 0.5f);
                if (!TileColor.IsNullOrEmpty())
                {
                    tileColor = TileColor[^1].ToString();
                }
                if (!DetailColor.IsNullOrEmpty())
                {
                    detailColor = DetailColor[^1].ToString();
                }
                E.ColorString = $"&{tileColor}";
                E.DetailColor = $"{detailColor}";
                E.CustomDraw = true;
                return false;
            }
            return true;
        }
    }
}
