using HNPS_GigantismPlus;
using System;
using XRL.Language;
using XRL.World.Parts.Mutation;
using static XRL.World.IManagedDefaultNaturalWeapon;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModAugmentedNaturalWeapon : ModNaturalWeaponBase<CyberneticsGiganticExoframe>
    {
        public ModAugmentedNaturalWeapon()
        {
        }

        public ModAugmentedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override int ApplyPriorityChanges(GameObject Object, INaturalWeapon NaturalWeapon)
        {
            Debug.Entry(4, $"* {nameof(ApplyPriorityChanges)}(GameObject Object, INaturalWeapon NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"{AssigningPart.Name}", Indent: 5);

            Render render = Object.Render;
            MeleeWeapon weapon = Object.GetPart<MeleeWeapon>();

            string[] vomitCats = new string[] { "Render" };
            int AdjectivePriority = NaturalWeapon.GetAdjectivePriority() - 100;
            int CurrentNounPriority = Object.GetIntProperty(CURRENT_NOUN_PRIORITY);

            // Debug.Entry(4, $"Using inverted Adjective Priority to force the Augmented Render changes through", Indent: 5);
            Debug.Entry(4, $"? if (AdjectivePriority != 0 && CurrentNounPriority > AdjectivePriority)", Indent: 5);
            if (AdjectivePriority != 0 && AdjectivePriority < CurrentNounPriority)
            {
                Debug.Entry(4,
                    $"+ AdjectivePriority != 0 && "
                    + $"CurrentNounPriority AdjectivePriority ({AdjectivePriority}) < ({CurrentNounPriority})",
                    Indent: 6);

                weapon.Vomit(4, "SpecialAdjectivePriority, Before", vomitCats, Indent: 6);

                render.Tile = NaturalWeapon.Tile ?? render.Tile;

                render.ColorString =
                    (render.ColorString == NaturalWeapon.ColorString)
                    ? NaturalWeapon.SecondColorString
                    : NaturalWeapon.ColorString;

                render.DetailColor =
                    (render.DetailColor == NaturalWeapon.DetailColor)
                    ? NaturalWeapon.SecondDetailColor
                    : NaturalWeapon.DetailColor;

                Object.SetSwingSound(NaturalWeapon.GetSwingSound());
                Object.SetBlockedSound(NaturalWeapon.GetBlockedSound());

                weapon.Vomit(4, "SpecialAdjectivePriority, After", vomitCats, Indent: 6);
            }
            else
            {
                Debug.Entry(4,
                    $"- AdjectivePriority == 0 || "
                    + $"AdjectivePriority ({AdjectivePriority}) >= CurrentNounPriority ({CurrentNounPriority})",
                    Indent: 6);
                weapon.Vomit(4, "AdjectivePriority, Unchanged", vomitCats, Indent: 6);
            }
            Debug.Entry(4, $"x if (AdjectivePriority != 0 && AdjectivePriority < CurrentNounPriority) ?//", Indent: 5);

            Debug.Entry(4, $"* base.{nameof(ApplyPriorityChanges)}(Object, NaturalWeapon)", Indent: 4);
            Debug.Entry(4, $"x {nameof(ApplyPriorityChanges)}(GameObject Object, INaturalWeapon NaturalWeapon) *//", Indent: 4);
            return base.ApplyPriorityChanges(Object, NaturalWeapon);
        }

        public override void ApplyModification(GameObject Object)
        {
            ApplyGenericChanges(Object, NaturalWeapon, GetInstanceDescription());

            ApplyPriorityChanges(Object, NaturalWeapon);

            ApplyPartAndPropChanges(Object, NaturalWeapon);

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
                E.AddAdjective(AssigningPart.GetNaturalWeaponColoredAdjective(), NaturalWeapon.GetAdjectivePriority());
            }
            return base.HandleEvent(E);
        }

        public override string GetInstanceDescription()
        {
            string cyberneticsObject = AssigningPart.ImplantObject.ShortDisplayName;
            string text = "weapon";
            string descriptionName = Grammar.MakeTitleCase(NaturalWeapon.GetColoredAdjective());
            string description = $"{descriptionName}: ";
            description += $"{(ParentObject.IsPlural ? ("These " + Grammar.Pluralize(text)) : ("This " + text))} ";
            description += $"has some of its bonuses applied by an implanted {cyberneticsObject}.";
            return description;
        }
    } //!-- public class ModElongatedNaturalWeapon : ModNaturalWeaponBase<ElongatedPaws>
}