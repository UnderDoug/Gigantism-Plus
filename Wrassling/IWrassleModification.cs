using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.ZoneBuilders;

using static XRL.UD_QudWrasslingEntertainment;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public abstract class IWrassleModification 
        : IModification
        , IWrassle
        , IModEventHandler<BeforeDescribeModificationEvent<IWrassleModification>>
        , IModEventHandler<DescribeModificationEvent<IWrassleModification>>
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
                "WID",  // WrassleID
                'X',    // Trace
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

        private Dictionary<int, string> WrassleShaderCache;

        public string EquipmentFrameColor => UD_QWE.GetEquipmentFrameColor(WrassleID.ID);

        public IWrassleModification()
        {
            WrassleShaderCache = new();
            PreloadedWrassleID = Guid.Empty;
        }
        public IWrassleModification(Guid ID)
            : this()
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
            WrassleShaderCache = new();
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
                Indent: indent + 1, Toggle: getDoDebug('X'));

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
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(GetWrassleShaderFor)}("
                + $"{nameof(Word)}: {Word ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (Word.IsNullOrEmpty() || Word.Length < 1)
            {
                Debug.CheckNah(4, $"{nameof(Word)} empty or null", Indent: indent + 2, Toggle: getDoDebug('X'));
                Debug.LastIndent = indent;
                return null;
            }
            if ((WrassleShaderCache ??= new()).IsNullOrEmpty() || !WrassleShaderCache.ContainsKey(Word.Length))
            {
                Debug.CheckYeh(4, $"{nameof(WrassleShaderCache)} is either empty or doesn't contain key {Word.Length}", 
                    Indent: indent + 2, Toggle: getDoDebug('X'));
                string shader = UD_QWE.GetWrassleShaderForWord(WrassleID, Word);
                if (shader.StartsWith(" sequence"))
                {
                    Debug.CheckNah(4, $"{nameof(shader)} functionally empty", Indent: indent + 2, Toggle: getDoDebug('X'));
                    Debug.LastIndent = indent;
                    return null;
                }
                WrassleShaderCache.TryAdd(Word.Length, shader);
            }

            Debug.Entry(4, $"{nameof(GetWrassleShaderFor)}({Word})", $"{WrassleShaderCache[Word.Length].Quote()}",
                Indent: indent + 2, Toggle: getDoDebug('X'));
            Debug.LastIndent = indent;
            return WrassleShaderCache[Word.Length];
        }

        public virtual bool WantModDisplayName()
        {
            return false;
        }
        public virtual bool WantModShortDescription()
        {
            return false;
        }
        public virtual string GetAdjective()
        {
            return "adjective?";
        }
        public virtual string GetColoredAdjective()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(GetColoredAdjective)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            string shader = null;
            string coloredAdjective = GetAdjective();
            if ((shader = GetWrassleShaderFor(GetAdjective())) != null)
            {
                coloredAdjective = coloredAdjective.Color(shader);
            }

            Debug.LastIndent = indent;
            return coloredAdjective;
        }
        public virtual string GetInstanceDescription(GameObject Object = null)
        {
            return DescribeModificationEvent<IWrassleModification>
                .Send(Object, GetColoredAdjective(), Context: NATURAL_EQUIPMENT)
                .Process();
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
                || ID == WrassleIDUpdatedEvent.ID
                || (WantModDisplayName() && ID == PooledEvent<GetDisplayNameEvent>.ID)
                || (WantModShortDescription() && ID == GetShortDescriptionEvent.ID);
        }
        public virtual bool HandleEvent(UpdateWrassleIDEvent E)
        {
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(WrassleIDUpdatedEvent E)
        {
            if (WrassleID != null && WrassleID.ID != E.FromWrassleID)
            {
                _PrimaryColor = null;
                _SecondaryColor = null;
                WrassleShaderCache = new();
                OnUpdatedWrassleID();
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (WantModDisplayName() && E.Understood() && !E.Object.HasProperName)
            {
                E.AddAdjective(GetColoredAdjective());
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (WantModShortDescription())
            {
                E.Postfix.AppendRules(GetInstanceDescription(ParentObject));
            }
            return base.HandleEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            _WrassleID.Write(Basis, Writer);

            bool writePreloadedWrassleID = _PreloadedWrassleID != Guid.Empty && _PreloadedWrassleID != default;
            Writer.Write(writePreloadedWrassleID);
            if (writePreloadedWrassleID)
            {
                Writer.Write(_PreloadedWrassleID);
            }
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            _WrassleID = Reader.ReadObject() as WrassleID;

            bool readPreloadedWrassleID = Reader.ReadBoolean();
            if (readPreloadedWrassleID)
            {
                _PreloadedWrassleID = Reader.ReadGuid();
            }
        }
        public override void FinalizeRead(SerializationReader Reader)
        {
            base.FinalizeRead(Reader);
            OnUpdatedWrassleID();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            IWrassleModification wrassleMod = base.DeepCopy(Parent, MapInv) as IWrassleModification;
            wrassleMod._PrimaryColor = null;
            wrassleMod._SecondaryColor = null;
            wrassleMod.WrassleShaderCache = new();
            return wrassleMod;
        }
    }
}
