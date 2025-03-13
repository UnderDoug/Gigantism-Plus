using System;
using XRL.World.Parts.Mutation;
using XRL.Language;
using System.Text;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
    {
        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ModBurrowingNaturalWeapon modBurrowingNaturalWeapon = base.DeepCopy(Parent, MapInv) as ModBurrowingNaturalWeapon;
            modBurrowingNaturalWeapon.AssigningPart = null;
            modBurrowingNaturalWeapon.Wielder = null;
            modBurrowingNaturalWeapon.NaturalWeapon = null;
            return modBurrowingNaturalWeapon;
        }

        public override void ApplyModification(GameObject Object)
        {
            ApplyGenericChanges(Object, NaturalWeapon, GetInstanceDescription());

            ApplyPriorityChanges(Object, NaturalWeapon, NaturalWeapon.GetNounPriority());

            Object.RequirePart<DiggingTool>();
            Object.RequirePart<BurrowingClawsProperties>();

            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level);
            burrowingClawsProperties.WallBonusPercentage = BurrowingClaws.GetWallBonusPercentage(Level);

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
                E.AddAdjective(NaturalWeapon.GetColoredAdjective(), NaturalWeapon.GetAdjectivePriority());
            }
            return base.HandleEvent(E);
        }

        public new string GetInstanceDescription()
        {
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            int dieSizeIncrease = GetDamageDieSize();
            string wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(Level).Signed();
            int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(Level, Wielder);

            StringBuilder stringBuilder = Event.NewStringBuilder().Append(descriptionName).Append(": ")
                .Append($"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} has ");

            if (dieSizeIncrease > 0)
            {
                stringBuilder.Append(dieSizeIncrease.Signed()).Append($" to {ParentObject.theirs} damage die size").Append(wallHitsRequired > 0 ? ", " : " and ");
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