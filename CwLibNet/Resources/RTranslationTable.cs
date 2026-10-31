using System.Text;
using CwLibNet.Extensions;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.IO.Streams;
using CwLibNet.Types.Data;
using CwLibNet.Util;

namespace CwLibNet.Resources;

public class RTranslationTable : Resource
{
    /// <summary>
    /// Calculates LAMS key ID from translation tag.
    /// </summary>
    /// <param name="tag"> Translation tag </param>
    /// <returns> Hashed key from translation tag </returns>
    public static long MakeLamsKeyID(string? tag)
    {
        tag ??= "";
        long v0 = 0, v1 = 0xC8509800L;
        for (var i = 32; i > 0; --i) {
            long c = 0x20;
            if (i - 1 < tag.Length)
                c = tag[i - 1];
            v0 = v0 * 0x1b + c;
        }
        
        if (tag.Length > 32) {
            v1 = 0;
            for (var i = 64; i > 32; --i) {
                long c = 0x20;
                if (i - 1 < tag.Length)
                    c = tag[i - 1];
                v1 = v1 * 0x1b + c;
            }
        }
        
        return (v0 + v1 * 0xDEADBEEFL) & 0xFFFFFFFFL;
    }

    public RTranslationTable(Dictionary<long, string?> translationTable)
    {
        lookup = translationTable;
    }

    public RTranslationTable(byte[]? data)
    {
        // Legacy RTranslationTable is just a text file.
        if ((ushort)Bytes.ToShortBE(data) == 0xFEFF) {
            string?[] lines = Encoding.Unicode.GetString(data).Split('\n');
            for (var i = 0; i < lines.Length; i+=2)
            {
                lookup.Add(MakeLamsKeyID(lines[i]), lines[i + 1]);
            }
            return;
        }
        
        var stream = new MemoryInputStream(data);
        var dataLength = stream.Length;

        var count = stream.I32();

        var tableOffset = 0x4 + count * 0x8;
        lookup = new Dictionary<long, string?>(count);
        for (var i = 0; i < count; ++i) {
            var key = stream.U32();
            var stringStart = stream.I32();

            var oldOffset = stream.Offset;

            stream.Seek(tableOffset + stringStart + 2, SeekMode.BEGIN);
            while (stream.Offset != dataLength)
                if (stream.U16() == 0xFEFF)
                    break;
            
            var stringEnd = stream.Offset;
            if (stringEnd != dataLength)
                stringEnd -= 2;
            
            stream.Seek(tableOffset + stringStart, SeekMode.BEGIN);

            var text = Encoding.BigEndianUnicode.GetString(stream.Bytes(stringEnd - stream.Offset)).Trim((char)0xFEFF);
            lookup.Add(key, text);

            stream.Seek(oldOffset, SeekMode.BEGIN);
        }
    }
        
    private readonly Dictionary<long, string?> lookup = new();
        
    public string? Translate(string tag) {
        return Translate(MakeLamsKeyID(tag));
    }
        
    public string? Translate(int key) { return Translate(key & 0xFFFFFFFF); }
        
    public string? Translate(long key)
    {
        return lookup.GetValueOrDefault(key);
    }
        
    public void Add(string key, string value) { 
        lookup.Add(MakeLamsKeyID(key), value);
    }

    public void Add(long key, string value) {
        lookup.Add(key, value);
    }

        
    public override SerializationData Build(Revision revision, byte compressionFlags)
    {
        return Build();
    }

    public override int GetAllocatedSize()
    {
        throw new NotImplementedException();
    }

    public override void Serialize(Serializer serializer)
    {
        throw new NotImplementedException();
    }

    public SerializationData Build()
    {
        var stringTableSize = lookup.Values
            .Select(element => (Encoding.BigEndianUnicode.GetBytes(element).Length + 1) * 2)
            .Aggregate(0, (total, element) => total + element) - 2;

        var offsets = new Dictionary<string, int>();
        var stringTable = new MemoryOutputStream(stringTableSize);
        var keyTable = new MemoryOutputStream(lookup.Count * 8 + 4);
        keyTable.I32(lookup.Count);

        var keys = lookup.Keys.ToList();
        keys.Sort((a, z) => a.CompareUnsigned(z));
        foreach (var key in keys)
        {
            var value = lookup[key];
            keyTable.U32(key);
            if (offsets.TryGetValue(value, out var offset1))
            {
                keyTable.I32(offset1);
                continue;
            }

            var offset = stringTable.Offset;
            offsets.Add(value, offset);

            keyTable.I32(offset);

            stringTable.U16(0xFEFF);
            stringTable.Wstr(value, Encoding.BigEndianUnicode.GetBytes(value).Length / 2);
        }

        stringTable.Shrink();
        return new SerializationData(Bytes.Combine(keyTable.Buffer, stringTable.Buffer));
    }
}