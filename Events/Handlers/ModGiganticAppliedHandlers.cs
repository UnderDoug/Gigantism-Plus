using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public class BeforeModGiganticAppliedHandler : IEventHandler, IModEventHandler<BeforeModGiganticAppliedEvent>
    {
        private static readonly BeforeModGiganticAppliedHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, BeforeModGiganticAppliedEvent.ID);
            
            return (bool)The.Game?.WasModEventHandlerRegistered<BeforeModGiganticAppliedHandler, BeforeModGiganticAppliedEvent>();
        }

        public bool HandleEvent(BeforeModGiganticAppliedEvent E)
        {
            GameObject Object = E.Object;

            LightSource lightSource = Object.GetPart<LightSource>();
            if (lightSource != null)
            {
                lightSource.Radius *= 2;
            }
            return true;
        }
    } //!-- public class BeforeModGiganticAppliedHandler : IEventHandler, IModEventHandler<BeforeModGiganticAppliedEvent>

    public class AfterModGiganticAppliedHandler : IEventHandler, IModEventHandler<AfterModGiganticAppliedEvent>
    {
        private static readonly AfterModGiganticAppliedHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, AfterModGiganticAppliedEvent.ID);

            return (bool)The.Game?.WasModEventHandlerRegistered<AfterModGiganticAppliedHandler, AfterModGiganticAppliedEvent>();
        }

        public bool HandleEvent(AfterModGiganticAppliedEvent E)
        {
            GameObject Object = E.Object;

            Backpack backpack = Object.GetPart<Backpack>();
            if (backpack != null)
            {
                backpack.WeightWhenWorn = (int)(backpack.WeightWhenWorn * 2.5f);
            }

            Armor Armor = Object.GetPart<Armor>();
            if (Armor != null && Armor.CarryBonus > 0)
            {
                Armor.CarryBonus = (int)(Armor.CarryBonus * 1.25f);
            }
            return true;
        }
    } //!-- public class AfterModGiganticAppliedHandler : IEventHandler, IModEventHandler<AfterModGiganticAppliedEvent>
}
