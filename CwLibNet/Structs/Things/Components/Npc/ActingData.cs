using CwLibNet.Enums;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Types.Data;

namespace CwLibNet.Structs.Things.Components.Npc;

public class ActingData: ISerializable
{
    public const int BaseAllocationSize = 0x120;

    public int State;
    public Thing? RecordingNpc;
    public InputRecording? Recording = new();
    public Thing? RecordingPlayer;
    public int CurrentFrame;
    public int RecordingCountdown;
    public ResourceDescriptor? VoIpRecording;
    public bool TransformOnRestart;
    public byte PreviousState;

    public void Serialize(Serializer serializer)
    {
        int version = serializer.GetRevision().GetVersion();
        int subVersion = serializer.GetRevision().GetSubVersion();

        if (version > 0x2d9)
        {
            State = serializer.I32(State);
            RecordingNpc = serializer.Thing(RecordingNpc);
        }

        if (version > 0x295)
        {
            Recording = serializer.Struct(Recording);
            RecordingPlayer = serializer.Thing(RecordingPlayer);
            CurrentFrame = serializer.I32(CurrentFrame);
        }

        if (version > 0x2a5)
            RecordingCountdown = serializer.S32(RecordingCountdown);
        if (version >= 0x33e)
            VoIpRecording = serializer.Resource(VoIpRecording, ResourceType.VoipRecording);

        if (subVersion >= 0xb6)
            TransformOnRestart = serializer.Bool(TransformOnRestart);
        if (subVersion >= 0xbd)
            PreviousState = serializer.I8(PreviousState);
    }

    
    public int GetAllocatedSize()
    {
        var size = BaseAllocationSize;
        if (Recording != null)
            size += Recording.GetAllocatedSize();
        return size;
    }
}