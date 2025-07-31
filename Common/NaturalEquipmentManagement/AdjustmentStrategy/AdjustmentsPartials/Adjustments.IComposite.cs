using XRL;
using XRL.World;

namespace HNPS_GigantismPlus
{
    public partial class Adjustments : IAdjustment, IComposite
    {
        public override void Write(SerializationWriter Writer)
        {
            Writer.WriteOptimized(Length);
            for (int i = 0; i < Length; i++)
            {
                Writer.WriteObject(Items[i]);
            }
        }
        public override void Read(SerializationReader Reader)
        {
            Size = (Length = Reader.ReadOptimizedInt32());
            Items = new IAdjustment[Size];
            for (int i = 0; i < Length; i++)
            {
                Items[i] = (IAdjustment)Reader.ReadObject();
            }
        }
    }
}
