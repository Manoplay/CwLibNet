using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class BehaviourFollow: BehaviourBase
{
    public new const int BaseAllocationSize = 0x20;

    public Thing? FollowThing;
    public int LastFollowUpdate;

    public override void Serialize(Serializer serializer)
    {
        base.Serialize(serializer);

        FollowThing = serializer.Thing(FollowThing);
        LastFollowUpdate = serializer.I32(LastFollowUpdate);
    }

    public override int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}