using System;
using System.Collections.Generic;

using XRL;
using XRL.Language;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public partial struct DescriptionElement : IComposite
    {
        public void Write(SerializationWriter Writer)
        {
            Writer.WriteOptimized(Priority);
            Writer.WriteOptimized(Verb);
            Writer.WriteOptimized(Effect);
        }
        public void Read(SerializationReader Reader)
        {
            Priority = Reader.ReadOptimizedInt32();
            Verb = Reader.ReadOptimizedString();
            Effect = Reader.ReadOptimizedString();
        }
    }
}
