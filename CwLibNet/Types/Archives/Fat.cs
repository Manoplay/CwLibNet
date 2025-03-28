namespace CwLibNet.Types.Archives;

public class Fat
{
    private Fart archive;

    /**
     * SHA1 signature of data.
     */
    private readonly SHA1 sha1;

    /**
     * Offset of the data in the archive.
     */
    private readonly long offset;

    /**
     * Size of the associated data buffer.
     */
    private readonly int size;

    public Fat(Fart archive, SHA1 sha1, long offset, int size)
    {
        if (archive == null)
            throw new NullReferenceException("File archive provided to Fat cannot be null!");

        this.archive = archive;
        this.sha1 = sha1 ?? throw new NullReferenceException("File SHA1 provided to Fat cannot be null!");
        this.offset = offset;
        this.size = size;
    }

    public SHA1 getSHA1()
    {
        return this.sha1;
    }

    public long getOffset()
    {
        return this.offset;
    }

    public int getSize()
    {
        return this.size;
    }

    public Fart getFileArchive()
    {
        return this.archive;
    }

    /**
     * Extracts this entry from the associated archive.
     *
     * @return Extracted resource.
     */
    public byte[]? extract()
    {
        return this.archive.Extract(this);
    }

    public override string ToString()
    {
        return String.Format("Fat (%h, offset=%d, size=%d)",
            this.sha1, this.offset, this.size);
    }
}