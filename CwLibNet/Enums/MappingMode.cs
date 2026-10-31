namespace CwLibNet.Enums;

public enum MappingMode
{
    // HIDDEN(0)
    HIDDEN,
    // <summary>
    // Standard XYZ UV Mapping.
    // </summary>
    // DAVE(1)
    DAVE,
    // <summary>
    // Wraps vertically; separate faces.
    // </summary>
    // CYLINDER_01(2)
    CYLINDER_01,
    // <summary>
    // Wraps vertical; all one face.
    // </summary>
    // CYLINDER(3)
    CYLINDER,
    // <summary>
    // Wraps vertically; seperate faces, stretches on Y axis.
    // </summary>
    // PLANARXZ_01(4)
    PLANARXZ_01,
    // <summary>
    // Wraps vertically; all one face, stretches on Y axis.
    // </summary>
    // PLANARXZ(5)
    PLANARXZ 
}

public sealed class MappingBody
{
    private readonly MappingMode value;

    public MappingBody(int value)
    {
        this.value = (MappingMode)value;
    }

    public MappingMode getValue()
    {
        return value;
    }

    public static MappingBody? fromValue(int value)
    {
        return Enum.IsDefined(typeof(MappingMode), value) ? new MappingBody(value) : null;
    }
}
