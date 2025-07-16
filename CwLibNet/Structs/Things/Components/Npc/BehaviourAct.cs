using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class BehaviourAct: BehaviourBase
{
    public new const int BaseAllocationSize = 0x20;

    public Thing? RecordingPlayer;
    public InputRecording Recording = new();
    public int CurrentFrame;

    public override void Serialize(Serializer serializer)
    {
        base.Serialize(serializer);
        if (serializer.GetRevision().GetVersion() <= 0x28f) return;

        RecordingPlayer = serializer.Thing(RecordingPlayer);
        Recording = serializer.Struct(Recording);
        CurrentFrame = serializer.I32(CurrentFrame);
    }

    public override int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}