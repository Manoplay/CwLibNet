using CwLibNet.Enums;
using CwLibNet.Structs.StaticMesh;
using CwLibNet.Structs.Texture;
using CwLibNet.Types.Data;

namespace CwLibNet.IO.Serializer;

public class SerializationData
{
    public readonly byte[]? Buffer;
    public readonly Revision? Revision;
    public readonly byte? CompressionFlags;
    public readonly ResourceType? Type;
    public readonly SerializationType? Method;
    public readonly ResourceDescriptor?[] Dependencies;
    public readonly CellGcmTexture? TextureInfo;
    public readonly StaticMeshInfo? StaticMeshInfo;

    public SerializationData(
        byte[]? buffer,
        Revision revision,
        byte compressionFlags,
        ResourceType type,
        SerializationType method,
        ResourceDescriptor?[] dependencies)
    {
        Buffer = buffer;
        Revision = revision;
        CompressionFlags = compressionFlags;
        Type = type;
        Method = method;
        Dependencies = dependencies;
        TextureInfo = null;
        StaticMeshInfo = null;
    }

    /// <summary>
    /// Creates a serialization data structure for texture data.
    /// </summary>
    /// <param name="buffer"> Decompressed texture data</param>
    public SerializationData(byte[]? buffer)
    {
        Buffer = buffer;
        Revision = null;
        CompressionFlags = CwLibNet.Enums.CompressionFlags.USE_NO_COMPRESSION;
        Type = ResourceType.Texture;
        Method = SerializationType.COMPRESSED_TEXTURE;
        Dependencies = [];
        TextureInfo = null;
        StaticMeshInfo = null;
    }

    /// <summary>
    /// Creates a serialization data structure for GTF texture data.
    /// </summary>
    /// <param name="buffer"> Decompressed texture data</param>
    /// <param name="info">   Texture metadata</param>
    public SerializationData(byte[] buffer, CellGcmTexture info)
    {
        Buffer = buffer;
        Revision = null;
        CompressionFlags = CwLibNet.Enums.CompressionFlags.USE_NO_COMPRESSION;
        Type = ResourceType.GtfTexture;
        Method = info.GetMethod();
        Dependencies = [];
        TextureInfo = info;
        StaticMeshInfo = null;
    }

    /// <summary>
    /// Creates a serialization data structure for static mesh data
    /// </summary>
    /// <param name="buffer">   Decompressed static mesh</param>
    /// <param name="revision"> Revision of the serialized resource</param>
    /// <param name="info">     Static mesh metadata</param>
    public SerializationData(byte[] buffer, Revision revision, StaticMeshInfo info)
    {
        Buffer = buffer;
        Revision = revision;
        CompressionFlags = CwLibNet.Enums.CompressionFlags.USE_NO_COMPRESSION;
        Type = ResourceType.StaticMesh;
        Method = SerializationType.BINARY;

        // Maybe I need a Method for gathering Dependencies without actually
        // serializing the data, can probably use reflection for it,
        // as to avoid serializing twice.

        Serializer serializer = new(info.GetAllocatedSize(), revision,
            CwLibNet.Enums.CompressionFlags.USE_NO_COMPRESSION);
        serializer.Struct(info);
        Dependencies = serializer.GetDependencies();

        TextureInfo = null;
        StaticMeshInfo = info;
    }
}