using System;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using Mods.GigantismPlus;
using static Qud.UI.CharacterAttributeLineData;
using System.Runtime.InteropServices.ComTypes;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModGiganticNaturalWeapon : IMeleeModification
    {
        public ModGiganticNaturalWeapon()
        {
        }

        public ModGiganticNaturalWeapon(int Tier)
            : base(Tier)
        {
        }
        
        public override void Configure()
        {
            WorksOnSelf = true;
        }

        private GameObject wielder = null;

        public GameObject Wielder { get => wielder; set => wielder = value; }

        private GigantismPlus gigantismPlus = null;

        public GigantismPlus GigantismPlus { get => gigantismPlus; set => gigantismPlus = value; }

        private const int naturalWeaponPriority = 10;

        public static int NaturalWeaponPriority => naturalWeaponPriority;

        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public int GetMaxStrengthCap()
        {
            if (Wielder == null && GigantismPlus == null) return 0;
            return GigantismPlus.FistMaxStrengthBonus;
        }

        public string GetBaseDamage()
        {
            if (Wielder == null && GigantismPlus == null) return GetFistDamage(1);
            return GetFistDamage(GigantismPlus.Level);
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            Object.GetContext(out GameObject objectContext, out var _);
            Wielder = objectContext;

            if (Wielder != null && Wielder.HasPart<GigantismPlus>())
            {
                GigantismPlus = Wielder.GetPart<GigantismPlus>();
            }

            // Change this to GigantismPlus "WeaponWatcher"
            // Object.RemovePart<WeaponElongator>();

            Object.GetPart<MeleeWeapon>().MaxStrengthBonus = GetMaxStrengthCap();
            Object.GetPart<MeleeWeapon>().BaseDamage = GetBaseDamage();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != PooledEvent<BeforeMeleeAttackEvent>.ID && ID != ExamineCriticalFailureEvent.ID && ID != ExamineFailureEvent.ID && ID != PooledEvent<GetDisplayNameEvent>.ID && ID != PooledEvent<GetItemElementsEvent>.ID)
            {
                return ID == GetShortDescriptionEvent.ID;
            }
            return true;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (E.Understood() && !E.Object.HasProperName)
            {
                E.AddAdjective("{{gigantic|gigantic}}");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("WeaponHit");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "WeaponHit")
            {

            }
            return base.FireEvent(E);
        }

        
        public static string GetFistDamage(int Level)
        {
            return GigantismPlus.GetFistBaseDamage(Level);
        }

        public static string GetFistDamageRange(int MinLevel, int MaxLevel = 0)
        {
            if (MaxLevel < MinLevel) MaxLevel = MinLevel;
            string lowDamage = GetFistDamage(Level: MinLevel);
            string highDamage = GetFistDamage(Level: MaxLevel);
            if (lowDamage == highDamage)
            {
                return lowDamage;
            }
            return $"between {lowDamage} at level {MinLevel}, and {highDamage} at level {MaxLevel}";
        }

        public static string GetDescription(int Level)
        {
            return $"Gigantic: This weapon's damage is derived from the wielder's Gigantism mutation level dealing {GetFistDamageRange(MinLevel: 1, MaxLevel: 16)}.";
        }

        public string GetInstanceDescription()
        {
            return $"Gigantic: This weapon's damage is derived from the wielder's Gigantism mutation level dealing {GetFistDamage(Level: GigantismPlus.Level)}.";
        }
    }

    [Serializable]
    public class ModElongatedNaturalWeapon : IMeleeModification
    {
        public ModElongatedNaturalWeapon()
        {
        }

        public ModElongatedNaturalWeapon(int Tier)
            : base(Tier)
        {
        }

        public override void Configure()
        {
            WorksOnSelf = true;
        }

        public override bool BeingAppliedBy(GameObject obj, GameObject who)
        {
            Wielder = who;
            return true;
        }

        private GameObject wielder = null;

        public GameObject Wielder { get => wielder; set => wielder = value; }

        private ElongatedPaws elongatedPaws = null;

        public ElongatedPaws ElongatedPaws { get => elongatedPaws; set => elongatedPaws = value; }

        private const int naturalWeaponPriority = 20;

        public static int NaturalWeaponPriority => naturalWeaponPriority;

        public override int GetModificationSlotUsage()
        {
            return 0;
        }

        public int GetDieSizeIncrease()
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.GetDieSizeIncrease", Indent: 4);
            if (Wielder == null && ElongatedPaws == null) return 2;
            Debug.Entry(4, $"Wielder not null, ElongatedPaws not null", Indent: 5);
            return ElongatedPaws.ElongatedDieSizeBonus;
        }

        public int GetBonusDamageIncrease()
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.GetBonusDamageIncrease", Indent: 4);
            if (Wielder == null && ElongatedPaws == null) return 2;
            Debug.Entry(4, $"Wielder not null, ElongatedPaws not null", Indent: 5);
            return ElongatedPaws.ElongatedBonusDamage;
        }

        public override bool ModificationApplicable(GameObject Object)
        {
            if (!Object.HasPart<MeleeWeapon>() && !Object.HasPart<Physics>() && Object.GetPart<Physics>().Category != "Natural Weapon")
            {
                return false;
            }
            return true;
        }

        public override void ApplyModification(GameObject Object)
        {
            Debug.Entry(4, $"@ ModElongatedNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped})", Indent: 4);
            
            if (Wielder != null && Wielder.HasPart<ElongatedPaws>())
            {
                Debug.Entry(4, $"Wielder not null, Wielder has ElongatedPaws", Indent: 5);
                ElongatedPaws = Wielder.GetPart<ElongatedPaws>();
            }
            else
            {
                Debug.Entry(4, $"Wielder null or Wielder lacks ElongatedPaws", Indent: 5);
            }

            Object.RemovePart<WeaponElongator>();
            Object.GetPart<MeleeWeapon>().AdjustDamageDieSize(GetDieSizeIncrease());
            Object.GetPart<MeleeWeapon>().AdjustDamage(GetBonusDamageIncrease());
            Object.DisplayName = "paw";
            Object.Render.Tile = "ElongatedPaw_Alt.png";
            Object.Render.ColorString = "&x";
            Object.Render.DetailColor = "z";
            Object.SetStringProperty("TemporaryDefaultBehavior", "ElongatedPaws", false);
            Debug.Entry(4, $"x ModElongatedNaturalWeapon.ApplyModification(Object: {Object.ShortDisplayNameStripped}) ]//", Indent: 4);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != PooledEvent<BeforeMeleeAttackEvent>.ID && ID != ExamineCriticalFailureEvent.ID && ID != ExamineFailureEvent.ID && ID != PooledEvent<GetDisplayNameEvent>.ID && ID != PooledEvent<GetItemElementsEvent>.ID)
            {
                return ID == GetShortDescriptionEvent.ID;
            }
            return true;
        }

        public override bool HandleEvent(BeforeMeleeAttackEvent E)
        {
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (E.Understood() && !E.Object.HasProperName)
            {
                E.AddAdjective("{{giant|elongated}}");
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetInstanceDescription(), GetEventSensitiveAddStatusSummary(E));
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("StatChange_Strength");
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "StatChange_Strength")
            {
                Debug.Entry(4, "BOI, we got that StatChange_Strength Event listened to!");
            }
            return base.FireEvent(E);
        }

        public static string GetDescription()
        {
            return $"Elongated: This natural weapon has its die size increased by up to 2 based on other present mutations, and gets half your Strength Modifier in Bonus Damage.";
        }

        public string GetInstanceDescription()
        {
            string description = "Elongated: This natural weapon ";
            if (Wielder != null && !Wielder.HasPart<GigantismPlus>() && !Wielder.HasPart<BurrowingClaws>())
            {
                description += $"has its die size increased by {GetDieSizeIncrease()}, and ";
            }
            string bonusDamageIncrease = "";
            if (GetBonusDamageIncrease() != 0) bonusDamageIncrease = $"({GetBonusDamageIncrease()}) ";
            description += $"gets half your Strength Modifier {bonusDamageIncrease}added as Bonus Damage.";
            return description;
        }
    }
}