using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using XRL.Liquids;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public static class Secrets
    {
        private static string[] BecomingStrings = new string[4]
        {
            "THE FINAL KING OF QUD",
            "A SHINING GOLDEN GOD",
            "A GOLD PLUS VIP MEMBER",
            "REALLY REALLY YELLOW"
        };

        private static string[] GoldExoframeNames = new string[4]
        {
            "EMPORER'S THRONE",
            "SHINY METAL ASS",
            "ULTIMATE EXOFRAME",
            "MILLION DRAM MACHINE"
        };

        public static void Become(GameObject User, string Model, GameObject ImplantObject)
        {
            if (Model == "YES")
            {
                int hadFrameCount = User.GetIntProperty(SecretExoframePart.HadFrameCountProperty);
                if (User == The.Player && hadFrameCount == 0)
                {
                    User.SetIntProperty(SecretExoframePart.HadFrameCountProperty, 3);
                    int modRnd = BecomingStrings.Length;
                    int index = RndGP.Next() % modRnd;
                    Popup.Show("...");
                    Popup.Show("You... You've done it...");
                    Popup.Show("...You've {{W|really}} done it...");
                    Popup.Show("At last, you have {{c|become}}...");
                    string finalMessage = "{{SECRETGOLDEN|";
                    finalMessage += BecomingStrings[index];
                    finalMessage += "}}";
                    Popup.Show(finalMessage);
                }
                User.RequirePart<SecretExoframePart>();
                if (User.TryGetPart(out SecretExoframePart secretExoframePart))
                {
                    hadFrameCount = User.GetIntProperty(SecretExoframePart.HadFrameCountProperty);
                    secretExoframePart.HadFrameCount = hadFrameCount;

                    int modRnd = GoldExoframeNames.Length;
                    int index = RndGP.Next() % modRnd;
                    secretExoframePart.exoframeObject = ImplantObject;
                    secretExoframePart.exoframeObject.DisplayName = "{{SECRETGOLDEN|" + GoldExoframeNames[index] + "}}";

                    secretExoframePart.OldBleedLiquid = User.GetStringProperty("BleedLiquid");
                    secretExoframePart.OldBleedPrefix = User.GetStringProperty("BleedPrefix");
                    secretExoframePart.OldBleedColor = User.GetStringProperty("BleedColor");
                    User.SetStringProperty("BleedLiquid", "secretliquid-1000");
                    User.SetStringProperty("BleedPrefix", "{{SECRETGOLDEN|shiny}}");
                    User.SetStringProperty("BleedColor", "&W");
                }
                User.RequirePart<Preacher>();
                if (User.TryGetPart(out Preacher preacher))
                {
                    string preach = "{{SECRETGOLDEN|I AM BECOME GOLDEN, SHINER OF {{GOLDENSECRET|" + User.GetCurrentZone().ZoneID + "}}!!}}";
                    preacher.Lines = new string[1] { preach };
                    preacher.Prefix = "=subject.T= =verb:yell= {{SECRETGOLDEN|";
                    preacher.PreacherHomily(User, false);
                    preacher.Lines = Array.Empty<string>();

                    preacher.Chance = 16;
                    preacher.ChatWait = 250;
                    preacher.Book = "SECRETGOLDEN QUOTES";
                    preacher.inOrder = false;
                }
            }
        }
        public static void Unbecome(GameObject User, string Model, GameObject ImplantObject)
        {
            if (Model == "YES")
            {
                string exoframeDisplayName = "{{SECRETGOLDEN|THE GIGANTIC EXOFRAME}}";
                if (User.TryGetPart(out SecretExoframePart secretExoframePart))
                {
                    User.SetStringProperty("BleedLiquid", secretExoframePart.OldBleedLiquid, RemoveIfNull: true);
                    User.SetStringProperty("BleedPrefix", secretExoframePart.OldBleedPrefix, RemoveIfNull: true);
                    User.SetStringProperty("BleedColor", secretExoframePart.OldBleedColor, RemoveIfNull: true);

                    int hadFrameCount = secretExoframePart.HadFrameCount - 1;
                    User.SetIntProperty(SecretExoframePart.HadFrameCountProperty, hadFrameCount, RemoveIfZero: true);

                    exoframeDisplayName = secretExoframePart.exoframeObject.ShortDisplayName;
                }
                User.RequirePart<SecretExoframePart>();
                User.RemovePart<SecretExoframePart>();

                if (User == The.Player)
                {
                    Popup.Show("Oh! To have tasted sweet {{SECRETGOLDEN|ambrosia}}...");
                }
                if (User.TryGetPart(out Preacher preacher))
                {
                    preacher.Lines = Array.Empty<string>();
                    string preach = "{{SECRETGOLDEN|I AM REDUCED AGAIN TO NOTHING, WITHOUT MY " + exoframeDisplayName + "!!}}";
                    preacher.Lines = new string[1] { preach };
                    preacher.PreacherHomily(User, false);
                }
                User.RequirePart<Preacher>();
                User.RemovePart<Preacher>();
            }

        }

        [Serializable]
        public class SecretExoframePart : IPart
        {
            public GameObject exoframeObject = null;
            public static string HadFrameCountProperty = "HADTHEGIGANTICEXOFRAME";
            public int HadFrameCount = 0;

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
        }
    }

    [IsLiquid]
    public class SecretLiquid : BaseLiquid
    {
        public SecretLiquid() 
            : base("secretliquid")
        {
            DefaultColors = new List<string>(3) { "Y", "W", "O" };
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
                Liquid.AddLight(0);
            }
        }

        public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
        {
            Message.Compound("Mmm-mm! Tastes {{SECRETGOLDEN|WEALTHY!}}");
            return true;
        }

        public override void ObjectInCell(LiquidVolume Liquid, GameObject GO)
        {
            base.ObjectInCell(Liquid, GO);
            if (Liquid.IsOpenVolume() && GO.IsAlive)
            {
                GO.Heal(15, Message: false, FloatText: true, RandomMinimum: true);
            }
        }
    }
}