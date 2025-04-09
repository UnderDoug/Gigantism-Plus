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
                int damageBonus = 2+ GetDamageBonus();
                int hitBonus = GetHitBonus();

                if (dieCount > 0) E.WeaponDescriptions
                        .Add(new() { "gain", $"{dieCount} additional damage die" });

                if (damageBonus != 0) E.WeaponDescriptions
                        .Add(new() { "have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage" });

                if (hitBonus != 0) E.WeaponDescriptions
                        .Add(new() { "have", $"a {hitBonus.Signed()} hit {hitBonus.Signed().BonusOrPenalty()}" });
            }
            return base.HandleEvent(E);
        }

        public bool HandleEvent(DescribeModGiganticEvent E)
        {
            if (E.Object == ParentObject)
            {
                List<string> elementToRemove = new() { "have", "+3 damage" };
                int indexToRemove = 0;
                foreach (List<string> entry in E.WeaponDescriptions)
                {
                    if (entry[0] == elementToRemove[0] && entry[1] == elementToRemove[1])
                        break;
                    indexToRemove++;
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

            return afterEvent.Send().Process();

            Debug.Entry(4, $"{typeof(ModGiganticNaturalWeapon).Name}.{nameof(GetInstanceDescription)}()");
            MeleeWeapon part = ParentObject.GetPart<MeleeWeapon>();
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            int damageBonus = GetDamageBonus();
            List<List<string>> list = new();
            string text = ParentObject.GetObjectNoun();
            if (part != null && ParentObject.HasTagOrProperty("ShowMeleeWeaponStats"))
            {
                list.Add(new List<string> { "have", $"{damageBonus.Signed()} damage" });
                if (GetDamageDieCount() != 0)
                {
                    list.Add(new List<string> { "have", $"{GetDamageDieCount().Signed()} damage die count" });
                }
                if (GetHitBonus() != 0)
                {
                    list.Add(new List<string> { "have", $"a {GetHitBonus().Signed()} {(GetHitBonus() >= 0 ? "bonus" : "penalty")} to hit" });
                }
                if (part.Skill == "Cudgel")
                {
                    list.Add(new List<string> { null, "twice as effective when you Slam with " + ParentObject.them });
                }
                else if (part.Skill == "Axe")
                {
                    list.Add(new List<string> { "cleave", $"for {(-damageBonus).Signed()} AV" });
                }
            }
            if (ParentObject.HasPart<DiggingTool>() || ParentObject.HasPart<Drill>())
            {
                list.Add(new List<string> { "dig", "twice as fast" });
            }
            if (list.Count == 0)
            {
                return $"{descriptionName}: " + $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} has {ParentObject.theirs} damage die count determined by {ParentObject.theirs} wielder's size.";
            }
            List<string> list2 = new();
            foreach (List<string> item in list)
            {
                list2.Add(GetProcessedItem(item, second: false, list, ParentObject));
            }
            return $"{descriptionName}: " + (ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text)) + " " + Grammar.MakeAndList(list2) + ".";
        }

    } //!-- public class ModGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}