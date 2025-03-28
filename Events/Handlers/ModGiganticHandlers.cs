using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL;
using XRL.World.Parts;

namespace HNPS_GigantismPlus.Events.Handlers
{
    public class AfterModGiganticAppliedHandler : IEventHandler, IModEventHandler<AfterModGiganticAppliedEvent>
    {

        private static readonly AfterModGiganticAppliedHandler Handler = new();

        public static void Register()
        {
            The.Game.RegisterEvent(Handler, AfterModGiganticAppliedEvent.ID);
            Debug.Entry(3, $"Registered", $"The.Game.RegisterEvent(Handler, {nameof(AfterModGiganticAppliedEvent)}.ID: {AfterModGiganticAppliedEvent.ID})", Indent: 2);
        }

        public bool HandleEvent(AfterModGiganticAppliedEvent E)
        {
            Debug.Entry(4, $"{typeof(AfterModGiganticAppliedHandler).Name}.{nameof(HandleEvent)}({typeof(AfterModGiganticAppliedEvent).Name} E)", Indent: 0);
            GameObject Object = E.Object;

            LightSource lightSource = Object.GetPart<LightSource>();
            if (lightSource != null)
            {
                lightSource.Radius *= 2;
            }
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

    public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
    {

        private static readonly BeforeDescribeModGiganticHandler Handler = new();

        public static void Register()
        {
            The.Game.RegisterEvent(Handler, BeforeDescribeModGiganticEvent.ID);
            Debug.Entry(3, $"Registered", $"The.Game.RegisterEvent(Handler, {nameof(BeforeDescribeModGiganticEvent)}.ID: {BeforeDescribeModGiganticEvent.ID})", Indent: 2);
        }

        public bool HandleEvent(BeforeDescribeModGiganticEvent E)
        {
            GameObject Object = E.Object;
            
            if (Object.HasPart<Backpack>())
            {
                E.GeneralDescriptions.Add(new List<string> { "support", "twice and a half as much weight" });
            }

            if (Object.HasPart<LightSource>())
            {
                E.GeneralDescriptions.Add(new List<string> { "illuminate", "twice as far" });
            }

            if (Object.GetPart<Armor>().CarryBonus > 0)
            {
                E.GeneralDescriptions.Add(new List<string> { "have", "a quarter more carry capcity" });
            }

            return true;
        }
    } //!-- public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
}
