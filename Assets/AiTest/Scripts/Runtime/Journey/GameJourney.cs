using Lumley.AiTest.GameShared;

namespace Lumley.AiTest.Journey
{
    /// <summary>
    /// Container of a single game represented in the journey
    /// </summary>
    public sealed class GameJourney
    {
        public GameInfo GameInfo = null!;
        public GameDifficulty Difficulty;
        public int GameIndex;
        public bool IsCompleted;
    }
}