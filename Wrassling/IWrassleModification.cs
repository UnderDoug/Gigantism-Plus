using AiUnity.NLog.Core.Time;
using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.ZoneBuilders;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.UD_QudWrasslingEntertainment;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class IWrassleModification : IModification, IWrassle
    {
        private static bool doDebug => getClassDoDebug(nameof(IWrassleModification));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
                "WID"   // WrassleID
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        [SerializeField]
        private WrassleID _WrassleID;
        public virtual WrassleID WrassleID => _WrassleID ??= GetWrassleID();

        public string PrimaryColor => WrassleID?.PrimaryColor;
        public string SecondaryColor => WrassleID?.SecondaryColor;

        public string EquipmentFrameColor => UD_QWE.GetEquipmentFrameColor(WrassleID.ID);

        public Guid PreloadedWrassleID;

        public IWrassleModification()
        {
            PreloadedWrassleID = Guid.Empty;
        }
        public IWrassleModification(Guid ID)
        {
            PreloadedWrassleID = ID;
        }
        public IWrassleModification(WrassleID Source)
            : this(Source.ID)
        {
        }
        public IWrassleModification(IWrassle Source)
            : this(Source.WrassleID)
        {
        }
        public IWrassleModification(int Tier)
            : base(Tier)
        {
            PreloadedWrassleID = Guid.Empty;
        }
        public IWrassleModification(int Tier, Guid ID)
            : this(Tier)
        {
            PreloadedWrassleID = ID;
        }
        public IWrassleModification(int Tier, WrassleID Source)
            : this(Tier, Source.ID)
        {
        }
        public IWrassleModification(int Tier, IWrassle Source)
            : this(Tier, Source.WrassleID)
        {
        }

        public override void Attach()
        {

            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(Attach)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"calling base.{nameof(Attach)}()",
                Indent: indent + 2, Toggle: getDoDebug('X'));
            base.Attach();

            if (WrassleID != null && PreloadedWrassleID != Guid.Empty && PreloadedWrassleID != default)
            {
                Debug.CheckYeh(4, $"{nameof(PreloadedWrassleID)} has Value",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                if (PreloadedWrassleID == WrassleID.ID || PreloadedWrassleID == WrassleID.SetID(PreloadedWrassleID))
                {
                    Debug.CheckYeh(4, $"{nameof(WrassleID)} set to {nameof(PreloadedWrassleID)}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));
                    PreloadedWrassleID = Guid.Empty;
                }
            }
            else
            {
                Debug.CheckNah(4, $"{nameof(WrassleID)} is null, or {nameof(PreloadedWrassleID)} is empty or default",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
            }
            Debug.LastIndent = indent;
        }

        public virtual WrassleID GetWrassleID()
        {
            return UD_QWE.RequireWrassleID(ParentObject);
        }
        public virtual WrassleID SetWrassleID(Guid WrassleID)
        {
            this.WrassleID.SetID(WrassleID);
            return this.WrassleID;
        }
        public virtual WrassleID SetWrassleID(WrassleID WrassleID)
        {
            return SetWrassleID(WrassleID.ID);
        }
        public virtual WrassleID SetWrassleID(IWrassle WrasslePart)
        {
            return SetWrassleID(WrasslePart.WrassleID);
        }
        public virtual Guid NewWrassleID()
        {
            return WrassleID.NewID();
        }
        public virtual void ClearWrassleID()
        {
            WrassleID.ClearID();
        }
        public virtual bool ClearCachedWrassleID()
        {
            return (_WrassleID = null) == null;
        }

        public virtual void OnUpdatedWrassleID()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassle)}."
                + $"{nameof(OnUpdatedWrassleID)}()",
                $"{WrassleID.ID}",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        }

        public override bool BeingAppliedBy(GameObject Gear, GameObject Wrassler)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(BeingAppliedBy)}("
                + $"{nameof(Gear)}: {Gear?.DebugName ?? NULL}, "
                + $"{nameof(Wrassler)}: {Wrassler?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (!UD_QWE.TrySyncWrassleID(Wrassler, Gear))
            {
                Debug.Warn(2,
                    $"{nameof(IWrassleModification)}",
                    $"{nameof(BeingAppliedBy)}",
                    $"Failed to Sync WrassleID: {Wrassler.DebugName ?? NULL} push to {Gear?.DebugName ?? NULL}",
                    Indent: 0);
            }

            Debug.LastIndent = indent;
            return base.BeingAppliedBy(Gear, Wrassler);
        }

        public virtual void SetEquipmentFrame()
        {
            if (!ParentObject.TryGetPart(out WrassleGear wrassleGear) || wrassleGear.ColorEquipmentFrame)
            {
                ParentObject?.SetEquipmentFrameColors(EquipmentFrameColor);
            }
        }

        public virtual string GetWrassleShaderFor(string Word)
        {
            return UD_QWE.GetWrassleShaderForWord(WrassleID, Word);
        }

        public override bool WantEvent(int ID, int Cascade)
        {
            return base.WantEvent(ID, Cascade)
                || ID == UpdateWrassleIDEvent.ID
                || ID == WrassleIDUpdatedEvent.ID;
        }
        public virtual bool HandleEvent(UpdateWrassleIDEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(WrassleIDUpdatedEvent E)
        {
            OnUpdatedWrassleID();
            return base.HandleEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            _WrassleID.Write(Basis, Writer);

            bool writePreloadedWrassleID = PreloadedWrassleID != Guid.Empty && PreloadedWrassleID != default;
            Writer.Write(writePreloadedWrassleID);
            if (writePreloadedWrassleID)
            {
                Writer.Write(PreloadedWrassleID);
            }
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            _WrassleID = Reader.ReadObject() as WrassleID;

            bool readPreloadedWrassleID = Reader.ReadBoolean();
            if (readPreloadedWrassleID)
            {
                PreloadedWrassleID = Reader.ReadGuid();
            }
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            IWrassleModification wrassleMod = base.DeepCopy(Parent, MapInv) as IWrassleModification;
            return wrassleMod;
        }
    }
}
