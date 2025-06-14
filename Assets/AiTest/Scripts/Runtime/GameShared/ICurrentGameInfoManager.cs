using UnityEngine.AddressableAssets;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Manager interface accessible globally to obtain information about the currently selected game.
    /// </summary>
    public interface ICurrentGameInfoManager
    {
        public GameDifficulty CurrentGameDifficulty { get; set; }
        
        public int CurrentGameIndex { get; set; }
        
        /// <summary>
        /// The current scene being played, null if not playing right now
        /// </summary>
        public AssetReference? CurrentGameAsset { get; set; }
    }
}