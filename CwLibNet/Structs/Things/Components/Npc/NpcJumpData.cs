using System.Numerics;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class NpcJumpData: ISerializable
{
    public const int BaseAllocationSize = 0x50;

    public float A, B, C;
    public Vector3? Min;
    public Vector3? Max;

    public bool Flipped;

    public NpcMoveCmd[]? CommandList;

    public Vector3? Apex;

    public void Serialize(Serializer serializer)
    {
        var version = serializer.GetRevision().GetVersion();

        A = serializer.F32(A);
        B = serializer.F32(B);
        C = serializer.F32(C);

        Min = serializer.V3(Min);
        Max = serializer.V3(Max);

        if (version <= 0x272) return;
        Flipped = serializer.Bool(Flipped);
        CommandList = serializer.Array(CommandList);
        Apex = serializer.V3(Apex);
    }

    public int GetAllocatedSize()
    {
        var size = BaseAllocationSize;
        if (CommandList != null)
            size += (CommandList.Length * NpcMoveCmd.BaseAllocationSize);
        return size;
    }
}