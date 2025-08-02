using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XRL.Language;
using XRL.Rules;
using XRL.World.Anatomy;
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

        public virtual List<ModNaturalEquipment<UD_ManagedCrystallinity>> NaturalEquipmentMods => GetNaturalEquipmentMods();

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
            ModCrystallineNaturalWeapon crystalinePointMod = new()
            {
                Manager = NewManager,
                BodyPartType = "Hand",

                ModPriority = 100,
                DescriptionPriority = 100,

                ForceNoun = true,
                Noun = "point",

                Adjective = "crystalline",
                AdjectiveColor = "crystallized",
                AdjectiveColorFallback = "M",

                PartAdjustments = new(),

                AddedParts = new(),

                AddedStringProps = new()
                {
                    { "SwingSound", "Sounds/Melee/shortBlades/sfx_melee_foldedCarbide_wristblade_swing" },
                    { "BlockedSound", "Sounds/Melee/multiUseBlock/sfx_melee_metal_blocked" },
                },
            };

            if (!EnablePrereleaseContent)
            {
                crystalinePointMod.AddedParts.Add(nameof(Inorganic));
            }
            else
            {
                crystalinePointMod.AddAdjustment(new AddPartAdjustment<Inorganic>(), false);
            }

            crystalinePointMod.AddSkillAdjustment("ShortBlades", true);

            crystalinePointMod.AddNounAdjustment(true);

            crystalinePointMod.AddTileAdjustment("Creatures/natural-weapon-claw.bmp", true);
            crystalinePointMod.AddColorStringAdjustment("&b", true);
            crystalinePointMod.AddTileColorAdjustment("&b", true);
            crystalinePointMod.AddDetailColorAdjustment("B", true);
            return crystalinePointMod;
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Parts)
        {
            if (Parts == null) return false;
            NaturalEquipmentMod.AddedParts ??= new();
            if (Parts.Contains(","))
            {
                string[] parts = Parts.Split(',');
                foreach (string part in parts)
                {
                    NaturalEquipmentMod.AddedParts.TryAdd(part);
                }
            }
            else
            {
                NaturalEquipmentMod.AddedParts.TryAdd(Parts);
            }
            return !NaturalEquipmentMod.AddedParts.IsNullOrEmpty();
        }
        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            Props.ParseProps(out NaturalEquipmentMod.AddedStringProps, out NaturalEquipmentMod.AddedIntProps);
            return !NaturalEquipmentMod.AddedStringProps.IsNullOrEmpty() || !NaturalEquipmentMod.AddedIntProps.IsNullOrEmpty();
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

        public virtual List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedParts;
        }
        public virtual Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedStringProps;
        }
        public virtual Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<UD_ManagedCrystallinity> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedIntProps;
        }

        public List<ModNaturalEquipment<UD_ManagedCrystallinity>> GetNaturalEquipmentMods(Predicate<ModNaturalEquipment<UD_ManagedCrystallinity>> Filter = null, NaturalEquipmentManager NewManager = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_ManagedBurrowingClaws)}."
                + $"{nameof(GetNaturalEquipmentMods)}("
                + $"{nameof(Filter)}, "
                + $"{nameof(NewManager)}: {NewManager?.Name})",
                Indent: indent + 1, Toggle: getDoDebug());

            NewManager ??= NaturalEquipmentManager;
            List<ModNaturalEquipment<UD_ManagedCrystallinity>> naturalEquipmentModsList = new();

            List<MethodInfo> managedBaseMethods = new(typeof(UD_ManagedCrystallinity).GetMethods());
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                managedBaseMethods.RemoveAll(m => !m.IsStatic || !m.IsPublic || !m.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<UD_ManagedCrystallinity>)));
            }
            if (!managedBaseMethods.IsNullOrEmpty())
            {
                Debug.CheckYeh(4, $"Have Methods", Indent: indent + 2, Toggle: getDoDebug());
                foreach (MethodInfo managedMethod in managedBaseMethods)
                {
                    if (!managedMethod.IsStatic || !managedMethod.IsPublic)
                    {
                        continue;
                    }
                    Debug.LoopItem(4, $"{nameof(managedMethod)}: {managedMethod.Name}", Indent: indent + 3, Toggle: getDoDebug());

                    Debug.LoopItem(4, $"{nameof(managedMethod.IsPublic)}: {managedMethod.IsPublic}",
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.IsStatic)}: {managedMethod.IsStatic}",
                        Indent: indent + 4, Toggle: getDoDebug());
                    Debug.LoopItem(4, $"{nameof(managedMethod.ReturnType)}: {managedMethod.ReturnType.Name}",
                        Indent: indent + 4, Toggle: getDoDebug());

                    if (managedMethod.ReturnType.InheritsFrom(typeof(ModNaturalEquipment<UD_ManagedCrystallinity>))
                        && managedMethod.IsStatic
                        && managedMethod.IsPublic)
                    {
                        ParameterInfo[] parameters = managedMethod.GetParameters();
                        if (parameters.Length == 1
                            && parameters[0].ParameterType.InheritsFrom(typeof(NaturalEquipmentManager)))
                        {
                            Debug.CheckYeh(4,
                                $"public static {managedMethod.ReturnType.Name} " +
                                $"{managedMethod.Name}(" +
                                $"{parameters[0].ParameterType.Name} {parameters[0].Name})",
                                Indent: indent + 5, Toggle: getDoDebug());

                            if (managedMethod.Invoke(null, new object[1] { NewManager }) is ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod)
                            {
                                Debug.CheckYeh(4, $"Successful {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: getDoDebug());
                                if (Filter(naturalEquipmentMod))
                                {
                                    Debug.CheckYeh(4, $"Passed {nameof(Filter)}, added to List", Indent: indent + 3, Toggle: getDoDebug());
                                    naturalEquipmentModsList.Add(naturalEquipmentMod);
                                }
                                else
                                {
                                    Debug.CheckNah(4, $"Failed {nameof(Filter)}", Indent: indent + 3, Toggle: getDoDebug());
                                }
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Failed {nameof(managedMethod.Invoke)}", Indent: indent + 3, Toggle: getDoDebug());
                            }
                        }
                    }
                }
            }
            Debug.LastIndent = indent;
            return naturalEquipmentModsList;
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

            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level);

            NaturalEquipmentMod.AddDamageDieCountAdjustment(GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level));
            NaturalEquipmentMod.AddDamageDieSizeAdjustment(GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level));
            NaturalEquipmentMod.AddDamageBonusAdjustment(GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level));
            NaturalEquipmentMod.AddHitBonusAdjustment(GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level));
            NaturalEquipmentMod.AddPenBonusAdjustment(GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level));

            NaturalEquipmentMod.Vomit(4, DamageOnly: true, Indent: indent + 2, Toggle: getDoDebug());

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
                || ID == GetPrioritisedNaturalEquipmentModsEvent.ID
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
        public virtual bool HandleEvent(GetPrioritisedNaturalEquipmentModsEvent E)
        {
            Debug.Entry(4,
                $"@ {nameof(UD_ManagedCrystallinity)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetPrioritisedNaturalEquipmentModsEvent)} E)",
                Indent: 0, Toggle: getDoDebug("getMods"));

            List<ModNaturalEquipment<UD_ManagedCrystallinity>> naturalEquipmentMods = 
                UpdateNaturalEquipmentMods(GetNaturalEquipmentMods(
                    mod => mod.BodyPartType == E.TargetBodyPart.Type), 
                    Level);

            foreach (ModNaturalEquipment<UD_ManagedCrystallinity> naturalEquipmentMod in naturalEquipmentMods)
            {
                E.AddNaturalEquipmentMod(naturalEquipmentMod);
            }
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
