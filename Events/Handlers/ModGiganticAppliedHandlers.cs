using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

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

            if (Object.InheritsFrom("FoldingChair"))
            {
                Object.SetIntProperty("IsImprovisedMelee", 0, true);
                Object.SetStringProperty("ShowMeleeWeaponStats", "true");
                MeleeWeapon meleeWeapon = Object.RequirePart<MeleeWeapon>();
                meleeWeapon.BaseDamage = "3d3";
                meleeWeapon.MaxStrengthBonus = 8;
                meleeWeapon.Skill = "Cudgel";
                meleeWeapon.Stat = "Strength";

                Object.SetIntProperty("IsImprovisedThrown", 0, true);
                ThrownWeapon thrownWeapon = Object.RequirePart<ThrownWeapon>();
                thrownWeapon.Damage = "3d3";
                thrownWeapon.Penetration = 8;
                thrownWeapon.Attributes = "Cudgel";
            }

            if (Object.TryGetPart(out LightSource lightSource))
            {
                lightSource.Radius *= 2;
            }

            if (Object.TryGetPart(out DeploymentMaintainer deploymentMaintainer))
            {
                deploymentMaintainer.Radius *= 2;
            }

            List<IActivePart> chargeUseIncrease2x = new()
            {
                Object.GetPart<Bed>(),
                Object.GetPart<Chair>(),
                Object.GetPart<ChargeSink>(),
                Object.GetPart<RealityStabilization>(),
                Object.GetPart<SolarArray>(),
                Object.GetPart<TemperatureAdjuster>(),
                Object.GetPart<LiquidProducer>(),
                Object.GetPart<Mill>(),
                Object.GetPart<RadiusEventSender>(),
                Object.GetPart<ItemConvertor>(),
                Object.GetPart<Fan>(),
                Object.GetPart<LiquidPump>(),
                Object.GetPart<RegenTank>(),
            };
            foreach (IActivePart part in chargeUseIncrease2x)
            {
                if (part != null)
                {
                    part.ChargeUse *= 2;
                }
            }
            if (Object.TryGetPart(out ConversationScript conversationScript))
            {
                conversationScript.ChargeUse *= 2;
            }

            if (Object.TryGetPart(out Capacitor capacitor))
            {
                capacitor.MaxCharge *= 2;
                capacitor.Charge *= 2;
                capacitor.ChargeRate *= 2;
            }

            if (Object.TryGetPart(out FusionReactor fusionReactor))
            {
                fusionReactor.ChargeRate *= 2;
                fusionReactor.ExplodeForce *= 2;
            }
            if (Object.TryGetPart(out ElectricalPowerTransmission electricalPowerTransmission))
            {
                electricalPowerTransmission.ChargeRate *= 2;
            }
            if (Object.TryGetPart(out HydraulicPowerTransmission hydraulicPowerTransmission))
            {
                hydraulicPowerTransmission.ChargeRate *= 2;
            }
            if (Object.TryGetPart(out BroadcastPowerReceiver broadcastPowerReceiver))
            {
                hydraulicPowerTransmission.ChargeRate *= 2;
            }

            if (Object.TryGetPart(out TemperatureAdjuster temperatureAdjuster) && Object.InheritsFrom("SolidHighTechInstallation"))
            {
                temperatureAdjuster.TemperatureAmount *= 2;
                temperatureAdjuster.TemperatureThreshold = (int)(temperatureAdjuster.TemperatureThreshold * 1.5);
            }

            if (Object.TryGetPart(out ItemConvertor itemConvertor))
            {
                if (itemConvertor.ConversionTag == "RockTumblerOutput")
                    itemConvertor.Chance *= 2;
                if (itemConvertor.ConversionTag == "WireExtruderOutput")
                    itemConvertor.GiganticFactor *= 2;
            }

            if (Object.TryGetPart(out LiquidProducer liquidProducer))
            {
                if (!liquidProducer.VariableRate.IsNullOrEmpty())
                {
                    string[] range = liquidProducer.VariableRate.Split("-");
                    int low = int.Parse(range[0]) / 2;
                    int high = int.Parse(range[1]) / 2;
                    liquidProducer.VariableRate = $"{low}-{high}";
                    liquidProducer.VariableRate.RollCached();
                }
                else
                {
                    liquidProducer.Rate /= 2;
                }
            }

            if (Object.TryGetPart(out DamageContents damageContents) && !damageContents.Damage.IsNullOrEmpty())
            {
                damageContents.Damage = $"2x{damageContents.Damage}";
            }

            if (Object.TryGetPart(out Fan fan))
            {
                fan.BlowStrength *= 2;
                fan.BlowRange = (int)(fan.BlowRange * 1.5);
            }

            if (Object.TryGetPart(out LiquidPump liquidPump))
            {
                liquidPump.Rate *= 5;
            }

            if (Object.TryGetPart(out RegenTank regenTank))
            {
                regenTank.MinTotalDrams *= 2;
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

            Enclosing Enclosing = Object.GetPart<Enclosing>();
            if (Enclosing != null)
            {
                Enclosing.AVBonus = Enclosing.AVBonus * 2;
                Enclosing.DVPenalty = (int)Math.Floor(Enclosing.DVPenalty * 1.5);
                Enclosing.ExitSaveTarget += 3;
            }
            return true;
        }
    } //!-- public class AfterModGiganticAppliedHandler : IEventHandler, IModEventHandler<AfterModGiganticAppliedEvent>
}
