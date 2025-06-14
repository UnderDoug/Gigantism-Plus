﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Qud.API;

using XRL;
using XRL.World;
using XRL.Names;
using XRL.Liquids;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.ObjectBuilders;
using XRL.World.ZoneBuilders;
using XRL.World.WorldBuilders;
using XRL.Wish;

using static XRL.World.ZoneBuilderPriority;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Skills.Cooking
{
    [HasWishCommand]
    public class SeriouslyThickStew : CookingRecipe
    {
        public SeriouslyThickStew()
        {
            Components.Add(new PreparedCookingRecipieComponentLiquid(LiquidNeutronFlux.ID));
            Components.Add(new PreparedCookingRecipeUnusualComponentBlueprint("Plump Mushroom"));
            Components.Add(new PreparedCookingRecipeUnusualComponentBlueprint("HulkHoneyTonic"));
            Components.Add(new PreparedCookingRecipeUnusualComponentBlueprint("GravityGrenade3"));
            Effects.Add(new CookingRecipeResultProceduralEffect(ProceduralCookingEffect.CreateSpecific(new List<string> { "CookingDomainSpecial_UnitGigantismTransform" })));
        }
        public override string GetDescription()
        {
            return "?????";
        }

        public override string GetApplyMessage()
        {
            return "";
        }

        public override string GetDisplayName()
        {
            return $"Seriously Thick Stew".OptionalColor("yuge", "w", Colorfulness);
        }

        public override bool ApplyEffectsTo(GameObject target, bool showMessage = true)
        {
            string text = "";
            if (showMessage)
            {
                text = GetApplyMessage();
            }
            foreach (ICookingRecipeResult effect in Effects)
            {
                text += effect.apply(target);
                text += "\n";
            }
            return true;
        }

        [WishCommand(Command = SCRT_GNT_RECIPE)]
        public static void WishSeriouslyThickStew()
        {
            CookingDomainSpecial_UnitGigantismTransform.ApplyTo(The.Player);
        }
        [WishCommand(Command = "gains")]
        public static void WishGains()
        {
            WishSeriouslyThickStew();
        }
        [WishCommand(Command = "thicc")]
        public static void WishThicc()
        {
            WishSeriouslyThickStew();
        }
    } //!-- public class SeriouslyThickStew : CookingRecipe
}
