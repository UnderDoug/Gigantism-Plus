using System;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class WeaponElongator
        : IScribedPart
        , IModEventHandler<BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>>
    {
        private static bool doDebug => getClassDoDebug(nameof(WeaponElongator));

        public GameObject Wielder => ParentObject?.Equipped;

        public ElongatedPaws ElongatedPaws => Wielder?.GetPart<ElongatedPaws>();

        public MeleeWeapon MeleeWeapon => ParentObject?.GetPart<MeleeWeapon>();
        public ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod => ElongatedPaws?.UpdateNaturalEquipmentMod(ElongatedPaws.NewElongatedArmMod(ElongatedPaws.NaturalEquipmentManager), (int)ElongatedPaws?.Level);

        public int AppliedElongatedBonusCap = 0;

        public bool Applied = false;

        public int ElongatedBonusCap => (int)ElongatedPaws?.GetNaturalWeaponDamageBonus();

        public override void Remove()
        {
            UnapplyElongatedBonusCap();
        }

        public void ApplyElongatedBonusCap(MeleeWeapon Weapon = null)
        {
            int indent = Debug.LastIndent;
            Weapon ??= MeleeWeapon;
            if (ElongatedPaws != null && Weapon != null)
            {
                UnapplyElongatedBonusCap(Weapon);

                if (!Applied && (Applied = Weapon.AdjustBonusCap(ElongatedBonusCap)))
                {
                    AppliedElongatedBonusCap = ElongatedBonusCap;
                }
            }
            else
            {
                MetricsManager.LogModWarning(ThisMod,
                    $"{nameof(WeaponElongator)}.{nameof(ApplyElongatedBonusCap)}, " +
                    $"{nameof(ElongatedPaws)} or its {nameof(NaturalEquipmentMod)} was null, no adjustments possible");
            }
            Debug.LastIndent = indent;
        }
        public void UnapplyElongatedBonusCap(MeleeWeapon Weapon = null)
        {
            Weapon ??= MeleeWeapon;
            if (Weapon != null && Applied && !(Applied = !Weapon.AdjustBonusCap(-AppliedElongatedBonusCap)))
            {
                AppliedElongatedBonusCap = 0;
            }
        }

        public override bool WantTurnTick()
        {
            return base.WantTurnTick();
        }
        public override void TurnTick(long TimeTick, int Amount)
        {
            if (Wielder == null || ElongatedPaws == null || MeleeWeapon == null)
            {
                ParentObject?.RemovePart(this);
            }
            base.TurnTick(TimeTick, Amount);
        }
        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID
                || ID == GetShortDescriptionEvent.ID
                || ID == BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>.ID
                || ID == StatChangeEvent.ID
                || ID == UnequippedEvent.ID
                || ID == EquippedEvent.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName && AppliedElongatedBonusCap > 0)
            {
                E.AddAdjective(new ModElongatedNaturalWeapon().GetColoredAdjective(), DescriptionBuilder.ORDER_ADJUST_EXTREMELY_EARLY);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (E.Object == ParentObject && AppliedElongatedBonusCap > 0)
            {
                E.Postfix.AppendRules(
                    DescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>
                        .Send(ParentObject, new ModElongatedNaturalWeapon().GetColoredAdjective(), Context: $"{nameof(WeaponElongator)}")
                        .Process());
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            if (E.Object == ParentObject && E.Context == $"{nameof(WeaponElongator)}")
            {
                string scalingStat = ElongatedPaws.SCALE_STAT;
                int penCapBonus = AppliedElongatedBonusCap;
                if (penCapBonus != 0)
                {
                    E.AddPrimaryElement("get", $"a {penCapBonus.Signed()} {penCapBonus.Signed().BonusOrPenalty()} to penetration cap");
                }
                if (E.PrimaryDescriptions.IsNullOrEmpty())
                {
                    E.AddPrimaryElement("have", $"{E.Object.its} penetration cap is increased by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                else
                {
                    E.AddPrimaryElement("", $"{E.Object.its} penetration cap increases by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            if (E.Item is GameObject item && item == ParentObject && ElongatedPaws != null)
            {
                if (!item.IsNaturalEquipment())
                {
                    ApplyElongatedBonusCap(MeleeWeapon);
                }
                else
                {
                    item.RemovePart(this);
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {

            if (E.Item is GameObject item && item == ParentObject && Wielder != null && ElongatedPaws != null)
            {
                UnapplyElongatedBonusCap(MeleeWeapon);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Object == Wielder && ElongatedPaws != null && E.Name == ElongatedPaws.SCALE_STAT)
            {
                ApplyElongatedBonusCap(MeleeWeapon);
            }
            return base.HandleEvent(E);
        }
    }
}
