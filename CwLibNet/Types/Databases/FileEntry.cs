using CwLibNet.Types.Data;

namespace CwLibNet.Types.Databases;

public class FileEntry(string path, Sha1 sha1, long size)
{
    public string Path = path;

    /// <summary>
    /// Size of the associated data buffer.
    /// </summary>
    public long Size = size;

    /// <summary>
    /// SHA1 signature of data.
    /// </summary>
    public Sha1 Sha1 = sha1;

    /// <summary>
    /// Unique key for data
    /// </summary>
    public object? Key;

    /// <summary>
    /// Resource metadata
    /// </summary>
    public ResourceInfo? Info;

    /// <summary>
    /// Map of resources assigned to this entry.
    /// </summary>
    private readonly object?[] resources = new object[10];

    public T? GetResource<T>(int key)
    {
        var value = resources[key];
        if (value == null) return default;
        return (T) value;
    }

    public void SetDetails(FileEntry entry)
    {
        Path = entry.Path;
        Size = entry.Size;
        Sha1 = entry.Sha1;
    }
}