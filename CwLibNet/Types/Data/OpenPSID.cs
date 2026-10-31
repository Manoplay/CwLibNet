using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Types.Data
{
    public class OpenPSID : ISerializable
    {
        public const int BaseAllocationSize = 0x8;

        public long High;
        public long Low;

        public OpenPSID() { }

        public OpenPSID(long high, long low)
        {
            High = high;
            Low = low;
        }

        public void Serialize(Serializer serializer)
        {
            High = serializer.U64(High);
            Low = serializer.U64(Low);
        }

        public int GetAllocatedSize()
        {
            return BaseAllocationSize;
        }
    }
}
