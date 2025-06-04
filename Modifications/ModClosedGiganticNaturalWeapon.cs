using System;
using System.Collections.Generic;

using XRL.World.Parts.Mutation;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModClosedGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
    {
        private static bool doDebug => getClassDoDebug(nameof(ModClosedGiganticNaturalWeapon));

        public ModClosedGiganticNaturalWeapon()
        {
        }

        public ModClosedGiganticNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            if (ParentObject.TryGetPart(out NaturalEquipmentManager manager))
            {
                manager.DoDynamicTile = false;
            }
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

            List<List<string>> descriptions = new()
            {
                new() { null, $"tightly clenched" },
                new() { "", $"functions as a cudgel" }
            };

            List<string> processedDescriptions = new();
            foreach (List<string> entry in descriptions)
            {
                processedDescriptions.Add(entry.GetProcessedItem(second: false, descriptions, Object));
            }

            return description += Grammar.MakeAndList(processedDescriptions) + ".";
        }

    } //!-- public class ModClosedGiganticNaturalWeapon : ModNaturalEquipment<GigantismPlus>
}