namespace CwLibNet.Enums;

public enum FlipType
{
    // <summary>
    // X.pos, X.rot
    // </summary>
    // MAX(0)
    MAX,
    // <summary>
    // Y.pos, Y.rot + prerot
    // </summary>
    // BIP_ROOT(1)
    BIP_ROOT,
    // <summary>
    // Z.pos, Z.rot + prerot
    // </summary>
    // BIP_PELVIS(2)
    BIP_PELVIS,
    // <summary>
    // Z.pos, Z.rot
    // </summary>
    // BIP_BONE(3)
    BIP_BONE,
    // <summary>
    // Z.pos, Z.rot + null prerot
    // </summary>
    // PARENT_WAS_BIP_BONE(4)
    PARENT_WAS_BIP_BONE,
    // <summary>
    // X.pos, X.rot + prerot
    // </summary>
    // GRANDPARENT_WAS_BIP_BONE(5)
    GRANDPARENT_WAS_BIP_BONE,
    // <summary>
    // No flip, straight copy from mirror bone.
    // </summary>
    // COPY(6)
    COPY 
}

public sealed class FlipBody
{
    private readonly FlipType value;

    private FlipBody(int value)
    {
        this.value = (FlipType)(value & 0xFF);
    }

    public FlipType getValue()
    {
        return value;
    }

    public static FlipBody fromValue(int value)
    {
        if (Enum.IsDefined(typeof(FlipType), value))
            return new FlipBody(value);
        return default(FlipBody);
    }
}
