using XRL.World;

namespace HNPS_GigantismPlus
{
    public abstract partial class IConditions<T> : Condition<T>, IComposite
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
            Items = new IConditional<T>[Size];
            for (int i = 0; i < Length; i++)
            {
                Items[i] = (IConditional<T>)Reader.ReadObject();
            }
        }
    }
}
