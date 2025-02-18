using System;
using System.Collections.Generic;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches; // Add this line

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws : BaseDefaultEquipmentMutation
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };
        
        private static readonly List<string> NaturalWeaponSupersedingMutations = new List<string>
        {
            "GigantismPlus",
            "BurrowingClaws",
            "Crystallinity"
        };

        public bool IsNaturalWeaponSuperseded
        {
            get
            {
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

        public GameObject ElongatedPawObject;
        public GameObject GiganticElongatedPawObject;
        public GameObject ElongatedBurrowingClawObject;
        public GameObject GiganticElongatedBurrowingClawObject;

        public int StrengthModifier => ParentObject.StatMod("Strength");

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}";
            base.Type = "Physical";
        }

        public override bool CanLevel() { return false; }

        public override bool AllowStaticRegistration() { return true; }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have {{giant|elongated paws}}, which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons {{gigantic|a size bigger}} than you are as though they were your size.\n\n"
                 + "Your {{giant|elongated paws}} count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|Current Strength Modifier}}\n\n"
                 + "+{{rules|100}} reputation with {{w|Barathrumites}}";
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
                OnRegenerateDefaultEquipment(body);
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
            if (body != null && !this.IsNaturalWeaponSuperseded)
            {

                foreach (BodyPart hand in body.GetParts())
                {
                    if (hand.Type == "Hand" && (hand.DefaultBehavior == ElongatedPawObject || hand.DefaultBehavior == GiganticElongatedPawObject || hand.DefaultBehavior == ElongatedBurrowingClawObject || hand.DefaultBehavior == GiganticElongatedBurrowingClawObject))
                    {
                        hand.DefaultBehavior = null;
                    }
                }
            }
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
            Debug.Entry(2, "**AddGiganticNaturalEquipmentTo(BodyPart part)");
            if (part != null && part.Type == "Hand")
            {
                Debug.Entry(3, "**if (ParentObject.HasPart<GigantismPlus>())");
                Debug.Entry(3, "**else if (ParentObject.HasPart<BurrowingClaws>())");
                int StatMod = StrengthModifier;
                if (ParentObject.HasPart<GigantismPlus>())
                {
                    Debug.Entry(3, "-- GigantismPlus is Present");
                    Debug.Entry(4, "**if (ParentObject.HasPart<BurrowingClaws>())");
                    if (ParentObject.HasPart<BurrowingClaws>())
                    {
                        Debug.Entry(3, "--- BurrowingClaws is Present");
                        var burrowingClaws = ParentObject.GetPart<BurrowingClaws>();
                        int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                        int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);

                        Debug.Entry(4, "**if (GiganticElongatedBurrowingClawObject == null)");
                        if (GiganticElongatedBurrowingClawObject == null)
                        {
                            Debug.Entry(3, "---- GiganticElongatedBurrowingClawObject was null, init");
                            GiganticElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedBurrowingClaw");
                        }
                        part.DefaultBehavior = GiganticElongatedBurrowingClawObject;
                        var gigantism = ParentObject.GetPart<GigantismPlus>();
                        var weapon = GiganticElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize}+{(StatMod / 2) + 3 + burrowingBonus}";
                        weapon.HitBonus = gigantism.FistHitBonus;
                        weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;

                        Debug.Entry(4, "**part.DefaultBehavior = GiganticElongatedBurrowingClawObject");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                    }
                    else
                    {
                        Debug.Entry(3, "--- BurrowingClaws not Present");
                        Debug.Entry(4, "**if (GiganticElongatedPawObject == null)");
                        if (GiganticElongatedPawObject == null)
                        {
                            Debug.Entry(3, "---- GiganticElongatedPawObject was null, init");
                            GiganticElongatedPawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedPaw");
                        }
                        part.DefaultBehavior = GiganticElongatedPawObject;
                        var gigantism = ParentObject.GetPart<GigantismPlus>();
                        var weapon = GiganticElongatedPawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize}+{(StatMod / 2) + 3}";
                        weapon.HitBonus = gigantism.FistHitBonus;
                        weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;

                        Debug.Entry(4, "**part.DefaultBehavior = GiganticElongatedPawObject");
                        Debug.Entry(4, $"--- Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus} | PenCap: {weapon.MaxStrengthBonus}");
                    }
                }
                else if (ParentObject.HasPart<BurrowingClaws>())
                {
                    Debug.Entry(3, "-- GigantismPlus not Present");
                    Debug.Entry(3, "-- BurrowingClaws is Present");
                    var burrowingClaws = ParentObject.GetPart<BurrowingClaws>();
                    int burrowingDieSize = BurrowingClaws_Patches.GetBurrowingDieSize(burrowingClaws.Level);
                    int burrowingBonus = BurrowingClaws_Patches.GetBurrowingBonusDamage(burrowingClaws.Level);

                    Debug.Entry(4, "**if (ElongatedBurrowingClawObject == null)");
                    if (ElongatedBurrowingClawObject == null)
                    {
                        Debug.Entry(3, "--- ElongatedBurrowingClawObject was null, init");
                        ElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("ElongatedBurrowingClaw");
                    }
                    part.DefaultBehavior = ElongatedBurrowingClawObject;
                    var weapon = ElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                    // Fix: Add burrowingBonus to final calculation
                    weapon.BaseDamage = $"1d{burrowingDieSize + 2}+{StatMod / 2}";

                    Debug.Entry(4, "**part.DefaultBehavior = ElongatedBurrowingClawObject");
                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | PenCap: {weapon.MaxStrengthBonus}");
                }
                else
                {
                    Debug.Entry(3, "-- GigantismPlus not Present");
                    Debug.Entry(3, "-- BurrowingClaws not Present");

                    Debug.Entry(4, "**if (ElongatedPawObject == null)");
                    if (ElongatedPawObject == null)
                    {
                        Debug.Entry(3, "--- ElongatedPawObject was null, init");
                        ElongatedPawObject = GameObjectFactory.Factory.CreateObject("ElongatedPaw");
                    }
                    part.DefaultBehavior = ElongatedPawObject;
                    var weapon = ElongatedPawObject.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = $"1d4+{StatMod / 2}";

                    Debug.Entry(4, "**part.DefaultBehavior = ElongatedPawObject");
                    Debug.Entry(4, $"-- Base: {weapon.BaseDamage} | PenCap: {weapon.MaxStrengthBonus}");
                }
            }
            Debug.Entry(2, "part null or not hand");
            Debug.Entry(2, "xxAddElongatedNaturalEquipmentTo(BodyPart part)");
        } //!--- public void AddElongatedNaturalEquipmentTo(BodyPart part)

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Debug.Entry(2, "__________________________________________________________________");
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[No Zone?]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Entry(2, "**ElongatedPaws.OnRegenerateDefaultEquipment(Body body)");
            Debug.Entry(2, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}");

            if (!this.IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, "- NaturalEquipment not Superseded");

                Debug.Entry(3, "**foreach (BodyPart hand in body.GetParts())\n**if (hand.Type == \"Hand\")");
                foreach (BodyPart hand in body.GetParts())
                {
                    Debug.Entry(4, $"-- {hand.Type}");
                    if (hand.Type == "Hand")
                    {
                        Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        Debug.Entry(3, $"--- {hand.Type} Found");

                        AddElongatedNaturalEquipmentTo(hand);

                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    }
                }
                Debug.Entry(3, "xxforeach (BodyPart hand in body.GetParts())");
            }
            else
            {
                Debug.Entry(3, "NaturalEquipment is Superseded");
                Debug.Entry(4, "xxAborting ElongatedPaws.OnRegenerateDefaultEquipment() Generation of Equipment");
            }

            Debug.Entry(3, "**base.OnRegenerateDefaultEquipment(body)");
            base.OnRegenerateDefaultEquipment(body);

            Debug.Entry(2, "==================================================================");
        }
    }
}