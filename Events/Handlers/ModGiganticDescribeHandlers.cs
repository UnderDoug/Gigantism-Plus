using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace HNPS_GigantismPlus
{
    public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModGiganticHandler));

        private static readonly BeforeDescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, BeforeDescribeModGiganticEvent.ID);

            return (bool)The.Game?.WasModEventHandlerRegistered<BeforeDescribeModGiganticHandler, BeforeDescribeModGiganticEvent>();
        }

        public bool HandleEvent(BeforeDescribeModGiganticEvent E)
        {
            GameObject Object = E.Object;

            /* Check out ObjectBlueprints/Data.xml -> GigantismPlusModGiganticDescriptions
             * 
            if (Object.HasPart<LightSource>())
            {
                E.GeneralDescriptions.Add(new List<string> { "illuminate", "twice as far" });
            }
            */

            if (Object.InheritsFrom("FoldingChair"))
            {
                E.AddWeaponDescription(null, "the ultimate in wrestling weapons");
            }

            if (Object.HasPart<Chair>())
            {
                if (!Object.InheritsFrom("FoldingChair"))
                E.AddGeneralDescription("support", "gigantic rumps");
            }
            if (Object.HasPart<Bed>())
            {
                E.AddWeaponDescription("support", "gigantic sleepers");
            }

            if (Object.LiquidVolume != null)
            {
                E.AddGeneralDescription("hold", "twice as much liquid");
            }
            if (Object.HasPart<EnergyCell>())
            {
                E.AddGeneralDescription("have", "twice the energy capacity");
            }
            if (Object.HasPartDescendedFrom<IGrenade>())
            {
                E.AddGeneralDescription("have", "twice as large a radius of effect");
            }
            if (Object.HasPart<Tonic>())
            {
                E.AddGeneralDescription("contain", "double the tonic dosage");
            }
            if (Object.GetIntProperty("Currency") > 0)
            {
                E.AddGeneralDescription(null, "much more valuable");
            }
            if (Object.HasPart<Container>())
            {
                E.AddGeneralDescription("store", "twice as many things (don't ask, it's fine)");
            }
            if (Object.HasPart<Enclosing>())
            {
                E.AddGeneralDescription("provide", "twice as much AV");
                E.AddGeneralDescription("penalise", "DV half again as much");
                E.AddGeneralDescription("have", "+3 higher save to exit");
            }
            if (Object.TryGetPart(out Chat chat) && chat.ShowInShortDescription)
            {
                E.AddGeneralDescription("convey", "its message with substantially more clarity");
            }
            if (Object.HasPart<Tombstone>() || Object.HasTagOrProperty("Burial"))
            {
                E.AddGeneralDescription("inspire", "greater sorrow");
            }

            if (Object.HasPart<Backpack>())
            {
                E.AddGeneralDescription("support", "twice and a half as much weight");
            }

            if (Object.HasPart<Armor>() && Object.GetPart<Armor>().CarryBonus > 0)
            {
                E.AddGeneralDescription("have", "a quarter more carry capcity");
            }

            if (Object.HasTagOrProperty("Ornate") || Object.HasTagOrProperty("Door"))
            {
                E.AddGeneralDescription("inspire", "awe with its immensity");
            }

            if (Object.HasTagOrProperty("Wall"))
            {
                E.AddGeneralDescription("stand", "much taller than usual");
            }

            MeleeWeapon meleeWeapon = Object.GetPart<MeleeWeapon>();
            bool isDefaultBehavior = Object.EquipAsDefaultBehavior();
            bool isDefaultBehaviorOrFloating = isDefaultBehavior || Object.IsEntirelyFloating();
            if (meleeWeapon != null && Object.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                E.AddWeaponDescription("have", "+3 damage");
                if (meleeWeapon.Skill == "Cudgel")
                {
                    E.AddWeaponDescription(null, $"twice as effective when you Slam with {Object.them}");
                }
                else if (meleeWeapon.Skill == "Axe")
                {
                    E.AddWeaponDescription("cleave", "for -3 AV");
                }
            }
            else if (Object.HasPart<MissileWeapon>())
            {
                E.AddWeaponDescription("have", "+3 damage");
            }
            else if (Object.HasPart<ThrownWeapon>())
            {
                if (!Object.HasPartDescendedFrom<IGrenade>())
                {
                    E.AddWeaponDescription("have", "+3 damage");
                }
            }

            bool isOrnate = Object.HasPropertyOrTag("Ornate");
            bool isFurniture = Object.InheritsFrom("Furniture");
            bool isWall = Object.InheritsFrom("Wall");
            bool isCorpse = Object.InheritsFrom("Corpse");
            bool isNatural = Object.IsNaturalEquipment();
            if (!isDefaultBehaviorOrFloating && !isOrnate && !isFurniture && !isWall && !isCorpse && !isNatural)
            {
                if (Object.UsesSlots == null &&
                    (!(meleeWeapon != null && meleeWeapon.IsImprovised())
                    || (Object.TryGetPart(out ThrownWeapon thrownWeapon) && !thrownWeapon.IsImprovised())
                    || Object.HasPart<MissileWeapon>()
                    || Object.HasPart<Shield>()))
                {
                    string effect = $"must be wielded {(Object.UsesTwoSlots ? "four" : "two")}-handed by non-gigantic creatures";
                    E.AddGeneralDescription("", effect);
                }
                else
                {
                    E.AddGeneralDescription("", "can only be equipped by gigantic creatures");
                }
            }

            if (Object.HasPart<DiggingTool>() || Object.HasPart<Drill>())
            {
                E.AddWeaponDescription("dig", "twice as fast");
            }

            if (Object.HasPart<DeploymentMaintainer>() && isFurniture)
            {
                E.AddGeneralDescription("enforce", "its effect twice far");
            }

            if (Object.TryGetPart(out LiquidProducer liquidProducer) && liquidProducer.Rate != 0 && isFurniture)
            {
                E.AddGeneralDescription("produce", "liquid twice as fast");
            }

            if (Object.TryGetPart(out ItemConvertor itemConvertor) && isFurniture)
            {
                if (itemConvertor.ConversionTag == "RockTumblerOutput")
                {
                    E.AddGeneralDescription("process", "rocks twice as effectively");
                }
                if (itemConvertor.ConversionTag == "WireExtruderOutput")
                {
                    E.AddGeneralDescription("extrudes", "double the additional wire from gigantic materials");
                }
            }

            if (Object.HasPart<Fan>() && isFurniture)
            {
                E.AddGeneralDescription("blow", "with twice the strength");
                E.AddGeneralDescription("blow", "half again as far");
            }

            if (Object.HasPart<LiquidPump>() && isFurniture)
            {
                E.AddGeneralDescription("pump", "five times as much liquid");
            }

            if (Object.HasPart<Capacitor>() && isFurniture)
            {
                E.AddGeneralDescription("hold", "twice the maximum charge");
            }

            if (Object.HasPart<RegenTank>() && isFurniture)
            {
                E.AddGeneralDescription("require", "twice the minimum liquid to operate");
            }

            if (Object.TryGetPart(out TemperatureAdjuster temperatureAdjuster) && isFurniture)
            {
                if (temperatureAdjuster.TemperatureAmount != 0)
                {
                    E.AddGeneralDescription("affect", "temperature twice as effectively");
                }
                if (temperatureAdjuster.TemperatureThreshold != 0)
                {
                    E.AddGeneralDescription("have", "half again its temperature threshold");
                }
            }

            if (Object.HasPart<DamageContents>() && isFurniture)
            {
                E.AddGeneralDescription(null, "twice as destructive to its contents");
            }

            bool chargeUseIncreased2x = isFurniture && (
                (Object.TryGetPart(out RealityStabilization realityStabilization)
                    && realityStabilization.ChargeUse != 0)
             || (Object.TryGetPart(out Bed bed)
                    && bed.ChargeUse != 0)
             || (Object.TryGetPart(out Chair chair)
                    && chair.ChargeUse != 0)
             || (Object.TryGetPart(out Capacitor capacitor)
                    && capacitor.ChargeUse != 0)
             || (Object.TryGetPart(out ChargeSink chargeSink)
                    && chargeSink.ChargeUse != 0)
             || (Object.TryGetPart(out ConversationScript conversationScript)
                    && conversationScript.ChargeUse != 0)
             || (Object.TryGetPart(out Mill mill)
                    && mill.ChargeUse != 0)
             || (Object.TryGetPart(out RadiusEventSender radiusEventSender)
                    && radiusEventSender.ChargeUse != 0)
             || (Object.TryGetPart(out Enclosing enclosing)
                    && enclosing.ChargeUse != 0)
             || (Object.TryGetPart(out DamageContents damageContents)
                    && damageContents.ChargeUse != 0)
             || (Object.TryGetPart(out LiquidPump liquidPump)
                    && liquidPump.ChargeUse != 0)
             || (Object.TryGetPart(out Fan fan)
                    && fan.ChargeUse != 0)
             || (Object.TryGetPart(out RegenTank regenTank)
                    && regenTank.ChargeUse != 0)
             || (Object.TryGetPart(out temperatureAdjuster)
                    && temperatureAdjuster.ChargeUse != 0)
             || (Object.TryGetPart(out itemConvertor)
                    && itemConvertor.ChargeUse != 0)
             || (Object.TryGetPart(out liquidProducer)
                    && liquidProducer.ChargeUse != 0));
            if (chargeUseIncreased2x)
            {
                E.AddGeneralDescription("draw", "twice the charge to power");
            }

            bool chargeRateIncreased2x = isFurniture && (
                (Object.TryGetPart(out FusionReactor fusionReactor)
                    && fusionReactor.ChargeRate != 0)
             || (Object.TryGetPart(out SolarArray solarArray)
                    && solarArray.ChargeRate != 0)
             || (Object.TryGetPart(out BroadcastPowerReceiver broadcastPowerReceiver)
                    && broadcastPowerReceiver.ChargeRate != 0)
             || (Object.TryGetPart(out MechanicalPowerTransmission mechanicalPowerTransmission)
                    && mechanicalPowerTransmission.ChargeRate != 0 && mechanicalPowerTransmission.IsConsumer)
             || (Object.TryGetPart(out HydraulicPowerTransmission hydraulicPowerTransmission)
                    && hydraulicPowerTransmission.ChargeRate != 0 && hydraulicPowerTransmission.IsConsumer)
             || (Object.TryGetPart(out ElectricalPowerTransmission electricalPowerTransmission)
                    && electricalPowerTransmission.ChargeRate != 0 && electricalPowerTransmission.IsConsumer));
            if (chargeRateIncreased2x)
            {
                E.AddGeneralDescription("charge", "at twice the rate");
            }

            bool chargeTransmissionIncreased2x = isFurniture && (
                (Object.TryGetPart(out electricalPowerTransmission)
                    && electricalPowerTransmission.ChargeRate != 0 && !electricalPowerTransmission.IsConsumer)
             || (Object.TryGetPart(out hydraulicPowerTransmission)
                    && hydraulicPowerTransmission.ChargeRate != 0 && !hydraulicPowerTransmission.IsConsumer)
             || (Object.TryGetPart(out mechanicalPowerTransmission)
                    && mechanicalPowerTransmission.ChargeRate != 0 && !mechanicalPowerTransmission.IsConsumer));
            if (chargeTransmissionIncreased2x)
            {
                E.AddGeneralDescription("transmit", "power at twice the rate");
            }

            if (!isDefaultBehavior)
            {
                E.AddGeneralDescription(null, "much heavier than usual");
            }
            else
            {
                E.AddGeneralDescription(null, "unusually large");
            }
            if (Object.HasPart<Campfire>())
            {
                E.AddGeneralDescription("", "could probably make some seriously thick stew..");
            }

            return true;
        }

    } //!-- public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>

    public class DescribeModGiganticHandler : IEventHandler, IModEventHandler<DescribeModGiganticEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModGiganticHandler));

        private static readonly DescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, DescribeModGiganticEvent.ID, EventOrder.EXTREMELY_EARLY + EventOrder.EXTREMELY_EARLY);

            return (bool)The.Game?.WasModEventHandlerRegistered<DescribeModGiganticHandler, DescribeModGiganticEvent>();
        }

        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            return true;
        }
    } //!-- public class DescribeModGiganticHandler : IEventHandler, IModEventHandler<DescribeModGiganticEvent>
}
