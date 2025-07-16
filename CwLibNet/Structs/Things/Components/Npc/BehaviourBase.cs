using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class BehaviourBase: ISerializable
{
    public const int BaseAllocationSize = 0x10;

    public Thing? Npc;
    public int Type;
    public int Attributes;

    public virtual void Serialize(Serializer serializer)
    {
        Npc = serializer.Thing(Npc);
        Type = serializer.S32(Type);
        Attributes = serializer.I32(Attributes);
    }

    public virtual int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}