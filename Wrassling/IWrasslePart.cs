using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Capabilities;
using XRL.World.Parts;

using SerializeField = UnityEngine.SerializeField;

namespace HNPS_GigantismPlus
{
    public abstract class IWrasslePart : IScribedPart, IWrasslePart<IWrasslePart>
    {
        [SerializeField]
        private Guid _WrassleID;
        public virtual Guid WrassleID 
        { 
            get => GetWrassleID();
            set => SetWrassleID(value);
        }

        public string PrimaryColor => UD_QWE.GetPrimaryWrassleColor(WrassleID);
        public string SecondaryColor => UD_QWE.GetSecondaryWrassleColor(WrassleID);

        public virtual Guid GetWrassleID()
        {
            return _WrassleID == Guid.Empty ? NewWrassleID() : _WrassleID;
        }
        public virtual Guid SetWrassleID(Guid WrassleID)
        {
            return _WrassleID = WrassleID;
        }
        public virtual Guid NewWrassleID()
        {
            return SetWrassleID(Guid.NewGuid());
        }
        public virtual Guid ClearWrassleID()
        {
            return SetWrassleID(Guid.Empty);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(_WrassleID);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            _WrassleID = Reader.ReadGuid();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            IWrasslePart wrassler = base.DeepCopy(Parent, MapInv) as IWrasslePart;
            wrassler.NewWrassleID();
            return wrassler;
        }
    }
    public interface IWrasslePart<T> where T : IPart
    {
        public abstract Guid WrassleID { get; set; }

        public abstract string PrimaryColor { get; }
        public abstract string SecondaryColor { get; }

        public abstract Guid GetWrassleID();
        public abstract Guid SetWrassleID(Guid WrassleID);
        public abstract Guid NewWrassleID();
        public abstract Guid ClearWrassleID();
    }
}
