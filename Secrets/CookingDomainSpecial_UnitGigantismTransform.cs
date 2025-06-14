﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsoleLib.Console;

using Qud.API;

using XRL;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.ObjectBuilders;
using XRL.World.ZoneBuilders;
using XRL.World.WorldBuilders;
using XRL.Language;
using XRL.Names;

using static XRL.World.ZoneBuilderPriority;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Effects
{
    public class CookingDomainSpecial_UnitGigantismTransform : ProceduralCookingEffectUnit
    {
        public static string JournalTextPrefix = $"You ate some {"Seriously Thick Stew".OptionalColor("yuge", "w", Colorfulness)}";
        public static List<string> JournalTexts = new()
        {
            $"{JournalTextPrefix} and achieved massive gains.",
            $"{JournalTextPrefix} and got absolutely swole.",
            $"{JournalTextPrefix} and satisfied a serious hankering.",
        };

        public static List<string> JournalMuralPrefixs = new()
        {
            $"Gains! Gains! ",
            $"Immensity!? ",
            $"O! Such ravenous hunger! ",
        };
        public static string JournalMuralInfix =
            $"On the {Calendar.GetDay()} of {Calendar.GetMonth()}, " +
            $"in the year {Calendar.GetYear()} AR";
        public static List<string> JournalMuralSuffixs = new()
        {
            ", =name= partook of the Seriously Thick Stew and achieved massive gains.",
            ", =name= learned the secret to getting most swole.",
            ", =name= ate, and ate, and ate, satisfying a serious hankering.",
        };

        public static string JournalGospelInfix =
            $"<spice.instancesOf.inYear.!random.capitalize> =year=, " + 
            $"<spice.instancesOf.afterTumultuousYears.!random>, =player.possessive= counselors suggested " + 
            $"=player.subjective= <spice.instancesOf.abdicate.!random> as sultan. " +
            $"Instead, =player.subjective= ate a meal and ";
        public static List<string> JournalGospelSuffixs = new()
        {
            $"achieved massive gains.",
            $"got swole.",
            $"satisfied a serious hankering.",
        };
        public override string GetDescription()
        {
            return "@they achieved massive gains.";
        }

        public override string GetTemplatedDescription()
        {
            return "?????";
        }

        public override void Init(GameObject target)
        {
        }

        public override void Apply(GameObject Object, Effect parent)
        {
            ApplyTo(Object);
        }

        public override void Remove(GameObject Object, Effect parent)
        {
        }

        public static void ApplyTo(GameObject Object)
        {
            if (!Object.TryGetPart(out StewBelly stewBelly))
            {
                stewBelly = Object.RequirePart<StewBelly>();
            }
            if (Object.IsPlayer() && !Object.HasPart<GigantismPlus>() && stewBelly.Stews == 0)
            {
                GigantismPlus gigantism = new();
                Popup.Show("...");
                Popup.Show("You feel... full.");
                Popup.Show("Your limbs suddenly feel constrained, like there's too much of them to fit in the space they occupy.");
                Popup.Show("You buckle under the weight of your rapidly increasing mass. The ground under your feet gives way.");
                Popup.Show("The world rushes away from you as your anatomy realizes its new stature.");
                Popup.Show($"You gained the mutation {gigantism.GetDisplayName()}!");
                JournalAPI.AddAccomplishment(
                    text: JournalTexts.GetRandomElement(), 
                    muralText: $"{JournalMuralPrefixs.GetRandomElement()}{JournalMuralInfix}{JournalMuralSuffixs.GetRandomElement()}",
                    gospelText: $"{JournalGospelInfix}{JournalGospelSuffixs.GetRandomElement()}",
                    aggregateWith: null, 
                    category: "general", 
                    muralCategory: MuralCategory.BodyExperienceNeutral, 
                    muralWeight: MuralWeight.VeryHigh, 
                    secretId: null, 
                    time: -1L);
            }
            if (!Object.IsTrueKin())
            {
                Object.Gigantify("Meal");
            }
            else
            {
                int Level = ExplodingDie(1, "1d2", Step: 1, Limit: 16, Indent: 2);
                Object.RequirePart<Mutations>().AddMutation(new GigantismPlus(), Level, true);
                string NamePrefix = "gigantic".MaybeColor("gigantic");
                
                if (!NamePrefix.IsNullOrEmpty())
                {
                    Object.RequirePart<DisplayNameAdjectives>().AddAdjective(NamePrefix);
                }
            }
            if (Object.IsPlayer())
            {
                if (stewBelly.Stews > 0)
                {
                    int breakpoint = (int)Math.Floor(stewBelly.GetLastHankering() / 2.0);
                    int hankering = stewBelly.Hankering;
                    Popup.Show("{{emote|*chew* *chew*}} That is one " + "Seriously Thick Stew".OptionalColorYuge(Colorfulness) + "!");
                    if (hankering == 1)
                    {
                        Popup.Show("That one hit the spot!");
                    }
                    if (hankering > 1 && hankering < breakpoint)
                    {
                        Popup.Show("Almost full!");
                    }
                    else
                    {
                        Popup.Show("Still have a serious hankering!");
                    }
                }
            }
            stewBelly.EatStew();
        }
    } //!-- public class CookingDomainSpecial_UnitGigantismTransform : ProceduralCookingEffectUnit
}
