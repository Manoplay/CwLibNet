using CwLibNet.IO;

namespace CwLibNet.Enums
{
    public enum LevelType : int
    {
        // MAIN_PATH(0)
        MAIN_PATH,
        // MINI_LEVEL(1)
        MINI_LEVEL,
        // MINI_GAME(2)
        MINI_GAME,
        // TUTORIAL(3)
        TUTORIAL,
        // RANDOM_CRAP(4)
        RANDOM_CRAP,
        // BOSS(5)
        BOSS,
        // VERSUS(6)
        VERSUS,
        // CUTSCENE(7)
        CUTSCENE,
        // LBP2_DLC_HUB(8)
        LBP2_DLC_HUB 
    }

    public sealed class LevelBody
    {
        private readonly LevelType value;

        public LevelBody(int value)
        {
            this.value = (LevelType)value;
        }

        public LevelType getType()
        {
            return this.value;
        }

        /// <summary>
        /// Attempts to get a LevelType from value.
        /// </summary>
        /// <param name="value">Game mode value</param>
        /// <returns>LevelType</returns>
        public static LevelBody fromValue(int value)
        {
            if (Enum.IsDefined(typeof(LevelType), value))
        {
            return new LevelBody(value);
        }
            return new LevelBody((int)LevelType.MAIN_PATH);
        }
    }
}