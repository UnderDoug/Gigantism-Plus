using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModGiganticNaturalWeapon 
        : ModNaturalEquipment<GigantismPlus>
        , IModEventHandler<BeforeDescribeModGiganticEvent>
        , IModEventHandler<DescribeModGiganticEvent>
    {
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
            Registrar.Register(BeforeDescribeModGiganticEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(DescribeModGiganticEvent.ID, EventOrder.EXTREMELY_LATE);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetCleaveAmountEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }
        public override bool HandleEvent(GetCleaveAmountEvent E)
        {
            if (IsObjectActivePartSubject(E.Object))
            {
                E.Amount += 1 + GetDamageBonus();
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }
        public bool HandleEvent(BeforeDescribeModGiganticEvent E)
        {
            if (E.Object == ParentObject && E.Context == "Natural Equipment")
            {
                int dieCount = GetDamageDieCount();
                int damageBonus = GetDamageBonus();
                int hitBonus = GetHitBonus();
                int cleaveBonus = -damageBonus;

                if (dieCount > 0)
                {
                    E.AddWeaponDescription("gain", $"{dieCount} additional damage die");
                }
                if (damageBonus != 0)
                {
                    E.AddWeaponDescription("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
                }
                if (damageBonus != 0 && ParentObject.TryGetPart(out MeleeWeapon weapon) && weapon.Skill == "Axe")
                {
                    E.AddWeaponDescription("has", $"a {cleaveBonus.Signed()} {(-cleaveBonus).Signed().BonusOrPenalty()} when cleaving AV");
                }
                if (hitBonus != 0)
                {
                    E.AddWeaponDescription("have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}");
                }
            }
            return base.HandleEvent(E);
        }
        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            if (E.Object == ParentObject && E.Context == "Natural Equipment")
            {
                E.RemoveWeaponDescription("have", "+3 damage");
                E.RemoveWeaponDescription("cleave", "for -3 AV");
            }
            return base.HandleEvent(E);
        }
        public override string GetInstanceDescription()
        {
            BeforeDescribeModGiganticEvent beforeEvent = BeforeDescribeModGiganticEvent.CollectFor(ParentObject, Context: "Natural Equipment");
            return DescribeModGiganticEvent.Send(beforeEvent).Process(PluralizeObject: true);

        }

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}