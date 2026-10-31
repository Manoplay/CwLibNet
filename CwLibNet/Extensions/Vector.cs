using System.Numerics;

namespace CwLibNet.Extensions;

public static class Vector
{
    extension(Vector4 v)
    {
        public float GetComponent(int index)
        {
            return index switch
            {
                0 => v.X,
                1 => v.Y,
                2 => v.Z,
                3 => v.W,
                _ => throw new IndexOutOfRangeException()
            };
        }

        public Vector4 SetComponent(int index, float value)
        {
            return index switch
            {
                0 => v with { X = value },
                1 => v with { Y = value },
                2 => v with { Z = value },
                3 => v with { W = value },
                _ => throw new IndexOutOfRangeException()
            };
        }

        public Vector4 Lerp(Vector4 b, float amount)
        {
            return Vector4.Lerp(v, b, amount);
        }
    }
}