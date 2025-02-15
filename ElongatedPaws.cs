using System;
using System.Collections.Generic;
using System.Linq;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World;
using Mods.GigantismPlus;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ElongatedPaws : BaseDefaultEquipmentMutation
    {
        private static readonly string[] AffectedSlotTypes = new string[3] { "Hand", "Hands", "Missile Weapon" };

        private static readonly string[] NaturalWeaponSupersedingMutations = new string[1]
        {
            "GigantismPlus"
        };

        public bool IsNaturalWeaponSuperseded
        {
            get
            {
                int count = 0;
                string i = "" + count;
                Debug.Entry(2, this.GetType().Name + "| IsNaturalWeaponSuperseded", i);
                foreach (string Mutation in NaturalWeaponSupersedingMutations)
                {
                    Debug.Entry(2, this.GetType().Name + "| IsNaturalWeaponSuperseded Loop", Mutation);
                    if (ParentObject.HasPart(Mutation)) count++;
                }
                i = "" + count;
                Debug.Entry(2, this.GetType().Name + "| IsNaturalWeaponSuperseded", i);
                return count > 0;
            }
        }

        public int NaturalWeaponDamageDieCount;

        public int NaturalWeaponDamageDieSize;

        private string NaturalWeaponBaseDamage;

        public int NaturalWeaponDamageBonus;

        public int StrMod => ParentObject.StatMod("Strength");

        public static readonly string[] NaturalWeapons = new string[2]
        {
            "ElongatedPaw",                                  // 0: Just ElongatedPaws(0)
            "ElongatedBurrowingClaw"                         // 1: BurrowingClaws(1)
            /* These are yet to be added but MUST be added 
             * in the order they appear below. Any other
             * entry in the list can serve as a placeholder
             * just make sure to note what the original was
            "ElongatedCrystallinePoint",                     // 2: Crystallinity(2)
            "ElongatedBurrowingCrystallinePoint"             // 3: ElongatedPaws(1) + Crystallinity(2)
            */
        };

        public static readonly string[] CompatibleMutations = new string[2]
        {
            // These are converted to the int value of their "bit position"
            // ElongatedPaws is always present, and forms the 0 value.
            "BurrowingClaws",    // 0 -> 1
            "Crystallinity"      // 1 -> 2
        };

        private int _NaturalWeaponIndex;

        private int NaturalWeaponIndex
        {
            get
            {
                _NaturalWeaponIndex = 0;
                string i = "" + _NaturalWeaponIndex;
                Debug.Entry(2, this.GetType().Name + "| NaturalWeaponIndex", i);
                foreach (var entry in CompatibleMutations.Select((Value, Index) => (Value, Index)))
                {
                    if (ParentObject.HasPart(entry.Value))
                    {
                        // 2^Index converts the index to the int value of its "bit position".
                        // adding these together according to which mutations are present
                        // gives a "unique" ID to each combination.
                        _NaturalWeaponIndex += (int)Math.Pow(2.0, (double)entry.Index);
                    }
                    i = "" + _NaturalWeaponIndex;
                    string j = "" + (int)Math.Pow(2.0, (double)entry.Index);
                    Debug.Entry(2, $"{this.GetType().Name}| NaturalWeaponIndex: {i} | [{j}] {entry.Value}");
                }
                return _NaturalWeaponIndex;
            }
        }

        public GameObject NaturalWeaponObject;

        private string _NaturalWeaponBlueprintName;

        public string NaturalWeaponBlueprintName
        {
            get
            {
                _NaturalWeaponBlueprintName = NaturalWeapons[NaturalWeaponIndex];
                Debug.Entry(2, this.GetType().Name + ", Get NaturalWeaponBlueprint", _NaturalWeaponBlueprintName);
                return _NaturalWeaponBlueprintName;
            }
        }

        [NonSerialized]
        protected GameObjectBlueprint _NaturalWeaponBlueprint;

        public GameObjectBlueprint NaturalWeaponBlueprint
        {
            get
            {
                _NaturalWeaponBlueprint = GameObjectFactory.Factory.GetBlueprint(NaturalWeaponBlueprintName);
                Debug.Entry(2, this.GetType().Name + ", Get NaturalWeaponBlueprint", NaturalWeaponBlueprintName);
                return _NaturalWeaponBlueprint;
            }
        }

        public static int GetNaturalWeaponDamageDieCount(int Level)
        {
            Debug.Entry(4, "ElongatedPaws", System.Reflection.MethodBase.GetCurrentMethod().Name);
            return 1;
        }

        public int GetNaturalWeaponDamageDieSize(int Level)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            int Value = 4;
            BurrowingClaws burrowingClaws = (BurrowingClaws)ParentObject.GetPart("BurrowingClaws");
            if (burrowingClaws != null)
            {
                Debug.Entry(3, System.Reflection.MethodBase.GetCurrentMethod().Name + " retrieved Burrowing Claws Damage");

                // adds Burrowing Claws Die Size(-2) to Elongated Burrowing Claws
                string ClawsDamageString = burrowingClaws.GetClawsDamage(burrowingClaws.Level);
                int ClawsDamage = int.Parse( ClawsDamageString.Substring(ClawsDamageString.Length-1) )-2;
                Value += ClawsDamage;
            }
            return Value;
        }

        public int GetNaturalWeaponDamageBonus()
        {
            Debug.Entry(4, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            int Bonus = StrMod;
            //return (int)Math.Floor((double)Bonus / 2.0);
            return StrMod;
        }

        public string GetNaturalWeaponBaseDamage(int Level)
        {
            Debug.Entry(4, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            string BonusString = GetNaturalWeaponDamageBonus() != 0 ? ("+" + StrMod) : "";
            return $"{GetNaturalWeaponDamageDieCount(Level)}d{GetNaturalWeaponDamageDieSize(Level)}{BonusString}";
        }

        public override bool CanLevel() { return false; } // Disable Leveling

        public override bool GeneratesEquipment() { return true; }

        public override bool AllowStaticRegistration() { return true; }

        public ElongatedPaws()
        {
            DisplayName = "{{giant|Elongated Paws}}";
            base.Type = "Physical";
        }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have {{giant|elongated paws}}, which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons {{gigantic|a size bigger}} than you are as though they were your size.\n\n"
                 + "Your {{giant|elongated paws}} count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|Current Strength Modifier}}\n\n"
                 + "+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

        public void GenerateNaturalWeapon(int Level)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            // update the NaturalWeapon properties.
            NaturalWeaponDamageDieCount = GetNaturalWeaponDamageDieCount(Level);
            NaturalWeaponDamageDieSize = this.GetNaturalWeaponDamageDieSize(Level);
            NaturalWeaponBaseDamage = this.GetNaturalWeaponBaseDamage(Level);
            NaturalWeaponDamageBonus = this.GetNaturalWeaponDamageBonus();

            // update the NaturalWeapon MeleeWeapon with new properties
            NaturalWeaponObject = GameObjectFactory.Factory.CreateObject(NaturalWeaponBlueprintName);
            if (NaturalWeaponObject != null)
            {
                Debug.Entry(3, this.GetType().Name+"."+ System.Reflection.MethodBase.GetCurrentMethod().Name, "Assigning Weapon Damage");
                MeleeWeapon NaturalWeapon = NaturalWeaponObject.GetPart<MeleeWeapon>();
                NaturalWeapon.BaseDamage = NaturalWeaponBaseDamage;
            }
        }

        public override string GetLevelText(int Level) { return ""; }

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

        public override bool ChangeLevel(int NewLevel)
        {
            // Technically redundant but its base calls OnRegenerateDefaultEquipment via ParentObject?.Body?.UpdateBodyParts().
            // This is here so that it replicates the GigantismPlus code in the Harmony Patch for Burrowing Claws.

            GenerateNaturalWeapon(NewLevel);

            return base.ChangeLevel(NewLevel);
        }

        public void AddNaturalWeaponTo(BodyPart part)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (part != null)
            {
                Debug.Entry(3, this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Part not null");
                // code to skip existing default behaviours if they're blacklisted in the Items.xml file could go here.
                part.DefaultBehavior = NaturalWeaponObject;
                part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", NaturalWeaponBlueprintName);
            }
        }

        public void AddNaturalWeaponsToPartsByType(Body Body, string Type = "Hand")
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            foreach (BodyPart part in Body.GetParts())
            {
                Debug.Entry(3, this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Part is " + part.Type);
                if (part.Type == Type)
                {
                    AddNaturalWeaponTo(part);
                }
            }
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (!this.IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, System.Reflection.MethodBase.GetCurrentMethod().Name + " Not Superseded");

                GenerateNaturalWeapon(Level);
                AddNaturalWeaponsToPartsByType(body);
            }

            /* Seems redundant.
             * 
            foreach (string Mutation in NaturalWeaponSupersedingMutations)
            {
                BaseDefaultEquipmentMutation MutationObject = (BaseDefaultEquipmentMutation)ParentObject.GetPart(Mutation);
                if (MutationObject != null)
                {
                    MutationObject.OnRegenerateDefaultEquipment(body);
                }
            }
            */

            base.OnRegenerateDefaultEquipment(body);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            Body body = GO.Body;
            if (!this.IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, System.Reflection.MethodBase.GetCurrentMethod().Name + " Not Superseded");
                GenerateNaturalWeapon(Level);
                AddNaturalWeaponsToPartsByType(body);
            }

            /* Seems redundant.
             * 
            foreach (string Mutation in NaturalWeaponSupersedingMutations)
            {
                BaseDefaultEquipmentMutation MutationObject = (BaseDefaultEquipmentMutation)ParentObject.GetPart(Mutation);
                if (MutationObject != null)
                {
                    MutationObject.OnRegenerateDefaultEquipment(body);
                }
            }
            */

            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            Body body = GO.Body;
            if (!this.IsNaturalWeaponSuperseded)
            {
                Debug.Entry(3, System.Reflection.MethodBase.GetCurrentMethod().Name + " Superseded");

                /* Seeing if commenting this out lets the game do its own garbage collection on the natural weapons.
                 * 
                if (body != null)
                {
                    foreach (BodyPart part in body.GetParts())
                    {
                        if (part.Type == "Hand" && part.DefaultBehavior != null && part.DefaultBehavior == NaturalWeaponObject)
                        {
                            part.DefaultBehavior = null;
                        }
                    }
                }
                */

                foreach (string Mutation in NaturalWeaponSupersedingMutations)
                {
                    BaseDefaultEquipmentMutation MutationObject = (BaseDefaultEquipmentMutation)ParentObject.GetPart((string)Mutation);
                    if (MutationObject != null)
                    {
                        MutationObject.ChangeLevel(MutationObject.Level);
                    }
                }

            }
            return base.Unmutate(GO);
        }

        /* This seems to always throw an exception. Not sure how to get it to stop doing that.
         * 
        public override void AfterUnmutate(GameObject GO)
        {
            Debug.Entry(2, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            ParentObject.SyncMutationLevelAndGlimmer();
        }
        */

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == StatChangeEvent.ID
                || ID == SyncMutationLevelsEvent.ID;
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
                Debug.Entry(2, this.GetType().Name, "StatChangeEvent");

                if (!this.IsNaturalWeaponSuperseded)
                {
                    GenerateNaturalWeapon(Level);
                    AddNaturalWeaponsToPartsByType(ParentObject.Body);
                }
                foreach (string Mutation in NaturalWeaponSupersedingMutations)
                {
                    BaseDefaultEquipmentMutation MutationObject = (BaseDefaultEquipmentMutation)ParentObject.GetPart(Mutation);
                    if (MutationObject != null)
                    {
                        MutationObject.HandleEvent(E);
                    }
                }
            }
            return base.HandleEvent(E);
        }

        /* Redundant?
         * 
        public override bool HandleEvent(SyncMutationLevelsEvent E)
        {
            Debug.Entry(2, this.GetType().Name, "SyncMutationLevelsEvent");
            int reinitNaturalWeaponIndex = NaturalWeaponIndex;
            bool reinitIsNaturalWeaponSuperseded = IsNaturalWeaponSuperseded;
            OnRegenerateDefaultEquipment(E.Object.Body);
            return base.HandleEvent(E);
        }
        */
    }
}