using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleLib.Console;

using XRL.Language;
using XRL.Rules;
using XRL.UI;
using XRL.World.Anatomy;
using XRL.World.Effects;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_SlogGlands : BaseDefaultEquipmentMutation
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_SlogGlands));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public const string EQUIPMENT_BLUEPRINT = "Bilge Sphincter";

        public GameObject SphincterObject;

        public const string MANAGER_ID = "Mutation::SlogGlands";

        public static readonly int COOLDOWN = 10;

        public static readonly int RANGE = 10;

        public UD_SlogGlands()
        {
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            stats.Set("Range", RANGE);
            stats.Set("Area", "3x3");
            stats.Set("KnockdownChance", "Strength / Agility vs. character level");
            stats.CollectCooldownTurns(MyActivatedAbility(ActivatedAbilityID), COOLDOWN);
        }

        public override string GetDescription()
        {
            return "You bear a sphincter-choked bilge hose that you use to slurp up nearby liquids and spew them at enemies, occasionally knocking them down.";
        }
        public override string GetLevelText(int Level)
        {
            StringBuilder SB = Event.NewStringBuilder();
            SB.AppendLine("+6 Strength");
            SB.AppendLine("+1 AV");
            SB.AppendLine("+100 Acid Resistance");
            SB.AppendLine("+300 reputation with mollusks");
            SB.AppendLine("Bilge sphincter acts as a melee weapon.");
            SB.AppendLine("+50 move speed when moving through tiles with 200+ drams of liquid");
            SB.AppendLine("You can spew liquid from your tile into a nearby area.");
            SB.AppendLine("Spew range: 10");
            SB.AppendLine("Spew area: 3x3");
            SB.AppendLine("Spew chance to knock the targets down: Strength/Agility save vs. character level");
            SB.AppendLine("Spew cooldown: 10 rounds");
            return Event.FinalizeString(SB);
        }

        private LiquidVolume FindSpitVolume()
        {
            GameObject gameObject = ParentObject.CurrentCell?.GetFirstObjectWithPart("LiquidVolume");
            if (gameObject != null)
            {
                LiquidVolume liquidVolume = gameObject.LiquidVolume;
                if (liquidVolume.MaxVolume == -1 && liquidVolume.Volume > 0)
                {
                    return liquidVolume;
                }
            }
            return null;
        }

        public void AddSphincterTo(BodyPart Limb)
        {
            SphincterObject?.Release();

            SphincterObject = GameObject.Create("Bilge Sphincter");

            SphincterObject.GetPart<Armor>().WornOn = Limb.Type;
            SphincterObject.RequirePart<SlogGladsItem>();

            bool asDefaultBehavior = SphincterObject.EquipAsDefaultBehavior();

            if (asDefaultBehavior && Limb.DefaultBehavior != null)
            {
                if (Limb.DefaultBehavior == SphincterObject)
                {
                    return;
                }
                Limb.DefaultBehavior = null;
            }
            if (!asDefaultBehavior && Limb.Equipped != null)
            {
                if (Limb.Equipped == SphincterObject)
                {
                    return;
                }
                if (Limb.Equipped.CanBeUnequipped(SemiForced: true))
                {
                    Limb.ForceUnequip(Silent: true);
                }
            }
            if (!Limb.Equip(SphincterObject, 0, Silent: true, SemiForced: true))
            {
                CleanUpMutationEquipment(ParentObject, ref SphincterObject);
            }
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if (body?.ParentObject?.GetBodyPartByManager(MANAGER_ID) is BodyPart bodyPart)
            {
                AddSphincterTo(bodyPart);
            }
            base.OnRegenerateDefaultEquipment(body);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (GO != null)
            {
                Stinger.AddTail(
                    Object: GO,
                    ManagerID: MANAGER_ID,
                    UseUnmanaged: GO.Body.Anatomy.StartsWith("Slug"));

                ActivatedAbilityID = AddMyActivatedAbility(
                    Name: "Spew",
                    Command: "CommandSlog",
                    Class: "Physical Mutations",
                    Description: "You slurp up nearby liquids and spew them with your bilge sphincter, occasionally knocking enemies prone.",
                    Icon: "\u00ad");
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            RemoveMyActivatedAbility(ref ActivatedAbilityID);
            CleanUpMutationEquipment(GO, FindBilgeSphincter());
            Stinger.RemoveTail(GO, MANAGER_ID);
            return base.Unmutate(GO);
        }

        private GameObject FindBilgeSphincter()
        {
            return SphincterObject ?? ParentObject.Body?.FindEquipmentOrDefaultByBlueprint("Bilge Sphincter");
        }

        private bool HasBilgeSphincter()
        {
            return FindBilgeSphincter() != null;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforeApplyDamage");
            Registrar.Register("CommandSlog");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AIGetOffensiveAbilityListEvent.ID
                || ID == GetItemElementsEvent.ID;
        }
        public override bool HandleEvent(AIGetOffensiveAbilityListEvent E)
        {
            if (E.Distance <= 10 
                && FindSpitVolume() != null 
                && IsMyActivatedAbilityAIUsable(ActivatedAbilityID) 
                && HasBilgeSphincter() 
                && GameObject.Validate(E.Target) 
                && E.Actor.HasLOSTo(E.Target, IncludeSolid: true, BlackoutStops: false, UseTargetability: true))
            {
                E.Add("CommandSlog");
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetItemElementsEvent E)
        {
            if (E.IsRelevantCreature(ParentObject))
            {
                E.Add("might", 1);
            }
            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandSlog")
            {
                if (!HasBilgeSphincter())
                {
                    if (ParentObject.IsPlayer())
                    {
                        Popup.ShowFail("Your bilge sphincter is missing.");
                    }
                    return false;
                }
                if (!ParentObject.CheckFrozen())
                {
                    return false;
                }
                LiquidVolume spitVolume = FindSpitVolume();
                if (spitVolume == null)
                {
                    if (ParentObject.IsPlayer())
                    {
                        Popup.ShowFail("There is no liquid here for you to spew.");
                    }
                    return false;
                }
                int statValue = ParentObject.GetStatValue("Level", 15);
                List<Cell> targetCells = PickBurst(1, 10, Locked: false, AllowVis.OnlyVisible, "Spew");
                if (targetCells == null)
                {
                    return false;
                }
                foreach (Cell item in targetCells)
                {
                    if (item.DistanceTo(ParentObject) > 10)
                    {
                        if (ParentObject.IsPlayer())
                        {
                            Popup.ShowFail("That is out of range! (10 squares)");
                        }
                        return false;
                    }
                }
                UseEnergy(1000, "Physical Mutation Bilge Sphincter");
                CooldownMyActivatedAbility(ActivatedAbilityID, 10);
                SlimeGlands.SlimeAnimation("&w", ParentObject.CurrentCell, targetCells[0]);
                List<LiquidVolume> liquids = new();
                int counter = 0;
                foreach (Cell targetCell in targetCells)
                {
                    if (counter != 0 && !80.in100())
                    {
                        continue;
                    }
                    GameObject poolObject = GameObject.Create("Water");
                    LiquidVolume poolObjectVolume = poolObject.LiquidVolume;
                    poolObjectVolume.ComponentLiquids.Clear();
                    foreach ((string liquid, int volume) in spitVolume.ComponentLiquids)
                    {
                        poolObjectVolume.ComponentLiquids.Add(liquid, volume);
                    }
                    liquids.Add(poolObjectVolume);
                    targetCell.AddObject(poolObject);
                    counter++;
                }
                if (spitVolume.Volume < liquids.Count)
                {
                    spitVolume.MixWith(new LiquidVolume("slime", liquids.Count - spitVolume.Volume));
                }
                foreach (LiquidVolume liquid in liquids)
                {
                    liquid.Volume = Math.Max(Math.Min(1000, spitVolume.Volume) / counter, 1);
                    liquid.Update();
                }
                spitVolume.UseDrams(1000);
                foreach (Cell item4 in targetCells)
                {
                    foreach (GameObject item5 in item4.GetObjectsWithPart("Combat"))
                    {
                        if (item5 != ParentObject && !item5.MakeSave("Agility,Strength", statValue, null, null, "SlogGlands Knockdown"))
                        {
                            item5.ApplyEffect(new Prone());
                        }
                    }
                }
                DidX("spew", "a pool of stinking liquid", "!", null, null, ParentObject);
            }
            return base.FireEvent(E);
        }
    }
}
