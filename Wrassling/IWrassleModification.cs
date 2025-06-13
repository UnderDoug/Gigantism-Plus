using System;
using System.Collections.Generic;
using System.Text;

using XRL.World;
using XRL.World.Capabilities;
using XRL.World.Parts;

using SerializeField = UnityEngine.SerializeField;

namespace HNPS_GigantismPlus
{
    public abstract class IWrassleModification : IModification, IWrasslePart<IWrassleModification>
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
            return _WrassleID == Guid.Empty ? Guid.NewGuid() : _WrassleID;
        }
        public virtual Guid SetWrassleID(Guid WrassleID)
        {
            return _WrassleID = WrassleID;
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
            // wrassler._WrassleID = Guid.NewGuid();
            return wrassler;
        }
    }
}
