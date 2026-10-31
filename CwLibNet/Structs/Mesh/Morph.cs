using System.Numerics;

namespace CwLibNet.Structs.Mesh;

public class Morph
{
    /// <summary>
    /// Relative offset of each vertex.
    /// </summary>
    public readonly Vector3[] Offsets;

    /// <summary>
    /// New normals for each vertex.
    /// </summary>
    public Vector3[] Normals;

    /// <summary>
    /// Creates a morph from offset and normal data.
    /// </summary>
    /// <param name="offsets">Relative offsets of each vertex</param>
    /// <param name="normals">New normals for each vertex</param>
    public Morph(Vector3[] offsets, Vector3[] normals)
    {
        Offsets = offsets;
        Normals = normals;
    }

    public Vector3[] GetOffsets()
    {
        return Offsets;
    }

    public Vector3[] GetNormals()
    {
        return Normals;
    }


}