using System;
using System.Collections.Generic;
using System.Text;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModBurrowingNaturalWeapon : ModNaturalEquipment<UD_ManagedBurrowingClaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModBurrowingNaturalWeapon));

        public ModBurrowingNaturalWeapon()
        {
        }

        public ModBurrowingNaturalWeapon(int Tier)
            : base(Tier)
        {
        }
        public ModBurrowingNaturalWeapon(ModNaturalEquipmentBase Conversion)
            : this()
        {
            Wielder = Conversion.Wielder;

            Adjustments = new();
            foreach (PartAdjustment adjustment in Conversion.Adjustments)
            {
                Adjustments.Add(adjustment);
            }

            BodyPartType = Conversion.BodyPartType;

            ModPriority = Conversion.ModPriority;
            DescriptionPriority = Conversion.DescriptionPriority;

            DamageDieCount = Conversion.DamageDieCount;
            DamageDieSize = Conversion.DamageDieSize;
            DamageBonus = Conversion.DamageBonus;
            HitBonus = Conversion.HitBonus;
            PenBonus = Conversion.PenBonus;

            Adjective = Conversion.Adjective;
            AdjectiveColor = Conversion.AdjectiveColor;
            AdjectiveColorFallback = Conversion.AdjectiveColorFallback;

            AddedParts = new();
            foreach (string addedPart in Conversion.AddedParts)
            {
                AddedParts.Add(addedPart);
            }
            AddedStringProps = new();
            foreach ((string prop, string value) in Conversion.AddedStringProps)
            {
                AddedStringProps[prop] = value;
            }
            AddedIntProps = new();
            foreach ((string prop, int value) in Conversion.AddedIntProps)
            {
                AddedIntProps[prop] = value;
            }
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4,
                $"\u2666 {typeof(ModBurrowingNaturalWeapon).Name}." +
                $"{nameof(ApplyModification)}(GameObject Object)",
                Indent: 3, Toggle: doDebug);
            base.ApplyModification(Object);

            Object.RequirePart<BurrowingClawsProperties>();
            BurrowingClawsProperties burrowingClawsProperties = Object.GetPart<BurrowingClawsProperties>();
            burrowingClawsProperties.WallBonusPenetration = UD_ManagedBurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            burrowingClawsProperties.WallBonusPercentage = UD_ManagedBurrowingClaws.GetWallBonusPercentage(AssigningPart.Level);
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

        public override string GetInstanceDescription(GameObject Object = null)
        {
            Object ??= ParentObject;
            string text = Object?.GetObjectNoun();
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            string description = $"{descriptionName}: ";
            description += Object != null && Object.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " ")
                        : ("This " + text + " ");

            string pluralPossessive = Object.IsPlural ? "their" : "its";
            int dieSize = GetDamageDieSize(); 
            int wallBonusPenetration = UD_ManagedBurrowingClaws.GetWallBonusPenetration(AssigningPart.Level);
            int wallHitsRequired = UD_ManagedBurrowingClaws.GetWallHitsRequired(AssigningPart.Level, Wielder);
           
            List<List<string>> descriptions = new();
            if (dieSize != 0)
            {
                descriptions.AddDescription("gain", $"{dieSize.Signed()} damage die size");
            }
            if (wallBonusPenetration != 0)
            {
                descriptions.AddDescription("get", $"{wallBonusPenetration.Signed()} penetration vs. walls");
            }
            if (wallHitsRequired != 0)
            {
                descriptions.AddDescription("destroy", $"walls after {wallHitsRequired} penetrating hits");
            }

            List<string> processedDescriptions = new();
            foreach (List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, Object));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }
    } //!-- public class ModBurrowingNaturalWeapon : ModNaturalWeaponBase<UD_ManagedBurrowingClaws>
}