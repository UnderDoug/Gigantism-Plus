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
            string descriptionName = !ParentObject.HasNaturalWeaponMods() ? "\n" : "";
            descriptionName += Grammar.MakeTitleCase(NaturalEquipmentSubpart.GetColoredAdjective());
            int dieSizeIncrease = 2 + GetDamageDieSize();
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append(ParentObject.IsPlural 
                        ? ("These " + Grammar.Pluralize(text) + " have ") 
                        : ("This " + text + " has "))
                .Append(dieSizeIncrease.Signed()).Append($" to {pluralPossessive} damage die size.");
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}