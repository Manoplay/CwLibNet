namespace CwLibNet.Enums
{
    public class SerializationType
    {
        // UNKNOWN(null)
        public static SerializationType UNKNOWN = new(null),
            // BINARY("b")
            BINARY = new("b"),
            // TEXT("t")
            TEXT = new("t"),
            // ENCRYPTED_BINARY("e")
            ENCRYPTED_BINARY = new("e"),
            // COMPRESSED_TEXTURE(" ")
            COMPRESSED_TEXTURE = new (" "),
            // GTF_SWIZZLED("s")
            GTF_SWIZZLED = new ("s"),
            // GXT_SWIZZLED("S")
            GXT_SWIZZLED = new ("S");

        // --------------------
        // TODO enum body members
        private readonly string? value;
        SerializationType(string? value) {
            this.value = value;
        }
        public string? GetValue() => this.value;
        public static SerializationType FromValue(String? value)
        {

            return value switch
            {
                null => UNKNOWN,
                "b" => BINARY,
                "t" => TEXT,
                "e" => ENCRYPTED_BINARY,
                " " => COMPRESSED_TEXTURE,
                "s" => GTF_SWIZZLED,
                "S" => GXT_SWIZZLED,
                _ => UNKNOWN
            };
        }
    }
}