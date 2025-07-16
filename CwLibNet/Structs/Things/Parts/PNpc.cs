using System.Numerics;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Structs.Things.Components.Npc;

namespace CwLibNet.Structs.Things.Parts;

public class PNpc: ISerializable
{
    public const int BaseAllocationSize = 0x4;

    public NpcJumpSolver JumpSolver = new();
    public byte[]? SoundRecording;
    public int[]? SoundRecordingDataNbytes;
    public int SoundRecordingPacket;
    public int SoundRecordingPacketOffset;
    public int[]? SackbotRecordingTimes;

    public BehaviourFollow? Follow;

    public BehaviourAct? Act;

    public NpcBehavior? RecordingBehavior;

    public int Flags;

    public Thing? BehaviorThing;

    public Thing? RootBehaviorThing;

    public Vector3? MoveTarget;

    public int WaitTime;

    public int PlayerNumber;

    public string? ActorName;

    public int LastTimeThrown, LastTimeHitTheGround, LastThrower;

    public byte CostumeToCopy;
    public bool CopyFormAsWell;

    public void Serialize(Serializer serializer)
    {
        var version = serializer.GetRevision().GetVersion();
        var subVersion = serializer.GetRevision().GetSubVersion();

        if (version < 0x273) return;

        JumpSolver = serializer.Struct(JumpSolver);

        if (version < 0x2ac) serializer.Bool(false);
        if (version < 0x293) serializer.Bool(false);
        if (version < 0x290)
        {
            serializer.Array<Input>(null);
            serializer.I32(0);
            serializer.Bool(false);
            serializer.Thing(null);
        }

        if (serializer.GetRevision().GetSubVersion() < 0x118)
            SoundRecording = serializer.Bytearray(SoundRecording);
        SoundRecordingDataNbytes = serializer.Intvector(SoundRecordingDataNbytes);
        SoundRecordingPacket = serializer.I32(SoundRecordingPacket);
        SoundRecordingPacketOffset = serializer.I32(SoundRecordingPacketOffset);
        SackbotRecordingTimes = serializer.Intvector(SackbotRecordingTimes);

        switch (version)
        {
            case > 0x286 and < 0x293:
                Follow = serializer.Reference(Follow);
                Act = serializer.Reference(Act);
                break;
            case > 0x2da:
            case < 0x29b and > 0x294:
                RecordingBehavior = serializer.Reference(RecordingBehavior);
                break;
        }

        ;

        if (version > 0x2ac)
        {
            // ENpcFlags
            // IS_JUMPING 1
            // IS_HOSTILE 2
            // IS_PAUSED 4
            Flags = serializer.I32(Flags);
            if (version < 0x2ce)
                serializer.S32(0); // jumpBackoff
        }

        if (version > 0x29a)
            BehaviorThing = serializer.Thing(BehaviorThing);
        if (version > 0x2d5)
            RootBehaviorThing = serializer.Thing(RootBehaviorThing);

        if (version > 0x2cd && version < 0x36e)
        {
            serializer.I32(0);
            serializer.I32(0);
            serializer.I32(0);
        }

        if (version > 0x2ac)
        {
            MoveTarget = serializer.V3(MoveTarget);

            if (version is > 0x2ac and < 0x36e) serializer.V3(null); // lookAt
            switch (version)
            {
                case > 0x2d6 and < 0x2e6:
                    serializer.V3(null);
                    break;
                case < 0x2ce:
                    serializer.S32(0); // stuckTime
                    break;
            }

            WaitTime = serializer.S32(WaitTime);

            if (version < 0x2ce)
                serializer.F32(0); // expectedVelLen
        }

        if (version > 0x2ae)
            PlayerNumber = serializer.I32(PlayerNumber);
        switch (version)
        {
            case > 0x2aa and < 0x2d5:
                serializer.I32(0); // color
                break;
            case > 0x338:
                ActorName = serializer.Wstr(ActorName);
                break;
        }

        if (version > 0x353)
        {
            if (version is 0x354 or 0x355)
            {
                serializer.F32(0); // lastTimeThrown
                serializer.F32(0); // lastTimeHitTheGround
            }
            else
            {
                LastTimeThrown = serializer.I32(LastTimeThrown);
                LastTimeHitTheGround = serializer.I32(LastTimeHitTheGround);
            }
            LastThrower = serializer.I32(LastThrower);
        }

        if (version > 0x391)
            CostumeToCopy = serializer.I8(CostumeToCopy);
        if (subVersion > 0x1a5)
            CopyFormAsWell = serializer.Bool(CopyFormAsWell);
    }

    public int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}