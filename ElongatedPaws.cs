using System;
using System.Collections.Generic;
using System.Linq;
using XRL;
using XRL.Core;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches; // Add this line
using static Mods.GigantismPlus.HelperMethods;
using XRL.World.Tinkering;

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
                Debug.Entry(4, $"@ ElongatedPaws.ElongatedBonusDamage", Indent: 4);
                Debug.Entry(4, $"Returning StrMod/2: {(int)Math.Floor((double)this.StrengthModifier / 2.0)}", Indent: 5);
                Debug.Entry(4, $"x ElongatedPaws.ElongatedBonusDamage >//", Indent: 4);
                return (int)Math.Floor((double)this.StrengthModifier / 2.0);
            }
        }

        public int ElongatedDieSizeBonus
        {
            get
            {
                Debug.Entry(4, $"@ ElongatedPaws.ElongatedDieSizeBonus", Indent: 4);
                int dieSize = 0;

                bool HasGigantism = ParentObject.HasPart<GigantismPlus>();
                bool HasBurrowing = ParentObject.HasPart<BurrowingClaws>();

                Debug.Entry(4, $"HasGigantism: {(HasGigantism ? "yeah" : "nah")}", Indent: 5);
                Debug.Entry(4, $"HasBurrowing: {(HasBurrowing ? "yeah" : "nah")}", Indent: 5);

                Debug.Entry(4, $"dieSize: {dieSize}", Indent: 4);

                dieSize += !HasGigantism ? 1 : 0;
                Debug.Entry(4, $"!HasGigantism? dieSize: {dieSize}", Indent: 4);
                dieSize += !HasBurrowing ? 1 : 0;
                Debug.Entry(4, $"!HasBurrowing? dieSize: {dieSize}", Indent: 4);

                Debug.Entry(4, $"Final dieSize: {dieSize}", Indent: 4);
                Debug.Entry(4, $"x ElongatedPaws.ElongatedDieSizeBonus >//", Indent: 4);
                return dieSize;
            }
        }

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}";
            Type = "Physical";
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
                CompatibleMutations = CompatibleMutations.Substring(CompatibleMutations.Length - 1);
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
                || ID == StatChangeEvent.ID
                || ID == EquipperEquippedEvent.ID
                || ID == UnequippedEvent.ID;
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
                body?.UpdateBodyParts();

                foreach (GameObject equipped in body.GetEquippedObjects())
                {
                    if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                    {
                        weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>(), E.Object);
                    }
                }

            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {

            foreach (GameObject equipped in GO.Body.GetEquippedObjects())
            {
                if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                {
                    weaponElongator.ApplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>(), GO);
                }
            }

            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            foreach (GameObject equipped in GO.Body.GetEquippedObjects())
            {
                if (equipped.TryGetPart(out WeaponElongator weaponElongator))
                {
                    weaponElongator.UnapplyElongatedBonusCap(equipped.GetPart<MeleeWeapon>());
                }
            }

            CheckAffected(GO, GO.Body);

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
            if (part != null && part.Type == "Hand" && !part.IsExternallyManagedLimb())
            {
                Debug.Entry(3, "* if (ParentObject.HasPart<GigantismPlus>())");
                Debug.Entry(3, "* else if (ParentObject.HasPart<BurrowingClaws>())");
                // int StatMod = ElongatedBonusDamage;

                Debug.Entry(4, "- Saving copy of current DefaultBehavior in case creation fails");
                GameObject OldDefaultBehavior = part.DefaultBehavior;

                Debug.Entry(3, $"- Setting part.DefaultBehaviour to new instance of \"{ElongatedPawBlueprintName}\"");
                // part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(ElongatedPawBlueprintName);

                Debug.Entry(3, $"- Checking that new GameObject was instantiated and assigned correctly");
                Debug.Entry(4, "* if (part.DefaultBehavior != null)");
                if (part.DefaultBehavior != null)
                {
                    
                    Debug.Entry(3, "-- part.DefaultBehavior not null, assigning stats");
                    part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", "ElongatedPaws", false);
                    var weapon = part.DefaultBehavior.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = $"1d2";

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
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, "ElongatedPaws", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            /* Testing without cascading delegation.
             * 
            if (body == null || IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, "NaturalEquipment is Superseded", Indent: 2);
                Debug.Entry(3, "x Aborting ElongatedPaws Generation of Equipment >//", Indent: 2);
                Debug.Entry(3, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
                Debug.Footer(3, "ElongatedPaws", $"OnRegenerateDefaultEquipment(body)");
                base.OnRegenerateDefaultEquipment(body);
            }
            */

            if (body == null) base.OnRegenerateDefaultEquipment(body);

            Debug.Entry(3, "Performing application of behavior to parts");

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"");
            Debug.Entry(4, "Generating List<BodyPart> list");
            // Just change the body part search logic
            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                    where p.Type == targetPartType  // Changed from VariantType to Type
                                    select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries");
            Debug.Entry(4, "* foreach (BodyPart part in list)");
            foreach (BodyPart part in list)
            {
                Debug.Entry(4, $"-- {part.Type}");
                if (part.Type == "Hand")
                {
                    Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.Entry(3, $"--- {part.Type} Found");

                    ItemModding.ApplyModification(part.DefaultBehavior, "ModElongatedNaturalWeapon", Actor: ParentObject);

                    /* Testing with simple Modification Application.
                    *
                    AddElongatedNaturalEquipmentTo(part);
                    */

                    Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                }
            }
            Debug.Entry(3, "x foreach (BodyPart part in list) ]//");
           
            Debug.Entry(3, "* base.OnRegenerateDefaultEquipment(body)");
            Debug.Entry(2, "==================================================================");
            base.OnRegenerateDefaultEquipment(body);
        }
    }   
} //!-- namespace XRL.World.Parts.Mutation

namespace XRL.World.Parts
{
    [Serializable]
    public class WeaponElongator : IPart
    {
        public GameObject Wielder = null;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == StatChangeEvent.ID
                || ID == UnequippedEvent.ID
                || ID == EquippedEvent.ID;
        }

        private int appliedElongatedBonusCap = 0;

        public void ApplyElongatedBonusCap(MeleeWeapon Weapon, GameObject Wielder)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"ApplyElongatedBonusCap()", Indent: 3);
            Debug.Entry(4, "* if (Wielder.TryGetPart(out ElongatedPaws elongatedPaws))", Indent: 3);
            if (Wielder.TryGetPart(out ElongatedPaws elongatedPaws))
            {
                Debug.Entry(4, "+ elongatedPaws not null", Indent: 4);
                UnapplyElongatedBonusCap(Weapon);
                appliedElongatedBonusCap = elongatedPaws.ElongatedBonusDamage;
                Weapon.AdjustBonusCap(appliedElongatedBonusCap);
                Debug.Entry(4, $"New appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 4);
            }
            Debug.Entry(4, "x WeaponWatcher", $"ApplyElongatedBonusCap() >//", Indent: 3);
        }

        public void UnapplyElongatedBonusCap(MeleeWeapon Weapon)
        {
            Debug.Entry(4, $"@ WeaponWatcher", "UnapplyElongatedBonusCap()", Indent: 4);
            Debug.Entry(4, $"Old appliedElongatedBonusCap: {appliedElongatedBonusCap}", Indent: 4);
            Weapon.AdjustBonusCap(-appliedElongatedBonusCap);
            appliedElongatedBonusCap = 0;
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(EquippedEvent E)", Indent: 2);
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                Wielder = E.Actor;
                Debug.Entry(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                ApplyElongatedBonusCap(item.GetPart<MeleeWeapon>(), E.Actor);
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(EquippedEvent E) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(UnequippedEvent E)", Indent: 2);
            GameObject item = E.Item;
            if (item == ParentObject)
            {
                Debug.Entry(4, $"Item: {item.ShortDisplayNameStripped}", Indent: 3);
                UnapplyElongatedBonusCap(item.GetPart<MeleeWeapon>());
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(UnequippedEvent E) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(StatChangeEvent E)
        {
            Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(StatChangeEvent E)", Indent: 2);
            Debug.Entry(4, $"E.Name: \"{E.Name}\" | E.Object: \"{E.Object.ShortDisplayNameStripped}\"", Indent: 2);
            if (Wielder != null) Debug.Entry(4, $"Wielder: \"{Wielder.ShortDisplayNameStripped}\"", Indent: 2);
            if (E.Name == "Strength" && E.Object == Wielder)
            {
                Debug.Entry(4, "@ WeaponWatcher", $"HandleEvent(StatChangeEvent {E.Name})", Indent: 2);
                ApplyElongatedBonusCap(ParentObject.GetPart<MeleeWeapon>(), E.Object);
                Debug.Entry(4, "x WeaponWatcher", $"HandleEvent(StatChangeEvent {E.Name}) >//", Indent: 2);
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class WeaponElongator : IPart

} //!-- namespace XRL.World.Parts
