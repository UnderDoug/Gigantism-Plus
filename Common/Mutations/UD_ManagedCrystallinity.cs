using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_ManagedCrystallinity 
        : Crystallinity
        , IManagedDefaultNaturalEquipment<UD_ManagedCrystallinity>
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_ManagedCrystallinity));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "getMods",
                'M',    // Manage
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

        public NaturalEquipmentManager NaturalEquipmentManager => ParentObject?.RequirePart<NaturalEquipmentManager>();

        public bool HasGigantism => ParentObject != null && ParentObject.HasPart<GigantismPlus>();

        public bool GiganticRefractAdded = false;

        public bool HasElongated => ParentObject != null && ParentObject.HasPart<ElongatedPaws>();

        public bool HasBurrowing => ParentObject != null && ParentObject.HasPartDescendedFrom<BurrowingClaws>();

        public UD_ManagedCrystallinity()
            : base()
        {
        }

        public UD_ManagedCrystallinity(Crystallinity Crystallinity)
            : this()
        {
            Level = Crystallinity.Level;
            RefractAdded = Crystallinity.RefractAdded;
        }

        public static ModCrystallineNaturalWeapon NewCrystallinePointMod(NaturalEquipmentManager NewManager)
        {
            return new(NewManager);
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod = null, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
                return 1;
            return 2;
        }
        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod = null, int Level = 1)
        {
            if (HasGigantism && (HasElongated || HasBurrowing))
            {
                return 1;
            }
            return 0;
        }
        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod = null, int Level = 1)
        {
            return 0;
        }

        public List<ModNaturalEquipment<UD_ManagedCrystallinity>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<UD_ManagedCrystallinity>> Filter = null)
        {
            return NaturalEquipmentManager.GetNaturalEquipmentMods(Filter);
        }

        public virtual ModNaturalEquipment<UD_ManagedCrystallinity> UpdateNaturalEquipmentMod(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{nameof(UD_ManagedCrystallinity)}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            NaturalEquipmentMod?.AdjustMeleeDamageDieCount(GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageDieSize(GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level))
                ?.AdjustMeleeDamageBonus(GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level))
                ?.AdjustMeleeHitBonus(GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level))
                ?.AdjustPenBonus(GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level))
                
                ?.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

            Debug.Entry(4,
                $"x {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(UpdateNaturalEquipmentMod)}("
                + $"{NaturalEquipmentMod.GetType().Name}[{nameof(UD_ManagedCrystallinity)}], "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMod;
        }
        public virtual List<ModNaturalEquipment<UD_ManagedCrystallinity>> UpdateNaturalEquipmentMods(List<ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods, int Level)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}[{nameof(UD_ManagedCrystallinity)}], "
                + $"{nameof(Level)}: {Level})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (!NaturalEquipmentMods.IsNullOrEmpty())
            {
                foreach (ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod in NaturalEquipmentMods)
                {
                    UpdateNaturalEquipmentMod(naturalEquipmentMod, Level);
                }
            }

            Debug.Entry(4,
                $"x {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(UpdateNaturalEquipmentMods)}("
                + $"{nameof(NaturalEquipmentMods)}[{nameof(UD_ManagedCrystallinity)}], "
                + $"{nameof(Level)}: {Level})"
                + $" *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return NaturalEquipmentMods;
        }

        public static int GetRefractChance(int Level)
        {
            return 25;
        }
        public int GetRefractChance()
        {
            return GetRefractChance(Level);
        }

        public static float GetGigantismRefractFactor(int Level)
        {
            return 0.4f;
        }
        public float GetGigantismRefractFactor()
        {
            return GetGigantismRefractFactor(Level);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            // GO.RegisterEvent(this, ManageDefaultEquipmentEvent.OperatorID, 0, Serialize: true);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            if (GO.TryGetPart(out RefractLight refractLight))
            {
                if (RefractAdded)
                {
                    refractLight.Chance -= GetRefractChance();
                    RefractAdded = false;
                }
                if (GiganticRefractAdded)
                {
                    refractLight.Chance -= (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = false;
                }
                if (refractLight.Chance < 1)
                {
                    GO.RemovePart(refractLight);
                }
            }
            return base.Unmutate(GO);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }
        public override string GetLevelText(int Level)
        {
            string levelText = base.GetLevelText(Level);
            if (ParentObject != null && ParentObject.TryGetPart(out GigantismPlus gigantism))
            {
                int baseRefractChance = GetRefractChance(Level);
                int giganticBonusRefractChance = (int)(baseRefractChance * GetGigantismRefractFactor(Level));
                int totalRefractChance = baseRefractChance + giganticBonusRefractChance;

                StringBuilder SB = Event.NewStringBuilder();

                SB.Append(totalRefractChance).Append("% chance to refract light-based attacks ");
                SB.Append("(").Append(baseRefractChance).Append("% base chance ");
                SB.AppendRule($"{giganticBonusRefractChance.Signed()}%").Append(" from ");
                SB.Append(gigantism.GetDisplayName()).Append(")");

                return levelText.Replace("25% chance to refract light-based attacks", Event.FinalizeString(SB));
            }
            return levelText;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            if(body != null && ParentObject.Body == body)
            {
                if (!ParentObject.TryGetPart(out RefractLight refractLight))
                {
                    refractLight = ParentObject.RequirePart<RefractLight>();
                }
                if (!RefractAdded)
                {
                    refractLight.Chance += GetRefractChance();
                    RefractAdded = true;
                }
                if (HasGigantism && !GiganticRefractAdded)
                {
                    refractLight.Chance += (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = true;
                }
                if (!HasGigantism && GiganticRefractAdded)
                {
                    refractLight.Chance -= (int)(GetRefractChance() * GetGigantismRefractFactor());
                    GiganticRefractAdded = false;
                }
            }
            // base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            base.OnDecorateDefaultEquipment(body);
        }
        public virtual void OnBeforeManageDefaultNaturalEquipment(NaturalEquipmentOperator Manager, BodyPart TargetBodyPart)
        {
            Zone InstanceObjectZone = ParentObject?.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, 
                $"{nameof(UD_ManagedCrystallinity)}",
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}(body)", 
                Toggle: getDoDebug('M'));
            Debug.Entry(4, $"TARGET {ParentObject?.DebugName} in zone {InstanceObjectZoneID}", 
                Indent: 0, Toggle: getDoDebug('M'));

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            // Debug.Divider(4, HONLY, Count: 25, Indent: 1, Toggle: getDoDebug());

            Debug.Footer(4,
                $"{nameof(UD_ManagedCrystallinity)}",
                $"{nameof(OnBeforeManageDefaultNaturalEquipment)}" +
                $"(body of: {ParentObject?.Blueprint})", 
                Toggle: getDoDebug('M'));
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            Registrar.Register("CookedAt");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == GetNaturalEquipmentModsEvent.ID
                || ID == BeforeManageDefaultNaturalEquipmentEvent.ID;
        }
        public virtual bool HandleEvent(BodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterBodyPartsUpdatedEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(GetNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug("getMods"));

            E.AddNaturalEquipmentMods(UpdateNaturalEquipmentMods(GetNaturalEquipmentMods(
                m => m.BodyPartType == E.TargetBodyPart.Type),
                Level));

            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeManageDefaultNaturalEquipmentEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(BeforeManageDefaultNaturalEquipmentEvent)} E)",
                Indent: 0, Toggle: getDoDebug('M'));

            if (E.Creature == ParentObject)
            {
                OnBeforeManageDefaultNaturalEquipment(E.Operator, E.BodyPart);
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(ManageDefaultNaturalEquipmentEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterManageDefaultNaturalEquipmentEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(BeforeRapidAdvancementEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AfterRapidAdvancementEvent E)
        {
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeMutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == ParentObject)
                {
                    // Do Code?
                }
            }
            else if (E.ID == "MutationAdded")
            {
                GameObject Actor = E.GetParameter("Object") as GameObject;
                string Mutation = E.GetParameter("Mutation") as string;
                if (Actor == ParentObject)
                {
                    // ProcessNaturalEquipment(Actor?.Actor);
                }
            }
            else if (E.ID == "Actor")
            {
                if (E.GetParameter("Object") is GameObject Actor && Actor == ParentObject && Actor.Body != null)
                {
                    Actor.Body.UpdateBodyParts();
                }
            }
            return base.FireEvent(E);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_ManagedCrystallinity mutation = base.DeepCopy(Parent, MapInv) as UD_ManagedCrystallinity;

            return mutation;
        }
    } //!-- public class UD_ManagedCrystallinity : Crystallinity, IManagedDefaultNaturalEquipment<UD_ManagedCrystallinity>
}
