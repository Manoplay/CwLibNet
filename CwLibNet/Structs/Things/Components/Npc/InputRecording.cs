using System.Numerics;
using CwLibNet.Enums;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class InputRecording: ISerializable
{
    public const int BaseAllocationSize = 0x80;

    public byte[]? InputBuffer;
    public int[]? OffsetBuffer;
    public int[]? AbsoluteExpressionBuffer;
    public Matrix4x4? StartWorldTransform;
    public Matrix4x4?[]? StartLocalSceneGraph;
    public Vector3? StartFootPos;
    public Vector3?[]? StartVelocities;
    public bool RecordingContainsMoveData;
    public bool RecordingContainsVitaData;

    public void Serialize(Serializer serializer)
    {
        var version = serializer.GetRevision().GetVersion();

        if (version > 0x28f)
            InputBuffer = serializer.Bytearray(InputBuffer);
        if (version > 0x28f)
            OffsetBuffer = serializer.Intvector(OffsetBuffer);
        if (version > 0x2a5)
            AbsoluteExpressionBuffer = serializer.Intvector(AbsoluteExpressionBuffer);

        if (version > 0x2dd)
        {
            StartWorldTransform = serializer.M44(StartWorldTransform);
            if (!serializer.IsWriting())
                StartLocalSceneGraph = new Matrix4x4?[serializer.GetInput().I32()];
            else
            {
                StartLocalSceneGraph ??= [];
                serializer.GetOutput().I32(StartLocalSceneGraph.Length);
            }
            for (var i = 0; i < StartLocalSceneGraph.Length; ++i)
                StartLocalSceneGraph[i] = serializer.M44(StartLocalSceneGraph[i]);
        }

        if (version > 0x2df)
            StartFootPos = serializer.V3(StartFootPos);

        if (version > 0x2e0)
        {
            if (!serializer.IsWriting())
                StartVelocities = new Vector3?[serializer.GetInput().I32()];
            else
            {
                StartVelocities ??= [];
                serializer.GetOutput().I32(StartVelocities.Length);
            }
            for (var i = 0; i < StartVelocities.Length; ++i)
                StartVelocities[i] = serializer.V3(StartVelocities[i]);
        }

        if (version > 0x3c4)
            RecordingContainsMoveData = serializer.Bool(RecordingContainsMoveData);

        if (serializer.GetRevision().Has(Branch.Double11, 0x3a))
            RecordingContainsVitaData = serializer.Bool(RecordingContainsVitaData);
    }

    public int GetAllocatedSize()
    {
        var size = BaseAllocationSize;
        if (InputBuffer != null) size += InputBuffer.Length;
        if (OffsetBuffer != null) size += (OffsetBuffer.Length * 0x4);
        if (AbsoluteExpressionBuffer != null)
            size += (AbsoluteExpressionBuffer.Length * 0x4);
        if (StartLocalSceneGraph != null)
            size += (StartLocalSceneGraph.Length * 0x40);
        if (StartVelocities != null) size += (StartVelocities.Length * 0x10);
        return size;
    }
}