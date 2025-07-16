using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class NpcMoveCmd: ISerializable
{
    public const int BaseAllocationSize = 0x10;

    public int Buttons;
    public byte StickX, StickY;

    public void Serialize(Serializer serializer)
    {
        Buttons = serializer.I32(Buttons);
        StickX = serializer.I8(StickX);
        StickY = serializer.I8(StickY);
        if (serializer.GetRevision().GetVersion() < 0x280)
            serializer.U8(0);
    }

    public int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}