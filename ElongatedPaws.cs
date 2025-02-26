using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches; // Add this line
using static Mods.GigantismPlus.HelperMethods;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws : BaseDefaultEquipmentMutation
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };
        
        private static readonly List<string> NaturalWeaponSupersedingMutations = new List<string>
        {
          //"CyberneticsGiganticExoframe",
            "GigantismPlus",
            "BurrowingClaws",
            "Crystallinity"
        };

        public bool IsNaturalWeaponSuperseded
        {
            get
            {
                if (ParentObject == null) return false;
                int count = 0;
                foreach (string mutation in NaturalWeaponSupersedingMutations)
                {
                    if (ParentObject.HasPart(mutation))
                    {
                        count++;
                    }
                }
                return count > 0;
            }
        }

        private List<string> GetCompatibleMutations()
        {
            List<string> list = new List<string>();
            MutationEntry mutationEntry;
            string displayName = "";
            foreach (string mutation in NaturalWeaponSupersedingMutations)
            {
                if (ParentObject.HasPart(mutation))
                {
                    XRL.MutationFactory.TryGetMutationEntry(mutation, out mutationEntry);
                    displayName = mutationEntry.DisplayName;
                    list.Add(displayName);
                }
            }
            return list;
        }

        // public GameObject ElongatedPawObject;

        public static string ElongatedPawBlueprintName = "ElongatedPaw";

        public int StrengthModifier => ParentObject.StatMod("Strength");

        public int ElongatedBonusDamage
        {
            get
            {
                return (int)Math.Floor((double)this.StrengthModifier / 2.0);
            }
        }

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}";
            base.Type = "Physical";
        }

        /* May be redundant.
         * 
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            ElongatedPaws paws = base.DeepCopy(Parent, MapInv) as ElongatedPaws;
            paws.ElongatedPawObject = null;
            paws.GiganticElongatedPawObject = null;
            paws.ElongatedBurrowingClawObject = null;
            paws.GiganticElongatedBurrowingClawObject = null;
            return paws;
        }
        */

        public override bool CanLevel() { return false; }

        public override bool AllowStaticRegistration() { return true; }

        public override string GetDescription()
        {
            string CompatibleMutations = "";
            if (IsNaturalWeaponSuperseded)
            {
                CompatibleMutations = "\nThe above damage is being improved by";
                foreach (string mutation in GetCompatibleMutations())
                {
                    CompatibleMutations += " " + mutation + ",";
                }
                CompatibleMutations = CompatibleMutations.Substring(CompatibleMutations.Length-1);
                CompatibleMutations += ".";
            }
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have {{giant|elongated paws}}, which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons {{gigantic|a size bigger}} than you are as though they were your size."
                 + "\n\nYour {{giant|elongated paws}} count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|(StrMod/2)}}"
                 + CompatibleMutations
                 + "\n\n+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

        public override string GetLevelText(int Level) { return ""; }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == StatChangeEvent.ID;
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            if (Array.IndexOf(AffectedSlotTypes, E.SlotType) >= 0 && E.Actor == ParentObject)
            {
                E.Decreases++;
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            if (E.Name == "Strength")
            {
                Body body = E.Object.Body;
                if (body != null)
                {
                    body.UpdateBodyParts();
                }
            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Body body = GO.Body;
           
            /* Checking if this is redundant.
             * 
            if (body != null && !this.IsNaturalWeaponSuperseded)
            {
                foreach (BodyPart hand in body.GetParts(EvenIfDismembered: true))
                {
                    if (hand.Type == "Hand" && hand.DefaultBehavior.Blueprint == ElongatedPawBlueprintName)
                    {
                        hand.DefaultBehavior = null;
                    }
                }
            } */
            
            CheckAffected(GO, body);
            return base.Unmutate(GO);
        }

        public void CheckAffected(GameObject Actor, Body Body)
        {
            if (Actor == null || Body == null)
            {
                return;
            }
            List<GameObject> list = Event.NewGameObjectList();
            foreach (BodyPart item in Body.LoopParts())
            {
                if (Array.IndexOf(AffectedSlotTypes, item.Type) < 0)
                {
                    continue;
                }
                GameObject equipped = item.Equipped;
                if (equipped != null && !list.Contains(equipped))
                {
                    list.Add(equipped);
                    int partCountEquippedOn = Body.GetPartCountEquippedOn(equipped);
                    int slotsRequiredFor = equipped.GetSlotsRequiredFor(Actor, item.Type);
                    if (partCountEquippedOn != slotsRequiredFor && item.TryUnequip(Silent: true, SemiForced: true) && partCountEquippedOn > slotsRequiredFor)
                    {
                        equipped.SplitFromStack();
                        item.Equip(equipped, 0, Silent: true, ForDeepCopy: false, Forced: false, SemiForced: true);
                    }
                }
            }
        }

        public void AddElongatedNaturalEquipmentTo(BodyPart part)
        {
            Debug.Entry(2, "@ AddGiganticNaturalEquipmentTo(BodyPart part)");
            if (part != null && part.Type == "Hand")
            {
                Debug.Entry(3, "* if (ParentObject.HasPart<GigantismPlus>())");
                Debug.Entry(3, "* else if (ParentObject.HasPart<BurrowingClaws>())");
                int StatMod = ElongatedBonusDamage;

                Debug.Entry(4, "- Saving copy of current DefaultBehavior in case creation fails");
                GameObject OldDefaultBehavior = part.DefaultBehavior;

                Debug.Entry(3, $"- Setting part.DefaultBehaviour to new instance of \"{ElongatedPawBlueprintName}\"");
                part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(ElongatedPawBlueprintName);

                Debug.Entry(3, $"- Checking that new GameObject was instantiated and assigned correctly");
                Debug.Entry(4, "* if (part.DefaultBehavior != null)");
                if (part.DefaultBehavior != null)
                {
                    Debug.Entry(3, "-- part.DefaultBehavior not null, assigning stats");
                    part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "ElongatedPaws", false);
                    var weapon = part.DefaultBehavior.GetPart<MeleeWeapon>();
                    
                    weapon.BaseDamage = $"1d4+{StatMod}";

                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | PenCap: {weapon.MaxStrengthBonus}");
                }
                else
                {
                    Debug.Entry(3, $"-- part.DefaultBehavior was null, invalid blueprint name \"{ElongatedPawBlueprintName}\"");
                    part.DefaultBehavior = OldDefaultBehavior;
                    Debug.Entry(3, $"-- OldDefaultBehavior reassigned");
                }
            }
            else
            {
                Debug.Entry(2, "- part null or not type \"Hand\"");
            }
            Debug.Entry(2, "x AddElongatedNaturalEquipmentTo(BodyPart part) ]//");
        } //!--- public void AddElongatedNaturalEquipmentTo(BodyPart part)

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Debug.Entry(2, "__________________________________________________________________");
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2, "@ ElongatedPaws.OnRegenerateDefaultEquipment(Body body)");
            Debug.Entry(2, $"TARGET: {ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            if (!this.IsNaturalWeaponSuperseded && body != null)
            {
                Debug.Entry(3, "- NaturalEquipment not Superseded");

                Debug.Entry(3, "Performing application of behavior to parts");

                string targetPartType = "Hand";
                Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
                Debug.Entry(4, "Generating List<BodyPart> list");
                // Just change the body part search logic
                List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                       where p.VariantType == targetPartType  // Changed from VariantType to Type
                                       select p).ToList<BodyPart>();

                Debug.Entry(4, "Checking list of parts for expected entries");
                Debug.Entry(4, "* foreach (BodyPart part in list)");
                foreach (BodyPart part in list)
                {
                    Debug.Entry(4, $"-- {part.Type}");
                    if (part.Type == "Hand")
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Debug.Entry(3, $"--- {part.Type} Found");

                        AddElongatedNaturalEquipmentTo(part);

                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    }
                }
                Debug.Entry(3, "x foreach (BodyPart part in list) ]//");
            }
            else
            {
                Debug.Entry(3, "Handling of NaturalEquipment is Superseded");
                Debug.Entry(4, "x Aborting ElongatedPaws.OnRegenerateDefaultEquipment() generation of equipment ]//");
            }

            Debug.Entry(3, "* base.OnRegenerateDefaultEquipment(body)");
            base.OnRegenerateDefaultEquipment(body);

            Debug.Entry(2, "==================================================================");
        }
    }
}
