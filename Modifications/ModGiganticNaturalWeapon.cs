using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
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
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"{nameof(ModGiganticNaturalWeapon)}." +
                $"{nameof(HandleEvent)}({nameof(DescribeModGiganticEvent)} E.Context: {E.Context})",
                Indent: indent + 1, Toggle: doDebug);

            if (E.Object == ParentObject && E.Context == "Natural Equipment")
            {
                Debug.CheckYeh(4, $"Ye", Indent: indent + 2, Toggle: doDebug);

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

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"{nameof(ModGiganticNaturalWeapon)}." +
                $"{nameof(HandleEvent)}({nameof(DescribeModGiganticEvent)} E.Context: {E.Context})",
                Indent: indent + 1, Toggle: doDebug);

            if (E.Object == ParentObject && E.Context == "Natural Equipment")
            {
                Debug.CheckYeh(4, $"Ye", Indent: indent + 2, Toggle: doDebug);
                E.RemoveWeaponDescription("have", "+3 damage");
                E.RemoveWeaponDescription("cleave", "for -3 AV");
            }
            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public override string GetInstanceDescription(GameObject Object = null)
        {
            Object ??= ParentObject;
            BeforeDescribeModGiganticEvent beforeEvent = BeforeDescribeModGiganticEvent.CollectFor(Object, Context: "Natural Equipment");
            return DescribeModGiganticEvent.Send(beforeEvent).Process(PluralizeObject: true);
        }

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}