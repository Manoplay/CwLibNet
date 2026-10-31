using CwLibNet.Types.Data;

namespace CwLibNet.Types.Archives;

public class Fat(Fart archive, Sha1 sha1, long offset, int size)
{
    private readonly Fart archive = archive ?? throw new NullReferenceException("File archive provided to Fat cannot be null!");

    /**
     * SHA1 signature of data.
     */
    private readonly Sha1 sha1 = sha1 ?? throw new NullReferenceException("File SHA1 provided to Fat cannot be null!");

    /**
     * Offset of the data in the archive.
     */
    private readonly long offset = offset;

    /**
     * Size of the associated data buffer.
     */
    private readonly int size = size;

    public Sha1 GetSha1()
    {
        return sha1;
    }

    public long GetOffset()
    {
        return offset;
    }

    public int GetSize()
    {
        return size;
    }

    public Fart GetFileArchive()
    {
        return archive;
    }

    /**
     * Extracts this entry from the associated archive.
     *
     * @return Extracted resource.
     */
    public byte[]? Extract()
    {
        return archive.Extract(this);
    }

    public override string ToString()
    {
        return $"Fat ({sha1}, offset={offset}, size={size})";
    }
}