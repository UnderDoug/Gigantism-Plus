using System;
using System.Collections.Generic;
using System.Text;
using XRL.World.Parts.Mutation;
using XRL.Language;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

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
            string text = ParentObject.GetObjectNoun();
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            int dieSize = GetDamageDieSize(); 
            int wallBonusPenetration = BurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            int wallHitsRequired = BurrowingClaws.GetWallHitsRequired(AssigningPart.Level, Wielder);
            string description = $"{descriptionName}: ";
            description += ParentObject.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " ")
                        : ("This " + text + " ");

            List<List<string>> descriptions = new();
            if (dieSize != 0) descriptions
                    .Add(new() { "gain", $"{dieSize.Signed()} damage die size" });
            if (wallBonusPenetration != 0) descriptions
                    .Add(new() { "get", $"{wallBonusPenetration.Signed()} penetration vs. walls" });
            if (wallHitsRequired != 0) descriptions
                    .Add(new() { "destroy", $"walls after {wallHitsRequired} penetrating hits" });

            List<string> processedDescriptions = new();
            foreach (List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, ParentObject));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }
    } //!-- public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
}