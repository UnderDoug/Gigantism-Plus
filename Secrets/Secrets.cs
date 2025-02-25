using System;
using System.Collections.Generic;
using System.Linq;
using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;
using static Mods.GigantismPlus.HelperMethods;

namespace Mods.GigantismPlus
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

        public static void Become(GameObject User, string Model)
        {
            if (Model == "YES")
            {
                if (User == The.Player)
                {
                    int index = Stat.TinkerRandom(0, BecomingStrings.Length - 1);
                    Popup.Show("...");
                    Popup.Show("You... You've done it...");
                    Popup.Show("...You've {{W|really}} done it...");
                    Popup.Show("At last, you have {{c|become}}...");
                    string finalMessage = "{{Y-W-W-W-O-O-O distribution|";
                    finalMessage += BecomingStrings[index];
                    finalMessage += "}}";
                    Popup.Show(finalMessage);
                }
                User.RequirePart<SecretExoframePart>();
            }
        }
        public static void Unbecome(GameObject User, string Model)
        {
            if (Model == "YES")
            {
                User.RequirePart<SecretExoframePart>();
                User.RemovePart<SecretExoframePart>();
                if (User == The.Player)
                {
                    Popup.Show("Oh! To have tasted sweet {{Y-W-W-W-O-O-O distribution|ambrosia}}...");
                }
            }

        }

        [Serializable]
        public class SecretExoframePart : IPart
        {
            public static readonly int ICON_COLOR_PRIORITY = 999;
            private bool MutationColor = XRL.UI.Options.MutationColor;
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
}