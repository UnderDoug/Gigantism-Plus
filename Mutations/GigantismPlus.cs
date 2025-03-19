using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World.Anatomy;
using XRL.World.Parts.Skill;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Extensions;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GigantismPlus : BaseManagedDefaultEquipmentMutation<GigantismPlus>
    {
        public int AppliedJumpBonus = 0;
        private double _stunningForceLevelFactor = 0.5;
        public double StunningForceLevelFactor
        {
            get 
            {
                var cybernetics = ParentObject.Body.GetBody().Cybernetics;
                if (cybernetics != null && cybernetics.TryGetPart(out CyberneticsGiganticExoframe exoframe))
                {
                    return exoframe.StunningForceLevelFactor;
                }
                return _stunningForceLevelFactor; 
            }
            set 
            { 
                _stunningForceLevelFactor = value;
            }
        }
        public int StunningForceDistance = 3;

        public static readonly string HUNCH_OVER_COMMAND_NAME = "CommandToggleGigantismPlusHunchOver";

        public Guid EnableActivatedAbilityID = Guid.Empty;

        public int HunchedOverAVModifier = 4;
        public int HunchedOverDVModifier = -6;
        public int HunchedOverQNModifier = -60;
        public int HunchedOverMSModifier = -60;

        public bool IsVehicleCreature => ParentObject.HasPart(typeof(Vehicle));

        private string HunchedOverAbilityHunched
        {
            get
            {
                if (IsCyberGiant)
                    return "Compact";
                return "Hunched";
            }
        }
        private string HunchedOverAbilityUpright
        {
            get
            {
                if (IsCyberGiant)
                    return "Regular"; // was "Standard" but it's one too many characters
                return "Upright";
            }
        }

        public static int GetHunchedOverAVModifier(int Level)
        {
            return 4;
        }
        public static int GetHunchedOverDVModifier(int Level)
        {
            return -6;
        }
        public static int GetHunchedOverQNModifier(int Level)
        {
            return Math.Min(-70 + (int)Math.Floor(Level * 10.0), -10);
        }
        public static int GetHunchedOverMSModifier(int Level)
        {
            return Math.Min(-70 + (int)Math.Floor(Level * 10.0), -10);
        }

        public int GetJumpRangeBonus(int Level)
        {
            int baseBonus = 1;
            var cybernetics = ParentObject.Body.GetBody().Cybernetics;
            if (GiganticExoframe != null)
            {
                return baseBonus + GiganticExoframe.JumpDistanceBonus;
            }
            return baseBonus;
        }
        public int GetStunningForceLevel(int Level)
        {
            return (int)Math.Max(Math.Floor(Level * StunningForceLevelFactor), 1);
        }
        public int GetStunningForceDistance(int Level)
        {
            return StunningForceDistance;
        }

        public bool IsGiganticCreature // basically a wrapper but forces you to not be PseudoGigantic at the same time 
        {
            get
            {
                if (ParentObject == null) return false;
                return ParentObject.IsGiganticCreature;
            }
            private set
            {
                ParentObject.IsGiganticCreature = value;
                if (IsPseudoGiganticCreature == value)
                {
                    IsPseudoGiganticCreature = !value;
                }
            }
        }
        public bool IsPseudoGiganticCreature // ensures you aren't (typically) Gigantic and PseudoGigantic at the same time 
        {
            get
            {
                if (ParentObject == null) return false;
                return ParentObject.HasPart<PseudoGigantism>();
            }
            set
            {
                if (value) ParentObject.RequirePart<PseudoGigantism>();
                else ParentObject.RemovePart<PseudoGigantism>();

                if (IsGiganticCreature == value)
                {
                    IsGiganticCreature = !value;
                }

            }
        }

        public bool IsCyberGiant
        {
            get
            {
                if (ParentObject == null) return false;
                return GiganticExoframe != null;
            }
        }

        private CyberneticsGiganticExoframe _giganticExoframe;
        public CyberneticsGiganticExoframe GiganticExoframe => _giganticExoframe ??= ParentObject?.Body?.GetPartByName("body").Cybernetics?.GetPart<CyberneticsGiganticExoframe>();

        public bool UnHunchImmediately = false;

        public bool IsHunchFree = false;
        private int _hunchOverEnergyCost = 500;
        public int HunchOverEnergyCost
        {
            get
            {
                if (IsHunchFree)
                {
                    IsHunchFree = false;
                    return 0;
                }
                return _hunchOverEnergyCost;
            }
            private set
            {
                _hunchOverEnergyCost = value;
            }
        }

        public GigantismPlus()
        {
            DisplayName = "{{gigantic|Gigantism}} ({{r|D}})";
            Type = "Physical";

            NaturalWeaponSubpart Fists = new()
            {
                Level = 1,
                DamageDieCount = 1,
                DamageDieSize = 2,
                DamageBonus = 0,
                ModPriority = 10,
                Adjective = "gigantic",
                AdjectiveColor = "gigantic",
                Noun = "fist",
                Skill = "Cudgel",
                Stat = "Strength",
                Tile = "NaturalWeapons/GiganticFist.png",
                ColorString = "&x",
                DetailColor = "z",
                SecondColorString = "&X",
                SecondDetailColor = "Z",
                SwingSound = "Sounds/Melee/cudgels/sfx_melee_cudgel_fistOfTheApeGod_swing",
                BlockedSound = "Sounds/Melee/multiUseBlock/sfx_melee_cudgel_fistOfTheApeGod_block",
                AddedIntProps = new()
                {
                    { "ModGiganticNoShortDescription", 1 }
                }
            };
            NaturalWeaponSubparts.Add("Hand", Fists);
        }
        public GigantismPlus(Dictionary<string, NaturalWeaponSubpart> naturalWeaponSubparts)
        {
            GigantismPlus gigantism = new();

            DisplayName = gigantism.DisplayName;
            Type = gigantism.Type;

            NaturalWeaponSubparts = new(naturalWeaponSubparts);
        }

        public override int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return (int)Math.Min(1 + Math.Floor(Level / 3.0), 8);
        }
        public override int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return (int)Math.Max(0, Math.Floor((Level - 9) / 3.0));
        }
        public override int  GetNaturalWeaponHitBonus(NaturalWeaponSubpart NaturalWeaponSubpart, int Level = 1)
        {
            return -3 + (int)Math.Floor(Level / 2.0);
        }

        public override bool CanLevel() { return true; } // Enable leveling

        public override bool GeneratesEquipment() { return true; }

        public override bool ChangeLevel(int NewLevel)
        {
            Debug.Header(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            // Straighten up if hunching.
            // Hunch over if hunched before level up.
            bool WasHunched = false;

            Debug.Entry(4, "? if (IsPseudoGiganticCreature and !IsVehicleCreature)", Indent: 1);
            if (IsPseudoGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(4, "+ Creature is PsuedoGigantic and not a Vehicle", Indent: 2);
                Debug.Entry(4, "Sending StraightenUp (silent)", Indent: 2);
                WasHunched = true;
                IsHunchFree = true;
                StraightenUp(Message: false);
            }
            else
            {
                Debug.Entry(4, $"- IsPseudoGiganticCreature: {IsPseudoGiganticCreature}", Indent: 2);
                Debug.Entry(4, $"- !IsVehicleCreature: {!IsVehicleCreature}", Indent: 2);
            }
            Debug.Entry(4, "x if (IsPseudoGiganticCreature and !IsVehicleCreature) ?//", Indent: 1);

            Debug.Entry(4, "Start of Change Level updates", Indent: 1);
            // Start of Change Level updates.

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "Jump Bonus", Indent: 1);
            // Jump Bonus
            Debug.Entry(4, "? if (AppliedJumpBonus > 0)", Indent: 1);
            if (AppliedJumpBonus > 0)
            {
                Debug.Entry(4, $"+ AppliedJumpBonus: {AppliedJumpBonus}", Indent: 2);
                Debug.Entry(4, $"JumpRangeModifier: {ParentObject.GetIntProperty("JumpRangeModifier")}", Indent: 2);
                ParentObject.ModIntProperty("JumpRangeModifier", -AppliedJumpBonus);
                Debug.Entry(4, $"JumpRangeModifier reduced to {ParentObject.GetIntProperty("JumpRangeModifier")}", Indent: 2);
            }
            else
            {
                Debug.Entry(4, $"- AppliedJumpBonus: {AppliedJumpBonus}", Indent: 2);
            }
            Debug.Entry(4, "x if (AppliedJumpBonus > 0) ?//", Indent: 1);

            AppliedJumpBonus = GetJumpRangeBonus(NewLevel);
            Debug.Entry(4, $"Calculated new AppliedJumpBonus: {AppliedJumpBonus}", Indent: 1);
            Debug.Entry(4, $"JumpRangeModifier: {ParentObject.GetIntProperty("JumpRangeModifier")}", Indent: 1);
            ParentObject.ModIntProperty("JumpRangeModifier", AppliedJumpBonus);
            Debug.Entry(4, $"JumpRangeModifier reduced to {ParentObject.GetIntProperty("JumpRangeModifier")}", Indent: 1);
            Acrobatics_Jump.SyncAbility(ParentObject);


            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "Stunning Force", Indent: 1);
            // Stunning Force
            Debug.Entry(4, "? if (ParentObject.TryGetPart(out StunningForceOnJump stunning))", Indent: 1);
            CheckStunningForceOnJump:
            if (ParentObject.TryGetPart(out StunningForceOnJump stunning))
            {
                Debug.Entry(4, $"+ Have StunningForceOnJump part", Indent: 2);
                Debug.Entry(4, $"stunning.Level: {stunning.Level}", Indent: 3);
                Debug.Entry(4, $"stunning.Distance: {stunning.Distance}", Indent: 3);
                stunning.Level = GetStunningForceLevel(NewLevel); // Scale stunning force with mutation level
                stunning.Distance = StunningForceDistance;
                Debug.Entry(4, $"New values calculated and assigned", Indent: 2);
                Debug.Entry(4, $"stunning.Level: {stunning.Level}", Indent: 3);
                Debug.Entry(4, $"stunning.Distance: {stunning.Distance}", Indent: 3);
            }
            else
            {
                Debug.Entry(4, $"- No StunningForceOnJump part", Indent: 2);
                ParentObject.RequirePart<StunningForceOnJump>();
                Debug.Entry(4, $"ParentObject.RequirePart<StunningForceOnJump>()", Indent: 3);
                Debug.Entry(4, $"goto CheckStunningForceOnJump", Indent: 2);
                goto CheckStunningForceOnJump;
            }
            Debug.Entry(4, "x if (ParentObject.TryGetPart(out StunningForceOnJump stunning)) ?//", Indent: 1);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "Hunch Over Penalties", Indent: 1);
            // Hunch Over Penalties
            Debug.Entry(4, $"Values Before", Indent: 2);
            Debug.Entry(4, $"HunchedOverAVModifier: {HunchedOverAVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverDVModifier: {HunchedOverDVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverMSModifier: {HunchedOverMSModifier}", Indent: 3);
            HunchedOverAVModifier = GetHunchedOverAVModifier(NewLevel);
            HunchedOverDVModifier = GetHunchedOverDVModifier(NewLevel);
            HunchedOverMSModifier = GetHunchedOverMSModifier(NewLevel);
            Debug.Entry(4, $"Values After", Indent: 2);
            Debug.Entry(4, $"HunchedOverAVModifier: {HunchedOverAVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverDVModifier: {HunchedOverDVModifier}", Indent: 3);
            Debug.Entry(4, $"HunchedOverMSModifier: {HunchedOverMSModifier}", Indent: 3);

            Debug.Divider(4, "-", Count: 25, Indent: 1);
            Debug.Entry(4, "End of Change Level updates", Indent: 1);
            // End of Change Level updates
            Debug.Entry(4, "? if (WasHunched and !IsVehicleCreature)", Indent: 1);
            if (WasHunched && !IsVehicleCreature)
            {
                Debug.Entry(4, "+ Creature was Hunched and not a Vehicle", Indent: 1);
                Debug.Entry(4, "Sending HunchOver (silent)", Indent: 1);
                IsHunchFree = true;
                HunchOver(Message: false);
            }
            else
            {
                Debug.Entry(4, $"- WasHunched: {WasHunched}", Indent: 2);
                Debug.Entry(4, $"- !IsVehicleCreature: {!IsVehicleCreature}", Indent: 2);
            }
            Debug.Entry(4, "x if (WasHunched and !IsVehicleCreature) ?//", Indent: 1);

            Debug.Footer(4, "GigantismPlus", $"ChangeLevel({NewLevel})");
            return base.ChangeLevel(NewLevel);
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            // Currently unused but will comprise part of the stat-shifting for Hunch Over.
            int HunchedOverAV = GetHunchedOverAVModifier(Level);
            int HunchedOverDV = GetHunchedOverDVModifier(Level);
            int HunchedOverMS = GetHunchedOverMSModifier(Level);
            stats.Set("HunchedOverAV", "+" + HunchedOverAV);
            stats.Set("HunchedOverDV", HunchedOverDV);
            stats.Set("HunchedOverMS", HunchedOverMS);
        }

        // method to swap Gigantism mutation category between Physical and PhysicalDefects
        // - Rapid advancement checks the Physical MutationCategory Entries.
        private void SwapMutationCategory(bool Before = true)
        {
            string state = Before ? "true" : "false";
            Debug.Header(3, "GigantismPlus",$"SwapMutationCategory(Before: {state})");

            // prefer this for repeated uses of strings.
            string Physical = "Physical";
            string PhysicalDefects = "PhysicalDefects";

            // direction of swap depends on whether before or after LevelGain
            string IntoCategory = Before ? Physical : PhysicalDefects;
            string OutOfCategory = Before ? PhysicalDefects : Physical;

            Debug.Entry(3, $"Into Category:   \"{IntoCategory}\"", Indent: 1);
            Debug.Entry(3, $"Out Of Category: \"{OutOfCategory}\"", Indent: 1);

            MutationEntry GigantismEntry = MutationFactory.GetMutationEntryByName(Name);

            Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 1);
            foreach (MutationCategory category in MutationFactory.GetCategories())
            {
                Debug.LoopItem(4, category.Name, Indent: 2);
                if (category.Name == IntoCategory)
                {
                    Debug.DiveIn(4, $"Found Category: \"{IntoCategory}\"", Indent: 2);

                    Debug.Entry(3, $"Adding \"{GigantismEntry.DisplayName}\" Mutation to \"{IntoCategory}\" Category", Indent: 3);
                    category.Add(GigantismEntry);
                    category.Entries.Sort((x, y) => x.DisplayName.CompareTo(y.DisplayName));

                    Debug.Entry(4, $"Displaying all entries in \"{IntoCategory}\" Category", Indent: 3);
                    Debug.Entry(4, "> foreach (MutationCategory category in MutationFactory.GetCategories())", Indent: 3);
                    foreach (MutationEntry entry in category.Entries)
                    {
                        Debug.LoopItem(4, entry.DisplayName, Indent: 4);
                    }
                    Debug.DiveOut(3, $"x {IntoCategory} >//", Indent: 2);
                }
                if (category.Name == OutOfCategory)
                {
                    Debug.DiveIn(3, $"Found Category: \"{OutOfCategory}\"", Indent: 2);
                    Debug.Entry(3, $"Removing \"{GigantismEntry.DisplayName}\" from \"{OutOfCategory}\" Category", Indent: 3);
                    category.Entries.RemoveAll(r => r == GigantismEntry);
                    Debug.DiveOut(3, $"x {OutOfCategory} >//", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (MutationCategory category in MutationFactory.GetCategories()) ]//", Indent: 1);
            Debug.Footer(3, "GigantismPlus", $"SwapMutationCategory(Before: {state})");
        } //!--- private void SwapMutationCategory(bool Before = true)

        private bool ShouldRapidAdvance(int Level, GameObject Actor)
        {
            bool IsMutant = Actor.IsMutant();
            bool RapidAdvancement = IsMutant
                                 && (Level + 5) % 10 == 0
                                 && !Actor.IsEsper()
                                 && EnableGigantismRapidAdvance;

            return RapidAdvancement;
        } //!--- private bool ShouldRapidAdvance(int Level, GameObject Actor)

        public override bool WantEvent(int ID, int cascade)
        {
            // Add once Hunch Over Stat-Shift is implemented: SingletonEvent<BeforeAbilityManagerOpenEvent>.
            return base.WantEvent(ID, cascade)
                || ID == BeforeLevelGainedEvent.ID
                || ID == AfterLevelGainedEvent.ID
                || ID == CanEnterInteriorEvent.ID
                || ID == GetExtraPhysicalFeaturesEvent.ID
                || ID == PooledEvent<GetSlotsRequiredEvent>.ID
                || ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(BeforeLevelGainedEvent E)
        {
            if (ShouldRapidAdvance(E.Level, E.Actor))
            {
                SwapMutationCategory(true);
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AfterLevelGainedEvent E)
        {
            if (ShouldRapidAdvance(E.Level, E.Actor))
            {
                SwapMutationCategory(false);
            }
            if (IsCyberGiant)
            {
                Body body = E.Actor.Body;
                body?.UpdateBodyParts();
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetExtraPhysicalFeaturesEvent E)
        {
            E.Features.Add("gigantic".OptionalColor("gianter", "w", Colorfulness) + " stature");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetSlotsRequiredEvent E)
        {
            // Lets you equip non-gigantic equipment that is flagged as "GiganticEquippable" with half the slots it would normally take, provided it's not now too small.
            // exceptions are 
            if (E.Actor.IsGiganticCreature && !E.Object.IsGiganticEquipment && E.Object.HasTagOrProperty("GiganticEquippable"))
            {
                E.Decreases++;
                if (E.SlotType != "Floating Nearby" && E.SlotType != "Thrown Weapon" && !E.Object.HasPart<CyberneticsBaseItem>())
                {
                    E.CanBeTooSmall = true;
                }
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(CanEnterInteriorEvent E)
        {
            Debug.Entry(1,"Checking CanEnterInteriorEvent");
            if (ParentObject == E.Object)
            {
                // This check is necessary because both the enterer and enteree handle this event.
                Debug.Entry(1,"Parent Object is the Target of Entry, Skip to base CanEnterInteriorEvent");
                return base.HandleEvent(E);
            }
            GameObject actor = E.Actor;
            if (actor != null && actor.IsGiganticCreature && !IsVehicleCreature)
            {
                Debug.Entry(2,"We are big, gonna HunchOver");
                IsHunchFree = true;
                CommandEvent.Send(actor, HUNCH_OVER_COMMAND_NAME);
                Debug.Entry(3, "HunchOver Sent for CanEnterInteriorEvent");
                bool check = CanEnterInteriorEvent.Check(E.Actor, E.Object, E.Interior, ref E.Status, ref E.Action, ref E.ShowMessage);
                E.Status = check ? 0 : E.Status;
                string status = "";
                status += E.Status;
                Debug.Entry(3, "E.Status", status);

                Popup.Show("You try to squeeze into the space.");
            }
            else
            {
                Debug.Entry(2, "CanEnterInteriorEvent - We aren't big.");
            }
            Debug.Entry(1, "Sending to base CanEnterInteriorEvent");
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            // DescribeMyActivatedAbility(EnableActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }

        public override string GetDescription()
        {
            string WeaponNoun = "fist";
            int stunningForceDistance = 3;
            double stunningForceLevelFactor = 0.5;
            if (ParentObject != null)
            {
                WeaponNoun = ParentObject.Body.GetFirstPart("Hand").DefaultBehavior.Render.DisplayName;
                stunningForceDistance = StunningForceDistance;
                stunningForceLevelFactor = StunningForceLevelFactor;
            }

            string gigantismSource = (!IsCyberGiant) ? "unusually" : "cybernetically".Color("c");
            string exoframeName = string.Empty;
            if (IsCyberGiant) exoframeName = GiganticExoframe.ImplantObject.ShortDisplayName;

            StringBuilder SB = Event.NewStringBuilder();
            
            SB.Append($"You are {gigantismSource} large, ").Append("will ").AppendRule("struggle to enter small spaces")
                .Append(" without ").AppendColored("g", "hunching over").Append(", ").Append("and can typically ")
                .AppendRule("only").Append(" use ").AppendGigantic("gigantic").Append(" equipment.").AppendLine();

            SB.Append("You weigh ").AppendRule("5x").Append(" as much, ")
                .Append("can carry ").AppendRule("2x").Append(" as much weight, and ")
                .Append("all of your natural weapons are ").AppendGigantic("gigantic").Append(".")
                .AppendLine().AppendLine();

            SB.Append("You you cause a ").AppendRule("shockwave").Append(" where you land ")
                .Append("after jumping at least ").AppendRule($"{stunningForceDistance}").Append(" tiles.").AppendLine()
                .Append("Your shockwave's ").AppendRule("damage").Append(" and ").AppendRule("force")
                .Append(" increases every ").AppendRule($"{(int)(1 / stunningForceLevelFactor)} levels");
            if (IsCyberGiant) 
                SB.AppendLine().Append("This amount is being boosted by your ").Append(exoframeName);
            SB.AppendLine().AppendLine();

            SB.Append("Your ").AppendGigantic("gigantic").Append($" {WeaponNoun.Pluralize()} gain:").AppendLine()
                .AppendRule("1d").Append(" damage every ").AppendRule($"{3} levels").Append(" (Max 8)").AppendLine()
                .AppendRule("+1").Append(" damage every ").AppendRule($"{3} levels").Append(" (Min 3)").AppendLine()
                .AppendRule("+1").Append(" to hit every ").AppendRule($"{2} levels").AppendLine();
            

            return Event.FinalizeString(SB);
            
            /* Old mutation description kept for posterity.
             * 
               + "You are " + GigantismSource + " large, will {{rules|struggle to enter small spaces}} without {{g|hunching over}},"
               + "and can typically {{rules|only}} use {{gigantic|gigantic}} equipment.\n"
               + "You are {{rules|heavy}}, can carry {{rules|twice}} as much weight,"
               + "and all your natural weapons are {{gigantic|gigantic}}.\n\n"
               + "Your " + WeaponNoun + "s gain:\n"
               + "{{rules|+1}} To-Hit every {{rules|2 mutation levels}}\n"
               + "{{B|d1}} damage every {{B|3 mutation levels}}\n"
               + "{{W|1d}} damage every {{W|5 mutation levels}}\n"
               + "They have {{rules|uncapped penetration}}, but are harder {{rules|to hit}} with due to their size."
            */
        }

        public override string GetLevelText(int Level)
        {

            string MSPenalty;
            if (GetHunchedOverMSModifier(Level) >= 0)
            {
                MSPenalty = "No}} MS pentalty";
            }
            else
            {
                MSPenalty = GetHunchedOverMSModifier(Level) + "}} MS";
            }

            string WeaponNoun = "fist";
            int FistDamageDieCount = 1;
            int FistDamageBonus = 3;
            int FistHitBonus = -3;
            int StunningForceJumpLevel = 1;
            string stunningForceDamageIncrement = StunningForce.GetDamageIncrement(1);
            if (ParentObject != null)
            {
                NaturalWeaponSubpart NaturalWeaponSubpart = NaturalWeaponSubparts["Hand"];
                WeaponNoun = ParentObject.Body.GetFirstPart("Hand").DefaultBehavior.Render.DisplayName;
                FistDamageDieCount = GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart, Level);
                FistDamageBonus = Math.Max(3, GetNaturalWeaponDamageBonus(NaturalWeaponSubpart, Level));
                FistHitBonus = GetNaturalWeaponHitBonus(NaturalWeaponSubpart, Level);
                StunningForceJumpLevel = GetStunningForceLevel(Level);
                stunningForceDamageIncrement = StunningForce.GetDamageIncrement(StunningForceJumpLevel);
            }
            StringBuilder SB = Event.NewStringBuilder();

            SB.Append($"Shockwave is Stunning Force level ").AppendRule($"{StunningForceJumpLevel}").AppendLine()
                .Append($"Force damage increment: ").AppendRule($"{stunningForceDamageIncrement}").AppendLine();

            SB.AppendGigantic("Gigantic").Append($" modifier gives your natural {WeaponNoun} weapons:").AppendLine()
                .AppendRule($"{FistDamageDieCount.Signed()}").Append($" damage die count.").AppendLine()
                .AppendRule($"{FistDamageBonus.Signed()}").Append($" damage {FistDamageBonus.BonusOrPenalty()}.").AppendLine()
                .AppendRule($"{FistHitBonus.Signed()}").Append($" hit {FistHitBonus.BonusOrPenalty()}.");

            return Event.FinalizeString(SB);
            
            /* Hunch Over penalties.
             * 
               "When {{g|Hunched Over}}:\n"
               + "{{rules|" + GetHunchedOverQNModifier(Level) + "}} Quickness."
               + "{{rules|" + GetHunchedOverMSModifier(Level) + "}} Movespeed.";
            */
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            Body body = GO.Body;

            Debug.Entry(4, "? if (body != null)", Indent: 1);
            if (body != null)
            {
                Debug.LoopItem(4, "+ Have Body", Indent: 2);
                GO.RemovePart<Gigantism>();
                Debug.LoopItem(4, "RemovePart<Gigantism>()", Indent: 2);
                IsGiganticCreature = true; // Enable the Gigantic flag
                Debug.LoopItem(4, "IsGiganticCreature = true", Indent: 2);
                GO.RequirePart<StunningForceOnJump>();
                Debug.LoopItem(4, "RequirePart<StunningForceOnJump>()", Indent: 2);
            }
            else
            {
                Debug.LoopItem(4, "- Haven't Body", Indent: 2);
            }
            Debug.Entry(4, "x if (body != null) ?//", Indent: 1);

            Debug.Entry(4, "? if (!GO.HasPart<Vehicle>())", Indent: 1);
            if (!GO.HasPart<Vehicle>())
            {
                Debug.LoopItem(4, "+ Not Vehicle", Indent: 2);
                /* AddActivatedAbility() - Full Method Arguments.
                 * AddActivatedAbility(Name, Command, Class, Description, Icon, DisabledMessage, Toggleable, DefaultToggleState, ActiveToggle, IsAttack, IsRealityDistortionBased, IsWorldMapUsable, Silent, AIDisable, AlwaysAllowToggleOff, AffectedByWillpower, TickPerTurn, Distinct: false, Cooldown, CommandForDescription, UITileDefault, UITileToggleOn, UITileDisabled, UITileCoolingDown); */
                EnableActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: "{{C|" + "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}/" + this.HunchedOverAbilityHunched + "}}",
                        Command: HUNCH_OVER_COMMAND_NAME,
                        Class: "Physical Mutations",
                        Description: null,
                        Icon: "&#214",
                        DisabledMessage: null,
                        Toggleable: true,
                        DefaultToggleState: false,
                        ActiveToggle: true, 
                        IsAttack: false,
                        IsRealityDistortionBased: false,
                        IsWorldMapUsable: false
                        );

                Debug.LoopItem(4, "Activeated Ability Assigned", Indent: 2);
                ActivatedAbilityEntry abilityEntry = GO.GetActivatedAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName = 
                    "{{C|" + 
                    "{{W|[}}" + this.HunchedOverAbilityUpright + "{{W|]}}\n" +
                                this.HunchedOverAbilityHunched + "\n" +
                       "}}";

                Debug.LoopItem(4, "Activeated Ability DisplayName Changed", Indent: 2);
                /* This causes a village generation crash.
                 * 
                if (this.IsCyberGiant)
                {
                    abilityEntry.UITileDefault.ColorString = "b";
                    abilityEntry.UITileDefault.DetailColor = char.Parse("B");
                    abilityEntry.UITileToggleOn.ColorString = "b";
                    abilityEntry.UITileToggleOn.DetailColor = char.Parse("B");
                }
                */
            }
            else
            {
                Debug.LoopItem(4, "- Is Vehicle", Indent: 2);
            }
            Debug.Entry(4, "x if (!GO.HasPart<Vehicle>()) ?//", Indent: 1);

            Debug.Entry(4, "deferring to base.Mutate(GO, Level)", Indent: 0);
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Debug.Header(4, $"GigantismPlus", $"Unmutate (GO: {GO.DebugName}, Level: {Level})");
            
            Debug.Entry(4, "? if (GO != null)", Indent: 1);
            if (GO != null)
            {
                Debug.LoopItem(4, "+ GO not null", Indent: 2);
                // Remove jumping properties
                GO.ModIntProperty("JumpRangeModifier", -AppliedJumpBonus, RemoveIfZero: true);
                AppliedJumpBonus = 0;
                Acrobatics_Jump.SyncAbility(GO);
                Debug.LoopItem(4, "JumpRangeModifier reverted", Indent: 2);

                Debug.LoopItem(4, "Removing StunningForceOnJump", Indent: 2);

                Debug.Entry(4, "? if (GO.HasPart<StunningForceOnJump>())", Indent: 2);
                if (GO.HasPart<StunningForceOnJump>())
                {
                    Debug.LoopItem(4, "+ StunningForceOnJump part found", Indent: 3);
                    var stunning = GO.GetPart<StunningForceOnJump>();
                    Debug.LoopItem(4, $"- Found StunningForceOnJump: Level={stunning.Level}, Distance={stunning.Distance}", Indent: 3);
                    GO.RemovePart<StunningForceOnJump>();
                    Debug.Entry(4, "StunningForceOnJump removed", Indent: 3);
                }
                else
                {
                    Debug.LoopItem(4, "- No StunningForceOnJump part", Indent: 3);
                }
                Debug.Entry(4, "x if (GO.HasPart<StunningForceOnJump>()) ?//", Indent: 2);


                Debug.Entry(4, "Attempting to StraightenUp()", Indent: 2);
                StraightenUp();
                GO.RemovePart<PseudoGigantism>();
                Debug.Entry(4, "RemovePart<PseudoGigantism>()", Indent: 2);
                GO.IsGiganticCreature = false; // Revert the Gigantic flag
                Debug.Entry(4, "IsGiganticCreature = false", Indent: 2);


                Debug.Entry(4, "? if (EnableActivatedAbilityID != Guid.Empty)", Indent: 2);
                if (EnableActivatedAbilityID != Guid.Empty)
                {
                    Debug.LoopItem(4, "+ EnableActivatedAbilityID not Empty", Indent: 3);
                    RemoveMyActivatedAbility(ref EnableActivatedAbilityID);
                    Debug.LoopItem(4, "RemoveMyActivatedAbility(ref EnableActivatedAbilityID)", Indent: 3);
                }
                else
                {
                    Debug.LoopItem(4, "- EnableActivatedAbilityID was Empty", Indent: 3);
                }
                Debug.Entry(4, "x if (EnableActivatedAbilityID != Guid.Empty) ?//", Indent: 2);

                Debug.Entry(4, "GO.WantToReequip()", Indent: 2);
                GO.WantToReequip();
                Debug.Entry(4, "GO.CheckAffectedEquipmentSlots()", Indent: 2);
                GO.CheckAffectedEquipmentSlots();
            }
            else
            {
                Debug.LoopItem(4, "- GO is null", Indent: 2);
            }
            Debug.Entry(4, "x if (GO != null) ?//", Indent: 1);

            Debug.Entry(4, "deferring to base.Unmutate(GO, Level)", Indent: 0);
            Debug.Header(4, $"GigantismPlus", $"Mutate (GO: {GO.DebugName}, Level: {Level})");
            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{nameof(GigantismPlus)}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                goto Exit;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            List<string> targetPartTypes = new();
            foreach ((string type, NaturalWeaponSubpart subpart) in NaturalWeaponSubparts)
            {
                targetPartTypes.Add(type);
                Debug.Entry(4, $"targetPartType \"{type}\" added", Indent: 1);
            }

            Debug.Entry(4, "Generating List<BodyPart> partsList", Indent: 1);
            List<BodyPart> partsList = (from p in body.GetParts(EvenIfDismembered: true)
                                   where targetPartTypes.Contains(p.Type) 
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "> foreach (BodyPart part in partsList)", Indent: 1);
            foreach (BodyPart part in partsList)
            {
                Debug.DiveIn(4, $"\u00BB: {part.Description} [{part.ID}:{part.Type}]", Indent: 2);

                NaturalWeaponSubpart NaturalWeaponSubpart = NaturalWeaponSubparts[part.Type];
                part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod<GigantismPlus>(NaturalWeaponSubpart), Actor: ParentObject);

                Debug.DiveOut(4, $"\u00AB: {part.Description} [{part.ID}:{part.Type}]", Indent: 2);
            }
            Debug.Entry(4, "x foreach (BodyPart part in partsList) >//", Indent: 1);

            Exit:
            Debug.Entry(4, $"* base.{nameof(OnRegenerateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(3, $"{nameof(GigantismPlus)}", $"{nameof(OnRegenerateDefaultEquipment)}(body: {ParentObject.Blueprint})");
            base.OnRegenerateDefaultEquipment(body);
        } //!--- public override void OnRegenerateDefaultEquipment(Body body)

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(HUNCH_OVER_COMMAND_NAME);
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == HUNCH_OVER_COMMAND_NAME)
            {
                GameObject actor = ParentObject;
                
                // Things that might stop you from taking this action
                if (actor.CurrentZone.ZoneWorld == "Interior" && !IsGiganticCreature)
                {
                    Popup.Show("This space is too small for you to stand upright!");
                    return base.FireEvent(E);
                }

                if (IsVehicleCreature)
                {
                    return base.FireEvent(E);
                }

                // Not prevented from taking action
                ToggleMyActivatedAbility(EnableActivatedAbilityID, null, Silent: true, null);
                Debug.Entry(3, "Hunch Ability Toggled");

                Debug.Entry(3, "Proceeding to Hunch Ability Effects");
                if (IsMyActivatedAbilityToggledOn(EnableActivatedAbilityID))
                    HunchOver(true); // Hunch
                else
                    StraightenUp(true); // Stand upright

                Debug.Entry(2, "IsPseudoGiganticCreature", IsPseudoGiganticCreature ? "true" : "false");
                Debug.Entry(2, "IsGiganticCreature", IsGiganticCreature ? "true" : "false");

            }

            The.Core.RenderBase();
            return base.FireEvent(E);
        }

        // Want to move the bulk of the Active Ability here.
        public void HunchOver(bool Message = false)
        {
            GameObject actor = ParentObject;
            if (IsPseudoGiganticCreature) // Already hunched over
            {
                Debug.Entry(1, "Tried to hunch, but was already PseudoGigantic");
                return;
            }

            IsPseudoGiganticCreature = true;

            if (!IsGiganticCreature && IsPseudoGiganticCreature)
            {
                // Action happened 
                UseEnergy(HunchOverEnergyCost, "Physical Defect Mutation Gigantism Hunch Over");
                //
                // Add the stat shifting code here.
                //

                actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_positiveVitality");
                if (Message)
                {
                    actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_positiveVitality");
                    Popup.Show("You hunch over, allowing you access to smaller spaces.");
                }

                ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName =
                    "{{C|" + 
                                HunchedOverAbilityUpright + "\n" +
                    "{{W|[}}" + HunchedOverAbilityHunched + "{{W|]}}\n" +
                       "}}";

            }
            Debug.Entry(1, "Should be Hunched Over");
        } //!--- public void HunchOver(bool Message = false)

        // Want to move the bulk of the Active Ability here.
        public void StraightenUp(bool Message = false)
        {
            GameObject actor = ParentObject;
            if (!IsPseudoGiganticCreature) // Already Upright over
            {
                IsHunchFree = false;
                UnHunchImmediately = false;
                Debug.Entry(1, "Tried to straighten up, but wasn't PseudoGigantic");
                return;
            }

            IsPseudoGiganticCreature = false;

            if (IsGiganticCreature && !IsPseudoGiganticCreature)
            {
                // Action happened 
                UseEnergy(HunchOverEnergyCost, "Physical Defect Mutation Gigantism Hunch Over");
                
                //
                // Add the stat shifting code here.
                //

                if (Message)
                {
                    actor.PlayWorldSound("Sounds/StatusEffects/sfx_statusEffect_negativeVitality");
                    Popup.Show("You stand tall, relaxing into your immense stature.");
                }

                ActivatedAbilityEntry abilityEntry = actor.ActivatedAbilities.GetAbility(EnableActivatedAbilityID);
                abilityEntry.DisplayName =
                    "{{C|" +
                    "{{W|[}}" + HunchedOverAbilityUpright + "{{W|]}}\n" +
                                HunchedOverAbilityHunched + "\n" +
                       "}}";
            }

            actor.CheckAffectedEquipmentSlots();
            Debug.Entry(1, "Should be Standing Tall");
        } //!-- public void StraightenUp(bool Message = false)

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(EnableActivatedAbilityID);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            EnableActivatedAbilityID = Reader.ReadGuid();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            GigantismPlus gigantism = base.DeepCopy(Parent, MapInv) as GigantismPlus;
            gigantism.NaturalWeaponSubparts = new(NaturalWeaponSubparts);
            gigantism.NaturalWeaponSubpart = new(NaturalWeaponSubpart);
            gigantism._giganticExoframe = null;
            return gigantism;
        }

    } //!-- public class GigantismPlus : BaseDefaultEquipmentMutation
}