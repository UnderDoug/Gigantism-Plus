using System;
using System.Collections.Generic;
using System.Text;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModCrystallineNaturalWeapon : ModNaturalEquipment<UD_ManagedCrystallinity>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModCrystallineNaturalWeapon));

        public ModCrystallineNaturalWeapon()
        {
        }

        public ModCrystallineNaturalWeapon(int Tier)
            : base(Tier)
        {
        }
        public ModCrystallineNaturalWeapon(ModNaturalEquipmentBase Conversion)
            : this()
        {
            Wielder = Conversion.Wielder;

            Adjustments = new();
            foreach (HNPS_Adjustment adjustment in Conversion.Adjustments)
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
            int damageBonus = GetDamageBonus();

            List<List<string>> descriptions = new();
            if (dieSize > 0)
            {
                descriptions.AddDescription("gain", $"{dieSize.Signed()} damage die size");
            }
            if (damageBonus != 0)
            {
                descriptions.AddDescription("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
            }
            descriptions.AddDescription(null, $"inorganic");

            List<string> processedDescriptions = new();
            foreach (List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, Object));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }
    } //!-- public class ModCrystallineNaturalWeapon : ModNaturalWeaponBase<UD_ManagedCrystallinity>
}