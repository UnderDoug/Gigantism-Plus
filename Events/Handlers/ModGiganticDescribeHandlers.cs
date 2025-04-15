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
    public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>
    {
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

            if (Object.HasPart<Chair>())
            {
                E.GeneralDescriptions.Add(new List<string> { "support", "gigantic rumps" });
            }
            if (Object.HasPart<Bed>())
            {
                E.GeneralDescriptions.Add(new List<string> { "support", "gigantic sleepers" });
            }
            return true;
        }

    } //!-- public class BeforeDescribeModGiganticHandler : IEventHandler, IModEventHandler<BeforeDescribeModGiganticEvent>

    public class DescribeModGiganticHandler : IEventHandler, IModEventHandler<DescribeModGiganticEvent>
    {
        private static readonly DescribeModGiganticHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, DescribeModGiganticEvent.ID);

            return (bool)The.Game?.WasModEventHandlerRegistered<DescribeModGiganticHandler, DescribeModGiganticEvent>();
        }

        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            GameObject Object = E.Object;

            if (Object.LiquidVolume != null)
            {
                E.GeneralDescriptions.Add(new() { "hold", "twice as much liquid" });
            }
            if (Object.HasPart<EnergyCell>())
            {
                E.GeneralDescriptions.Add(new() { "have", "twice the energy capacity" });
            }
            if (Object.HasPartDescendedFrom<IGrenade>())
            {
                E.GeneralDescriptions.Add(new() { "have", "twice as large a radius of effect" });
            }
            if (Object.HasPart<Tonic>())
            {
                E.GeneralDescriptions.Add(new() { "contain", "double the tonic dosage" });
            }
            if (Object.GetIntProperty("Currency") > 0)
            {
                E.GeneralDescriptions.Add(new() { null, "much more valuable" });
            }
            if (Object.HasPart<Container>())
            {
                E.GeneralDescriptions.Add(new() { "store", "twice as many things (don't ask. It's fine)" });
            }
            if (Object.HasPart<Enclosing>())
            {
                E.GeneralDescriptions.Add(new() { "provide", "twice as much AV" });
                E.GeneralDescriptions.Add(new() { "penalise", "DV half again as much" });
                E.GeneralDescriptions.Add(new() { "have", "+3 higher save to exit" });
            }
            if (Object.TryGetPart(out Chat chat) && chat.ShowInShortDescription)
            {
                E.GeneralDescriptions.Add(new() { "convey", "its message with substantially more clarity" });
            }
            if (Object.HasPart<Tombstone>() || Object.HasTagOrProperty("Burial"))
            {
                E.GeneralDescriptions.Add(new() { "inspire", "greater sorrow" });
            }

            bool isDefaultBehavior = Object.EquipAsDefaultBehavior();
            if (!isDefaultBehavior)
            {
                E.GeneralDescriptions.Add(new() { null, "much heavier than usual" });
            }
            else
            {
                E.GeneralDescriptions.Add(new() { null, "unusually large" });
            }

            if (Object.HasPart<Backpack>())
            {
                E.GeneralDescriptions.Add(new List<string> { "support", "twice and a half as much weight" });
            }

            if (Object.HasPart<Armor>() && Object.GetPart<Armor>().CarryBonus > 0)
            {
                E.GeneralDescriptions.Add(new List<string> { "have", "a quarter more carry capcity" });
            }

            if (Object.HasTagOrProperty("Ornate"))
            {
                E.GeneralDescriptions.Add(new() { "inspire", "awe with its immensity" });
            }

            if (Object.HasTagOrProperty("Wall"))
            {
                E.GeneralDescriptions.Add(new() { "stand", "much taller than usual" });
            }

            MeleeWeapon meleeWeapon = Object.GetPart<MeleeWeapon>();
            bool isDefaultBehaviorOrFloating = isDefaultBehavior || Object.IsEntirelyFloating();
            if (meleeWeapon != null && Object.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                E.WeaponDescriptions.Add(new() { "have", "+3 damage" });
                if (meleeWeapon.Skill == "Cudgel")
                {
                    E.WeaponDescriptions.Add(new() { null, "twice as effective when you Slam with " + Object.them });
                }
                else if (meleeWeapon.Skill == "Axe")
                {
                    E.WeaponDescriptions.Add(new() { "cleave", "for -3 AV" });
                }
            }
            else if (Object.HasPart<MissileWeapon>())
            {
                E.WeaponDescriptions.Add(new() { "have", "+3 damage" });
            }
            else if (Object.HasPart<ThrownWeapon>())
            {
                if (!Object.HasPartDescendedFrom<IGrenade>())
                {
                    E.WeaponDescriptions.Add(new() { "have", "+3 damage" });
                }
            }

            if (!isDefaultBehaviorOrFloating)
            {
                if (Object.UsesSlots == null &&
                    (!(meleeWeapon != null && meleeWeapon.IsImprovised())
                    || (Object.TryGetPart(out ThrownWeapon thrownWeapon) && !thrownWeapon.IsImprovised())
                    || Object.HasPart<MissileWeapon>()
                    || Object.HasPart<Shield>()))
                {
                    E.GeneralDescriptions.Add(new() { "", Object.it + " must be wielded " + (Object.UsesTwoSlots ? "four" : "two") + "-handed by non-gigantic creatures" });
                }
                else
                {
                    E.GeneralDescriptions.Add(new() { "", "can only be equipped by gigantic creatures" });
                }
            }

            if (Object.HasPart<DiggingTool>() || Object.HasPart<Drill>())
            {
                E.WeaponDescriptions.Add(new() { "dig", "twice as fast" });
            }

            return true;
        }
    } //!-- public class DescribeModGiganticHandler : IEventHandler, IModEventHandler<DescribeModGiganticEvent>
}
