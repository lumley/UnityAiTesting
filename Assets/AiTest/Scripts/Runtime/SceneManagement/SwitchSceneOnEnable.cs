using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Lumley.AiTest.SceneManagement
{
    /// <summary>
    /// Switches to a scene whenever the object is enabled
    /// </summary>
    public sealed class SwitchSceneOnEnable : MonoBehaviour
    {
        [SerializeField, Tooltip("Target scene to load")]
        private AssetReference _targetScene = null!;

        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        private void OnEnable()
        {
            SwitchScene();
        }

        private async void SwitchScene()
        {
            try
            {
                await Addressables.LoadSceneAsync(_targetScene, _loadSceneMode).Task;
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}