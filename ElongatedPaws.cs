using System;
using System.Collections.Generic;
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

        public override bool CanLevel()
        {
            return false;
        }

        public override string GetDescription()
        {
            return "An array of long, slender, digits fan from your paws, fluttering with composed and expert precision.\n\n"
                 + "You have {{giant|elongated paws}}, which are unusually large and end in spindly fingers.\n"
                 + "Their odd shape and size allow you to {{rules|equip}} equipment {{rules|on your hands}} and {{rules|wield}} melee and missile weapons {{gigantic|a size bigger}} than you are as though they were your size.\n\n"
                 + "Your {{giant|elongated paws}} count as natural short blades {{rules|\x1A}}{{rules|4}}{{k|/\xEC}} {{r|\x03}}{{z|1}}{{w|d}}{{z|4}}{{w|+}}{{rules|Current Strength Modifier}}\n\n"
                 + "+{{rules|100}} reputation with {{w|Barathrumites}}";
        }

        public override string GetLevelText(int Level)
        {
            return "";
        }

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
            Body body = E.Object.Body;
            foreach (BodyPart hand in body.GetParts())
            {
                if (hand.Type == "Hand")
                {
                    AddElongatedPawTo(hand);
                }
            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Body body = GO.Body;
            if (body != null)
            {
                foreach (BodyPart hand in body.GetParts())
                {
                    if (hand.Type == "Hand")
                    {
                        AddElongatedPawTo(hand);
                    }
                }
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Body body = GO.Body;
            if (body != null)
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
            CleanUpMutationEquipment(GO, ref ElongatedPawObject);
            CleanUpMutationEquipment(GO, ref GiganticElongatedPawObject);
            CleanUpMutationEquipment(GO, ref ElongatedBurrowingClawObject);
            CleanUpMutationEquipment(GO, ref GiganticElongatedBurrowingClawObject);
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

        public void AddElongatedPawTo(BodyPart part)
        {
            if (part != null && part.Type == "Hand")
            {
                int StatMod = StrengthModifier;
                if (ParentObject.HasPart<GigantismPlus>())
                {
                    if (ParentObject.HasPart<XRL.World.Parts.Mutation.BurrowingClaws>())
                    {
                        if (GiganticElongatedBurrowingClawObject == null)
                        {
                            GiganticElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedBurrowingClaw");
                        }
                        part.DefaultBehavior = GiganticElongatedBurrowingClawObject;
                        var gigantism = ParentObject.GetPart<GigantismPlus>();
                        var weapon = GiganticElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize}+{(StatMod / 2) + 3}";
                        weapon.HitBonus = gigantism.FistHitBonus;
                        weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                    }//GiganticElongatedBurrowingClawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                    else
                    {
                        if (GiganticElongatedPawObject == null)
                        {
                            GiganticElongatedPawObject = GameObjectFactory.Factory.CreateObject("GiganticElongatedPaw");
                        }
                        part.DefaultBehavior = GiganticElongatedPawObject;
                        var gigantism = ParentObject.GetPart<GigantismPlus>();
                        var weapon = GiganticElongatedPawObject.GetPart<MeleeWeapon>();
                        weapon.BaseDamage = $"{gigantism.FistDamageDieCount}d{gigantism.FistDamageDieSize}+{(StatMod / 2) + 3}";
                        weapon.HitBonus = gigantism.FistHitBonus;
                        weapon.MaxStrengthBonus = gigantism.FistMaxStrengthBonus;
                    }//GiganticElongatedPawObject uses FistDamageDieCount d FistDamageDieSize + (StrengthMod / 2) + 3
                }
                else if (ParentObject.HasPart<XRL.World.Parts.Mutation.BurrowingClaws>())
                {
                    if (ElongatedBurrowingClawObject == null)
                    {
                        ElongatedBurrowingClawObject = GameObjectFactory.Factory.CreateObject("ElongatedBurrowingClaw");
                    }
                    part.DefaultBehavior = ElongatedBurrowingClawObject;
                    var weapon = ElongatedBurrowingClawObject.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = $"1d5+{StatMod}";
                }//ElongatedBurrowingClawObject uses 1d5 + StrengthMod
                else
                {
                    if (ElongatedPawObject == null)
                    {
                        ElongatedPawObject = GameObjectFactory.Factory.CreateObject("ElongatedPaw");
                    }
                    part.DefaultBehavior = ElongatedPawObject;
                    var weapon = ElongatedPawObject.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = $"1d4+{StatMod}";
                }//ElongatedPawObject uses 1d4 + StrengthMod
            }
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            foreach (BodyPart hand in body.GetParts())
            {
                if (hand.Type == "Hand")
                {
                    AddElongatedPawTo(hand);
                }
            }

            base.OnRegenerateDefaultEquipment(body);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    }
}