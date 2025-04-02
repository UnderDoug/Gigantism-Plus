using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModElongatedNaturalWeapon : ModNaturalEquipment<ElongatedPaws>
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

        public override string GetInstanceDescription()
        {
            string text = ParentObject.GetObjectNoun();
            string descriptionName = Grammar.MakeTitleCase(GetColoredAdjective());
            string pluralPossessive = ParentObject.IsPlural ? "their" : "its";
            int dieSize = GetDamageDieSize();
            int damageBonus = GetDamageBonus();
            string description = $"{descriptionName}: ";
            description += ParentObject.IsPlural
                        ? ("These " + Grammar.Pluralize(text) + " ")
                        : ("This " + text + " ");

            List<List<string>> descriptions = new();
            if (dieSize > 0 && (!Wielder.HasPart<GigantismPlus>() || !Wielder.HasPart<BurrowingClaws>())) descriptions
                    .Add(new() { "gain", $"{dieSize.Signed()} damage die size" });

            if (damageBonus != 0) descriptions
                    .Add(new() { "have", $"a {damageBonus.Signed()} {damageBonus.Signed().BonusOrPenalty()} to damage" });
            descriptions
                    .Add(new() { "", $"{pluralPossessive} bonus damage scales by half {pluralPossessive} wielder's Strength Modifier" });

            List<string> processedDescriptions = new();
            foreach(List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, ParentObject));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}