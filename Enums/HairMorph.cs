using CwLibNet.IO;

namespace CwLibNet.Enums
{
    public enum HairMorph : int
    {
        // HAT(0)
        HAT,
        // HELMET(1)
        HELMET,
        // FRINGE(2)
        FRINGE
    }

    public sealed class HairBody
    {

        private readonly HairMorph value;
        HairBody(int value)
        {
            this.value = (HairMorph)value;
        }

        public int getValue()
        {
            return (int)this.value;
        }




        public static HairBody fromValue(int value)
        {
            if (Enum.IsDefined(typeof(HairMorph), value))
            {
                return new HairBody(value);
            }
            return default(HairBody);
        }
    }
}