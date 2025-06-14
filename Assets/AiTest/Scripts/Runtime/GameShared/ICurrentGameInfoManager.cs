namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Manager interface accessible globally to obtain information about the currently selected game.
    /// </summary>
    public interface ICurrentGameInfoManager
    {
        public GameManager.Difficulty CurrentGameDifficulty { get; set; }
        
        public int CurrentGameIndex { get; set; }
    }
}