using System.Numerics;
using CwLibNet.Enums;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Types.Data;

namespace CwLibNet.Structs.Things.Components.Npc;

public class NpcBehavior: ISerializable
{
    public const int BaseAllocationSize = 0x100;

    public Thing? Npc;
    public Thing? TargetThing;
    public int Type, Attributes;
    public float MaxMoveSpeed;
    public int MaxWaitTime;

    public string? WaypointKeyName;
    public string? PoiKeyName;
    public int WaypointKeyColorIndex;
    public int PoiKeyColorIndex;
    public ActingData? ActingData;
    public float AwarenessRadius;
    public int SharedStateTimer;
    public Vector3? IdleLookAtPos;
    public Vector3? LastGoodPosition;
    public bool LastPositionValid;
    public Vector3? PatrolDirection;
    public int LastPatrolGridX, LastPatrolGridZ;
    public int TargetPatrolGridX, TargetPatrolGridZ;
    public int GridDirectionX, GridDirectionZ;
    public int PatrolStationaryCounter, PatrolUnblockedCounter;
    public int AnimSet;
    public byte ExpressionType, ExpressionLevel;
    public bool WillRecordAudio;
    public byte MultiJumpLevel;
    public int AwarenessRange;
    public float LookAtSpeed;
    public bool ShowAdvancedOptions;

    public void Serialize(Serializer serializer)
    {
        Revision revision = serializer.GetRevision();
        int version = revision.GetVersion();
        int subVersion = revision.GetSubVersion();

        if (version <= 0x293) return;

        Npc = serializer.Thing(Npc);
        TargetThing = serializer.Thing(TargetThing);
        // ENpcBehaviourType
        // 0 = PATROL
        // 1 = FOLLOW
        // 2 = FLEE
        // 3 = IDLE
        // 4 = ACT
        // 5 = WAYPOINT
        Type = serializer.S32(Type);

        Attributes = serializer.I32(Attributes);
        // ATTRIBUTES
        // 0x0 = NONE
        // 0x1 = CAN_CHANGE_HEIGHT
        // 0x2 = CAN_JUMP
        // 0x4 = CAN_CHANGE_LAYER
        // 0x8 = UNUSED
        // 0x10 = HOSTILE

        MaxMoveSpeed = serializer.F32(MaxMoveSpeed);
        MaxWaitTime = serializer.S32(MaxWaitTime);

        if (version < 0x2cf) serializer.S32(0);

        if (version > 0x2e5)
            WaypointKeyName = serializer.Wstr(WaypointKeyName);

        if (version > 0x2d4)
            WaypointKeyColorIndex = serializer.S32(WaypointKeyColorIndex);

        if (version > 0x2e5)
        {
            PoiKeyName = serializer.Wstr(PoiKeyName);
            PoiKeyColorIndex = serializer.S32(PoiKeyColorIndex);
        }

        if (version > 0x295)
            ActingData = serializer.Reference(ActingData);

        if (version > 0x2ac)
        {
            AwarenessRadius = serializer.F32(AwarenessRadius);
            SharedStateTimer = serializer.I32(SharedStateTimer);
            IdleLookAtPos = serializer.V3(IdleLookAtPos);
        }

        if (version > 0x2d7)
            LastGoodPosition = serializer.V3(LastGoodPosition);
        if (version > 0x2d8)
            LastPositionValid = serializer.Bool(LastPositionValid);
        if (version > 0x2ce)
        {
            PatrolDirection = serializer.V3(PatrolDirection);
            LastPatrolGridX = serializer.S32(LastPatrolGridX);
            LastPatrolGridZ = serializer.S32(LastPatrolGridZ);
            TargetPatrolGridX = serializer.S32(TargetPatrolGridX);
            TargetPatrolGridZ = serializer.S32(TargetPatrolGridZ);
            GridDirectionX = serializer.S32(GridDirectionX);
            GridDirectionZ = serializer.S32(GridDirectionZ);
            PatrolStationaryCounter = serializer.S32(PatrolStationaryCounter);
            PatrolUnblockedCounter = serializer.S32(PatrolUnblockedCounter);
        }

        if (version > 0x2e6)
            AnimSet = serializer.S32(AnimSet);
        if (version > 0x371)
        {
            ExpressionType = serializer.I8(ExpressionType);
            ExpressionLevel = serializer.I8(ExpressionLevel);
        }

        if (version > 0x375)
            WillRecordAudio = serializer.Bool(WillRecordAudio);

        if (revision.Has(Branch.Double11, 0x16)) // 0x3d4
            MultiJumpLevel = serializer.I8(MultiJumpLevel);

        if (subVersion > 0xc6)
            AwarenessRange = serializer.I32(AwarenessRange);
        if (subVersion > 0x10e)
            LookAtSpeed = serializer.F32(LookAtSpeed);
        if (subVersion > 0x175)
            ShowAdvancedOptions = serializer.Bool(ShowAdvancedOptions);
    }

    public int GetAllocatedSize()
    {
        var size = BaseAllocationSize;
        if (WaypointKeyName != null) size += WaypointKeyName.Length * 0x2;
        if (PoiKeyName != null) size += PoiKeyName.Length * 0x2;
        if (ActingData != null) size += ActingData.GetAllocatedSize();
        return size;
    }
}