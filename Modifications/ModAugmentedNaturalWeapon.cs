using HNPS_GigantismPlus;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Parts.Mutation;

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
            base.ApplyModification(Object);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID;
        }

        public override string GetInstanceDescription()
        {
            string cyberneticsObject = AssigningPart.ImplantObject.ShortDisplayName;
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(AssigningPart.GetNaturalWeaponColoredAdjective());
            string description = $"{descriptionName}: ";
            description += $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} ";
            description += $"has some of its bonuses applied by an implanted {cyberneticsObject}.";
            return description;
        }
    } //!-- public class ModAugmentedNaturalWeapon : ModNaturalWeaponBase<CyberneticsGiganticExoframe>
}