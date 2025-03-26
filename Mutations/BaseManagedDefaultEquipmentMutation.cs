using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using XRL.World.Anatomy;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentMutation<T> : BaseDefaultEquipmentMutation, IManagedDefaultNaturalEquipment<T> 
        where T : BaseManagedDefaultEquipmentMutation<T>, IManagedDefaultNaturalEquipment<T>, new()
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalEquipmentMod for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalEquipmentMod.Type).
        public Dictionary<string, ModNaturalEquipment<T>> NaturalEquipmentMods { get; set; }
        public ModNaturalEquipment<T> NaturalEquipmentMod { get; set; }

        public BaseManagedDefaultEquipmentMutation()
        {
            NaturalEquipmentMods = new();
        }

        // Takes an existing NaturalEquipmentMods Dictionary
        public BaseManagedDefaultEquipmentMutation(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, T NewParent)
            : this()
        {
            Dictionary<string, ModNaturalEquipment<T>> NewNaturalEquipmentMods = new();
            foreach ((string BodyPartType, ModNaturalEquipment<T> NaturalEquipmentMod) in naturalEquipmentMods)
            {
                ModNaturalEquipment<T> naturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
                NewNaturalEquipmentMods.Add(BodyPartType, naturalEquipmentMod);
            }
            NaturalEquipmentMods = NewNaturalEquipmentMods;
        }

        public BaseManagedDefaultEquipmentMutation(ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
            : this()
        {
            NaturalEquipmentMod = new(naturalEquipmentMod, NewParent);
        }

        public BaseManagedDefaultEquipmentMutation(Dictionary<string, ModNaturalEquipment<T>> naturalEquipmentMods, ModNaturalEquipment<T> naturalEquipmentMod, T NewParent)
            : this(naturalEquipmentMods, NewParent)
        {
            NaturalEquipmentMod = new(NaturalEquipmentMod, NewParent);
        }

        public virtual bool ProcessNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod, string Parts)
        {
            if (Parts == null) return false;
            NaturalEquipmentMod.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalEquipmentMod.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalEquipmentAddedProps(ModNaturalEquipment<T> NaturalEquipmentMod, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalEquipmentMod.AddedStringProps = StringProps;
                NaturalEquipmentMod.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponDamageDieSize(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponDamageBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual int GetNaturalWeaponHitBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }
        public virtual int GetNaturalWeaponPenBonus(ModNaturalEquipment<T> NaturalEquipmentMod, int Level = 1)
        {
            return 0;
        }

        public virtual List<string> GetNaturalEquipmentAddedParts(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalEquipmentAddedStringProps(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalEquipmentAddedIntProps(ModNaturalEquipment<T> NaturalEquipmentMod)
        {
            return NaturalEquipmentMod.AddedIntProps;
        }

        public virtual bool UpdateNaturalEquipmentMod(ModNaturalEquipment<T> NaturalEquipmentMod, int Level)
        {
            // List<string> vomitCats = new() { "Meta", "Damage", "Additions", "Render" };
            // NaturalEquipmentMod.Vomit(4, "| Before", vomitCats, Indent: 2);
            Debug.Divider(4, "\u2500", 40, Indent: 2);

            //NaturalEquipmentMod.Level = Level;
            NaturalEquipmentMod.DamageDieCount = GetNaturalWeaponDamageDieCount(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageDieCount", true, Indent: 3);
            NaturalEquipmentMod.DamageDieSize = GetNaturalWeaponDamageDieSize(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageDieSize", true, Indent: 3);
            NaturalEquipmentMod.DamageBonus = GetNaturalWeaponDamageBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "DamageBonus", true, Indent: 3);
            NaturalEquipmentMod.HitBonus = GetNaturalWeaponHitBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "HitBonus", true, Indent: 3);
            NaturalEquipmentMod.PenBonus = GetNaturalWeaponPenBonus(NaturalEquipmentMod, Level)
                .Vomit(4, "PenBonus", true, Indent: 3);

            Debug.Divider(4, "\u2500", 40, Indent: 2);
            // NaturalEquipmentMod.Vomit(4, "|  After", vomitCats, Indent: 2);
            return true;
        }
        public override bool ChangeLevel(int NewLevel)
        {
            // UnprocessNaturalEquipment(ParentObject.Body);
            foreach ((_, ModNaturalEquipment<T> NaturalEquipmentMod) in NaturalEquipmentMods)
            {
                UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            }
            if (NaturalEquipmentMod != null) UpdateNaturalEquipmentMod(NaturalEquipmentMod, NewLevel);
            return base.ChangeLevel(NewLevel);
        }

        public virtual bool ProcessNaturalEquipment(Body body)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart bodyPart in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{bodyPart.Description} [{bodyPart.ID}:{bodyPart.Type}]", Indent: 2);
                    ModNaturalEquipment<T> naturalEquipmentMod = null;
                    if (NaturalEquipmentMod != null && bodyPart.Type == NaturalEquipmentMod.BodyPartType)
                    {
                        naturalEquipmentMod = new(NaturalEquipmentMod);
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Property", Indent: 3);
                    }
                    else if (NaturalEquipmentMods.ContainsKey(bodyPart.Type))
                    {
                        naturalEquipmentMod = NaturalEquipmentMods[bodyPart.Type];
                        Debug.Entry(4, $"NaturalEquipmentMod for this BodyPart contained in Dictionary", Indent: 3);
                    }
                    else
                    {
                        Debug.Entry(4, $"No NaturalEquipmentMod for this BodyPart", Indent: 3);
                    }

                    if (naturalEquipmentMod == null) continue;

                    Debug.Entry(4, $"modNaturalWeapon: {naturalEquipmentMod?.Name}", Indent: 3);

                    if (bodyPart.DefaultBehavior != null)
                    {
                        Debug.Entry(4, $"{bodyPart.DefaultBehavior.ShortDisplayNameStripped} is DefaultBehavior", Indent: 3);
                        if (bodyPart.DefaultBehavior.HasPart(naturalEquipmentMod.Name))
                        {
                            Debug.Entry(4, $"{bodyPart.DefaultBehavior.ShortDisplayName} already has {naturalEquipmentMod.Name}", Indent: 3);
                            continue;
                        }
                        bodyPart.DefaultBehavior.ApplyModification(naturalEquipmentMod, Actor: body.ParentObject);
                    }
                    else if (bodyPart.Equipped != null && bodyPart.Equipped.HasPart<NaturalEquipment>())
                    {
                        Debug.Entry(4, $"{bodyPart.Equipped.ShortDisplayNameStripped} is Equipped", Indent: 3);
                        if (bodyPart.Equipped.HasPart(naturalEquipmentMod.Name))
                        {
                            Debug.Entry(4, $"{bodyPart.Equipped.ShortDisplayName} alreadry has {naturalEquipmentMod.Name}", Indent: 3);
                            continue;
                        }
                        bodyPart.Equipped.ApplyModification(naturalEquipmentMod, Actor: body.ParentObject);
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(ProcessNaturalEquipment)} @//",
                Indent: 1);
            return true;
        }

        public override void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalEquipment(body);

            Debug.Entry(4, $"* base.{nameof(OnRegenerateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(4, 
                $"{typeof(T).Name}", 
                $"{nameof(OnRegenerateDefaultEquipment)}(body: {ParentObject.Blueprint})");

            base.OnRegenerateDefaultEquipment(body);
        }
        public override void OnDecorateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnDecorateDefaultEquipment)}(body)");
            Debug.Entry(4, $"TARGET {ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalEquipment(body);

            Debug.Entry(4, $"* base.{nameof(OnDecorateDefaultEquipment)}(body)", Indent: 1);
            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnDecorateDefaultEquipment)}(body: {ParentObject.Blueprint})");
            base.OnDecorateDefaultEquipment(body);
        }

        public override bool Unmutate(GameObject GO)
        {
            return base.Unmutate(GO);
        }
        public override void AfterUnmutate(GameObject GO)
        {
            UnprocessNaturalEquipment(GO.Body);

            GO.GetPart<Mutations>().ActiveMutationList.Vomit(4, $"{GO.Blueprint}'s Active Mutations", Indent: 3);
            foreach (BaseMutation mutation in GO.GetPart<Mutations>().ActiveMutationList)
            {
                if (mutation.GetMutationClass() == GetMutationClass()) continue;
                BaseManagedDefaultEquipmentMutation<T> castMutation = mutation as BaseManagedDefaultEquipmentMutation<T>;
                if (castMutation?.NaturalEquipmentMod != null || castMutation?.NaturalEquipmentMods != null && castMutation.NaturalEquipmentMods.IsNullOrEmpty()) continue;
                mutation.ChangeLevel(mutation.Level);
            }

            base.AfterUnmutate(GO);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("BeforeMutationAdded");
            Registrar.Register("MutationAdded");
            base.Register(Object, Registrar);
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
                    ProcessNaturalEquipment(Actor?.Body);
                }
            }
            return base.FireEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            BaseManagedDefaultEquipmentMutation<T> mutation = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentMutation<T>;
            mutation.NaturalEquipmentMods = new();
            foreach ((string bodyPartType, ModNaturalEquipment<T> naturalEquipmentMod) in NaturalEquipmentMods)
            {
                mutation.NaturalEquipmentMods.Add(bodyPartType, new(naturalEquipmentMod, (T)mutation));
            }
            mutation.NaturalEquipmentMod = new(NaturalEquipmentMod, (T)mutation);
            return mutation;
        }
    }
}
