using System.Numerics;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Structs.Things.Components.Npc;

public class Input: ISerializable
{
    public const int BaseAllocationSize = 0x100;

    public bool Alive;
    public Vector3? LeftStick;
    public Vector3? RightStick;
    public int Buttons, ButtonsOld;
    public short[] SensorData;
    public float[] SensorDathSmooth;
    public Vector4? SensorDir;
    public Vector4? SensorDirOld;
    public short[] PressureData;
    public int PlayerNumber;
    public bool ControllingPauseMenu;

    public void Serialize(Serializer serializer)
    {
        Alive = serializer.Bool(Alive);
        LeftStick = serializer.V3(LeftStick);
        RightStick = serializer.V3(RightStick);
        Buttons = serializer.I32(Buttons);
        ButtonsOld = serializer.I32(ButtonsOld);
        SensorData = serializer.Shortarray(SensorData);
        SensorDathSmooth = serializer.Floatarray(SensorDathSmooth);
        SensorDir = serializer.V4(SensorDir);
        SensorDirOld = serializer.V4(SensorDirOld);
        PressureData = serializer.Shortarray(PressureData);
        PlayerNumber = serializer.I32(PlayerNumber);
        ControllingPauseMenu = serializer.Bool(ControllingPauseMenu);
        if (serializer.GetRevision().GetVersion() < 0x210)
            serializer.Bool(false);
    }

    public int GetAllocatedSize()
    {
        return BaseAllocationSize;
    }
}