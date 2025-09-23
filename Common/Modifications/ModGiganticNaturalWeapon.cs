using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModGiganticNaturalWeapon
        : ModNaturalEquipment<GigantismPlus>
        , IDescribeModificationHandler<ModGigantic>
    // , IModEventHandler<BeforeDescribeModificationEvent<ModGigantic>>
    // , IModEventHandler<DescribeModificationEvent<ModGigantic>>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModGiganticNaturalWeapon));

        public ModGiganticNaturalWeapon()
        {
            ModPriority = 40;
            DescriptionPriority = 40;

            Adjective = "gigantic";
            AdjectiveColor = "gigantic";
            AdjectiveColorFallback = "w";

            AdjustMeleeStat("Strength", -100, new GameObjectHasPart<MeleeWeapon>());

            AdjustColorString("&Z", true);
            AdjustTileColor("&Z", true);
            AdjustDetailColor("z", true, new GameObjectIsForSlot("Hand"));

            AddAdjustment(new DisableModGiganticShortDescription());
            AddAdjustment(new DisableModGiganticDisplayName());
        }
        public ModGiganticNaturalWeapon(NaturalEquipmentManager NewManager)
            : this()
        {
            Manager = NewManager;
        }

        public override void ApplyModification(GameObject Object)
        {
            base.ApplyModification(Object);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(BeforeDescribeModificationEvent<ModGigantic>.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(DescribeModificationEvent<ModGigantic>.ID, EventOrder.EXTREMELY_LATE);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetCleaveAmountEvent.ID;
        }
        public override bool HandleEvent(GetCleaveAmountEvent E)
        {
            if (IsObjectActivePartSubject(E.Object))
            {
                int damageBonus = 0;
                if (!Adjustments.IsNullOrEmpty())
                {
                    foreach (IAdjustment adjustment in Adjustments)
                    {
                        if (adjustment is AdjustMeleeDamageBonus meleeWeaponDamageBonus)
                        {
                            damageBonus = (int)meleeWeaponDamageBonus.Amount;
                            break;
                        }
                    }
                }
                E.Amount += Math.Max(0, damageBonus - 2);
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(BeforeDescribeModificationEvent<ModGigantic> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                int cleaveBonus = -GetCleaveAmountEvent.GetFor(ParentObject, Wielder, null);

                if (cleaveBonus != 0 && ParentObject.TryGetPart(out MeleeWeapon weapon) && weapon.Skill == "Axe")
                {
                    E.AddPrimaryElement("cleave", $"for an additional {cleaveBonus.Signed()} AV");
                }
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(DescribeModificationEvent<ModGigantic> E)
        {
            if (E.Object == ParentObject && E.Context == NATURAL_EQUIPMENT)
            {
                E.RemovePrimaryElement("have", "+3 damage");
                E.RemovePrimaryElement("cleave", "for -3 AV");
            }
            return base.HandleEvent(E);
        }
        public override string GetInstanceDescription(GameObject Object = null)
        {
            Object ??= ParentObject;
            if (Object == null)
            {
                return base.GetInstanceDescription(Object);
            }

            DescribeModificationEvent<ModNaturalEquipment<GigantismPlus>>.Send(
                Object: Object,
                Adjective: GetColoredAdjective(),
                WeaponDescriptions: out List<DescriptionElement> weaponDescriptions,
                GeneralDescriptions: out List<DescriptionElement> generalDescriptions,
                Context: NATURAL_EQUIPMENT);

            return DescribeModificationEvent<ModGigantic>.Send(
                Object: Object,
                Adjective: GetColoredAdjective(),
                WeaponDescriptions: weaponDescriptions,
                GeneralDescriptions: generalDescriptions,
                Context: NATURAL_EQUIPMENT)
                .Process(PluralizeObject: true);
        }
    }
}