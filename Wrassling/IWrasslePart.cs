using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Parts;
using XRL.World.Capabilities;

using static XRL.UD_QudWrasslingEntertainment;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class IWrasslePart : IScribedPart, IWrassle
    {
        private static bool doDebug => getClassDoDebug(nameof(IWrasslePart));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                'X',    // Trace
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

        public virtual string PrimaryColor => WrassleID?.PrimaryColor;
        public virtual string SecondaryColor => WrassleID?.SecondaryColor;

        public Guid PreloadedWrassleID;

        public IWrasslePart()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"~ {nameof(IWrasslePart)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            PreloadedWrassleID = Guid.Empty;

            Debug.LastIndent = indent;
        }
        public IWrasslePart(Guid ID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"~ {nameof(IWrasslePart)}("
                + $"{nameof(Guid)} {nameof(ID)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            PreloadedWrassleID = ID;

            Debug.LastIndent = indent;
        }
        public IWrasslePart(WrassleID Source)
            : this(Source.ID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"~ {nameof(IWrasslePart)}("
                + $"{nameof(WrassleID)} {nameof(Source)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
        }
        public IWrasslePart(IWrassle Source)
            : this(Source.WrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"~ {nameof(IWrasslePart)}("
                + $"{nameof(IWrassle)} {nameof(Source)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
        }

        public override void Attach()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrasslePart)}."
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
        public virtual void ClearWrassleID()
        {
            WrassleID.ClearID();
        }
        public virtual bool ClearCachedWrassleID()
        {
            return (_WrassleID = null) == null;
        }
        public virtual Guid NewWrassleID()
        {
            return WrassleID.NewID();
        }
        public virtual void OnUpdatedWrassleID()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrasslePart)}."
                + $"{nameof(OnUpdatedWrassleID)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
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
            IWrasslePart wrasslePart = base.DeepCopy(Parent, MapInv) as IWrasslePart;
            return wrasslePart;
        }
    }
}
