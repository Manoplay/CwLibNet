using CwLibNet.Enums;
using CwLibNet.Types.Data;
using System.Numerics;
using System.Text;

namespace CwLibNet.IO.Streams;

public enum SeekMode
{
    BEGIN,
    RELATIVE,
    END
}

public class MemoryInputStream
{
    private readonly byte[]? buffer;

    public bool IsLittleEndian;

    /// <summary>
    /// Creates a memory input stream from a byte array.
    /// </summary>
    /// <param name="buffer">Byte array to use as source</param>
    public MemoryInputStream(byte[]? buffer)
    {
        this.buffer = buffer ?? throw new NullReferenceException("Buffer supplied to MemoryInputStream cannot be " +
                                                                 "null!");
        Length = buffer.Length;
    }

    /// <summary>
    /// Creates a memory input stream from byte array with compression flags.
    /// </summary>
    /// <param name="buffer">Byte array to use as source</param>
    /// <param name="compressionFlags">Flags for compression methods used</param>
    public MemoryInputStream(byte[]? buffer, byte compressionFlags): this(buffer)
    {
        this.CompressionFlags = compressionFlags;
    }

    /// <summary>
    /// Creates a memory input stream from file at path.
    /// </summary>
    /// <param name="path">Location to read data from</param>
    public MemoryInputStream(string path)
    {
        if (path == null)
            throw new NullReferenceException("Path supplied to MemoryInputStream cannot be " +
                                             "null!");
        var data = File.ReadAllBytes(path) ?? throw new ArgumentException("File provided could not be read!");
        buffer = data;
        Length = data.Length;
    }

    /// <summary>
    /// Creates a memory input stream from file at path with compression flags.
    /// </summary>
    /// <param name="path">Location to read data from</param>
    /// <param name="compressionFlags">Flags for compression methods used</param>
    public MemoryInputStream(string path, byte compressionFlags): this(path)
    {
        this.CompressionFlags = compressionFlags;
    }

    /// <summary>
    /// Reads an arbitrary number of bytes from the stream.
    /// </summary>
    /// <param name="size">Number of bytes to read from the stream</param>
    /// <returns>Bytes read from the stream</returns>
    public byte[]? Bytes(int size)
    {
        Offset += size;
        var secondBuffer = buffer[(Offset - size)..Offset]; // buffer.Skip(offset - size).ToArray();
//            Array.Resize(ref secondBuffer, size);
//             Array.Copy(this.buffer, 0, this.buffer, this.offset - size, this.offset);
        return secondBuffer;
    }

    /// <summary>
    /// Reads a byte array from the stream.
    /// </summary>
    /// <returns>Bytes read from the stream</returns>
    public byte[]? Bytearray()
    {
        var size = I32();
        return Bytes(size);
    }

    /// <summary>
    /// Reads a bool from the stream.
    /// </summary>
    /// <returns>bool read from the stream</returns>
    public bool Boole()
    {
        return I8() != 0;
    }

    /// <summary>
    /// Reads an array of bools from the stream.
    /// </summary>
    /// <returns>bool array read from the stream</returns>
    public bool[] Boolarray()
    {
        var count = I32();
        var elements = new bool[count];
        for (var i = 0; i < count; ++i)
            elements[i] = Boole();
        return elements;
    }

    /// <summary>
    /// Reads a byte from the stream.
    /// </summary>
    /// <returns>Byte read from the stream</returns>
    public byte I8()
    {
        return buffer[Offset++];
    }

    /// <summary>
    /// Reads an unsigned byte from the stream as an integer.
    /// </summary>
    /// <returns>Byte read from the stream</returns>
    public int U8()
    {
        return buffer[Offset++] & 0xFF;
    }

    /// <summary>
    /// Reads a short from the stream.
    /// </summary>
    /// <returns>Short read from the stream</returns>
    public short I16()
    {
        var bytes = Bytes(2);
        if (IsLittleEndian) return Util.Bytes.ToShortLE(bytes);
        return Util.Bytes.ToShortBE(bytes);
    }

    /// <summary>
    /// Reads an unsigned short from the stream as an integer.
    /// </summary>
    /// <returns>Short read from the stream</returns>
    public int U16()
    {
        return I16() & 0xFFFF;
    }

    /// <summary>
    /// Reads an unsigned 24-bit integer from the stream.
    /// </summary>
    /// <returns>Integer read from the stream</returns>
    public int U24()
    {
        var b = Bytes(3);
        if (IsLittleEndian)
            return (b[2] & 0xFF) << 16 | (b[1] & 0xFF) << 8 | b[0] & 0xFF;
        return (b[0] & 0xFF) << 16 | (b[1] & 0xFF) << 8 | b[2] & 0xFF;
    }

    /// <summary>
    /// Reads a 32-bit integer from the stream, compressed depending on flags.
    /// </summary>
    /// <param name="force32">Whether to read as a 32-bit integer, regardless of compression flags.</param>
    /// <returns>Integer read from the stream</returns>
    public int I32(bool force32 = false)
    {
        if (!force32 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return (int)(Uleb128() & 0xFFFFFFFF);
        var bytes = Bytes(4);
        return IsLittleEndian ? Util.Bytes.ToIntegerLE(bytes) : Util.Bytes.ToIntegerBE(bytes);
    }
        
    public long I64(bool force64) {
        if (!force64 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0)
            return Uleb128();
        var b = Bytes(8);
        if (IsLittleEndian) {
            return	(b[7] & 0xFFL) << 56 |
                    (b[6] & 0xFFL) << 48 |
                    (b[5] & 0xFFL) << 40 |
                    (b[4] & 0xFFL) << 32 |
                    (b[3] & 0xFFL) << 24 |
                    (b[2] & 0xFFL) << 16 |
                    (b[1] & 0xFFL) << 8 |
                    (b[0] & 0xFFL) << 0;
        }
        return	(b[0] & 0xFFL) << 56 |
                (b[1] & 0xFFL) << 48 |
                (b[2] & 0xFFL) << 40 |
                (b[3] & 0xFFL) << 32 |
                (b[4] & 0xFFL) << 24 |
                (b[5] & 0xFFL) << 16 |
                (b[6] & 0xFFL) << 8 |
                (b[7] & 0xFFL) << 0;
    }

    /// <summary>
    /// Reads a signed 32-bit integer from the stream, compressed depending on flags.
    /// This function modifies the value written to the stream to fit an unsigned value, prefer i32
    /// </summary>
    /// <returns>Signed integer read from the stream</returns>
    public int S32()
    {
        if ((CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) == 0)
            return I32(true);
        var v = (int)Uleb128();
        return v >> 1 ^ -(v & 1);
    }

    /// <summary>
    /// Reads a long as an unsigned integer from the stream, compressed depending on flags.
    /// </summary>
    /// <param name="force64">Whether or not to read as a 32-bit integer, regardless of compression flags.</param>
    /// <returns>Unsigned integer read from the stream</returns>
    public long U32(bool force32 = false)
    {
        if (force32 || (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) == 0)
            return I32(true) & 0xFFFFFFFFL;
        return Uleb128();
    }

    /// <summary>
    /// Reads a long from the stream, compressed depending on flags.
    /// </summary>
    /// <param name="force64">Whether to read as a 64-bit long, regardless of compression flags.</param>
    /// <returns>Long read from the stream</returns>
    public long U64(bool force64 = false)
    {
        if (!force64 && (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) != 0) return Uleb128();
        var b = Bytes(8);
        if (IsLittleEndian)
        {
            return (b[7] & 0xFF) << 56 |
                   (b[6] & 0xFF) << 48 |
                   (b[5] & 0xFF) << 40 |
                   (b[4] & 0xFF) << 32 |
                   (b[3] & 0xFF) << 24 |
                   (b[2] & 0xFF) << 16 |
                   (b[1] & 0xFF) << 8 |
                   (b[0] & 0xFF) << 0;
        }
        return (b[0] & 0xFF) << 56 |
               (b[1] & 0xFF) << 48 |
               (b[2] & 0xFF) << 40 |
               (b[3] & 0xFF) << 32 |
               (b[4] & 0xFF) << 24 |
               (b[5] & 0xFF) << 16 |
               (b[6] & 0xFF) << 8 |
               (b[7] & 0xFF) << 0;
    }

    /// <summary>
    /// Reads a "signed" long from the stream, compressed depending on flags.
    /// </summary>
    /// <param name="force64">Whether to read as a 64-bit long, regardless of compression flags.</param>
    /// <returns>Long read from the stream</returns>
    public long S64(bool force64 = false)
    {
        if (force64 || (CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_INTEGERS) == 0)
            return U64(true);
        var v = Uleb128();
        return v >> 1 ^ -(v & 1L);
    }

    /// <summary>
    /// Reads a variable length quantity from the stream.
    /// </summary>
    /// <returns>Long value read from the stream</returns>
    public long Uleb128()
    {
        long result = 0;
        var i = 0;
        while (true)
        {
            var b = U8() & 0xFFL;
            result |= (b & 0x7f) << 7 * i;
            if ((b & 0x80L) == 0L)
                break;
            ++i;
        }
        return result >>> 0;
    }

    /// <summary>
    /// Reads a 16-bit integer array from the stream.
    /// </summary>
    /// <returns>Short array read from the stream</returns>
    public short[] Shortarray()
    {
        var count = I32();
        var elements = new short[count];
        for (var i = 0; i < count; ++i)
            elements[i] = I16();
        return elements;
    }

    /// <summary>
    /// Reads a 32-bit integer array from the stream.
    /// </summary>
    /// <returns>Integer array read from the stream</returns>
    public int[] Intarray()
    {
        var count = I32();
        var elements = new int[count];
        for (var i = 0; i < count; ++i)
            elements[i] = I32();
        return elements;
    }

    /// <summary>
    /// Reads a 64-bit integer array from the stream.
    /// </summary>
    /// <returns>Long array read from the stream</returns>
    public long[] Longarray()
    {
        var count = I32();
        var elements = new long[count];
        for (var i = 0; i < count; ++i)
            elements[i] = U64();
        return elements;
    }

    /// <summary>
    /// Reads a 16 bit floating point number from the stream.
    /// https://stackoverflow.com/questions/6162651/half-precision-floating-point-in-java
    /// </summary>
    /// <returns>Float read from the stream</returns>
    public float F16()
    {
        var half = U16();
        var mant = half & 0x03ff;
        var exp = half & 0x7c00;
        if (exp == 0x7c00) exp = 0x3fc00;
        else if (exp != 0)
        {
            exp += 0x1c000;
            if (mant == 0 && exp > 0x1c400)
                return BitConverter.Int32BitsToSingle((half & 0x8000) << 16 | exp << 13 | 0x3ff);
        }
        else if (mant != 0)
        {
            exp = 0x1c400;
            do
            {
                mant <<= 1;
                exp -= 0x400;
            } while ((mant & 0x400) != 0);
            mant &= 0x3ff;
        }
        return BitConverter.Int32BitsToSingle((half & 0x8000) << 16 | (exp | mant) << 13);
    }

    /// <summary>
    /// Reads a 32 bit floating point number from the stream.
    /// </summary>
    /// <returns>Float read from the stream</returns>
    public float F32()
    {
        return BitConverter.Int32BitsToSingle(I32(true));
    }

    /// <summary>
    /// Reads a 32-bit floating point number array from the stream.
    /// </summary>
    /// <returns>Float array read from the stream</returns>
    public float[] FloatArray()
    {
        var count = I32();
        var elements = new float[count];
        for (var i = 0; i < count; ++i)
            elements[i] = F32();
        return elements;
    }

    /// <summary>
    /// Reads a 2-dimensional floating point vector from the stream.
    /// </summary>
    /// <returns>Vector2f read from the stream</returns>
    public Vector2 V2()
    {
        return new Vector2(F32(), F32());
    }

    /// <summary>
    /// Reads a 3-dimensional floating point vector from the stream.
    /// </summary>
    /// <returns>Vector3f read from the stream</returns>
    public Vector3 V3()
    {
        return new Vector3(F32(), F32(), F32());
    }

    /// <summary>
    /// Reads a 4-dimensional floating point vector from the stream.
    /// </summary>
    /// <returns>Vector4f read from the stream</returns>
    public Vector4 V4()
    {
        return new Vector4(F32(), F32(), F32(), F32());
    }

    /// <summary>
    /// Reads an array of 4-dimensional floating point vectors from the stream.
    /// </summary>
    /// <returns>Vector array read from the stream</returns>
    public Vector4[] VectorArray()
    {
        var count = I32();
        var elements = new Vector4[count];
        for (var i = 0; i < count; ++i)
            elements[i] = V4();
        return elements;
    }

    /// <summary>
    /// Reads a Matrix4x4 from the stream, compressed depending on flags.
    /// </summary>
    /// <returns>Matrix4x4 read from the stream</returns>
    public Matrix4x4 M44()
    {
        float[] matrix = [1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1];

        var flags = 0xFFFF;
        if ((CompressionFlags & Enums.CompressionFlags.USE_COMPRESSED_MATRICES) != 0)
            flags = I16();

        for (var i = 0; i < 16; ++i)
            if ((flags & (1 << i)) != 0)
                matrix[i] = F32();

        var mat = new Matrix4x4(matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], matrix[6], matrix[7], matrix[8], matrix[9], matrix[10], matrix[11], matrix[12], matrix[13], matrix[14], matrix[15]);
        return mat;
    }

    /// <summary>
    /// Reads a string of specified size from the stream.
    /// </summary>
    /// <param name="size">Size of string to read</param>
    /// <returns>String value read from the stream</returns>
    public string Str(int size)
    {
        return size == 0 ? "" : Encoding.ASCII.GetString(Bytes(size)).Replace("\0", "");
    }

    /// <summary>
    /// Reads a wide string of specified size from the stream.
    /// </summary>
    /// <param name="size">Size of string to read</param>
    /// <returns>String value read from the stream</returns>
    public string Wstr(int size)
    {
        return size == 0 ? "" : Encoding.BigEndianUnicode.GetString(Bytes(size * 2)).Replace("\0", "");
    }

    /// <summary>
    /// Reads a length-prefixed string from the stream.
    /// </summary>
    /// <returns>String value read from the stream</returns>
    public string Str()
    {
        return Str(S32());
    }

    /// <summary>
    /// Reads a length-prefixed wide string from the stream.
    /// </summary>
    /// <returns>String value read from the stream</returns>
    public string Wstr()
    {
        return Wstr(S32());
    }

    /// <summary>
    /// Reads a SHA1 hash from the stream.
    /// </summary>
    /// <returns>SHA1 hash read from the stream</returns>
    public Sha1 Sha1()
    {
        return new Sha1(Bytes(0x14));
    }

    /// <summary>
    /// Reads a GUID (uint32_t) from the stream.
    /// </summary>
    /// <param name="force32">Whether to read as a 32 bit integer, regardless of compression flags.</param>
    /// <returns>GUID read from the stream</returns>
    public GUID? Guid(bool force32 = false)
    {
        var number = U32(force32);
        if (number == 0) return null;
        return new GUID(number);
    }

    /// <summary>
    /// Reads an 8-bit integer from the stream and resolves the enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="enumeration">Enum class</param>
    /// <returns>Resolved enum constant</returns>
    public T? Enum8<T>() where T : Enum
    {
        var number = I8();
        foreach (var constant in Enum.GetValues(typeof(T)))
            if ((int)constant == number)
                return (T)constant;
        return default;
    }

    /// <summary>
    /// Reads an 32-bit integer from the stream and resolves the enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <returns>Resolved enum constant</returns>
    public T Enum32<T>() where T : Enum
    {
        return Enum32<T>(false);
    }

    /// <summary>
    /// Reads an 32-bit integer from the stream and resolves the enum value.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="signed">Whether to read a signed value</param>
    /// <returns>Resolved enum constant</returns>
    public T? Enum32<T>(bool signed)
    {
        var number = signed ? S32() : I32();
        var constants = Enum.GetValues(typeof(T));
        foreach (var constant in constants)
            if ((int)constant == number)
                return (T)constant;
        return default;
    }

    /// <summary>
    /// Reads a series of 8-bit integers from the stream and resolves them
    /// as an enum array.
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <returns>Resolved enum constant</returns>
    public T[] EnumArray<T>() where T: Enum
    {
        var count = I32();
        var elements = new T[count];
        for (var i = 0; i < count; ++i)
            elements[i] = Enum8<T>();
        return elements;
    }

    /// <summary>
    /// Seeks to position relative to seek mode.
    /// </summary>
    /// <param name="offset">Offset relative to seek position</param>
    /// <param name="mode">Seek origin</param>
    public void Seek(int offset, SeekMode mode = SeekMode.RELATIVE)
    {
        if (mode == null)
            throw new NullReferenceException("SeekMode cannot be null!");
        if (offset < 0) throw new ArgumentException("Can't seek to negative offsets.");
        switch (mode)
        {
            case SeekMode.BEGIN:
            {
                if (offset > Length)
                    throw new ArgumentException("Can't seek past stream length.");
                this.Offset = offset;
                break;
            }
            case SeekMode.RELATIVE:
            {
                var newOffset = this.Offset + offset;
                if (newOffset > Length || newOffset < 0)
                    throw new ArgumentException("Can't seek outside bounds of " +
                                                "stream.");
                this.Offset = newOffset;
                break;
            }
            case SeekMode.END:
            {
                if (offset < 0 || Length - offset < 0)
                    throw new ArgumentException("Can't seek outside bounds of " +
                                                "stream.");
                this.Offset = Length - offset;
                break;
            }
        }
    }

    public byte[]? GetBuffer()
    {
        return buffer;
    }

    public int Offset { get; private set; }

    public int Length { get; }

    public byte CompressionFlags { get; }
}