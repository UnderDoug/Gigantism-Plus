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

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetCleaveAmountEvent.ID
                || ID == PooledEvent<GetDisplayNameEvent>.ID
                || ID == BeforeDescribeModGiganticEvent.ID
                || ID == DescribeModGiganticEvent.ID;
        }

        public override bool HandleEvent(GetCleaveAmountEvent E)
        {
            bool isReady = IsReady(UseCharge: true,
                                   IgnoreCharge: false,
                                   IgnoreLiquid: false,
                                   IgnoreBootSequence: false,
                                   IgnoreBreakage: false,
                                   IgnoreRust: false, IgnoreEMP: false,
                                   IgnoreRealityStabilization: false,
                                   IgnoreSubject: false,
                                   IgnoreLocallyDefinedFailure: false, 1, null,
                                   UseChargeIfUnpowered: false, 0L, null);

            if (IsObjectActivePartSubject(E.Object) && isReady)
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
            if (E.Object == ParentObject)
            {
                int dieCount = GetDamageDieCount();
                int damageBonus = GetDamageBonus();
                int hitBonus = GetHitBonus();
                int cleaveBonus = -damageBonus;

                if (dieCount > 0) E.WeaponDescriptions
                        .Add(new() { "gain", $"{dieCount} additional damage die" });

                if (damageBonus != 0) E.WeaponDescriptions
                        .Add(new() { "have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage" });

                if (damageBonus != 0 && ParentObject.TryGetPart(out MeleeWeapon weapon) && weapon.Skill == "Axe") E.WeaponDescriptions
                        .Add(new() { "has", $"a {cleaveBonus.Signed()} {(-cleaveBonus).Signed().BonusOrPenalty()} when cleaving AV" });

                if (hitBonus != 0) E.WeaponDescriptions
                        .Add(new() { "have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}" });
            }
            return base.HandleEvent(E);
        }

        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            if (E.Object == ParentObject)
            {
                List<List<string>> elementsToRemove = new() 
                {
                    new List<string>() { "have", "+3 damage" },
                    new List<string>() { "cleave", "for -3 AV" },
                };
                
                int indexToRemove = 0;
                List<List<string>> InumerateWeaponDescriptions = new(E.WeaponDescriptions); 
                foreach (List<string> entry in InumerateWeaponDescriptions)
                {
                    if (elementsToRemove.Contains(entry))
                        E.WeaponDescriptions.Remove(entry);

                }
                if (indexToRemove < E.WeaponDescriptions.Count)
                    E.WeaponDescriptions.RemoveAt(indexToRemove);
            }
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            BeforeDescribeModGiganticEvent beforeEvent = new(ParentObject, null);
            beforeEvent.Send();
            DescribeModGiganticEvent afterEvent = new(ParentObject, null, beforeEvent);

            return afterEvent.Send().Process(PluralizeObject: false);
        }

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}