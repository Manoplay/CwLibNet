using CwLibNet.Enums;
using CwLibNet.IO.Streams;
using CwLibNet.Types.Data;
using System.Numerics;
using CwLibNet.IO.Serializer;

namespace CwLibNet.Util;

public class Bytes
{
    private static readonly char[] HEX_ARRAY = "0123456789ABCDEF".ToCharArray();

    /// <summary>
    /// Converts a byte array into a hex string.
    /// Implementation sourced from
    /// </summary>
    /// <a href="https://stackoverflow.com/questions/9655181/how-to-convert-a-byte-array-to-a-hex-string-in-java">here</a>.
    ///
    /// <param name="bytes">Byte array to hexify</param>
    /// <returns>Hex representation of byte array</returns>
    public static string ToHex(byte[]? bytes)
    {
        if (bytes == null)
            throw new NullReferenceException("Can't convert null byte array to hexadecimal string!");
        var hex = new char[bytes.Length * 2];
        for (var i = 0; i < bytes.Length; ++i)
        {
            var b = bytes[i] & 0xFF;
            hex[i * 2] = HEX_ARRAY[b >>> 4];
            hex[i * 2 + 1] = HEX_ARRAY[b & 0xF];
        }
        return string.Join("", hex);
    }

    /// <summary>
    /// Converts an integer into a hex string.
    /// </summary>
    /// <param name="value">Integer to hexify</param>
    /// <returns>Hex representation of integer</returns>
    public static string ToHex(int value)
    {
        return ToHex(ToBytesBE(value));
    }

    /// <summary>
    /// Converts a hex string into a byte array.
    /// </summary>
    /// <param name="v">Hex string</param>
    /// <returns>Byte array from hex string</returns>
    public static byte[] FromHex(string v)
    {
        var length = v.Length;
        var b = new byte[length / 2];
        for (var i = 0; i < length; i += 2)
            b[i / 2] = (byte)(
                (Convert.ToInt32(v[i].ToString(), 16) << 4) + Convert.ToInt32(v[i + 1].ToString(), 16)
            );
        return b;
    }

    /// <summary>
    /// Converts a big-order byte array to a short primitive.
    /// </summary>
    /// <param name="b">2-byte array containing big-order short</param>
    /// <returns>the short from the byte array</returns>
    public static short ToShortBE(byte[]? b)
    {
        if (b == null)
            throw new NullReferenceException("Can't read data type from null byte array!");
        return (short)((b[0] & 0xFF) << 8 | (b[1] & 0xFF));
    }

    /// <summary>
    /// Converts a little-order byte array to a short primitive.
    /// </summary>
    /// <param name="b">2-byte array containing litte-order short</param>
    /// <returns>the short from the byte array</returns>
    public static short ToShortLE(byte[]? b)
    {
        if (b == null)
            throw new NullReferenceException("Can't read data type from null byte array!");
        return (short)((b[0] & 0xFF) | (b[1] & 0xFF) << 8);
    }

    /// <summary>
    /// Converts a big-order byte array to an integer primitive.
    /// </summary>
    /// <param name="b">4-byte array containing big-order integer</param>
    /// <returns>the integer from the byte array</returns>
    public static int ToIntegerBE(byte[]? b)
    {
        if (b == null)
            throw new NullReferenceException("Can't read data type from null byte array!");
        return (b[0] & 0xFF) << 24 |
               (b[1] & 0xFF) << 16 |
               (b[2] & 0xFF) << 8 |
               (b[3] & 0xFF);
    }

    /// <summary>
    /// Converts a little-order byte array to an integer primitive.
    /// </summary>
    /// <param name="b">4-byte array containing little-order integer</param>
    /// <returns>the integer from the byte array</returns>
    public static int ToIntegerLE(byte[]? b)
    {
        if (b == null)
            throw new NullReferenceException("Can't read data type from null byte array!");
        return (b[0] & 0xFF) |
               (b[1] & 0xFF) << 8 |
               (b[2] & 0xFF) << 16 |
               (b[3] & 0xFF) << 24;
    }

    /// <summary>
    /// Converts a short into a big-order byte array.
    /// </summary>
    /// <param name="v">short primitive</param>
    /// <returns>the big-order byte array containing the short</returns>
    public static byte[] ToBytesBE(short v)
    {
        return [
            (byte) (v >>> 8),
            (byte) (v & 0xFF)
        ];
    }

    /// <summary>
    /// Converts a short into a little-order byte array.
    /// </summary>
    /// <param name="v">short primitive</param>
    /// <returns>the little-order byte array containing the short</returns>
    public static byte[] ToBytesLE(short v)
    {
        return [
            (byte) (v & 0xFF),
            (byte) (v >>> 8)
        ];
    }

    /// <summary>
    /// Converts a integer into a big-order byte array.
    /// </summary>
    /// <param name="v">integer primitive</param>
    /// <returns>the big-order byte array containing the integer</returns>
    public static byte[] ToBytesBE(int v)
    {
        return [
            (byte) (v >>> 24),
            (byte) (v >>> 16),
            (byte) (v >>> 8),
            (byte) (v & 0xFF)
        ];
    }

    /// <summary>
    /// Converts a integer into a little-order byte array.
    /// </summary>
    /// <param name="v">integer primitive</param>
    /// <returns>the little-order byte array containing the integer</returns>
    public static byte[] ToBytesLE(int v)
    {
        return [
            (byte) (v & 0xFF),
            (byte) (v >>> 8),
            (byte) (v >>> 16),
            (byte) (v >>> 24),
        ];
    }

    public static byte[]? GetIntegerBuffer(long value, byte compressionFlags)
    {
        var output = new MemoryOutputStream(0x8, compressionFlags);
        output.U32(value);
        output.Shrink();
        return output.Buffer;
    }

    public static byte[]? GetResourceReference(ResourceDescriptor? res, Revision revision,
        byte compressionFlags)
    {
        var serializer = new Serializer(0x1c + 0x4, revision, compressionFlags);
        serializer.Resource(res, ResourceType.Invalid, true);
        return serializer.GetBuffer();
    }

    /// <summary>
    /// Converts an array of 32 bit big endian integers to a byte array.
    /// </summary>
    /// <param name="data">Integer array</param>
    /// <returns>Converted byte array</returns>
    public static byte[] FromIntArrayBE(int[] data)
    {
        if (data == null)
            throw new NullReferenceException("Integer stream cannot be null!");
        var output = new byte[data.Length * 4];
        for (var i = 0; i < data.Length; ++i)
        {
            var v = data[i];
            var dest = i * 4;
            output[dest] = (byte)(v >>> 24);
            output[dest + 1] = (byte)(v >>> 16);
            output[dest + 2] = (byte)(v >>> 8);
            output[dest + 3] = (byte)(v & 0xFF);
        }
        return output;
    }

    /// <summary>
    /// Converts a big endian byte array to an array of 32 bit integers.
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <returns>Converted integer array</returns>
    public static int[] ToIntArrayBE(byte[] data)
    {
        if (data == null)
            throw new NullReferenceException("Byte stream cannot be null!");
        if (data.Length % 4 != 0)
            throw new ArgumentException("Byte stream length must be divisible by 4!");
        var output = new int[data.Length / 4];
        for (var i = 0; i < output.Length; ++i)
        {
            var src = i * 4;
            output[i] = (data[src] & 0xFF) << 24 |
                        (data[src + 1] & 0xFF) << 16 |
                        (data[src + 2] & 0xFF) << 8 |
                        (data[src + 3] & 0xFF);
        }
        return output;
    }

    /// <summary>
    /// Splits a byte array into a series of chunks.
    /// Implementation sourced from
    /// </summary>
    /// <a href="https://stackoverflow.com/questions/3405195/divide-array-into-smaller-parts/26695737">here</a>.
    ///
    /// <param name="data">Byte array to split</param>
    /// <param name="size">Size of each byte chunk</param>
    /// <returns>Chunked byte arrays</returns>
    public static byte[][] Split(byte[]? data, int size)
    {
        var _out = new byte[(int)Math.Ceiling(data.Length / (double)size)][];
        var start = 0;
        for (var i = 0; i < _out.Length; ++i)
        {
            var end = Math.Min(data.Length, start + size);
            _out[i] = new byte[end + start];
            Array.Copy(data, _out[i], end + start);
            _out = _out.Skip(start).ToArray();
            start += size;
        }
        return _out;
    }

    /// <summary>
    /// Combines a series of byte arrays into one.
    /// Implementation sourced from
    /// </summary>
    /// <a href="https://stackoverflow.com/questions/5513152/easy-way-to-concatenate-two-byte-arrays">here</a>.
    ///
    /// <param name="arrays">Byte arrays to combine</param>
    /// <returns>Combined byte arrays.</returns>
    public static byte[] Combine(params byte[]?[] arrays)
    {
        var totalLength = 0;
        foreach (var bytes in arrays) totalLength += bytes.Length;
        var result = new byte[totalLength];
        var currentIndex = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, result, currentIndex, array.Length);
            currentIndex += array.Length;
        }
        return result;
    }

    /// <summary>
    /// Replaces all instances of pattern inside bytearray with another.
    /// Does in-place modification if the lengths are the same, otherwise
    /// a new array is created.
    /// </summary>
    /// <param name="source">Buffer to replace patterns in</param>
    /// <param name="original">Original pattern to replace</param>
    /// <param name="replacement">Data to replace original pattern with</param>
    /// <returns>Bytearray with replaced patterns</returns>
    public static byte[]? Replace(byte[]? source, byte[]? original, byte[]? replacement)
    {
        if (Equals(original, replacement)) return source;

        var offsets = Matcher.IndicesOf(source, original);

        // If the original/replacement buffer are the same length,
        // we can save time and memory by just copying into
        // those regions.
        if (original.Length == replacement.Length)
        {
            foreach (var offset in offsets)
                Array.Copy(replacement, 0, source, offset, replacement.Length);
            return source;
        }

        var diff = replacement.Length - original.Length;
        var buffer =
            new byte[source.Length - original.Length * offsets.Length + replacement.Length * offsets.Length];

        var sourceOffset = 0;
        var destOffset = 0;
        for (var i = 0; i < offsets.Length; ++i)
        {
            var offset = offsets[i];
            var dest = offset + diff * i;

            Array.Copy(source, sourceOffset, buffer, sourceOffset + diff * i,
                offset - sourceOffset);
            Array.Copy(replacement, 0, buffer, dest, replacement.Length);

            sourceOffset = offset + original.Length;
            destOffset = dest + replacement.Length;
        }

        var remaining = source.Length - sourceOffset;
        if (remaining != 0)
            Array.Copy(source, sourceOffset, buffer, destOffset, remaining);

        return buffer;
    }

    /// <summary>
    /// Gets a byte array containing a ULEB-128 encoded value.
    /// </summary>
    /// <param name="value">Value to encode</param>
    /// <returns>Encoded byte array</returns>
    public static byte[]? PackULEB128(long value)
    {
        var stream = new MemoryOutputStream(0x10,
            CompressionFlags.USE_ALL_COMPRESSION);
        return stream.U32(value).Shrink().Buffer;
    }

    /// <summary>
    /// Unpacks a 11/11/10 normal value.
    /// </summary>
    /// <param name="value">Value to unpack</param>
    /// <returns>Unpacked vector</returns>
    public static Vector3 UnpackNormal32(long value)
    {
        // There's probably a much better way to handle
        // the fact that this is probably just signed data
        // but this works, so maybe I'll come back to it
        // at some point, who knows.

        var output = new Vector3(0, 0, 0);

        var x = (float)(value & 0x3ffL);
        var x_sign = ((value >>> 10) & 1L) > 0L;

        var y = (float)((value >>> 11) & 0x3ffL);
        var y_sign = ((value >>> 21) & 1L) > 0L;

        var z = (float)((value >>> 22) & 0x1ffL);
        var z_sign = (value >>> 31 & 1L) > 0L;

        if (x_sign) output.X = -((1023f - x) / 1023f);
        else output.X = x / 1023f;

        if (y_sign) output.Z = -((1023f - y) / 1023f);
        else output.Y = y / 1023f;

        if (z_sign) output.Z = -((511f - z) / 511f);
        else output.Z = z / 511f;

        return output;
    }

    /// <summary>
    /// Unpacks a 12/11/1 normal value.
    /// </summary>
    /// <param name="normal">Value to unpack</param>
    /// <returns>Unpacked vector</returns>
    public static Vector3 UnpackNormal24(int normal)
    {
        var x = (float)(normal & 0x7ff);
        x = (normal & 0x800) != 0 ? -(0x800 - x) / 0x7ff : x / 0x7ff;

        var y = (float)((normal >> 12) & 0x3ff);
        y = ((normal >> 12) & 0x400) != 0 ? -(0x400 - y) / 0x3ff : y / 0x3ff;

        var z = (float)(Math.Pow(-1, normal >> 23) *
                        Math.Sqrt(1 - (Math.Pow(x, 2) + Math.Pow(y, 2))));

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Packs a 11/11/10 normal value.
    /// </summary>
    /// <param name="normal">Vertex normal to pack</param>
    /// <returns>Packed normal</returns>
    public static int PackNormal32(Vector3 normal)
    {
        if (normal == null)
            throw new NullReferenceException("Can't pack null vertex normal!");

        var x = (int)Math.Round(normal.X * 0x3ff) & 0x7ff;
        var y = (int)Math.Round(normal.Y * 0x3ff) & 0x7ff;
        var z = (int)Math.Round(normal.Z * 0x1ff) & 0x3ff;

        return x | (y << 11) | (z << 22);
    }

    /// <summary>
    /// Packs a 12/11/1 normal value.
    /// </summary>
    /// <param name="normal">Vertex normal to pack</param>
    /// <returns>Packed normal</returns>
    public static int PackNormal24(Vector3 normal)
    {
        if (normal == null)
            throw new NullReferenceException("Can't pack null vertex normal!");

        var x = (int)Math.Round(normal.X * 0x7ff) & 0xfff;
        var y = (int)Math.Round(normal.Y * 0x3ff) & 0x7ff;
        var z = normal.Z < 0 ? 1 : 0;

        return x | (y << 12) | (z << 23);
    }
}