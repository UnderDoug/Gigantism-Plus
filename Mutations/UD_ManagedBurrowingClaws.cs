using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    public class UD_ManagedBurrowingClaws : BurrowingClaws, IManagedDefaultNaturalWeapon
    {
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {

        }

        public INaturalWeapon NaturalWeapon = new()
        {
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

            ModPriority = 30,
            Skill = "ShortBlades",
            Adjective = "burrowing",
            AdjectiveColor = "W",
            Noun = "claw",
            Tile = "Creatures/natural-weapon-claw.bmp",
            RenderColorString = "&w",
            RenderDetailColor = "W",
            SecondRenderColorString = "&w",
            SecondRenderDetailColor = "W",
        };
        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        public virtual string GetNaturalWeaponMod(bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
        }

        private bool _HasGigantism = false;
        public bool HasGigantism
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPart<GigantismPlus>();
                return _HasGigantism;
            }
            set
            {
                _HasGigantism = value;
            }
        }

        private bool _HasElongated = false;
        public bool HasElongated
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPart<ElongatedPaws>();
                return _HasElongated;
            }
            set
            {
                _HasElongated = value;
            }
        }

        private bool _HasCrystallinity = false;
        public bool HasCrystallinity
        {
            get
            {
                if (ParentObject != null)
                    return ParentObject.HasPartDescendedFrom<Crystallinity>();
                return _HasCrystallinity;
            }
            set
            {
                _HasCrystallinity = value;
            }
        }

        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            if (HasGigantism)
            {
                NaturalWeapon.DamageDieSize = 3;
                return true;
            }
            DieRoll baseDamage = new(GetClawsDamage(Level));
            NaturalWeapon.DamageDieSize = baseDamage.RightValue;
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1) { return true; }
        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1) { return true; }
        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1) { return true; }

        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Cache]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, "UD_ManagedBurrowingClaws", $"OnRegenerateDefaultEquipment(body)");
            Debug.Entry(3, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);


            if (body == null)
            {
                Debug.Entry(3, "No Body. Aborting", Indent: 1);
                Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
                Debug.Footer(3, "UD_ManagedBurrowingClaws", $"OnRegenerateDefaultEquipment(body)");
                base.OnRegenerateDefaultEquipment(body);
                return;
            }

            Debug.Entry(3, "Performing application of behavior to parts", Indent: 1);

            string targetPartType = "Hand";
            Debug.Entry(4, $"targetPartType is \"{targetPartType}\"", Indent: 1);
            Debug.Entry(4, "Generating List<BodyPart> list", Indent: 1);

            List<BodyPart> list = (from p in body.GetParts(EvenIfDismembered: true)
                                   where p.Type == targetPartType
                                   select p).ToList();

            Debug.Entry(4, "Checking list of parts for expected entries", Indent: 1);
            Debug.Entry(4, "* foreach (BodyPart part in list)", Indent: 1);
            foreach (BodyPart part in list)
            {
                Debug.LoopItem(4, $"{part.Type}", Indent: 2);
                if (part.Type == "Hand")
                {
                    Debug.DiveIn(4, $"{part.Type} Found", Indent: 2);

                    part.DefaultBehavior.ApplyModification(GetNaturalWeaponMod(), Actor: ParentObject);

                    Debug.DiveOut(4, $"x {part.Type} >//", Indent: 2);
                }
            }
            Debug.Entry(4, "x foreach (BodyPart part in list) ]//", Indent: 1);

            Debug.Entry(4, "* base.OnRegenerateDefaultEquipment(body)", Indent: 1);
            Debug.Footer(3, "UD_ManagedBurrowingClaws", $"OnRegenerateDefaultEquipment(body)");
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedBurrowingClaws burrowingClaws = base.DeepCopy(Parent, MapInv) as UD_ManagedBurrowingClaws;
            burrowingClaws.NaturalWeapon = null;
            return burrowingClaws;
        }
    }
}
