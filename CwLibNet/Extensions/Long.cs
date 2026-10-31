namespace CwLibNet.Extensions;

public static class Longs
{
    extension(long value)
    {
        /// <summary>
        /// Counts the leading zeros in a number.
        /// </summary>
        public int NumberOfLeadingZeros()
        {
            if (value == 0)
                return 64;

            var count = 0;
            var unsignedValue = (ulong)value;

            while ((unsignedValue & 0x8000000000000000) == 0)
            {
                count++;
                unsignedValue <<= 1;
            }

            return count;
        }

        public int CompareUnsigned(long y)
        {
            var ux = (ulong)value;
            var uy = (ulong)y;
            return ux.CompareTo(uy);
        }
    }
}