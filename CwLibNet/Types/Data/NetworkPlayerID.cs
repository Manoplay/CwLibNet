using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Types.Data;

public class NetworkPlayerID: ISerializable
{
    public const int BaseAllocationSize = NetworkOnlineId.BaseAllocationSize + 0x10;

    public NetworkOnlineId? Handle = new();
    private byte[]? opt = new byte[8];
    private byte[]? reserved = new byte[8];

    public NetworkPlayerID() { }

    public NetworkPlayerID(string psid)
    {
        Handle = new NetworkOnlineId(psid);
    }

    
    public void Serialize(Serializer serializer)
    {
        Handle = serializer.Struct(Handle);

        var lengthPrefixed = serializer.GetRevision().GetVersion() < 0x234;

        if (lengthPrefixed) serializer.I32(8);
        opt = serializer.Bytes(opt, 8);

        if (lengthPrefixed) serializer.I32(8);
        reserved = serializer.Bytes(reserved, 8);
    }


    public int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }

    
    public NetworkPlayerID Clone()
    {
        var id = new NetworkPlayerID();
        id.Handle = new NetworkOnlineId(id.Handle?.ToString());
        if (opt != null)
            id.opt = (byte[])opt.Clone();
        if (reserved != null)
            id.reserved = (byte[])reserved.Clone();
        return id;
    }

    public override bool Equals(object? other)
    {
        if (other == this) return true;
        return other is NetworkPlayerID { Handle: not null } otherId && otherId.Handle.Equals(Handle);
    }

    public override int GetHashCode()
    {
        return Handle?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return Handle?.ToString() ?? "No handle";
    }


}