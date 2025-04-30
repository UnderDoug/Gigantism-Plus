using System;
using System.Collections.Generic;

using XRL.Language;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModAugmentedNaturalWeapon : ModNaturalEquipment<CyberneticsGiganticExoframe>
    {
        public ModAugmentedNaturalWeapon()
        {
        }

        public ModAugmentedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void ApplyModification(GameObject Object)
        {
            if(ParentObject.TryGetPart(out NaturalEquipmentManager manager))
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

        public override string GetColoredAdjective()
        {
            return AssigningPart?.GetNaturalEquipmentColoredAdjective() ?? base.GetColoredAdjective();
        }

        public override string GetInstanceDescription()
        {
            string cyberneticsObject = AssigningPart.ImplantObject.ShortDisplayName;
            string text = ParentObject.GetObjectNoun();
            string descriptionName = Grammar.MakeTitleCase(AssigningPart.GetNaturalEquipmentColoredAdjective());
            string description = $"{descriptionName}: ";
            description += $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text) + " have ") : ("This " + text + " has "))} ";
            description += $"some of its bonuses applied by an implanted {cyberneticsObject}.";
            return description;
        }
    } //!-- public class ModAugmentedNaturalWeapon : ModNaturalWeaponBase<CyberneticsGiganticExoframe>
}