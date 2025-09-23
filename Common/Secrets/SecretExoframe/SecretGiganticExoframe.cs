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
using XRL.Liquids;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

using static XRL.World.Parts.SecretExoframeColorizer;

namespace HNPS_GigantismPlus
{
    [HasWishCommand]
    [Serializable]
    public static class SecretGiganticExoframe
    {
        private static readonly List<string> BecomingStrings = new()
        {
            "THE FINAL KING OF QUD",
            "A SHINING GOLDEN GOD",
            "A GOLD PLUS VIP MEMBER",
            "REALLY REALLY YELLOW",
            "A CRITIQUE OF CAPITALISM",
        };

        private static readonly List<string> GoldExoframeNames = new()
        {
            "EMPORER'S THRONE",
            "SHINY METAL ASS",
            "ULTIMATE EXOFRAME",
            "MILLION DRAM MACHINE",
            "OBSCENE WEALTH",
            "OVERCOMPENSATION",
        };

        public static void Becoome(GameObject Becoomer, string Model, GameObject ImplantObject)
        {
            if (Model == "YES")
            {
                int hadFrameCount = Becoomer.GetIntProperty(HadFrameCountProperty);

                if (Becoomer.IsPlayerControlled() && hadFrameCount == 0)
                {
                    Becoomer.SetIntProperty(HadFrameCountProperty, 3);
                    Popup.Show("...");
                    Popup.Show("You... You've done it...");
                    Popup.Show("...You've {{W|really}} done it...");
                    Popup.Show("At last, you have {{c|become}}...");
                    string finalMessage = "{{SECRETGOLDEN|";
                    finalMessage += BecomingStrings.GetRandomElementCosmetic();
                    finalMessage += "}}";
                    Popup.Show(finalMessage);
                }

                SecretExoframeColorizer secretExoframeColorizer = Becoomer.RequirePart<SecretExoframeColorizer>();
                hadFrameCount = Becoomer.GetIntProperty(HadFrameCountProperty);
                secretExoframeColorizer.HadExoframeCount = hadFrameCount;
                secretExoframeColorizer.ExoframeObject = ImplantObject;
                secretExoframeColorizer.ExoframeObject.DisplayName = "{{SECRETGOLDEN|" + GoldExoframeNames.GetRandomElementCosmetic() + "}}";

                secretExoframeColorizer.OldBleedLiquid = Becoomer.GetStringProperty("BleedLiquid");
                secretExoframeColorizer.OldBleedPrefix = Becoomer.GetStringProperty("BleedPrefix");
                secretExoframeColorizer.OldBleedColor = Becoomer.GetStringProperty("BleedColor");
                Becoomer.SetStringProperty("BleedLiquid", "secretliquid-1000");
                Becoomer.SetStringProperty("BleedPrefix", "{{SECRETGOLDEN|shiny}}");
                Becoomer.SetStringProperty("BleedColor", "&W");

                Becoomer.RequirePart<Preacher>();
                if (Becoomer.TryGetPart(out Preacher preacher))
                {
                    string preach = "{{SECRETGOLDEN|I AM BECOME GOLDEN, SHINER OF {{GOLDENSECRET|" + Becoomer?.GetCurrentZone()?.ZoneID + "}}!!}}";
                    preacher.Lines = new string[1] { preach };
                    preacher.Prefix = "=subject.T= =verb:yell= {{SECRETGOLDEN|'";
                    preacher.PreacherHomily(Becoomer, false);
                    preacher.Lines = Array.Empty<string>();

                    preacher.Chance = 16;
                    preacher.ChatWait = 250;
                    preacher.Book = "SECRETGOLDEN QUOTES";
                    preacher.inOrder = false;
                }
            }
        }
        public static void Unbecoome(GameObject Unbecoomer, string Model, GameObject ImplantObject)
        {
            if (Model == "YES" && Unbecoomer.TryGetPart(out SecretExoframeColorizer secretExoframeColorizer))
            {
                string exoframeDisplayName = secretExoframeColorizer.ExoframeObject.ShortDisplayName;

                Unbecoomer.SetStringProperty("BleedLiquid", secretExoframeColorizer.OldBleedLiquid, RemoveIfNull: true);
                Unbecoomer.SetStringProperty("BleedPrefix", secretExoframeColorizer.OldBleedPrefix, RemoveIfNull: true);
                Unbecoomer.SetStringProperty("BleedColor", secretExoframeColorizer.OldBleedColor, RemoveIfNull: true);

                int hadFrameCount = secretExoframeColorizer.HadExoframeCount - 1;
                Unbecoomer.SetIntProperty(HadFrameCountProperty, hadFrameCount, RemoveIfZero: true);

                Unbecoomer.RemovePart(secretExoframeColorizer);

                if (Unbecoomer.IsPlayerControlled())
                {
                    Popup.Show("Oh! To have tasted sweet {{SECRETGOLDEN|ambrosia}}...");
                }
                if (Unbecoomer.TryGetPart(out Preacher preacher))
                {
                    preacher.Lines = Array.Empty<string>();
                    string preach = "{{SECRETGOLDEN|I AM REDUCED AGAIN TO NOTHING, WITHOUT MY " + exoframeDisplayName + "!!}}";
                    preacher.Lines = new string[1] { preach };
                    preacher.PreacherHomily(Unbecoomer, false);
                    Unbecoomer.RemovePart(preacher);
                }
            }
        }

        [WishCommand(Command = "I AM BECOME")]
        public static void Become()
        {
            Become("");
        }

        [WishCommand(Command = "I AM BECOME")]
        public static void Become(string Degree)
        {
            List<string> degrees = new() { "SHINY", "SHINIER", "SHINIEST" };
            Degree = Degree.ToUpper();
            if (Degree != "" && !degrees.Contains(Degree))
            {
                Popup.Show(
                    $"ARISTOCRAT".Color("GOLDENSECRET") + ", " +
                    $"THOU MOST CERTAINLY ARE NOT BECOME ".Color("SECRETGOLDEN") +
                    $"{Degree}".Color("R"));
                return;
            }

            BodyPart body = The.Player?.Body?.GetBody();
            GameObject cybernetics = body.Cybernetics;
            if (cybernetics == null) goto DoImplant;
            if (cybernetics.TryGetPart(out CyberneticsGiganticExoframe exoframe))
            {
                if (exoframe.Model == "YES")
                {
                    Popup.Show($"DOTH THINE {cybernetics.ShortDisplayName} NOT SUFFICE?".Color("SECRETGOLDEN"));
                    goto CalcDegrees;
                }
                body.Unimplant();
            }

        DoImplant:
            GameObject secretGiganticExoframe = GameObjectFactory.create("THEGIGANTICEXOFRAME");
            body.Implant(secretGiganticExoframe);

        CalcDegrees:
            int xpToAward = 0;
            List<string> skillsToLearn = new()
            {
                "Acrobatics",
                "Acrobatics_Jump",
                "Cudgel",
                "Cudgel_Expertise",
                "Endurance",
                "Endurance_ShakeItOff"
            };
            List<string> shinySkills = new()
            {
                "Acrobatics_SwiftReflexes",
                "Cudgel_Bludgeon",
                "Cudgel_Slam",
                "SingleWeaponFighting",
                "SingleWeaponFighting_OpportuneAttacks"
            };
            List<string> shinierSkills = new()
            {
                "Acrobatics_Dodge",
                "Endurance_Weathered",
                "Tactics",
                "Tactics_Charge",
                "Cudgel_ChargingStrike",
                "Cudgel_Backswing",
                "SingleWeaponFighting_WeaponExpertise"
            };
            List<string> shiniestSkills = new()
            {
                "Endurance_Calloused",
                "Tactics_Juke",
                "Cudgel_Conk",
                "Cudgel_SmashUp",
                "SingleWeaponFighting_PenetratingStrikes",
                "SingleWeaponFighting_WeaponMastery"
            };
            SortedDictionary<int, (string, int, List<string>)> Shininesses = new()
            {
                { 1, ("SHINY",     15100, shinySkills) },
                { 2, ("SHINIER",  219375, shinierSkills) },
                { 3, ("SHINIEST", 725625, shiniestSkills) }
            };

            if (!degrees.Contains(Degree)) goto DoDegrees;
            foreach ((int Key, (string Shininess, int XpToAward, List<string> Skills)) in Shininesses)
            {
                xpToAward += XpToAward;
                List<string> skillsList = new(skillsToLearn.Count + Skills.Count);
                skillsList.AddRange(skillsToLearn);
                skillsList.AddRange(Skills);
                skillsToLearn = skillsList;
                if (Degree == Shininess) goto DoDegrees;
            }

        DoDegrees:
            Popup.Suppress = true;
            The.Player.AwardXP(xpToAward);
            Popup.Suppress = false;
            foreach (string skill in skillsToLearn)
            {
                The.Player.AddSkill(skill);
            }

            The.Player.GigantifyInventory(EnableGiganticStartingGear, EnableGiganticStartingGear_Grenades);
        }
    }
}