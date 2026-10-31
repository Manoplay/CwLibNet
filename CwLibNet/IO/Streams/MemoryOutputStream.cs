using CwLibNet.Enums;
using CwLibNet.Types.Data;
using System.Numerics;
using System.Text;

namespace CwLibNet.IO.Streams;

/// <summary>
/// Big-endian binary output stream.
/// </summary>
public class MemoryOutputStream
{
    private byte[]? buffer;

    /// <summary>
    /// Creates a memory output stream with specified size.
    /// </summary>
    /// <param name="size">Size of stream</param>
    public MemoryOutputStream(int size)
    {
        Length = size;
        buffer = new byte[size];
    }

    /// <summary>
    /// Creates a memory output stream with specified size and compression flags.
    /// </summary>
    /// <param name="size">Size of stream</param>
    /// <param name="compressionFlags">Flags for compression methods used</param>
    public MemoryOutputStream(int size, byte compressionFlags) : this(size)
    {
        this.CompressionFlags = compressionFlags;
    }

    /// <summary>
    /// Writes an arbitrary number of bytes to the stream.
    /// </summary>
    /// <param name="value">Bytes to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Bytes(byte[]? value)
    {
        Array.Copy(value, 0, buffer, Offset, value.Length);
        Offset += value.Length;
        return this;
    }

    /// <summary>
    /// Writes a byte array to the stream.
    /// </summary>
    /// <param name="value">Bytes to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Bytearray(byte[]? value)
    {
        if (value == null) return I32(0);
        I32(value.Length);
        return Bytes(value);
    }

    /// <summary>
    /// Writes a bool to the stream.
    /// </summary>
    /// <param name="value">bool to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Boole(bool value)
    {
        return U8(value ? 1 : 0);
    }

    /// <summary>
    /// Writes an array of bools to the stream.
    /// </summary>
    /// <param name="values">bool array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream BoolArray(bool[]? values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            Boole(value);
        return this;
    }

    /// <summary>
    /// Writes a byte to the stream.
    /// </summary>
    /// <param name="value">Byte to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream I8(byte value)
    {
        buffer[Offset++] = value;
        return this;
    }

    /// <summary>
    /// Writes an integer to the stream as a byte.
    /// </summary>
    /// <param name="value">Byte to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream U8(int value)
    {
        buffer[Offset++] = (byte)(value & 0xFF);
        return this;
    }

    /// <summary>
    /// Writes a short to the stream.
    /// </summary>
    /// <param name="value">Short to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream I16(ushort value)
    {
        return Bytes(IsLittleEndian ? Util.Bytes.ToBytesLE(value) : Util.Bytes.ToBytesBE(value));
    }
        
    public MemoryOutputStream I16(short value)
    {
        return Bytes(IsLittleEndian ? Util.Bytes.ToBytesLE(value) : Util.Bytes.ToBytesBE(value));
    }

    /// <summary>
    /// Writes an integer to the stream as an unsigned short.
    /// </summary>
    /// <param name="value">Short to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream U16(int value)
    {
        return I16((short)(value & 0xFFFF));
    }

    /// <summary>
    /// Writes a 24-bit unsigned integer to the stream.
    /// </summary>
    /// <param name="value">Integer to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream U24(int value)
    {
        value &= 0xFFFFFF;
        byte[]? b;
        if (IsLittleEndian)
        {
            b =
            [
                (byte) (value & 0xFF),
                (byte) (value >>> 8),
                (byte) (value >>> 16)
            ];
        }
        else
        {
            b =
            [
                (byte) (value >>> 16),
                (byte) (value >>> 8),
                (byte) (value & 0xFF)
            ];
        }
        return Bytes(b);
    }

    /// <summary>
    /// Writes a 32-bit integer to the stream, compressed depending on flags.
    /// </summary>
    /// <param name="value">Integer to write</param>
    /// <param name="force32">Whether or not to write as a 32-bit integer, regardless of compression</param>
    ///                flags.
    /// <returns>This output stream</returns>
    public MemoryOutputStream I32(int value, bool force32 = false)
    {
        if (!force32 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return Uleb128(value & 0xFFFFFFFFL);
        return Bytes(IsLittleEndian ? Util.Bytes.ToBytesLE(value) : Util.Bytes.ToBytesBE(value));
    }

    /// <summary>
    /// Writes a 32-bit signed integer to the stream, compressed depending on flags.
    /// This function modifies the value written to the stream to fit an unsigned value, prefer i32
    /// </summary>
    /// <param name="value">Signed integer to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream S32(int value)
    {
        return (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0 ? Uleb128((long)(value & 0x7fffffff) << 1 ^ (value >> 0x1f)) : I32(value, true);
    }

    /// <summary>
    /// Writes a long as a 32-bit integer to the stream, compressed depending on flags.
    /// </summary>
    /// <param name="value">Integer to write</param>
    /// <param name="force32">Whether to write as a 32-bit integer, regardless of compression</param>
    ///                flags.
    /// <returns>This output stream</returns>
    public MemoryOutputStream U32(long? value, bool force32)
    {
        if (!force32 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return Uleb128(value & 0xFFFFFFFFL);
        return Bytes(IsLittleEndian ? Util.Bytes.ToBytesLE((int)(value & 0xFFFFFFFF)) : Util.Bytes.ToBytesBE((int)(value & 0xFFFFFFFF)));
    }

    /// <summary>
    /// Writes a long to the stream, compressed depending on flags.
    /// </summary>
    /// <param name="value">Long to write</param>
    /// <param name="force64">Whether or not to write as a 64-bit integer, regardless of compression</param>
    ///                flags.
    /// <returns>This output stream</returns>
    public MemoryOutputStream U64(long value, bool force64 = false)
    {
        if (!force64 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return Uleb128(value);
        if (IsLittleEndian)
        {
            return Bytes([
                (byte) value,
                (byte) (value >>> 8),
                (byte) (value >>> 16),
                (byte) (value >>> 24),
                (byte) (value >>> 32),
                (byte) (value >>> 40),
                (byte) (value >>> 48),
                (byte) (value >>> 56),
            ]);
        }
        return Bytes([
            (byte) (value >>> 56),
            (byte) (value >>> 48),
            (byte) (value >>> 40),
            (byte) (value >>> 32),
            (byte) (value >>> 24),
            (byte) (value >>> 16),
            (byte) (value >>> 8),
            (byte) value
        ]);
    }

    /// <summary>
    /// Writes a 64-bit signed integer to the stream, compressed depending on flags.
    /// </summary>
    /// <param name="value">Long to write</param>
    /// <param name="force64">Whether or not to write as a 64-bit integer, regardless of compression</param>
    ///                flags.
    /// <returns>This output stream</returns>
    public MemoryOutputStream S64(long value, bool force64 = false)
    {
        return !force64 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0
            ? Uleb128(value << 1 ^ (value >> 0x3f))
            : U64(value, true);
    }

    /// <summary>
    /// Writes a long as an unsigned integer to the stream.
    /// </summary>
    /// <param name="value">Integer to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream U32(long value)
    {
        return U32(value, false);
    }

    public MemoryOutputStream I64(long value, bool force64) {
        if (!force64 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return Uleb128(value);
        if (IsLittleEndian) {
            return Bytes([
                (byte) value,
                (byte) (value >>> 8),
                (byte) (value >>> 16),
                (byte) (value >>> 24),
                (byte) (value >>> 32),
                (byte) (value >>> 40),
                (byte) (value >>> 48),
                (byte) (value >>> 56)
            ]);
        }
        return Bytes([
            (byte) (value >>> 56),
            (byte) (value >>> 48),
            (byte) (value >>> 40),
            (byte) (value >>> 32),
            (byte) (value >>> 24),
            (byte) (value >>> 16),
            (byte) (value >>> 8),
            (byte) value
        ]);
    }

    /// <summary>
    /// Writes a variable length quantity to the stream.
    /// </summary>
    /// <param name="value">Long to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Uleb128(long? value)
    {
        while (true)
        {
            var b = (byte)(value & 0x7f)!;
            value >>>= 7;
            if (value > 0L) b |= 128;
            I8(b);
            if (value == 0) break;
        }
        return this;
    }

    /// <summary>
    /// Writes a 16-bit integer array to the stream.
    /// </summary>
    /// <param name="values">Short array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream ShortArray(short[]? values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            I16(value);
        return this;
    }

    /// <summary>
    /// Writes a 32-bit integer array to the stream.
    /// </summary>
    /// <param name="values">Integer array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream IntArray(int[]? values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            I32(value);
        return this;
    }

    /// <summary>
    /// Writes a 64-bit integer array to the stream.
    /// </summary>
    /// <param name="values">Long array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream LongArray(long[]? values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            U64(value);
        return this;
    }

    /// <summary>
    /// Writes a 16 bit floating point number to the stream.
    /// https://stackoverflow.com/questions/6162651/half-precision-floating-point-in-java
    /// </summary>
    /// <param name="value">Float to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream F16(float value)
    {
        var fbits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        var sign = fbits >>> 16 & 0x8000;
        var val = (fbits & 0x7fffffff) + 0x1000;

        switch (val)
        {
            case >= 0x47800000:
            {
                if ((fbits & 0x7fffffff) >= 0x47800000)
                {
                    if (val < 0x7f800000)
                        return U16(sign | 0x7c00);
                    return U16(sign | 0x7c00 | (fbits & 0x007fffff) >>> 13);
                }
                return U16(sign | 0x7bff);
            }
            case >= 0x38800000:
                return U16(sign | val - 0x38000000 >>> 13);
            case < 0x33000000:
                return U16(sign);
            default:
                val = (fbits & 0x7fffffff) >>> 23;
                return U16(sign | ((fbits & 0x7fffff | 0x800000) + (0x800000 >>> val - 102) >>> 126 - val));
        }
    }

    /// <summary>
    /// Writes a 32 bit floating point number to the stream.
    /// </summary>
    /// <param name="value">Float to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream F32(float? value)
    {
        return I32(BitConverter.ToInt32(BitConverter.GetBytes(value ?? 0), 0), true);
    }

    /// <summary>
    /// Writes a 32-bit floating point number array to the stream.
    /// </summary>
    /// <param name="values">Float array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Floatarray(float[] values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            F32(value);
        return this;
    }

    /// <summary>
    /// Writes a 2-dimensional floating point vector to the stream.
    /// </summary>
    /// <param name="value">Vector2f to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream V2(Vector2? value)
    {
        value ??= Vector2.Zero;
        F32(value.Value.X);
        F32(value.Value.Y);
        return this;
    }

    /// <summary>
    /// Writes a 3-dimensional floating point vector to the stream.
    /// </summary>
    /// <param name="value">Vector3 to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream V3(Vector3? value)
    {
        value ??= Vector3.Zero;
        F32(value?.X);
        F32(value?.Y);
        F32(value?.Z);
        return this;
    }

    /// <summary>
    /// Writes a 3-dimensional floating point vector to the stream.
    /// </summary>
    /// <param name="value">Vector3 to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream V3(float x, float y, float z)
    {
        F32(x);
        F32(y);
        F32(z);
        return this;
    }

    /// <summary>
    /// Writes a 3-dimensional floating point vector to the stream.
    /// </summary>
    /// <param name="value">Vector3f to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream V3(Vector4? value)
    {
        value ??= Vector4.Zero;
        F32(value.Value.X);
        F32(value.Value.Y);
        F32(value.Value.Z);
        return this;
    }

    /// <summary>
    /// Writes a 4-dimensional floating point vector to the stream.
    /// </summary>
    /// <param name="value">Vector4f to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream V4(Vector4? value)
    {
        value ??= Vector4.Zero;
        F32(value.Value.X);
        F32(value.Value.Y);
        F32(value.Value.Z);
        F32(value.Value.W);
        return this;
    }

    /// <summary>
    /// Writes an array of 4-dimensional 32-bit floating point vectors to the stream.
    /// </summary>
    /// <param name="values">Vector array to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Vectorarray(Vector4[]? values)
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            V4(value);
        return this;
    }

    /// <summary>
    /// Writes a Matrix4x4 to the stream, compressed depending on flags.
    /// </summary>
    /// <param name="value">Matrix4x4 to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream M44(Matrix4x4? value)
    {
        value ??= Matrix4x4.Identity;

        float[] identity = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1];

        float[] values =
            // value.get(values);
            [
            value.Value.M11, value.Value.M12, value.Value.M13, value.Value.M14,
            value.Value.M21, value.Value.M22, value.Value.M23, value.Value.M24,
            value.Value.M31, value.Value.M32, value.Value.M33, value.Value.M34,
            value.Value.M41, value.Value.M42, value.Value.M43, value.Value.M44
        ];

        var flags = 0xFFFF;
        if ((CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_MATRICES) != 0)
        {
            flags = 0;
            for (var i = 0; i < 16; ++i)
                if (values[i] != identity[i])
                    flags |= 1 << i;
            I16((short)flags);
        }

        for (var i = 0; i < 16; ++i)
            if ((flags & (1 << i)) != 0)
                F32(values[i]);

        return this;
    }

    /// <summary>
    /// Writes a string of fixed size to the stream.
    /// </summary>
    /// <param name="value">String to write</param>
    /// <param name="size">Fixed size of string</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Str(string? value, int size)
    {
        if (value == null) return Bytes(new byte[size]);
        var data = Encoding.ASCII.GetBytes(value);
        if (data.Length > size)
            data = data.Take(size).ToArray();
        Bytes(data);
        Pad(size - data.Length);
        return this;
    }

    /// <summary>
    /// Writes a wide string of fixed size to the stream.
    /// </summary>
    /// <param name="value">String to write</param>
    /// <param name="size">Fixed size of string</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Wstr(string value, int size)
    {
        size *= 2;
        if (value == null) return Bytes(new byte[size]);
        var str = Encoding.BigEndianUnicode.GetBytes(value);
        if (str.Length > size)
            str = [.. str[..size]];
        Bytes(str);
        Pad(size - str.Length);
        return this;
    }

    /// <summary>
    /// Writes a length-prefixed string to the stream.
    /// </summary>
    /// <param name="value">String to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Str(string? value)
    {
        if (value == null) return I32(0);
        var _string = Encoding.ASCII.GetBytes(value);
        S32(_string.Length);
        return Bytes(_string);
    }

    /// <summary>
    /// Writes a length-prefixed wide string to the stream.
    /// </summary>
    /// <param name="value">String to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Wstr(string? value)
    {
        if (value == null) return I32(0);
        var _string = Encoding.BigEndianUnicode.GetBytes(value);
        S32(_string.Length / 2);
        return Bytes(_string);
    }

    /// <summary>
    /// Writes a SHA1 hash to the stream.
    /// </summary>
    /// <param name="value">SHA1 hash to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Sha1(Sha1? value)
    {
        return value == null ? Pad(0x14) : Bytes(value.GetHash());
    }

    /// <summary>
    /// Writes a GUID (uint32_t) to the stream.
    /// </summary>
    /// <param name="value">GUID to write</param>
    /// <param name="force32">Whether to read as a 32-bit integer, regardless of compression flags.</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Guid(GUID? value, bool force32 = false)
    {
        return value == null ? U32(0, force32) : U32(value?.Value, force32);
    }


    /// <summary>
    /// Writes an 8-bit enum value to the stream.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">Enum value</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Enum8(Enum? value)
    {
        return value == null ? U8(0) : I8(Convert.ToByte(value));
    }

    /// <summary>
    /// Writes an 32-bit enum value to the stream.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">Enum value</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Enum32(Enum? value)
    {
        return I32(value == null ? 0 : Convert.ToInt32(value));
    }

    /// <summary>
    /// Writes an 32-bit enum value to the stream.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">Enum value</param>
    /// <param name="signed">Whether to write a s32</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Enum32(Enum? value, bool signed)
    {
        if (value == null) return I32(0);
        var v = Convert.ToInt32(value);
        return signed ? S32(v) : I32(v);
    }

    /// <summary>
    /// Writes an 32-bit enum value to the stream.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="value">Enum value</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Enumarray<T>(T[]? values) where T : Enum
    {
        if (values == null) return I32(0);
        I32(values.Length);
        foreach (var value in values)
            Enum8(value);
        return this;
    }

    /// <summary>
    /// Writes a series of null characters to the stream.
    /// </summary>
    /// <param name="size">Number of bytes to write</param>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Pad(int size)
    {
        Offset += size;
        return this;
    }

    /// <summary>
    /// Shrinks the size of the buffer to the current offset.
    /// </summary>
    /// <returns>This output stream</returns>
    public MemoryOutputStream Shrink()
    {
        Array.Resize(ref buffer, Offset);
        return this;
    }

    /// <summary>
    /// Seeks to position relative to seek mode.
    /// </summary>
    /// <param name="offset">Offset relative to seek position</param>
    /// <param name="mode">Seek origin</param>
    public void Seek(int off, SeekMode? mode = SeekMode.RELATIVE)
    {
        if (mode == null)
            throw new NullReferenceException("SeekMode cannot be null!");
        if (Offset < 0) throw new ArgumentException("Can't seek to negative offsets.");
        switch (mode)
        {
            case SeekMode.BEGIN:
            {
                if (Offset > Length)
                    throw new ArgumentException("Can't seek past stream length.");
                Offset = off;
                break;
            }
            case SeekMode.RELATIVE:
            {
                var newOffset = Offset + off;
                if (newOffset > Length || newOffset < 0)
                    throw new ArgumentException("Can't seek outside bounds of " +
                                                "stream.");
                Offset = newOffset;
                break;
            }
            case SeekMode.END:
            {
                if (off < 0 || Length - off < 0)
                    throw new ArgumentException("Can't seek outside bounds of " +
                                                "stream.");
                Offset = Length - off;
                break;
            }
        }
    }

    public byte[]? Buffer => buffer;

    public int Offset { get; private set; }

    public int Length { get; }

    public byte CompressionFlags { get; }

    public bool IsLittleEndian;
}