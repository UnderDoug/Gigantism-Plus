using System;
using XRL.Language;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
    {
        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            /*
            ApplyGenericChanges(Object, NaturalEquipmentMod, GetInstanceDescription());

            ApplyPriorityChanges(Object, NaturalEquipmentMod);

            ApplyPartAndPropChanges(Object, NaturalEquipmentMod);
            */
            base.ApplyModification(Object);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (!E.Object.HasProperName)
            {
                E.AddAdjective(NaturalEquipmentSubpart.GetColoredAdjective(), NaturalEquipmentSubpart.AdjectivePriority);
            }
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(NaturalEquipmentSubpart.GetColoredAdjective());
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            int dieSizeIncrease = 2 + GetDamageDieSize();
            int damageBonusIncrease = 1 + GetDamageBonus();
            string description = $"{descriptionName}: ";
            description += ParentObject.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " have ")
                        : ("This " + text + " has ");
            if (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"{dieSizeIncrease.Signed()} to {pluralPossessive} damage die size, and ";
            }
            if (damageBonusIncrease == 0)
            {
                description += $"half {pluralPossessive} wielder's Strength Modifier damage.";
            }
            else
            {
                description += $"{damageBonusIncrease.Signed()} damage (Strength Modifier/2).";
            }
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}