using System.Numerics;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class NpcJumpSolver: ISerializable
{
    public const int BaseAllocationSize = 0x100;

    public bool IsCurrentJumpFlipped;

    public Vector3? CurSource;
    public Vector3? CurTarget0;
    public Vector3? CurTarget1;

    public int CurrentJump, CurrentJumpPos;

    public NpcJumpData[]? JumpData;

    public float MaxEffectiveJumpHeight;

    public bool Trained;

    public NpcJumpData StandingJump;

    public void Serialize(Serializer serializer)
    {
        var version = serializer.GetRevision().GetVersion();

        switch (version)
        {
            case < 0x2c7:
                JumpData = serializer.Array(JumpData);
                MaxEffectiveJumpHeight = serializer.F32(MaxEffectiveJumpHeight);
                Trained = serializer.Bool(Trained);
                StandingJump = serializer.Struct(StandingJump);
                break;
            case > 0x2cd:
                IsCurrentJumpFlipped = serializer.Bool(IsCurrentJumpFlipped);
                CurSource = serializer.V3(CurSource);
                CurTarget0 = serializer.V3(CurTarget0);
                CurTarget1 = serializer.V3(CurTarget1);
                CurrentJump = serializer.S32(CurrentJump);
                CurrentJumpPos = serializer.I32(CurrentJumpPos);
                break;
        }
    }

    public int GetAllocatedSize()
    {
        var size = BaseAllocationSize;
        if (JumpData != null)
            size += (JumpData.Length * NpcJumpData.BaseAllocationSize);
        return size;
    }
}