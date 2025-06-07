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
        , IModEventHandler<BeforeDescribeModificationEvent<ModGigantic>>
        , IModEventHandler<DescribeModificationEvent<ModGigantic>>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModGiganticNaturalWeapon));

        public ModGiganticNaturalWeapon()
        {
        }

        public ModGiganticNaturalWeapon(int Tier)
            : base(Tier)
        {
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
                E.Amount += GetDamageBonus() - 2;
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(BeforeDescribeModificationEvent<ModGigantic> E)
        {
            if (E.Object == ParentObject && E.Context == "Natural Equipment")
            {
                int dieCount = GetDamageDieCount();
                int damageBonus = GetDamageBonus();
                int hitBonus = GetHitBonus();
                int cleaveBonus = -(damageBonus - 2);

                if (dieCount > 0)
                {
                    E.AddWeaponElement("gain", $"{dieCount} additional damage die");
                }
                if (damageBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (damageBonus != 0 && ParentObject.TryGetPart(out MeleeWeapon weapon) && weapon.Skill == "Axe")
                {
                    E.AddWeaponElement("has", $"a {cleaveBonus.Signed()} {(-cleaveBonus).Signed().BonusOrPenalty()} when cleaving AV");
                }
                if (hitBonus != 0)
                {
                    E.AddWeaponElement("have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}");
                }
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(DescribeModificationEvent<ModGigantic> E)
        {
            if (E.Object == ParentObject 
                && E.Context == "Natural Equipment")
            {
                E.RemoveWeaponElement("have", "+3 damage");
                E.RemoveWeaponElement("cleave", "for -3 AV");
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
            return DescribeModificationEvent<ModGigantic>
                .Send(Object, GetColoredAdjective(), Context: "Natural Equipment")
                .Process(PluralizeObject: true);
        }

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}