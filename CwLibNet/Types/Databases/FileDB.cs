using System.Collections;
using CwLibNet.Enums;
using CwLibNet.Extensions;
using CwLibNet.IO.Streams;
using CwLibNet.Types.Data;

namespace CwLibNet.Types.Databases;

public class FileDb: IEnumerable<FileDBRow>
{
    private const int DefaultCapacity = 10;
    public bool HasChanges = false;

    /**
     * Minimum set of GUIDs not used by any of the LittleBigPlanet games.
     */
    private const long MinSafeGuid = 0x00180000;

    private int revision;
    protected List<FileDBRow> Entries;
    public readonly DatabaseType Type;
    private String? file;


    protected Dictionary<long, FileDBRow> Lookup;
    
    protected FileDb(String? file, DatabaseType type, int revision)
    {
        this.file = file;
        this.Type = type;
        this.revision = revision;
        this.Entries = new List<FileDBRow>(DefaultCapacity);
        this.Lookup = new Dictionary<long, FileDBRow>(DefaultCapacity);
    }
    
    protected FileDb(String? file, DatabaseType type)
    {
        this.file = file;
        this.Type = type;
        this.Entries = new List<FileDBRow>(DefaultCapacity);
        this.Lookup = new Dictionary<long, FileDBRow>(DefaultCapacity);
    }

    
    public FileDb(int revision, int capacity = FileDb.DefaultCapacity): this(null, DatabaseType.FILE_DATABASE, revision)
    {
        this.revision = revision;
        if (capacity < 0)
            throw new ArgumentException("Cannot allocate entry array with negative " +
                                        "count!");
        this.Entries = new List<FileDBRow>(capacity);
        this.Lookup = new Dictionary<long, FileDBRow>(capacity);
    }

    public FileDb(String file): this(file, DatabaseType.FILE_DATABASE)
    {
        this.Process(new MemoryInputStream(file));
    }
    
    
    
    protected void Process(MemoryInputStream stream)
    {
        this.revision = stream.I32();
        bool isLbp3 = (this.revision >> 0x10) >= 0x148;
        int count = stream.I32();
        this.Entries = new List<FileDBRow>(count);
        this.Lookup = new Dictionary<long, FileDBRow>(count);

        for (int i = 0; i < count; ++i)
        {
            String path = stream.Str(isLbp3 ? stream.I16() : stream.I32());
            long timestamp = isLbp3 ? stream.U32() : stream.S64();
            long size = stream.U32();
            SHA1 sha1 = stream.Sha1();
            GUID guid = stream.Guid()!.Value;

            // If a GUID is duplicated, skip it
            if (this.Lookup.ContainsKey(guid.Value))
                continue;

            /* 	In LittleBigPlanet Vita, some versions of the databases don't store any filenames,
                only the extensions, so we'll use the hash of the resource in place of a name. */
            if (path.StartsWith("."))
            {
                path = String.Format("data/{0}{1}{2}",
                    FileDb.GetFolderFromExtension(path), sha1, path);
            }

            FileDBRow entry = new FileDBRow(path, timestamp, size, sha1, guid);

            this.Entries.Add(entry);
            this.Lookup.Add(guid.Value, entry);
        }
    }
    
    public bool Exists(long guid)
    {
        return this.Lookup.ContainsKey(guid);
    }
    
    public bool Exists(GUID guid)
    {
        return this.Exists(guid.Value);
    }

    public FileDBRow? Get(SHA1 sha1)
    {
        return this.Entries.FirstOrDefault(row => row.Sha1.Equals(sha1));
    }
    
    public FileDBRow? Get(long guid)
    {
        return this.Lookup.GetValueOrDefault(guid);
    }
    
    public FileDBRow? Get(GUID guid)
    {
        return this.Get(guid.Value);
    }
    
    public FileDBRow? Get(String path)
    {
        if (path == null)
            throw new NullReferenceException("Can't find null path!");
        path = path.ToLower(); // Ignore cases
        return this.Entries.FirstOrDefault(entry => entry.Path.ToLower().Contains(path));
    }
    
    public FileDBRow NewFileDbRow(String path, GUID guid)
    {
        if (this.Lookup.ContainsKey(guid.Value))
            throw new ArgumentException("GUID already exists in database!");
        FileDBRow entry = new FileDBRow( path, 0, 0, new SHA1(), guid);
        entry.updateDate();
        this.Entries.Add(entry);
        this.Lookup.Add(guid.Value, entry);
        return entry;
    }
    
    public FileDBRow NewFileDbRow(FileDBRow entry)
    {
        FileDBRow newEntry = this.NewFileDbRow(entry.Path, entry.GetGuid());
        newEntry.setDetails(entry);
        newEntry.Date = entry.Date;
        newEntry.Key = entry.Key;
        return newEntry;
    }
    
    public static String GetFolderFromExtension(String extension)
    {
        switch (extension.ToLower())
        {
            case ".slt":
                return "slots/";
            case ".tex":
                return "textures/";
            case ".bpr":
            case ".ipr":
                return "profiles/";
            case ".mol":
            case ".msh":
                return "models/";
            case ".gmat":
            case ".gmt":
                return "gfx/";
            case ".mat":
                return "materials/";
            case ".ff":
            case ".fsh":
                return "scripts/";
            case ".plan":
            case ".pln":
                return "plans/";
            case ".pal":
                return "palettes/";
            case ".oft":
                return "outfits/";
            case ".sph":
                return "skeletons/";
            case ".bin":
            case ".lvl":
                return "levels/";
            case ".vpo":
                return "shaders/vertex/";
            case ".fpo":
                return "shaders/fragment/";
            case ".anim":
            case ".anm":
                return "animations/";
            case ".bev":
                return "bevels/";
            case ".smh":
                return "static_meshes/";
            case ".mus":
                return "audio/settings/";
            case ".fsb":
                return "audio/music/";
            case ".txt":
                return "text/";
            default:
                return "unknown/";
        }
    }
    
    public void Remove(FileEntry entry)
    {
        this.Entries.Remove((FileDBRow)entry);
        this.Lookup.Remove(((GUID)entry.Key).Value);
    }
    
    public void Patch(FileDb patch)
    {
        foreach (FileDBRow entry in patch)
        {
            if (this.Exists(entry.GetGuid()))
                this.Get(entry.GetGuid())!.setDetails(entry);
            else
                this.NewFileDbRow(entry);
        }
    }
    
    public GUID GetNextGuid()
    {
        long lastGuid = FileDb.MinSafeGuid;
        while (this.Lookup.ContainsKey(lastGuid)) lastGuid++;
        return new GUID(lastGuid);
    }
    
    public int GetEntryCount()
    {
        return this.Entries.Count;
    }

    public byte[] Build()
    {
        // Just figure the GUIDs should be in ascending order.
        Entries.Sort((l, r) => l.GetGuid().Value.CompareUnsigned(r.GetGuid().Value));

        int pathSize = this.Entries
            .Select(e => e.Path.Length)
            .Aggregate(0, (total, element) => total + element);

        bool isLbp3 = (this.revision >> 0x10) >= 0x148;
        int baseEntrySize = (isLbp3) ? 0x22 : 0x28;
        MemoryOutputStream stream =
            new MemoryOutputStream(0x8 + (baseEntrySize * this.Entries.Count) + pathSize);
        stream.I32(this.revision);
        stream.I32(this.Entries.Count);
        foreach (FileDBRow entry in this.Entries)
        {
            int length = entry.Path.Length;
            if (isLbp3)
                stream.I16((short) length);
            else
                stream.I32(length);

            stream.Str(entry.Path, length);

            if (isLbp3) stream.U32(entry.Date);
            else stream.S64(entry.Date);

            stream.U32(entry.Size);
            stream.Sha1(entry.Sha1);
            stream.Guid(entry.GetGuid());
        }
        return stream.GetBuffer();
    }

    public void Save(String? file = null)
    {
        file ??= this.file;
        File.WriteAllBytes(file!, this.Build());
        HasChanges = false;
    }
    
    public IEnumerator<FileDBRow> GetEnumerator()
    {
        return this.Entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}