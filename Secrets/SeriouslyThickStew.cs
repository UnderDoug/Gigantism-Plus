using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using XRL;
using XRL.World;
using XRL.World.WorldBuilders;
using XRL.World.ZoneBuilders;
using static XRL.World.ZoneBuilderPriority;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.Names;
using Qud.API;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using XRL.World.Effects;
using HNPS_GigantismPlus;
using XRL.Wish;
using XRL.Liquids;

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

        [WishCommand(Command = "SeriouslyThickStew")]
        public static void Wish()
        {
            SeriouslyThickStew stew = new();
            stew.ApplyEffectsTo(The.Player);
        }
    } //!-- public class SeriouslyThickStew : CookingRecipe
}
