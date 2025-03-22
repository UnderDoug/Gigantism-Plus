using System;
using System.Collections.Generic;
using XRL.Language;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Options;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class BaseManagedDefaultEquipmentCybernetic<T> : IPart, IManagedDefaultNaturalWeapon<T>
        where T : BaseManagedDefaultEquipmentCybernetic<T>, IManagedDefaultNaturalWeapon<T>, new()
    {
        // Dictionary holds a BodyPart.Type string as Key, and NaturalWeaponSubpart for that BodyPart.
        // Property is for easier access if the mutation has only a single type (via NaturalWeaponSubpart.Type).
        public Dictionary<string, NaturalWeaponSubpart<T>> NaturalWeaponSubparts { get; set; }
        public NaturalWeaponSubpart<T> NaturalWeaponSubpart { get; set; }

        private GameObject _implantee = null;
        public GameObject Implantee
        {
            get
            {
                return _implantee ??= ImplantObject?.Equipped;
            }
            set => _implantee = value == null ? null : _implantee;
        }

        private GameObject _implantObject = null;
        public GameObject ImplantObject
        {
            get
            {
                return _implantObject ??= ParentObject;
            }
            set => _implantObject = value == null ? null : _implantObject;
        }

        public BaseManagedDefaultEquipmentCybernetic()
        {
            NaturalWeaponSubparts = new();
        }
        
        // Takes an existing NaturalWeaponSubparts Dictionary
        public BaseManagedDefaultEquipmentCybernetic(Dictionary<string, NaturalWeaponSubpart<T>> naturalWeaponSubparts, T NewParent)
            : this()
        {
            Dictionary<string, NaturalWeaponSubpart<T>> NewNaturalWeaponSubparts = new();
            foreach ((string Part, NaturalWeaponSubpart<T> Subpart) in naturalWeaponSubparts)
            {
                NaturalWeaponSubpart<T> subpart = new(Subpart, NewParent);
                NewNaturalWeaponSubparts.Add(Part, subpart);
            }
            NaturalWeaponSubparts = NewNaturalWeaponSubparts;
        }

        public BaseManagedDefaultEquipmentCybernetic(NaturalWeaponSubpart<T> naturalWeaponSubpart, T NewParent)
            : this()
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public BaseManagedDefaultEquipmentCybernetic(Dictionary<string, NaturalWeaponSubpart<T>> naturalWeaponSubparts, NaturalWeaponSubpart<T> naturalWeaponSubpart, T NewParent)
            : this(naturalWeaponSubparts, NewParent)
        {
            NaturalWeaponSubpart = new(naturalWeaponSubpart, NewParent);
        }

        public virtual string GetNaturalWeaponModName(NaturalWeaponSubpart<T> NaturalWeaponSubpart, bool Managed = true)
        {
            return NaturalWeaponSubpart.GetNaturalWeaponModName(Managed);
        }
        public virtual ModNaturalWeaponBase<T> GetNaturalWeaponMod(NaturalWeaponSubpart<T> NaturalWeaponSubpart, bool Managed = true)
        {
            ModNaturalWeaponBase<T> NaturalWeaponMod = NaturalWeaponSubpart.GetNaturalWeaponMod(Managed);
            NaturalWeaponMod.NaturalWeaponSubpart = NaturalWeaponSubpart;
            NaturalWeaponMod.AssigningPart = (T)this;
            NaturalWeaponMod.Wielder = ParentObject;
            return NaturalWeaponMod;
        }

        public virtual bool ProcessNaturalWeaponAddedParts(NaturalWeaponSubpart<T> NaturalWeaponSubpart, string Parts)
        {
            if (Parts == null) return false;
            NaturalWeaponSubpart.AddedParts ??= new();
            string[] parts = Parts.Split(',');
            foreach (string part in parts)
            {
                NaturalWeaponSubpart.AddedParts.Add(part);
            }
            return true;
        }

        public virtual bool ProcessNaturalWeaponAddedProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart, string Props)
        {
            if (Props == null) return false;
            if (Props.ParseProps(out Dictionary<string, string> StringProps, out Dictionary<string, int> IntProps))
            {
                NaturalWeaponSubpart.AddedStringProps = StringProps;
                NaturalWeaponSubpart.AddedIntProps = IntProps;
            }
            return true;
        }

        public virtual int GetNaturalWeaponDamageDieCount(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieCount;
        }

        public virtual int GetNaturalWeaponDamageDieSize(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageDieSize;
        }

        public virtual int GetNaturalWeaponDamageBonus(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.DamageBonus;
        }

        public virtual int GetNaturalWeaponHitBonus(NaturalWeaponSubpart<T> NaturalWeaponSubpart, int Level = 1)
        {
            return NaturalWeaponSubpart.HitBonus;
        }

        public virtual List<string> GetNaturalWeaponAddedParts(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedParts;
        }

        public virtual Dictionary<string, string> GetNaturalWeaponAddedStringProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedStringProps;
        }

        public virtual Dictionary<string, int> GetNaturalWeaponAddedIntProps(NaturalWeaponSubpart<T> NaturalWeaponSubpart)
        {
            return NaturalWeaponSubpart.AddedIntProps;
        }

        public virtual bool UpdateNaturalWeaponSubpart(NaturalWeaponSubpart<T> Subpart, int Level)
        {
            Subpart.Level = Level;
            Subpart.DamageDieCount = GetNaturalWeaponDamageDieCount(Subpart, Level);
            Subpart.DamageDieSize = GetNaturalWeaponDamageDieSize(Subpart, Level);
            Subpart.DamageBonus = GetNaturalWeaponDamageBonus(Subpart, Level);
            Subpart.HitBonus = GetNaturalWeaponHitBonus(Subpart, Level);
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return ID == ImplantedEvent.ID
                || ID == UnimplantedEvent.ID
                || ID == PooledEvent<RegenerateDefaultEquipmentEvent>.ID
                || ID == PooledEvent<DecorateDefaultEquipmentEvent>.ID;
        }

        public virtual void OnImplanted(GameObject Implantee, GameObject Implant)
        {
        } //!--- public override void OnImplanted(GameObject Object)

        public virtual void OnUnimplanted(GameObject Implantee, GameObject Implant)
        {
            UnprocessNaturalWeaponSubparts(Implantee.Body);
        } //!--- public override void OnUnimplanted(GameObject Object)

        public override bool HandleEvent(ImplantedEvent E)
        {
            Implantee = E.Implantee;
            ImplantObject = E.Item;
            OnImplanted(Implantee, ImplantObject);
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(UnimplantedEvent E)
        {
            OnUnimplanted(E.Implantee, E.Item);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(RegenerateDefaultEquipmentEvent E)
        {
            OnRegenerateDefaultEquipment(Implantee.Body);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(DecorateDefaultEquipmentEvent E)
        {
            OnDecorateDefaultEquipment(Implantee.Body);
            return base.HandleEvent(E);
        }
        public virtual bool ProcessNaturalWeaponSubparts(Body body, bool CosmeticOnly = false)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(ProcessNaturalWeaponSubparts)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{part.Description} [{part.ID}:{part.Type}]", Indent: 2);
                    ModNaturalWeaponBase<T> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null
                        && part.Type == NaturalWeaponSubpart.Type
                        && NaturalWeaponSubpart.IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                        Debug.Entry(4, $"NaturalWeaponSubpart", Indent: 3);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type)
                        && NaturalWeaponSubparts[part.Type].IsCosmeticOnly() == CosmeticOnly)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                        Debug.Entry(4, $"NaturalWeaponSubparts", Indent: 3);
                    }

                    Debug.Entry(4, $"modNaturalWeapon: {modNaturalWeapon?.Name}", Indent: 3);

                    if (modNaturalWeapon == null) continue;

                    if (part.DefaultBehavior != null)
                    {
                        part.DefaultBehavior.ApplyModification(modNaturalWeapon, Actor: body.ParentObject);
                    }
                    else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                    {
                        part.Equipped.ApplyModification(modNaturalWeapon, Actor: body.ParentObject);
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(ProcessNaturalWeaponSubparts)} @//",
                Indent: 1);
            return true;
        }
        public virtual bool UnprocessNaturalWeaponSubparts(Body body)
        {
            Debug.Entry(4,
                $"@ {typeof(T).Name}."
                + $"{nameof(UnprocessNaturalWeaponSubparts)}",
                Indent: 1);

            if (body != null)
            {
                List<BodyPart> partsList = body.GetParts(EvenIfDismembered: true);
                foreach (BodyPart part in partsList)
                {
                    Debug.Divider(4, "-", Count: 25, Indent: 2);
                    Debug.LoopItem(4, $"part", $"{part.Description} [{part.ID}:{part.Type}]", Indent: 2);
                    ModNaturalWeaponBase<T> modNaturalWeapon = null;
                    if (NaturalWeaponSubpart != null
                        && part.Type == NaturalWeaponSubpart.Type)
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubpart);
                        Debug.Entry(4, $"NaturalWeaponSubpart", Indent: 3);
                    }
                    else if (NaturalWeaponSubparts.ContainsKey(part.Type))
                    {
                        modNaturalWeapon = GetNaturalWeaponMod(NaturalWeaponSubparts[part.Type]);
                        Debug.Entry(4, $"NaturalWeaponSubparts", Indent: 3);
                    }

                    Debug.Entry(4, $"modNaturalWeapon: {modNaturalWeapon?.Name}", Indent: 3);

                    if (modNaturalWeapon == null) continue;

                    if (part.DefaultBehavior != null)
                    {
                        part.DefaultBehavior.RemovePart(modNaturalWeapon);
                        if (part.DefaultBehavior.TryGetPart(out NaturalWeaponDescriber naturalWeaponDescriber))
                        {
                            naturalWeaponDescriber.ResetShortDescription();
                            naturalWeaponDescriber.CollectNaturalWeaponMods();
                        }
                    }
                    else if (part.Equipped != null && part.Equipped.HasPart<NaturalEquipment>())
                    {
                        part.Equipped.RemovePart(modNaturalWeapon);
                        if (part.Equipped.TryGetPart(out NaturalWeaponDescriber naturalWeaponDescriber))
                        {
                            naturalWeaponDescriber.ResetShortDescription();
                            naturalWeaponDescriber.CollectNaturalWeaponMods();
                        }
                    }
                }
                Debug.Divider(4, "-", Count: 25, Indent: 2);
            }
            Debug.Entry(4,
                $"x {typeof(T).Name}."
                + $"{nameof(UnprocessNaturalWeaponSubparts)} @//",
                Indent: 1);
            return true;
        }

        public virtual void OnRegenerateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = body.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(4, $"{typeof(T).Name}", $"{nameof(OnRegenerateDefaultEquipment)}(body)");
            Debug.Entry(4, $"TARGET {body.ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: false);

            Debug.Footer(4,
                $"{typeof(T).Name}",
                $"{nameof(OnRegenerateDefaultEquipment)}(body: {body.ParentObject.Blueprint})");
        }
        public virtual void OnDecorateDefaultEquipment(Body body)
        {
            Zone InstanceObjectZone = body.ParentObject.GetCurrentZone();
            string InstanceObjectZoneID = "[Pre-build]";
            if (InstanceObjectZone != null) InstanceObjectZoneID = InstanceObjectZone.ZoneID;
            Debug.Header(3, $"{typeof(T).Name}", $"{nameof(OnDecorateDefaultEquipment)}(body)");
            Debug.Entry(3, $"TARGET {body.ParentObject.DebugName} in zone {InstanceObjectZoneID}", Indent: 0);

            if (body != null)
                ProcessNaturalWeaponSubparts(body, CosmeticOnly: true);

            Debug.Footer(3,
                $"{typeof(T).Name}",
                $"{nameof(OnDecorateDefaultEquipment)}(body: {body.ParentObject.Blueprint})");
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
            BaseManagedDefaultEquipmentCybernetic<T> cybernetic = base.DeepCopy(Parent, MapInv) as BaseManagedDefaultEquipmentCybernetic<T>;
            cybernetic.NaturalWeaponSubparts = new();
            foreach ((string type, NaturalWeaponSubpart<T> subpart) in NaturalWeaponSubparts)
            {
                cybernetic.NaturalWeaponSubparts.Add(type, new(subpart, (T)cybernetic));
            }
            cybernetic.NaturalWeaponSubpart = new(NaturalWeaponSubpart, (T)cybernetic);
            cybernetic.Implantee = null;
            cybernetic.ImplantObject = null;
            return cybernetic;
        }
    }
}
