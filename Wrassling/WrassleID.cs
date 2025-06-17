using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Capabilities;
using XRL.Wish;

using static XRL.UD_QudWrasslingEntertainment;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class WrassleID 
        : IScribedPart
        , IModEventHandler<GetWrassleIDEvent>
        , IModEventHandler<AddWrassleIDEvent>
        , IModEventHandler<UpdateWrassleIDEvent>
        , IModEventHandler<WrassleIDUpdatedEvent>
        , IModEventHandler<SyncWrassleIDEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(WrassleID));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                'S',    // Serialize
                'W',    // Wish
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

        public override int Priority => PRIORITY_HIGH;

        [SerializeField]
        private Guid _ID;
        public virtual Guid ID 
        { 
            get => GetID();
            set => SetID(value);
        }

        [SerializeField]
        private Guid _PreloadedWrassleID;
        public Guid PreloadedWrassleID
        {
            get => _PreloadedWrassleID;
            set => _PreloadedWrassleID = value;
        }

        private string _PrimaryColor;
        public string PrimaryColor => _PrimaryColor ??= UD_QWE.GetPrimaryWrassleColor(this);

        private string _SecondaryColor;
        public string SecondaryColor => _SecondaryColor ??= UD_QWE.GetSecondaryWrassleColor(this);

        public WrassleID()
        {
            PreloadedWrassleID = Guid.Empty;
        }
        public WrassleID(Guid ID)
        {
            PreloadedWrassleID = ID;
        }
        public WrassleID(WrassleID Source)
            : this (Source.ID)
        {
        }
        public WrassleID(IWrassle Source)
            : this(Source.WrassleID)
        {
        }

        public Guid GetID(bool SuppressEvent = false, bool Silent = false)
        {
            int indent = Debug.LastIndent;
            bool toggle = _ID == Guid.Empty || (!Silent && getDoDebug('X'));
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(GetID)}("
                + $"{nameof(SuppressEvent)}: {SuppressEvent}, "
                + $"{nameof(Silent)}: {Silent})"
                + $" for: {ParentObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: toggle);

            if (_ID == Guid.Empty)
            {
                Debug.LastIndent = indent;
                return NewID(SuppressEvent);
            }

            Debug.LastIndent = indent;
            return _ID;
        }
        public Guid SetID(Guid Value, bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(SetID)}("
                + $"{nameof(Guid)} {nameof(Value)}, "
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (SuppressEvent || UpdateWrassleIDEvent.CheckFor(this, ParentObject))
            {
                Guid oldID = _ID;
                _ID = Value;
                if (!SuppressEvent)
                {
                    WrassleIDUpdatedEvent.Send(this, ParentObject, oldID);
                }
            }

            Debug.LastIndent = indent;
            return _ID;
        }
        public Guid SetID(WrassleID Source, bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(SetID)}("
                + $"{nameof(WrassleID)} {nameof(Source)}, "
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Guid setID = SetID(Source.GetID(Silent: true), SuppressEvent);

            Debug.LastIndent = indent;
            return setID;
        }
        public virtual Guid SetID(IWrassle WrasslePart, bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(SetID)}("
                + $"{nameof(IWrassle)} {nameof(WrasslePart)}, "
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Guid setID = SetID(WrasslePart.WrassleID, SuppressEvent);

            Debug.LastIndent = indent;
            return setID;
        }
        public virtual Guid ClearID(bool SuppressEvent = true)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(ClearID)}("
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Guid clearID = SetID(Guid.Empty, SuppressEvent);

            Debug.LastIndent = indent;
            return clearID;
        }
        public virtual Guid NewID(bool SuppressEvent = true)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(NewID)}("
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Guid newID = SetID(Guid.NewGuid(), SuppressEvent);

            Debug.LastIndent = indent;
            return newID;
        }

        public virtual bool PushWrassleID(WrassleID ToWrassleID)
        {
            Guid oldWrassleID = ToWrassleID.ID;
            
            if (ToWrassleID.SetID(this) != oldWrassleID)
            {
                return true;
            }
            return false;
        }
        public virtual bool PullWrassleID(WrassleID FromWrassleID)
        {
            Guid oldWrassleID = GetID(Silent: true);
            
            if (SetID(FromWrassleID.GetID(Silent: true)) != oldWrassleID)
            {
                return true;
            }
            return false;
        }

        public virtual string GetWrassleShaderFor(string Word)
        {
            if (Word.IsNullOrEmpty())
            {
                return null;
            }
            return UD_QWE.GetWrassleShaderForWord(GetID(Silent: true), Word);
        }
        
        public virtual void OnUpdatedID()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(OnUpdatedID)}()",
                $"{GetID(Silent: true)}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            _PrimaryColor = null;
            _SecondaryColor = null;

            Debug.LastIndent = indent;
        }

        public bool SeededRandomBool(int? Stepper = null, int ChanceIn = 2)
        {
            return GetID(Silent: true).SeededRandomBool(Stepper, ChanceIn);
        }

        public override void Attach()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(Attach)}()",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{nameof(ParentObject)}", $"{ParentObject?.DebugName ?? NULL}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{nameof(WrassleID)}.{nameof(ID)}", $"{GetID(Silent: true)}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"calling base.{nameof(Attach)}()",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            base.Attach();

            ProcessPreloadedWrassleID(this, PreloadedWrassleID);

            Debug.LastIndent = indent;
        }

        public static bool ProcessPreloadedWrassleID(WrassleID WrassleID, Guid PreloadedWrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"= {nameof(WrassleID)}."
                + $"{nameof(ProcessPreloadedWrassleID)}("
                + $"{nameof(WrassleID)}, "
                + $"{nameof(PreloadedWrassleID)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (WrassleID != null && PreloadedWrassleID != Guid.Empty && PreloadedWrassleID != default)
            {
                Debug.CheckYeh(4, $"{nameof(PreloadedWrassleID)} has Value",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
                
                if (PreloadedWrassleID == WrassleID.GetID(Silent: true) || PreloadedWrassleID == WrassleID.SetID(PreloadedWrassleID))
                {
                    Debug.CheckYeh(4, $"{nameof(WrassleID)} set to {nameof(PreloadedWrassleID)}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));
                    PreloadedWrassleID = Guid.Empty;
                }
                Debug.LastIndent = indent;
                return PreloadedWrassleID == Guid.Empty;
            }
            else
            {
                Debug.CheckNah(4, $"{nameof(WrassleID)} is null, or {nameof(PreloadedWrassleID)} is empty or default",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
                Debug.LastIndent = indent;
                return false;
            }
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register(GetShortDescriptionEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(AddWrassleIDEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(SyncWrassleIDEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(UpdateWrassleIDEvent.ID, EventOrder.EXTREMELY_EARLY);
            Registrar.Register(WrassleIDUpdatedEvent.ID, EventOrder.EXTREMELY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int Cascade)
        {
            return base.WantEvent(ID, Cascade)
                || ID == GetWrassleIDEvent.ID;
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (WrassleIDDebugDescriptions)
            {
                // bool haveOrigin = Origin != null;

                StringBuilder SB = Event.NewStringBuilder();
                SB.AppendColored("M", $"{nameof(WrassleID)}");
                SB.AppendLine();
                SB.AppendColored("W", "ID: ").AppendColored("g", $"{GetID(Silent: true)}");
                SB.AppendLine();
                SB.AppendColored("W", "Colors");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored(PrimaryColor, $"{PrimaryColor}").Append($"){HONLY}{nameof(PrimaryColor)}");
                SB.AppendLine();
                SB.Append(TANDR).Append("(").AppendColored(SecondaryColor, $"{SecondaryColor}").Append($"){HONLY}{nameof(SecondaryColor)}");
                SB.AppendLine();

                E.Infix.AppendLine().AppendRules(Event.FinalizeString(SB));
            }
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(GetWrassleIDEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetWrassleIDEvent)} E) "
                + $"{nameof(E.WrassleObject)}: {E.WrassleObject?.DebugName ?? NULL}, "
                + $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}, "
                + $"{nameof(E.Context)}: {E.Context?.Quote()}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (E.WrassleObject == ParentObject)
            {
                Debug.Entry(4, $"{nameof(E.WrassleObject)} is {nameof(ParentObject)}",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                Debug.Entry(4, $"{nameof(WrassleID)}.{nameof(ID)}: {GetID(Silent: true)}",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                WrassleID wrassleID = this;
                E.SetWrassleIDTo(wrassleID);
                // E.WrassleID = wrassleID;

                Debug.Entry(4, $"{nameof(HandleEvent)}({nameof(GetWrassleIDEvent)} E) returning false",
                    Indent: indent + 1, Toggle: getDoDebug('X'));

                Debug.LastIndent = indent;
                return false;
            }

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(AddWrassleIDEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(AddWrassleIDEvent)} E) "
                + $"{nameof(E.WrassleObject)}: {E.WrassleObject?.DebugName ?? NULL}, "
                + $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}, "
                + $"{nameof(E.Context)}: {E.Context?.Quote()}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (E.WrassleID != this && E.WrassleObject != ParentObject)
            {
                Debug.Entry(4, $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL} pushing WrassleID",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                E.FromWrassleID = GetID(Silent: true);

                Debug.LastIndent = indent;
                return true;
            }

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(SyncWrassleIDEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(SyncWrassleIDEvent)} E) "
                + $"{nameof(E.WrassleObject)}: {E.WrassleObject?.DebugName ?? NULL}, "
                + $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}, "
                + $"{nameof(E.Context)}: {E.Context?.Quote()}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (E.WrassleID != null && E.WrassleObject == ParentObject && PullWrassleID(E.WrassleID))
            {
                Debug.Entry(4, $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL} pulled WrassleID from {E.WrassleID?.ParentObject?.DebugName ?? NULL}",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                Debug.LastIndent = indent;
                return true;
            }

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(UpdateWrassleIDEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(UpdateWrassleIDEvent)} E) "
                + $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}, "
                + $"{nameof(E.Context)}: {E.Context?.Quote()}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(WrassleIDUpdatedEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(WrassleIDUpdatedEvent)} E) "
                + $"{nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}, "
                + $"{nameof(E.Context)}: {E.Context?.Quote()}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            OnUpdatedID();

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }

        public override string ToString()
        {
            return GetID(Silent: true).ToString();
        }

        public virtual bool IsSyncedWith(WrassleID WrassleID, bool Silent = false)
        {
            return WrassleID.GetID(Silent: true) == GetID(Silent: true);
        }
        public virtual bool IsSyncedWith(Guid WrassleID, bool Silent = false)
        {
            return WrassleID == GetID(Silent: true);
        }

        public override bool SameAs(IPart p)
        {
            return p is WrassleID w
                && w.GetID(Silent: true) == GetID(Silent: true)
                && base.SameAs(p);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            ToString().Vomit(4, nameof(Write), Indent: Debug.LastIndent, Toggle: getDoDebug('S'));
            Writer.Write(_ID);

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
            ToString().Vomit(4, nameof(Read), Indent: Debug.LastIndent, Toggle: getDoDebug('S'));
            _ID = Reader.ReadGuid();

            bool readPreloadedWrassleID = Reader.ReadBoolean();
            if (readPreloadedWrassleID)
            {
                _PreloadedWrassleID = Reader.ReadGuid();
            }
        }
        public override void FinalizeRead(SerializationReader Reader)
        {
            base.FinalizeRead(Reader);
            OnUpdatedID();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(DeepCopy)}("
                + $"{nameof(Parent)}: {Parent?.DebugName ?? NULL}, "
                + $"{nameof(MapInv)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            WrassleID wrassleID = base.DeepCopy(Parent, MapInv) as WrassleID;
            wrassleID._PrimaryColor = null;
            wrassleID._SecondaryColor = null;
            wrassleID.SetID(GetID(Silent: true), true);

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public override void FinalizeCopyLate(GameObject Source, bool CopyEffects, bool CopyID, Func<GameObject, GameObject> MapInv)
        {
            base.FinalizeCopyLate(Source, CopyEffects, CopyID, MapInv);
            WrassleIDUpdatedEvent.Send(this, ParentObject, Guid.Empty);
        }

        [WishCommand(Command = "poke WrassleIDs")]
        public static void CascadeWrassleIDToParts()
        {
            foreach (GameObject wrassleObject in The.Player.CurrentZone.GetObjects())
            {
                if (wrassleObject.TryWrassleID(out WrassleID wrassleID))
                {
                    Debug.Entry(4,
                        $"/ {wrassleObject?.DebugName ?? NULL} poked",
                        Indent: 0, Toggle: true || getDoDebug('W'));
                    WrassleIDUpdatedEvent.Send(wrassleID, wrassleObject, Guid.Empty);
                    foreach (IWrassle wrasslePart in wrassleObject.GetPartsDescendedFrom<IWrassle>())
                    {
                        wrasslePart.OnUpdatedWrassleID();
                    }
                }
            }
        }
    }
}
