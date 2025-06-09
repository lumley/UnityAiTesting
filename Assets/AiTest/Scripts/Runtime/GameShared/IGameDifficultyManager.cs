namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Manager interface accessible globally to obtain which is the current game difficulty selected for the current game.
    /// </summary>
    public interface IGameDifficultyManager
    {
        public GameManager.Difficulty CurrentGameDifficulty { get; set; }
    }
}