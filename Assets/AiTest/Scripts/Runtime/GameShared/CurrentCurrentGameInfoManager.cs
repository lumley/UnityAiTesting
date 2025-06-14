using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Manager accessible globally to obtain information about the currently selected game.
    /// </summary>
    public sealed class CurrentCurrentGameInfoManager : MonoBehaviour, ICurrentGameInfoManager
    {
        // These values are only serialized to allow easy testing in editor.
        [Header("Runtime only values")]
        [SerializeField, Tooltip("This is the current game difficulty selected for the current match.")] private GameDifficulty _currentGameDifficulty = GameDifficulty.Medium;
        
        [SerializeField, Tooltip("This is the current game index selected for the current match.")]
        private int _currentGameIndex = 0;

        /// <summary>
        /// The current game difficulty selected for the current game.
        /// </summary>
        public GameDifficulty CurrentGameDifficulty
        {
            get => _currentGameDifficulty;
            set => _currentGameDifficulty = value;
        }

        public int CurrentGameIndex { get; set; }
        public AssetReference? CurrentGameAsset { get; set; }

        private void Reset()
        {
            _currentGameDifficulty = GameDifficulty.Medium;
        }
    }
}