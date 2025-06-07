using System;
using System.Collections.Generic;
using System.Text;

using XRL;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    public class BeforeDescribeModGiganticHandler 
        : IEventHandler
        , IModEventHandler<BeforeDescribeModificationEvent<ModGigantic>>

    {
        private static bool doDebug => getClassDoDebug(nameof(BeforeDescribeModGiganticHandler));

        private static readonly BeforeDescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, BeforeDescribeModificationEvent<ModGigantic>.ID);

            return (bool)The.Game?.WasModEventHandlerRegistered<BeforeDescribeModGiganticHandler, BeforeDescribeModificationEvent<ModGigantic>>();
        }

        public bool HandleEvent(BeforeDescribeModificationEvent<ModGigantic> E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(The)}.{nameof(The.Game)}."
                + $"{nameof(HandleEvent)}({E.GetType().Name},"
                + $" E.Object: {E.Object?.DebugName ?? NULL},"
                + $" E.Context: {E.Context})",
                Indent: indent + 1, Toggle: doDebug);

            // BeforeDescribeModification(E, E.ModPart);

            GameObject Object = E.Object;

            /* Check out ObjectBlueprints/Data.xml -> GigantismPlusModGiganticDescriptions
             * 
            if (Object.HasPart<LightSource>())
            {
                E.GeneralDescriptions.Add(new List<string> { "illuminate", "twice as far" });
            }
            */

            Debug.Entry(4,
                $"x {nameof(The)}.{nameof(The.Game)}"
                + $"{nameof(HandleEvent)}({E.GetType().Name},"
                + $" E.Object: {E.Object?.DebugName ?? NULL},"
                + $" E.Context: {E.Context})"
                + $" @//",
                Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return true;
        }
        public static void BeforeDescribeModification(BeforeDescribeModificationEvent<ModGigantic> E, IModification ModPart)
        {
            if (ModPart.InheritsFrom<ModGigantic>() || ModPart.InheritsFrom<ModNaturalEquipment<GigantismPlus>>())
            {
                BeforeDescribeModGigantic(E);
            }
        }
        public static void BeforeDescribeModGigantic(BeforeDescribeModificationEvent<ModGigantic> E)
        {
            // GameObject Object = E.Object;
        }
    } //!-- public class BeforeDescribeModGiganticHandler
      //        : IEventHandler
      //        , IModEventHandler<BeforeDescribeModificationEvent>

    public class DescribeModGiganticHandler 
        : IEventHandler
        , IModEventHandler<DescribeModificationEvent<ModGigantic>>
    {
        private static bool doDebug => getClassDoDebug(nameof(DescribeModGiganticHandler));

        private static readonly DescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, DescribeModificationEvent<ModGigantic>.ID, EventOrder.EXTREMELY_EARLY + EventOrder.EXTREMELY_EARLY);

            return (bool)The.Game?.WasModEventHandlerRegistered<DescribeModGiganticHandler, DescribeModificationEvent<ModGigantic>>();
        }

        public bool HandleEvent(DescribeModificationEvent<ModGigantic> E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(The)}.{nameof(The.Game)}."
                + $"{nameof(HandleEvent)}({E.GetType().Name},"
                + $" E.Object: {E.Object?.DebugName ?? NULL},"
                + $" E.Context: {E.Context})",
                Indent: indent + 1, Toggle: doDebug);

            GameObject Object = E.Object;

            GameObjectBlueprint GigantismPlusModGiganticDescriptions = GameObjectFactory.Factory.GetBlueprint(MODGIGANTIC_DESCRIPTIONBUCKET);
            E.WeaponDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "Before", "Weapon"));
            E.GeneralDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "Before", "General"));

            if (Object.InheritsFrom("FoldingChair"))
            {
                E.AddWeaponElement(null, "the ultimate in wrestling weapons");
            }

            if (Object.HasPart<Chair>())
            {
                if (!Object.InheritsFrom("FoldingChair"))
                    E.AddGeneralElement("support", "gigantic rumps");
            }
            if (Object.HasPart<Bed>())
            {
                E.AddWeaponElement("support", "gigantic sleepers");
            }

            if (Object.LiquidVolume != null)
            {
                E.AddGeneralElement("hold", "twice as much liquid");
            }
            if (Object.HasPart<EnergyCell>())
            {
                E.AddGeneralElement("have", "twice the energy capacity");
            }
            if (Object.HasPartDescendedFrom<IGrenade>())
            {
                E.AddGeneralElement("have", "twice as large a radius of effect");
            }
            if (Object.HasPart<Tonic>())
            {
                E.AddGeneralElement("contain", "double the tonic dosage");
            }
            if (Object.GetIntProperty("Currency") > 0)
            {
                E.AddGeneralElement(null, "much more valuable");
            }
            if (Object.HasPart<Container>())
            {
                E.AddGeneralElement("store", "twice as many things (don't ask, it's fine)");
            }
            if (Object.HasPart<Enclosing>())
            {
                E.AddGeneralElement("provide", "twice as much AV");
                E.AddGeneralElement("penalise", "DV half again as much");
                E.AddGeneralElement("have", "+3 higher save to exit");
            }
            if (Object.TryGetPart(out Chat chat) && chat.ShowInShortDescription)
            {
                E.AddGeneralElement("convey", "its message with substantially more clarity");
            }
            if (Object.HasPart<Tombstone>() || Object.HasTagOrProperty("Burial"))
            {
                E.AddGeneralElement("inspire", "greater sorrow");
            }

            if (Object.HasPart<Backpack>())
            {
                E.AddGeneralElement("support", "twice and a half as much weight");
            }

            if (Object.HasPart<Armor>() && Object.GetPart<Armor>().CarryBonus > 0)
            {
                E.AddGeneralElement("have", "a quarter more carry capcity");
            }

            if (Object.HasTagOrProperty("Ornate") || Object.HasTagOrProperty("Door"))
            {
                E.AddGeneralElement("inspire", "awe with its immensity");
            }

            if (Object.HasTagOrProperty("Wall"))
            {
                E.AddGeneralElement("stand", "much taller than usual");
            }

            MeleeWeapon meleeWeapon = Object.GetPart<MeleeWeapon>();
            bool isDefaultBehavior = Object.EquipAsDefaultBehavior();
            bool isDefaultBehaviorOrFloating = isDefaultBehavior || Object.IsEntirelyFloating();
            if (meleeWeapon != null && Object.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                E.AddWeaponElement("have", "+3 damage");
                if (meleeWeapon.Skill == "Cudgel")
                {
                    E.AddWeaponElement(null, $"twice as effective when you Slam with {Object.them}");
                }
                else if (meleeWeapon.Skill == "Axe")
                {
                    E.AddWeaponElement("cleave", "for -3 AV");
                }
            }
            else if (Object.HasPart<MissileWeapon>())
            {
                E.AddWeaponElement("have", "+3 damage");
            }
            else if (Object.HasPart<ThrownWeapon>())
            {
                if (!Object.HasPartDescendedFrom<IGrenade>())
                {
                    E.AddWeaponElement("have", "+3 damage");
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
                    E.AddGeneralElement("", effect);
                }
                else
                {
                    E.AddGeneralElement("", "can only be equipped by gigantic creatures");
                }
            }

            if (Object.HasPart<DiggingTool>() || Object.HasPart<Drill>())
            {
                E.AddWeaponElement("dig", "twice as fast");
            }

            if (Object.HasPart<DeploymentMaintainer>() && isFurniture)
            {
                E.AddGeneralElement("enforce", "its effect twice far");
            }

            if (Object.TryGetPart(out LiquidProducer liquidProducer) && liquidProducer.Rate != 0 && isFurniture)
            {
                E.AddGeneralElement("produce", "liquid twice as fast");
            }

            if (Object.TryGetPart(out ItemConvertor itemConvertor) && isFurniture)
            {
                if (itemConvertor.ConversionTag == "RockTumblerOutput")
                {
                    E.AddGeneralElement("process", "rocks twice as effectively");
                }
                if (itemConvertor.ConversionTag == "WireExtruderOutput")
                {
                    E.AddGeneralElement("extrudes", "double the additional wire from gigantic materials");
                }
            }

            if (Object.HasPart<Fan>() && isFurniture)
            {
                E.AddGeneralElement("blow", "with twice the strength");
                E.AddGeneralElement("blow", "half again as far");
            }

            if (Object.HasPart<LiquidPump>() && isFurniture)
            {
                E.AddGeneralElement("pump", "five times as much liquid");
            }

            if (Object.HasPart<Capacitor>() && isFurniture)
            {
                E.AddGeneralElement("hold", "twice the maximum charge");
            }

            if (Object.HasPart<RegenTank>() && isFurniture)
            {
                E.AddGeneralElement("require", "twice the minimum liquid to operate");
            }

            if (Object.TryGetPart(out TemperatureAdjuster temperatureAdjuster) && isFurniture)
            {
                if (temperatureAdjuster.TemperatureAmount != 0)
                {
                    E.AddGeneralElement("affect", "temperature twice as effectively");
                }
                if (temperatureAdjuster.TemperatureThreshold != 0)
                {
                    E.AddGeneralElement("have", "half again its temperature threshold");
                }
            }

            if (Object.HasPart<DamageContents>() && isFurniture)
            {
                E.AddGeneralElement(null, "twice as destructive to its contents");
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
                E.AddGeneralElement("draw", "twice the charge to power");
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
                E.AddGeneralElement("charge", "at twice the rate");
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
                E.AddGeneralElement("transmit", "power at twice the rate");
            }

            if (!isDefaultBehavior)
            {
                E.AddGeneralElement(null, "much heavier than usual");
            }
            else
            {
                E.AddGeneralElement(null, "unusually large");
            }
            if (Object.HasPart<Campfire>())
            {
                E.AddGeneralElement("", "could probably make some seriously thick stew..");
            }

            E.WeaponDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "After", "Weapon"));
            E.GeneralDescriptions.AddRange(IterateDataBucketTags(Object, GigantismPlusModGiganticDescriptions, "After", "General"));

            // DescribeModification(E, E.ModPart);

            Debug.Entry(4,
                $"x {nameof(The)}.{nameof(The.Game)}"
                + $"{nameof(HandleEvent)}({E.GetType().Name},"
                + $" E.Object: {E.Object?.DebugName ?? NULL},"
                + $" E.Context: {E.Context})"
                + $" @//",
                Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
            return true;
        }

        public static void DescribeModification(DescribeModificationEvent<ModGigantic> E, IModification ModPart)
        {
            if (ModPart.InheritsFrom<ModGigantic>() || ModPart.InheritsFrom<ModNaturalEquipment<GigantismPlus>>())
            {
                DescribeModGigantic(E);
            }
        }
        public static void DescribeModGigantic(DescribeModificationEvent<ModGigantic> E)
        {
            
        }

    } //!-- public class DescribeModGiganticHandler
      //        : IEventHandler
      //        , IModEventHandler<DescribeModificationEvent>
}
