using System;
using System.Text;
using System.Collections.Generic;

using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Wish;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.Liquids
{
    [HasWishCommand]
    [IsLiquid]
    public class HNPS_SecretLiquid : BaseLiquid
    {
        public HNPS_SecretLiquid()
            : base("secretliquid")
        {
            DefaultColors = new(3) { "Y", "W", "O" };
            InterruptAutowalk = true;
            CirculatoryLossNoun = "gush";
            CirculatoryLossTerm = "gushing";
            Glows = true;
            Weight = 0.1;
            PureElectricalConductivity = 100;
            MixedElectricalConductivity = 100;
            Temperature = 15;
            Evaporativity = 0;
            FlameTemperature = 200;
            VaporTemperature = 200;
            Combustibility = 1;
            ThermalConductivity = 100;
            Fluidity = 200;
            Staining = 100;
            Cleansing = 5;
            SlipperyWhenWet = false;
            SlipperyWhenFrozen = false;
        }

        public override void BaseRenderPrimary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.Render.ColorString = "&Y^W";
            Liquid.ParentObject.Render.TileColor = "&Y";
            Liquid.ParentObject.Render.DetailColor = "W";
        }

        public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (Liquid.Volume < 90)
            {
                return;
            }
            if (Liquid.ParentObject.IsFrozen())
            {
                eRender.RenderString = "~";
                eRender.TileVariantColors("&Y^W", "&Y", "W");
                return;
            }
            Render render = Liquid.ParentObject.Render;
            int num = (XRLCore.CurrentFrame + Liquid.FrameOffset) % 20;
            if (Stat.RandomCosmetic(1, 300) == 1)
            {
                eRender.RenderString = "\u000f";
                eRender.TileVariantColors("&Y^W", "&Y", "W");
            }
            if (Stat.RandomCosmetic(1, 20) == 1)
            {
                if (num < 6)
                {
                    render.RenderString = "÷";
                    render.ColorString = "&Y^O";
                    render.TileColor = "&Y";
                    render.DetailColor = "O";
                }
                else if (num < 11)
                {
                    render.RenderString = "~";
                    render.ColorString = "&Y^O";
                    render.TileColor = "&Y";
                    render.DetailColor = "O";
                }
                else if (num < 16)
                {
                    render.RenderString = "\t";
                    render.ColorString = "&W^Y";
                    render.TileColor = "&W";
                    render.DetailColor = "Y";
                }
                else
                {
                    render.RenderString = "~";
                    render.ColorString = "&O^Y";
                    render.TileColor = "&O";
                    render.DetailColor = "Y";
                }
            }
        }

        public override void BaseRenderSecondary(LiquidVolume Liquid)
        {
            Liquid.ParentObject.Render.ColorString += "&W";
        }

        public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (eRender.ColorsVisible)
            {
                eRender.ColorString += "&Y";
            }
        }

        public override void RenderBackgroundPrimary(LiquidVolume Liquid, RenderEvent eRender)
        {
            if (eRender.ColorsVisible)
            {
                eRender.ColorString = "^O" + eRender.ColorString;
            }
        }

        public override string GetWaterRitualName()
        {
            return "secret liquid";
        }

        public override float GetValuePerDram()
        {
            return 9999f;
        }

        public override string GetColor()
        {
            return "O";
        }

        public override string GetAdjective(LiquidVolume Liquid)
        {
            return "{{SECRETGOLDEN|shiny}}";
        }

        public override string GetSmearedAdjective(LiquidVolume Liquid)
        {
            return "{{SECRETGOLDEN|shiny}}";
        }

        public override string GetSmearedName(LiquidVolume Liquid)
        {
            return "{{SECRETGOLDEN|shiny}}";
        }

        public override string GetStainedName(LiquidVolume Liquid)
        {
            return "{{SECRETGOLDEN|shine}}";
        }

        public override string GetName(LiquidVolume Liquid)
        {
            return "{{SECRETGOLDEN|secret liquid}}";
        }

        public override void BeforeRender(LiquidVolume Liquid)
        {
            if (!Liquid.Sealed || Liquid.LiquidVisibleWhenSealed)
            {
                Liquid.AddLight(1);
            }
        }

        public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
        {
            if (Target.IsPlayer())
            {
                if (!Target.IsTrueKin())
                {
                    Message.Compound("Mmm-mm! Tastes {{SECRETGOLDEN|WEALTHY!}}");
                }
                else
                {
                    Message.Compound("SWEET {{SECRETGOLDEN|AMBROSIA}}!");
                }
            }
            if (Liquid.IsPureLiquid("secretliquid") && Target.IsAlive)
            {
                Statistic hitpoints = Target.GetStat("Hitpoints");
                int healAmount = (int)Math.Max(1.0, hitpoints != null ? hitpoints.BaseValue * 0.5 : 75.0);
                Target.Heal(healAmount, Message: true, FloatText: true, RandomMinimum: true);
            }
            return true;
        }

        public override void ObjectInCell(LiquidVolume Liquid, GameObject GO)
        {
            if (Liquid.IsPureLiquid("secretliquid") && Liquid.IsOpenVolume() && GO.IsAlive)
            {
                Statistic hitpoints = GO.GetStat("Hitpoints");
                int healAmount = (int)Math.Max(1.0, hitpoints != null ? hitpoints.BaseValue * 0.01 : 15.0);
                GO.Heal(healAmount, Message: false, FloatText: true, RandomMinimum: true);
            }
            base.ObjectInCell(Liquid, GO);
        }
    }
}