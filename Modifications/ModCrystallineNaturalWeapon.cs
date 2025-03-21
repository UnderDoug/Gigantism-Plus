using System;
using System.Text;
using XRL.Language;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
    {
        public ModCrystallineNaturalWeapon()
        {
        }

        public ModCrystallineNaturalWeapon(int Tier)
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
                || ID == GetShortDescriptionEvent.ID
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

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            // E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public new string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = !ParentObject.HasNaturalWeaponMods() ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalWeaponSubpart.GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append($"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} has ").Append(dieSizeIncrease.Signed()).Append($" to {ParentObject.theirs} damage die size.");
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}