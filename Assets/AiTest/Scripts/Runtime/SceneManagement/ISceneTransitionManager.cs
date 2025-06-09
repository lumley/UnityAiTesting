using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.SceneManagement
{
    /// <summary>
    /// Manager that handles scene transitions in the game with an animated loading screen.
    /// </summary>
    public interface ISceneTransitionManager
    {
        /// <summary>
        /// Transitions to the given scene
        /// </summary>
        /// <param name="sceneReference"><see cref="AssetReference"/> with a reference to the scene to transition</param>
        /// <param name="loadSceneMode"><see cref="LoadSceneMode"/> of the scene loaded</param>
        /// <returns><see cref="Task"/> completed once the scene is fully loaded and transitioned to it</returns>
        public Task TransitionToSceneAsync(AssetReference sceneReference, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
    }
}