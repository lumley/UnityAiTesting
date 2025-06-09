using UnityEngine;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Manager accessible globally to obtain which is the current game difficulty selected for the current game. 
    /// </summary>
    public sealed class CurrentGameDifficultyManager : MonoBehaviour, IGameDifficultyManager
    {
        [SerializeField, Tooltip("This is the current game difficulty selected for the current match.")] private GameManager.Difficulty _currentGameDifficulty = GameManager.Difficulty.Medium;

        /// <summary>
        /// The current game difficulty selected for the current game.
        /// </summary>
        public GameManager.Difficulty CurrentGameDifficulty
        {
            get => _currentGameDifficulty;
            set => _currentGameDifficulty = value;
        }

        private void Reset()
        {
            _currentGameDifficulty = GameManager.Difficulty.Medium;
        }
    }
}