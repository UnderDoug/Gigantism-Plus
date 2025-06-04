using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModElongatedNaturalWeapon : ModNaturalEquipment<ElongatedPaws>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModElongatedNaturalWeapon));

        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            base.ApplyModification(Object);
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
            if (dieSize > 0 && (!AssigningPart.HasGigantism || !AssigningPart.HasBurrowing))
            {
                descriptions.AddDescription("gain", $"{dieSize.Signed()} damage die size");
            }

            if (damageBonus != 0)
            {
                descriptions.AddDescription("have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage");
            }

            descriptions.AddDescription("", $"{pluralPossessive} bonus damage scales by half {pluralPossessive} wielder's Strength Modifier");

            List<string> processedDescriptions = new();
            foreach(List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, Object));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}