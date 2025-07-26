using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using XRL.Collections;
using XRL.World;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : ICondition<T>, IComposite
        where T : class, new()
    {
        public virtual void Write(SerializationWriter Writer)
        {
            Writer.WriteOptimized(Length);
            for (int i = 0; i < Length; i++)
            {
                Writer.WriteObject(Items[i]);
            }
        }
        public virtual void Read(SerializationReader Reader)
        {
            Size = (Length = Reader.ReadOptimizedInt32());
            Items = new ICondition<T>[Size];
            for (int i = 0; i < Length; i++)
            {
                Items[i] = (ICondition<T>)Reader.ReadObject();
            }
        }
    }
}
