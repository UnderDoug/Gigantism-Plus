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

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModCrystallineNaturalWeapon modCrystallineNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModCrystallineNaturalWeapon;
            modCrystallineNaturalWeapon.AssigningMutation = null;
            modCrystallineNaturalWeapon.Wielder = null;
            modCrystallineNaturalWeapon.NaturalWeapon = null;
            return modCrystallineNaturalWeapon;
        }

        public override void ApplyModification(GameObject Object)
        {
            ApplyGenericChanges(Object, NaturalWeapon, GetInstanceDescription());

            ApplyPriorityChanges(Object, NaturalWeapon, NaturalWeapon.GetNounPriority());

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
                E.AddAdjective(NaturalWeapon.GetColoredAdjective(), NaturalWeapon.GetAdjectivePriority());
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
            descriptionName += Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append($"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} has ").Append(dieSizeIncrease.Signed()).Append($" to {ParentObject.theirs} damage die size.");
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}