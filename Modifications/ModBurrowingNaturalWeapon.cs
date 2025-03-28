using System;
using XRL.World.Parts.Mutation;
using XRL.Language;
using System.Text;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalEquipment<UD_ManagedBurrowingClaws>
    {
        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            base.ApplyModification(Object);

            Object.RequirePart<BurrowingClawsProperties>();
            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            burrowingClawsProperties.WallBonusPercentage = BurrowingClaws.GetWallBonusPercentage(AssigningPart.Level);

        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            int Level = AssigningPart.Level;
            string wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level).Signed();
            int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(Level, Wielder);
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append(ParentObject.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " have ")
                        : ("This " + text + " has "));

            if (dieSizeIncrease > 0)
            {
                stringBuilder.Append(dieSizeIncrease.Signed()).Append($" to {pluralPossessive} damage die size").Append(wallHitsRequired > 0 ? ", " : " and ");
            }
            stringBuilder.Append(wallBonusPenetration).Append(" penetration vs. walls").Append(dieSizeIncrease > 0 ? ", " : " ")
                .Append(wallHitsRequired > 0 ? "and " : ".");

            if (wallHitsRequired > 0)
            {
                stringBuilder.Append("destroys walls after ").Append(wallHitsRequired).Append(" penetrating hits.");
            }
            return Event.FinalizeString(stringBuilder);
        }
    } //!-- public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
}