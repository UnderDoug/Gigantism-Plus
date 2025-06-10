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

        private GameObject _wielder = null;
        public GameObject Wielder
        {
            get => _wielder ??= ParentObject?.Equipped;
            set => _wielder = value;
        }

        private ElongatedPaws _elongatedPaws = null;
        public ElongatedPaws ElongatedPaws
        {
            get => _elongatedPaws ??= Wielder?.GetPart<ElongatedPaws>();
            set
            {
                _elongatedPaws = value;
                if (Wielder == null) _elongatedPaws = null;
            }
        }

        public MeleeWeapon MeleeWeapon => ParentObject?.GetPart<MeleeWeapon>();

        ModNaturalEquipment<ElongatedPaws> NaturalEquipmentMod =>
            ElongatedPaws?.UpdateNaturalEquipmentMod(
                ElongatedPaws?.GetNaturalEquipmentMod(),
                    (int)ElongatedPaws?.Level);

        public int AppliedElongatedBonusCap = 0;

        public void ApplyElongatedBonusCap(MeleeWeapon Weapon)
        {
            Debug.Entry(4,
                $"* {nameof(ApplyElongatedBonusCap)}("
                + $"{nameof(Weapon)}: {Weapon?.ParentObject?.DebugName ?? NULL})",
                Indent: 3, Toggle: doDebug);

            if (ElongatedPaws != null && NaturalEquipmentMod != null)
            {
                UnapplyElongatedBonusCap(Weapon);

                AppliedElongatedBonusCap = NaturalEquipmentMod.GetDamageBonus();
                Weapon.AdjustBonusCap(AppliedElongatedBonusCap);

                Debug.LoopItem(4, $"New {nameof(AppliedElongatedBonusCap)}", $"{AppliedElongatedBonusCap}",
                    Indent: 4, Toggle: doDebug);
            }
            else
            {
                Debug.LoopItem(4, $"{nameof(ElongatedPaws)} or its {nameof(NaturalEquipmentMod)} was null, no adjustments possible",
                    Indent: 4, Toggle: doDebug);
            }

            Debug.Entry(4,
                $"x {nameof(ApplyElongatedBonusCap)}("
                + $"{nameof(Weapon)}: {Weapon?.ParentObject?.DebugName ?? NULL}) *//",
                Indent: 3, Toggle: doDebug);
        }
        public void UnapplyElongatedBonusCap(MeleeWeapon Weapon)
        {
            Debug.Entry(4,
                $"* {nameof(UnapplyElongatedBonusCap)}("
                + $"{nameof(Weapon)}: {Weapon?.ParentObject?.DebugName ?? NULL})",
                Indent: 4, Toggle: doDebug);

            Debug.LoopItem(4, $"Old {nameof(AppliedElongatedBonusCap)}", $"{AppliedElongatedBonusCap}", Indent: 5, Toggle: doDebug);

            Weapon.AdjustBonusCap(-AppliedElongatedBonusCap);
            AppliedElongatedBonusCap = 0;

            Debug.Entry(4,
                $"x {nameof(UnapplyElongatedBonusCap)}("
                + $"{nameof(Weapon)}: {Weapon?.ParentObject?.DebugName ?? NULL}) *//",
                Indent: 4, Toggle: doDebug);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            bool haveNaturalEquipmentMod = NaturalEquipmentMod != null;
            return base.WantEvent(ID, cascade)
                || (AppliedElongatedBonusCap > 0 && haveNaturalEquipmentMod && ID == PooledEvent<GetDisplayNameEvent>.ID)
                || (AppliedElongatedBonusCap > 0 && haveNaturalEquipmentMod && ID == GetShortDescriptionEvent.ID)
                || (AppliedElongatedBonusCap > 0 && haveNaturalEquipmentMod && ID == BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>.ID)
                || ID == StatChangeEvent.ID
                || ID == UnequippedEvent.ID
                || ID == EquippedEvent.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName && NaturalEquipmentMod != null && AppliedElongatedBonusCap > 0)
            {
                E.AddAdjective(NaturalEquipmentMod.GetColoredAdjective(), DescriptionBuilder.ORDER_ADJUST_EXTREMELY_EARLY);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (E.Object == ParentObject && NaturalEquipmentMod != null && AppliedElongatedBonusCap > 0)
            {
                E.Postfix.AppendRules(
                    DescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>
                        .Send(ParentObject, NaturalEquipmentMod.GetColoredAdjective(), Context: $"{nameof(WeaponElongator)}")
                        .Process());
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>> E)
        {
            if (E.Object == ParentObject && NaturalEquipmentMod != null && E.Context == $"{nameof(WeaponElongator)}")
            {
                string scalingStat = ElongatedPaws.SCALE_STAT;
                int damageBonus = NaturalEquipmentMod.GetDamageBonus();
                if (damageBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (E.WeaponDescriptions.IsNullOrEmpty())
                {
                    E.AddWeaponElement("have", $"{E.Object.its} bonus damage scale by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
                else
                {
                    E.AddWeaponElement("", $"{E.Object.its} bonus damage scales by half {E.Object.its} wielder's {scalingStat} Modifier");
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                if (!item.IsNaturalEquipment())
                {
                    if (ElongatedPaws != null)
                    {
                        Debug.Entry(4,
                            $"* {nameof(WeaponElongator)}."
                            + $"{nameof(HandleEvent)}("
                            + $"{nameof(EquippedEvent)} E)",
                            Indent: 2, Toggle: doDebug);

                        Debug.LoopItem(4, $"Item: {item?.DebugName ?? NULL}",
                            Indent: 3, Toggle: doDebug);

                        ApplyElongatedBonusCap(item.GetPart<MeleeWeapon>());

                        Debug.Entry(4,
                            $"x {nameof(WeaponElongator)}"
                            + $"{nameof(HandleEvent)}("
                            + $"{nameof(EquippedEvent)} E)"
                            + $" *//",
                            Indent: 2, Toggle: doDebug);
                    }
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
            GameObject item = E.Item;
            if (item.Is(ParentObject) && !Wielder.Is(null) && !ElongatedPaws.Is(null))
            {
                Debug.Entry(4,
                    $"@ {nameof(WeaponElongator)}."
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(UnequippedEvent)} E)",
                    Indent: 2, Toggle: doDebug);

                Debug.LoopItem(4, $"Item: {item?.DebugName ?? NULL}", Indent: 3, Toggle: doDebug);

                UnapplyElongatedBonusCap(MeleeWeapon);

                ElongatedPaws = null;
                Wielder = null;

                Debug.Entry(4,
                    $"x {nameof(WeaponElongator)}"
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(UnequippedEvent)} E)"
                    + $" @//",
                    Indent: 2, Toggle: doDebug);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Object == Wielder && ElongatedPaws != null && E.Name == ElongatedPaws.SCALE_STAT)
            {
                Debug.Entry(4,
                    $"@ {nameof(WeaponElongator)}."
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(StatChangeEvent)} E: {E.Name})",
                    Indent: 2, Toggle: doDebug);

                Debug.LoopItem(4, $" E.Object: {E.Object?.DebugName ?? NULL}", Indent: 2, Toggle: doDebug);

                ApplyElongatedBonusCap(MeleeWeapon);

                Debug.Entry(4,
                    $"x {nameof(WeaponElongator)}."
                    + $"{nameof(HandleEvent)}("
                    + $"{nameof(StatChangeEvent)} E)"
                    + $" @//",
                    Indent: 2, Toggle: doDebug);
            }
            return base.HandleEvent(E);
        }
    } //!-- public class WeaponElongator
}     //        : IScribedPart
      //        , IModEventHandler<BeforeDescribeModificationEvent<ModNaturalEquipment<ElongatedPaws>>>