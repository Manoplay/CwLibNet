using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Animation;

public class AnimBone: ISerializable
{
    public const int BaseAllocationSize = 0x20;

    /// <summary>
    /// Unique identifier for the bone, generated from the name.
    /// </summary>
    public int AnimHash;

    /// <summary>
    /// Index of the parent of this bone.
    /// </summary>
    public int Parent = -1;

    /// <summary>
    /// Index of the first child of this bone.
    /// </summary>
    public int FirstChild = -1;

    /// <summary>
    /// Index of the next sibling of this bone.
    /// </summary>
    public int NextSibling = -1;

    public AnimBone() { }

    public AnimBone(int animHash, int parent, int firstChild, int nextSibling)
    {
        AnimHash = animHash;
        Parent = parent;
        FirstChild = firstChild;
        NextSibling = nextSibling;
    }

    
    public virtual void Serialize(Serializer serializer)
    {
        AnimHash = serializer.I32(AnimHash);
        Parent = serializer.S32(Parent);
        FirstChild = serializer.S32(FirstChild);
        NextSibling = serializer.S32(NextSibling);
    }

    
    public virtual int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }


}