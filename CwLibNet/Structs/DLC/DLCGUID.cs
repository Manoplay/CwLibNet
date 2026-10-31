using CwLibNet.Enums;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Types.Data;

namespace CwLibNet.Structs.DLC;

public class DLCGUID: ISerializable
{
    public const int BaseAllocationSize = 0x10;
        
    public GUID? Guid;
    public int Flags = DLCFileFlags.NONE;

    public void Serialize(Serializer serializer)
    {
        Guid = serializer.Guid(Guid);
        Flags = serializer.I32(Flags);
    }

    public int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}