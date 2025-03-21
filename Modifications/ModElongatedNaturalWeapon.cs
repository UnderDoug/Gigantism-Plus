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
            ApplyGenericChanges(Object, NaturalWeaponSubpart, GetInstanceDescription());

            ApplyPriorityChanges(Object, NaturalWeaponSubpart);

            ApplyPartAndPropChanges(Object, NaturalWeaponSubpart);
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
                E.AddAdjective(NaturalWeaponSubpart.GetColoredAdjective(), NaturalWeaponSubpart.AdjectivePriority);
            }
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(NaturalWeaponSubpart.GetColoredAdjective());
            string description = $"{descriptionName}: ";
            description += $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} ";
            if (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"has {GetDamageDieSize().Signed()} to {ParentObject.theirs} damage die size, and ";
            }
            if (GetDamageBonus() == 0)
            {
                description += $"adds half {ParentObject.theirs} wielder's Strength Modifier damage.";
            }
            else
            {
                description += $"has {GetDamageBonus().Signed()} damage (Strength Modifier/2).";
            }
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}