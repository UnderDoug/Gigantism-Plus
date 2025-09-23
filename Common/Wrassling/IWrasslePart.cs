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
    public abstract class IWrasslePart
        : IScribedPart
        , IWrassle
    {
        private static bool doDebug => getClassDoDebug(nameof(IWrasslePart));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
                'X',    // Trace
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        [NonSerialized]
        private WrassleID _WrassleID;
        public virtual WrassleID WrassleID => _WrassleID ??= GetWrassleID();

        [SerializeField]
        private Guid _PreloadedWrassleID;
        public Guid PreloadedWrassleID
        {
            get => _PreloadedWrassleID;
            set => _PreloadedWrassleID = value;
        }

        private string _PrimaryColor;
        public string PrimaryColor => _PrimaryColor ??= WrassleID?.PrimaryColor;

        private string _SecondaryColor;
        public string SecondaryColor => _SecondaryColor ??= WrassleID?.SecondaryColor;

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

            WrassleID.ProcessPreloadedWrassleID(WrassleID, PreloadedWrassleID);

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

        public override bool AllowStaticRegistration()
        {
            return base.AllowStaticRegistration()
                || true;
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
            if (WrassleID != null && WrassleID.GetID(Silent: true) != E.FromWrassleID)
            {
                _PrimaryColor = null;
                _SecondaryColor = null;
                OnUpdatedWrassleID();
            }
            return base.HandleEvent(E);
        }
        /*
        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
        }
        */
        public override void FinalizeRead(SerializationReader Reader)
        {
            base.FinalizeRead(Reader);
            OnUpdatedWrassleID();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            IWrasslePart wrasslePart = base.DeepCopy(Parent, MapInv) as IWrasslePart;
            wrasslePart._PrimaryColor = null;
            wrasslePart._SecondaryColor = null;
            return wrasslePart;
        }
    }
}
