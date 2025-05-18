using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using XRL.World.Anatomy;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_HornsPlus : BaseDefaultEquipmentMutation
    {
        public const string BASE_DISPLAY_NAME = "Horns";

        [NonSerialized]
        public GameObject HornsObject;

        public UD_HornsPlus()
        {
            DisplayName = BASE_DISPLAY_NAME;
            Type = "Physical";
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }

        public override string GetDescription()
        {
            if (Variant == null)
            {
                return $"{DisplayName} jut out of your head.";
            }
            GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(Variant);
            string gender = blueprint.GetPropertyOrTag("Gender");
            string displayName = blueprint.CachedDisplayNameStripped;
            if (gender == "plural")
            {
                return $"{Grammar.InitCap(displayName)} jut out of your head.";
            }
            return $"{Grammar.A(displayName, Capitalize: true)} juts out of your head.";
        }

        public override string GetLevelText(int Level)
        {
            string baseDamage = GetBaseDamage(Level);
            int aV = GetAV(Level);
            StringBuilder SB = Event.NewStringBuilder();
            SB.Append("20% chance on melee attack to gore your opponent").AppendLine();
            SB.Append("Damage increment: ").AppendRule(baseDamage).AppendLine();
            SB.Append("To-hit bonus: ").AppendRule(HornsProperties.GetToHitBonus(Level).Signed()).AppendLine();
            if (Level == base.Level)
            {
                SB.Append("Goring attacks may cause bleeding").AppendLine();
            }
            else if (Level % 4 != 1)
            {
                SB.AppendRule("Increased bleeding save difficulty").AppendLine();
            }
            else
            {
                SB.AppendRule("Increased bleeding save difficulty and intensity").AppendLine();
            }
            bool hornsPlural = true;
            string hornsNoun = DisplayName;
            if (Variant != null)
            {
                GameObjectBlueprint blueprint = GameObjectFactory.Factory.GetBlueprint(Variant);
                hornsPlural = blueprint.GetPropertyOrTag("Gender") == "Plural";
                hornsNoun = blueprint.CachedDisplayNameStripped;
            }
            string isAre = hornsPlural ? "are" : "is";
            SB.Append(Grammar.InitCap(hornsNoun)).Append($" {isAre} a short-blade class natural weapon.").AppendLine();
            SB.AppendRule(aV.Signed()).Append(" AV").AppendLine();
            SB.Append("Cannot wear helmets").AppendLine();
            SB.Append("+100 reputation with ").AppendColored("w", "antelopes").Append(" and ").AppendColored("w", "goatfolk").AppendLine();

            return Event.FinalizeString(SB);
        }

        public static int GetAV(int Level)
        {
            return 1 + (Level - 1) / 3;
        }
        public int GetAV()
        {
            return GetAV(Level);
        }

        public static string GetBaseDamage(int Level)
        {
            return "2d" + (3 + Level / 3);
        }
        public string GetBaseDamage()
        {
            return GetBaseDamage(Level);
        }

        public override void SetVariant(string Variant)
        {
            base.SetVariant(Variant);
            if (HornsObject != null && HornsObject.Blueprint != Variant)
            {
                RegrowHorns();
            }
        }

        public void RegrowHorns()
        {
            Body body = ParentObject.Body;
            if (body != null && !Variant.IsNullOrEmpty())
            {
                string targetBodyPart = GameObjectFactory.Factory.GetBlueprint(Variant).GetPartParameter("MeleeWeapon", "Slot", "Head");
                BodyPart bodyPart = HornsObject?.DefaultOrEquippedPart() ?? body.GetFirstPart(targetBodyPart);

                if (bodyPart != null || HornsObject?.Blueprint != Variant)
                {
                    if (HornsObject != null && (!HornsObject.IsValid() || HornsObject.Blueprint != Variant))
                    {
                        GameObject.Release(ref HornsObject);
                    }

                    HornsObject ??= GenerateHornsObject(Variant, bodyPart, Level);

                    ParentObject.ForceEquipObject(HornsObject, bodyPart, Silent: true, 0);

                    DisplayName = GetVariantName() ?? DisplayName;
                }
            }
        }

        public static GameObject GenerateHornsObject(string Variant, BodyPart BodyPart, int Level = 0)
        {
            if (Variant.IsNullOrEmpty() || BodyPart == null || Level < 1) 
                return null;

            GameObject hornsObject = GameObject.Create(Variant);

            MeleeWeapon hornsWeapon = hornsObject.GetPart<MeleeWeapon>();
            hornsWeapon.Slot = BodyPart.Type;
            hornsWeapon.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            hornsWeapon.BaseDamage = GetBaseDamage(Level);
            Armor HornsArmor = hornsObject.GetPart<Armor>();
            HornsArmor.WornOn = BodyPart.Type;
            HornsArmor.AV = GetAV(Level);

            return hornsObject;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (Variant.IsNullOrEmpty())
            {
                Variant = GetVariants().GetRandomElement();
                DisplayName = GetVariantName() ?? DisplayName;
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            CleanUpMutationEquipment(GO, ref HornsObject);
            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            RegrowHorns();
            base.OnRegenerateDefaultEquipment(body);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetItemElementsEvent>.ID;
        }
        public override bool HandleEvent(GetItemElementsEvent E)
        {
            if (E.IsRelevantCreature(ParentObject))
            {
                E.Add("might", BaseElementWeight);
            }
            return base.HandleEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.WriteGameObject(HornsObject);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            HornsObject = Reader.ReadGameObject();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_HornsPlus hornsPlus = base.DeepCopy(Parent, MapInv) as UD_HornsPlus;
            hornsPlus.HornsObject = null;
            return hornsPlus;
        }

    } //!-- public class HornsPlus : BaseDefaultEquipmentMutation

}
