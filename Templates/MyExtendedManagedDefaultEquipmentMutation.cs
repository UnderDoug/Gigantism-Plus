using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.World.Anatomy;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts.Mutation
{
    public class MyExtendedManagedDefaultEquipmentMutation : MyDefaultEquipmentMutation, IManagedDefaultNaturalWeapon
    {
        // Required by IManagedDefaultNaturalWeapon to ensure the implementation of the INaturalWeapon Part
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {

        }

        public INaturalWeapon NaturalWeapon = new()
        {
            // Base values for the first level of mutation.
            // These are used as a fallback in case there's a breakdown between
            // the mutation and its modification.
            DamageDieCount = 1,             // left side of a DieRoll (default is 1)
            DamageDieSize = 2,              // right side of a DieRoll (default is 2)
            DamageBonus = 3,                // the + side of a DieRoll (default is -1)
            HitBonus = 0,                   // additional chance to hit (default is 0)
            // 1d2+3

            // These determine which skill the weapon uses and which attribute its penetration scales off
            Skill = "Axes",                 // Must match the skill name, weapon is an Axe (default is Cudgels)
            Stat = "Strength",              // PV bonus comes from Agility (default is Strength)

            // These define how and where the modification is displayed.
            ModPriority = 50,               // There'll be a rundow of this on the mod's GitHub Wiki
            Noun = "spade",                 // Describes the resultant limb (default is fist) 
            Adjective = "hooked",           // Appears in the display name: "hooked spade" (no default)
            AdjectiveColor = "R",           // Can be a shader, alters the color of the displayed adjective (no default)
            Tile = "Creatures/natural-weapon-drill.bmp", // The path to the Tile that will appear in the equipment slot
            RenderColorString = "&w",       // The black pixels of the Tile
            RenderDetailColor = "r",        // The white pixels of the Tile
            SecondRenderColorString = "&W", // If the tile is already the above color, this get used instead
            SecondRenderDetailColor = "R",  // If the tile is already the above color, this get used instead
        };

        // Required by IManagedDefaultNaturalWeapon and allows the Modification to get the above part from the mutation.
        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        // Required by IManagedDefaultNaturalWeapon and can be used further down to easily reference the Modification.
        public virtual string GetNaturalWeaponMod()
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon";
        }

        // Optional: allows you to easily check the presence of another mutation if you wanted to adjust any calculations on that basis
        public bool HasGigantism
        {
            get
            {
                return ParentObject.HasPart<GigantismPlus>();
            }
        }

        // Required by IManagedDefaultNaturalWeapon and is used to calculate the relevant damage component
        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            if (HasGigantism) // Only apply the calculation if Gigantism isn't present
            {
                NaturalWeapon.DamageDieSize = 2; 
                return true;
            }
            NaturalWeapon.DamageDieSize = 1 + (int)Math.Floor((double)Level/3); // Starting at 2, increase every 3rd level
            return true;
        }

        // Required by IManagedDefaultNaturalWeapon and is used to calculate the relevant damage component
        // Can be altered as above to be as simple or complex as you like 
        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1) { return true; }
        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1) { return true; }
        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1) { return true; }

        // ChangeLevel is the first call in the process for default equipment being updated/generated
        // so it makes sense to calculate the values here
        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        // OnRegenerateDefaultEquipment gets called at a few points so it makes sense to apply the modifications at this point.
        // The call is after the base default fists have been destroyed and recreated.
        // While reliable in catching all Hand and Hand variants, the code below is optional and simply serves as an example
        // of how you might loop through parts and apply the modification.
        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (body == null)
            {
                base.OnRegenerateDefaultEquipment(body);
                return;
            }

            string targetPartType = "Hand";

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            foreach (BodyPart part in list)
            {
                if (part.Type == "Hand")
                {
                    // This is the important part, and is how the modification gets applied.
                    // the process will fail if there's no default behavior (Hand parts always have them), so you may want to 
                    // do a (part.DefaultBehavior != null) check first and add a default behavior to Modify.
                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod(), Actor: ParentObject);
                }
            }
        }

        // You'll want some variation of this to ensure that things like Temporal Fugue clones don't delete your 
        // NaturalWeapon Part by being copied with a reference to it.
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            MyExtendedManagedDefaultEquipmentMutation myMutation = base.DeepCopy(Parent, MapInv) as MyExtendedManagedDefaultEquipmentMutation;
            myMutation.NaturalWeapon = null;
            return myMutation;
        }
    }

    // The mutation you're extending. It would contain the rest of the Mutation's capabilities.
    [Serializable]
    public class MyDefaultEquipmentMutation : BaseDefaultEquipmentMutation
    {

    }
}