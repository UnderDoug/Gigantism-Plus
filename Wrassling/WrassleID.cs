using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
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
        private Guid _ID;
        public virtual Guid ID 
        { 
            get => GetID();
            set => SetID(value);
        }

        public string PrimaryColor => UD_QWE.GetPrimaryWrassleColor(this);
        public string SecondaryColor => UD_QWE.GetSecondaryWrassleColor(this);

        public WrassleID()
        {
        }
        public WrassleID(Guid ID)
        {
            SetID(ID, SuppressEvent: true);
        }
        public WrassleID(WrassleID Source)
            : this (Source.ID)
        {
        }
        public WrassleID(IWrassle Source)
            : this(Source.WrassleID)
        {
        }

        public Guid GetID(bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(GetID)}("
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

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
                _ID = Value;
                if (!SuppressEvent)
                {
                    WrassleIDUpdatedEvent.Send(this, ParentObject);
                }
            }

            Debug.LastIndent = indent;
            return _ID;
        }
        public Guid SetID(WrassleID Source, bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
                + $"{nameof(SetID)}("
                + $"{nameof(WrassleID)} {nameof(Source)}, "
                + $"{nameof(SuppressEvent)}: {SuppressEvent})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Guid setID = SetID(Source.ID, SuppressEvent);

            Debug.LastIndent = indent;
            return setID;
        }
        public virtual Guid SetID(IWrassle WrasslePart, bool SuppressEvent = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"@ {nameof(WrassleID)}."
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
                $"@ {nameof(WrassleID)}."
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
                $"@ {nameof(WrassleID)}."
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
            Guid oldWrassleID = ID;
            
            if (SetID(FromWrassleID) != oldWrassleID)
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
            return UD_QWE.GetWrassleShaderForWord(ID, Word);
        }
        
        public virtual void OnUpdatedID()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(WrassleID)}."
                + $"{nameof(OnUpdatedID)}()",
                $"{ID}",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
        }

        public bool SeededRandomBool(int? Stepper = null, int ChanceIn = 2)
        {
            return ID.SeededRandomBool(Stepper, ChanceIn);
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

            Debug.Entry(4, $"{nameof(WrassleID)}.{nameof(ID)}", $"{ID}",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"calling base.{nameof(Attach)}()",
                Indent: indent + 2, Toggle: getDoDebug('X'));

            base.Attach();

            Debug.LastIndent = indent;
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
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

                Debug.Entry(4, $"{nameof(WrassleID)}.{nameof(ID)}: {ID}",
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

                E.FromWrassleID = ID;

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
            return ID.ToString();
        }

        public virtual bool IsSyncedWith(WrassleID WrassleID)
        {
            return WrassleID.ID == ID;
        }
        public virtual bool IsSyncedWith(Guid WrassleID)
        {
            return WrassleID == ID;
        }

        public override bool SameAs(IPart p)
        {
            return p is WrassleID w 
                && w.ID == ID 
                && base.SameAs(p);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(_ID);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            _ID = Reader.ReadGuid();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            WrassleID wrassleID = base.DeepCopy(Parent, MapInv) as WrassleID;
            return wrassleID;
        }
    }
}
